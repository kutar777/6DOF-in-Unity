using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.JsonData;
using Assets.Scripts.Core.Utils;
using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// 自己位置計算(階数、内観/外観)
    /// 本計算はCrossing Number Algorithm（交差数判定）に基づき
    /// 内観外観判定を行っている。
    /// </summary>
    public static class CurrentSelfPosition
    {
        // フロア区分情報
        public static IReadOnlyList<FloorSegment> FloorSegments => floorSegments;
        // 施主名
        public static string ownerName;

        private static List<FloorSegment> floorSegments;
        // 区画線分情報
        private static ArrayContainer<LineSegment> lineSegments;
        // 登録済み判定
        private static bool isRegistered = false;
        // 内観中心座標
        private static Vector3? centerPos;

        // 階情報
        public class FloorSegment
        {
            // 階数
            public readonly int floor;
            // 階の基準地点(一番低い高さ)
            public readonly float floor_height;
            // 階の高さ
            public readonly float floor_base;
            // 小屋裏判定
            public bool koyaura;

            public FloorSegment(int f, float fh, float fb, bool koyaura)
            {
                this.floor = f;
                this.floor_height = fh;
                this.floor_base = fb;
                this.koyaura = koyaura;
            }
        }

        // 線分情報
        public struct LineSegment
        {
            // 階数
            public int floor;
            // 始点 X座標
            public float Px;
            // 始点 Y座標
            public float Py;
            // 終点 X座標
            public float Qx;
            // 終点 Y座標
            public float Qy;
            // 線分の勾配
            public float slope;
            // 線分の切片
            public float intercept;
            // 定数判定(勾配・切片が存在しない x = aのような)
            public bool isConstant;

            public LineSegment(int f, float px, float py, float qx, float qy, float s, float i, bool c = false)
            {
                floor = f;
                Px = px;
                Py = py;
                Qx = qx;
                Qy = qy;
                slope = s;
                intercept = i;
                isConstant = c;
            }
        }

        /// <summary>
        /// プラン情報登録
        /// </summary>
        /// <param name="planData">プランデータ</param>
        public static void Register(JsonPlanData planData)
        {
            if (planData == null)
            {
                floorSegments = new List<FloorSegment>(0);
                lineSegments = new ArrayContainer<LineSegment>(0);
                ownerName = null;
                isRegistered = false;
            }
            else
            {
                // LineSegmentに情報を詰め込む
                // フロア数分メモリを確保する
                floorSegments = new List<FloorSegment>(planData.plan.floors.Count());
                floorSegments.AddRange(planData.plan.floors.Select(i => CreateFloorSegment(i)));

                // 線分数分メモリを確保する
                lineSegments = new ArrayContainer<LineSegment>((planData.plan.floors.SelectMany(i => i.coords).Count()) / 2);
                foreach (var data in planData.plan.floors) lineSegments.Append(CreateLineSegment(data));

                // 施主名保存
                ownerName = planData.plan.owner_name + " 様邸";

                isRegistered = true;
            }
        }

        /// <summary>
        /// プラン情報登録済み判定
        /// </summary>
        /// <returns></returns>
        public static bool IsRegistered()
        {
            return isRegistered;
        }

        private static FloorSegment CreateFloorSegment(JsonFloorData data)
        {
            var floorheight = new FloorSegment(
                data.floor,
                data.floor_height,
                data.floor_base,
                data.is_koyaura);

            return floorheight;
        }

        private static List<LineSegment> CreateLineSegment(JsonFloorData data)
        {
            int floor = data.floor;
            float[] coords = data.coords;
            int[] num_of_coords = data.num_of_coords;

            int coord_offset = 0;
            var output = new List<LineSegment>();

            foreach (var num in num_of_coords)
            {
                // 頂点の数が不足している場合は例外を投げる
                if (num < 3) throw new FormatException("Number of Coords is too shot.");

                // フロアに属する頂点情報を収集する
                int pair = num * 2;
                float[] subcoords = new float[pair];
                Array.Copy(coords, coord_offset, subcoords, 0, pair);
                coord_offset += pair;

                for (int current = 0; current < num; current++)
                {
                    int next = current + 1;
                    // 始点が最終に到達した場合、終点は先頭要素に移す
                    if (next >= num) next = 0;

                    // 始点P, 終点Q
                    float px = subcoords[current * 2];
                    float py = subcoords[current * 2 + 1];
                    float qx = subcoords[next * 2];
                    float qy = subcoords[next * 2 + 1];

                    if (px == qx)
                    {
                        if (py == qy)
                        {
                            // 点が重なっていたら例外を投げる
                            throw new FormatException("Points Duplecated.");
                        }

                        // Y軸に並行な直線
                        output.Add(new LineSegment(floor, px, py, qx, qy, 0, 0, true));
                        continue;
                    }
                    else
                    {
                        // 始点・終点・勾配・切片を登録する
                        float slope = (py - qy) / (px - qx);
                        float intercept = (qx * py - px * qy) / (qx - px);
                        output.Add(new LineSegment(floor, px, py, qx, qy, slope, intercept, false));
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// 内観外観判定
        /// </summary>
        /// <param name="position">自己位置</param>
        /// <returns>true: 内観, false: 外観</returns>
        public static bool IsInside(Vector3 position)
        {
            // XZ平面をXY平面と捉える
            float x = position.x;
            float y = position.z;

            // いずれかの階高に属しているか
            int floorNum = FloorNumber(position);
            // フロア外の場合は直ちに外観とする
            if (floorNum < 0) return false;

            int crossingNum = 0;
            // 属している階の床（矩形）を取得する
            for (int i = 0; i < lineSegments.Count; i++)
            {
                float px = lineSegments.items[i].Px;
                float py = lineSegments.items[i].Py;
                float qx = lineSegments.items[i].Qx;
                float qy = lineSegments.items[i].Qy;
                float slope = lineSegments.items[i].slope;
                float intercept = lineSegments.items[i].intercept;

                // 階が違う場合はスキップする
                if (lineSegments.items[i].floor != floorNum) continue;

                // Y成分の範囲に入っているか
                if (((py >= qy) && (py >= y) && (qy < y)) ||
                    ((py < qy) && (py <= y) && (qy > y)))
                {
                    // 傾きが垂直でX座標が左側に場合は交点ありとする
                    if (lineSegments.items[i].isConstant && (x <= px))
                    {
                        crossingNum++;
                        continue;
                    }
                    // 傾きが0で対象点が線分上 or 左側にある場合は交点ありとする
                    if ((slope == 0) && (y == py) && ((x <= px) || (x < qx)))
                    {
                        crossingNum++;
                        continue;
                    }
                    // 傾きが正で対象点が線分の上側にある場合
                    if ((slope > 0) && (y >= (slope * x + intercept))){
                        crossingNum++;
                        continue;
                    }
                    // 傾きが負の場合
                    if ((slope < 0) && (y <= (slope * x + intercept)))
                    {
                        crossingNum++;
                        continue;
                    }
                }
                else
                {
                    // Y成分の範囲に入っていない場合は次の線分へ
                    continue;
                }
            }

            if (crossingNum % 2 == 1)
            {
                // 交点が奇数なら内観
                return true;
            }
            else
            {
                // 交点が偶数なら外観
                return false;
            }
        }

        /// <summary>
        /// 階数判定
        /// </summary>
        /// <param name="position">自己位置</param>
        /// <returns>階数 (外観の場合は0)</returns>
        public static int FloorNumber(Vector3 position)
        {
            int floorNum = 0;

            for (int i = 0; i < floorSegments.Count; i++)
            {
                float bottom = floorSegments[i].floor_base;
                float top = floorSegments[i].floor_base + floorSegments[i].floor_height;
                if (bottom <= position.y && top > position.y)
                {
                    floorNum = floorSegments[i].floor;
                    break;
                }
            }
            return floorNum;
        }

        /// <summary>
        /// 視点高さ
        /// </summary>
        /// <param name="position">視点位置</param>
        /// <returns>視点高さ情報(文字列)</returns>
        public static string ViewPointHeight(Vector3 position)
        {
            float baseLine = 0.0f;
            int floorNum = 0;
            bool koyaura = false;

            // 階層情報が登録されていなければGLと判断する
            if (isRegistered)
            {
                // 内観でない場合はGLを基準とする
                if (IsInside(position))
                {
                    // 視点が属するフロアーを調べる
                    floorNum = FloorNumber(position);
                    for (int i = 0; i < floorSegments.Count; i++)
                    {
                        if (floorSegments[i].floor == floorNum)
                        {
                            baseLine = floorSegments[i].floor_base;
                            koyaura = floorSegments[i].koyaura;
                            break;
                        }
                    }
                }
            }

            // ミリメートル単位で高さを算出
            float height = (position.y - baseLine) * 1000;
            string information;

            if (koyaura)
            {
                information = string.Format("視点高さ  {0:D}mm (基準：小屋裏 FL)", (int)height);
                return information;
            }

            if(baseLine >= 0)
            {
                // 外観・内観それぞれのフォーマットに数値を当てはめる
                if (floorNum == 0)
                {
                    information = string.Format("視点高さ  {0:D}mm (基準：GL)", (int)height);
                }
                else
                {
                    information = string.Format("視点高さ  {0:D}mm (基準：{1:D}F FL)", (int)height, floorNum);
                }
            }
            else
            {
                // 地下階
                information = string.Format("視点高さ  {0:D}mm (基準：地下{1:D}F FL)", (int)height, floorNum - 256);
            }
            return information;
        }

        /// <summary>
        /// 階高取得
        /// </summary>
        /// <param name="position">視点（カメラ）の位置</param>
        /// <returns>階高</returns>
        public static float CurrentFloorLevel(Vector3 position)
        {
            float baseLine = 0.0f;
            int floorNum = 0;

            // 階層情報が登録されていなければGLと判断する
            if (isRegistered)
            {
                // 内観でない場合はGLを基準とする
                if (IsInside(position))
                {
                    // 視点が属するフロアーを調べる
                    floorNum = FloorNumber(position);
                    for (int i = 0; i < floorSegments.Count; i++)
                    {
                        if (floorSegments[i].floor == floorNum)
                        {
                            baseLine = floorSegments[i].floor_base;
                        }
                    }
                }
            }
            return baseLine;
        }

        /// <summary>
        /// 内観中心座標
        /// </summary>
        /// <returns>内観中心座標（内観が存在しない場合はnull）</returns>
        public static Vector3? CenterPos
        {
            get
            {
                if (!centerPos.HasValue) { centerPos = GetCenterPos(); }
                return centerPos;
            }
        }

        private static Vector3? GetCenterPos()
        {
            if (!isRegistered || lineSegments.Count == 0) return null;

            var min = default(Vector3);
            var max = default(Vector3);
            for (var i = 0; i < lineSegments.Count; i++)
            {
                var px = lineSegments.items[i].Px;
                var py = lineSegments.items[i].Py;
                var qx = lineSegments.items[i].Qx;
                var qy = lineSegments.items[i].Qy;
                if (i == 0)
                {
                    min.x = Mathf.Min(px, qx);
                    min.z = Mathf.Min(py, qy);
                    max.x = Mathf.Max(px, qx);
                    max.z = Mathf.Max(py, qy);
                }
                else
                {
                    min.x = Mathf.Min(px, qx, min.x);
                    min.z = Mathf.Min(py, qy, min.z);
                    max.x = Mathf.Max(px, qx, max.x);
                    max.z = Mathf.Max(py, qy, max.z);
                }
            }
            for (var i = 0; i < floorSegments.Count; i++)
            {
                var min_y = floorSegments[i].floor_base;
                var max_y = min_y + floorSegments[i].floor_height;
                if (i == 0)
                {
                    min.y = min_y;
                    max.y = max_y;
                }
                else
                {
                    min.y = Mathf.Min(min_y, min.y);
                    max.y = Mathf.Max(max_y, max.y);
                }
            }

            return (min + max) / 2;
        }
    }
}
