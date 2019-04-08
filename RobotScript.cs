using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotScript : MonoBehaviour {

    // ロボットのポーズデータ
    Dictionary<string, Sprite> pose = new Dictionary<string, Sprite>()
    {
        {"Normal", null},
        {"Right", null},
        {"Left", null},
        {"Both", null}
    };
    //　ロボットのモーションデータのリスト
    List<RobotMotion> motions = new List<RobotMotion>();

    // ポーズをランダムでとるか、マニュアルで操作するか決めるパプリック変数
    public bool manualDance = false;

	void Start () {
        // ポーズデータのキーのリストにポーズテクスチャを割り当てる
        foreach(var key in new List<string>(pose.Keys))
        {
            Texture2D tex = (Texture2D)Resources.Load("Robot_" + key);
            pose[key] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

    }

    //　モーションデータをリストに追加する
    public void AddMotion(RobotMotion m)
    {
        motions.Add(m);
    }

    // ロボットのポーズデータを名前で呼び、ポーズを取らせる
    public void Pose(string p)
    {
        GetComponent<SpriteRenderer>().sprite = pose[p];
    }

    void Update()
    {
        //　（ロボット別）モーションデータが保存されている場合
        if (motions.Count > 0)
        {
            bool finished = motions[0].Animate(this, Time.deltaTime);
            if (finished)
            {

                motions.RemoveAt(0);
            }
        }

        // manualDanceがチェックされていると、マウスで操作する
        if (manualDance)
        { 

            if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                Pose("Both");
            }
            else if (Input.GetMouseButton(0))
            {
                Pose("Right");

            }
            else if (Input.GetMouseButton(1))
            {
                Pose("Left");
            }
            else
            {
                Pose("Normal");
            }
        }


    }
}
