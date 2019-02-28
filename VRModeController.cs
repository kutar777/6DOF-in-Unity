using System.Linq;
using System.Collections;
using Assets.Scripts.ColorArrangement;
using Assets.Scripts.Core;
using Assets.Scripts.Core.StructData;
using Assets.Scripts.Core.Utils;
using Assets.Scripts.OptionalController;
using Assets.Scripts.SolarSimulation;
using Assets.Scripts.ToolShow;
using Assets.Scripts.Thumbnail;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using DebugWindow;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.VRMode
{
    /// <summary>
    /// VRモード制御
    /// </summary>
    public class VRModeController : SingletonMonoBehaviour<VRModeController>
    {
        [SerializeField]
        private GameObject normalCamera = null;
        [SerializeField]
        private GameObject vrCamera = null;

        [SerializeField]
        private GameObject rightController = null;
        [SerializeField]
        private GameObject leftController = null;
        [SerializeField]
        private GameObject gamepad = null;

        [SerializeField]
        private Button touchPanel = null;

        private static readonly string DeviceNone = "none";
        private static readonly string DeviceCardboard = "cardboard";
        private static readonly string DeviceHTCVive = "OpenVR";
        private static readonly string MockHTCVive = "split";

        private string targetDevice = null;

        ARSessionManager aRSession;
        MeasureHeight measure;

        protected override void Awake()
        {
            base.Awake();
            

            aRSession = FindObjectOfType<ARSessionManager>();
            measure = FindObjectOfType<MeasureHeight>();

            targetDevice = null;
            if (XRSettings.supportedDevices == null || XRSettings.supportedDevices.Length == 0)
            {
                // サポートしているデバイスがない場合は、直ちに復帰する。
                return;
            }

#if UNITY_STANDALONE_WIN
            if (XRSettings.supportedDevices.Any(i => i == DeviceHTCVive))
            {
                targetDevice = DeviceHTCVive;

                vrCamera?.SetActive(false);
                rightController.SetActive(false);
                leftController.SetActive(false);
                gamepad.SetActive(false);

                ActivateSelf(true);
            }

#elif UNITY_IOS
            // iPadはVR機能を有効化しない
            if (!SystemInfo.deviceModel.Contains("iPad"))
            {
                if (XRSettings.supportedDevices.Any(i => i == DeviceCardboard))
                {
                    targetDevice = DeviceCardboard;
                    touchPanel.gameObject.SetActive(false);
                }
            }
#else
            // NOP
#endif
        }

        /// <summary>
        /// 自オブジェクト有効化/無効化
        /// </summary>
        /// <param name="enable"></param>
        private void ActivateSelf(bool enable)
        {
            this.gameObject.GetComponent<Image>().enabled = enable;
            this.gameObject.GetComponent<Button>().enabled = enable;
        }

        /// <summary>
        /// VRモードボタン表示
        /// </summary>
        public void ActivateVRMode()
        {
            //Logging.Debug("VR has been enabled.");

            //// VRデバイスがない場合は直ちに復帰する
            //if (targetDevice == null) return;

            //// VRタッチパネルを無効化する
            //touchPanel.gameObject.SetActive(false);

            //Logging.Debug("XRSettings.isDeviceActive is " + XRSettings.isDeviceActive);

            //// ターゲットのVRデバイスと異なるデバイスがロードされている場合
            //if (string.IsNullOrEmpty(XRSettings.loadedDeviceName) 
            //    || XRSettings.loadedDeviceName != targetDevice)
            //{
            //    // ターゲットのデバイスをロードする
            //    StartCoroutine(LoadVRDevice(targetDevice));
            //}
            //else
            //{
            //    // Activate済みの場合は直ちにVRモードを起動する
            //    Logging.Debug("VR Device has already been activated.");
            //    PlayVRMode();
            //}
            Logging.Debug("VR mode : " + targetDevice.ToString());

            //MeasureHeight.Instance
            if (ARSubsystemManager.systemState > ARSystemState.Installing)
            {
                // ARセッションを起動する（スマホVRのみ）
                ARSubsystemManager.StartSubsystems();
                Logging.Debug("Tracking is now " + ARSubsystemManager.systemState.ToString());
                Logging.Debug("1.XRSettings : " + XRSettings.enabled);

                // カードボード（VR）をロードする
                StartCoroutine(LoadVRDevice(targetDevice));
            }

            ToolShowModeManager.Instance.SetToolShowMode(ToolShowMode.Hide);
            touchPanel.gameObject.SetActive(true);
        }

        /// <summary>
        /// VRデバイスロード
        /// </summary>
        /// <param name="device">ロードするデバイス名</param>
        /// <returns></returns>
        private IEnumerator LoadVRDevice(string device)
        {
            // ロードを8秒間(平面を検知するまでの想定時間)遅らせる, 
            Logging.Debug("Start Wating: " + Time.time);
            yield return new WaitForSeconds(8);
            Logging.Debug("End Wating: " + Time.time);
            
            Logging.Debug("Load VR Device");
            XRSettings.LoadDeviceByName(device);
            yield return null;

            XRSettings.enabled = true;
            Logging.Debug("2.XRSettings : " + XRSettings.enabled);

            Logging.Debug("VR Device : " + XRSettings.loadedDeviceName);
            if (string.IsNullOrEmpty(XRSettings.loadedDeviceName))
            {
                // Activateに失敗したらVRモードを起動しない
                Logging.Error("Load VR Device Failed.");
                Main.Instance.OnVRActivateResult(false);
            }
            else
            {
                // Activateに成功したらVRモードを起動する
                Logging.Debug("Load VR Device succeeded.");
                PlayVRMode();
            }
            yield break;
        }

        /// <summary>
        /// VRモード起動
        /// </summary>
        public void PlayVRMode()
        {
            Logging.Debug("Play VR");
            Logging.Debug("VR Device : " + XRSettings.loadedDeviceName);
            Logging.Debug("3.XRSettings : " + XRSettings.enabled);

            //// 太陽光シミュレーションを一旦OFFにする
      
            //if (targetDevice == DeviceCardboard)
            //{
            //    // スマホVRの場合は、nullになってしまう
            //    SolarOperation.Instance.SwitchOnOff(false);
            //}
            //// カラーコーディネートを一旦OFFにする
            //ColorCoordinate.Instance.SwitchOnOff(false);
            //// カラーチェンジを一旦OFFにする
            //ColorChange.Instance.SwitchOnOff(false);
            //// コントローラを一旦OFFにする
            //JoyStickSwitch.Instance.JoyStickStatusChange(false);
            //// 断面表示を一旦OFFにする
            //DanmenModeManager.Instance.SwitchOnOff(false);

            //if (targetDevice == DeviceHTCVive || targetDevice == MockHTCVive)
            //{
            //    // 通常モードのカメラ位置をVRモードのカメラ位置に反映する
            //    var currentPosition = normalCamera.transform.position;
            //    var vrPosition = new Vector3(currentPosition.x, CurrentSelfPosition.CurrentFloorLevel(currentPosition), currentPosition.z);
            //    vrCamera.transform.position = vrPosition;

            //    // 通常モードカメラからVRモードカメラに切り替える
            //    normalCamera.SetActive(false);
            //    vrCamera.SetActive(true);

            //    // コントローラを有効化する
            //    rightController.SetActive(true);
            //    leftController.SetActive(true);
            //    gamepad.SetActive(true);

            //    // 初期値としてはテレポートを無効にしておく
            //    TeleportController.Instance.Disable();
            //}

            if (targetDevice == DeviceCardboard)
            {
                // VR用スクリーンタッチパネルを有効にする
                touchPanel.gameObject.SetActive(true);

                // デバイスの高さを取得する
                if (measure.originHeight > 0)
                {
                    var camera = Camera.main;
                    camera.transform.parent.position = new Vector3(0f, measure.originHeight, 0f);
                    Logging.Debug("Device Height: " + measure.originHeight);
                }
                else
                    return;
            }


            // VRを有効化する
            XRSettings.enabled = true;

            // 全画面モードにする
            // (XRSettings.enabledの変更とタイミングを合わせる必要があるためここで呼ぶ)
            ToolShowModeButton.Instance.OnClick();

            // VRカメラを水平前方に向ける
            LookAtFront();

            //VRColorChange.Instance.UpdateThumbnailData();
        }

        /// <summary>
        /// VRモード停止
        /// </summary>
        public void StopVRMode()
        {
            Logging.Debug("Stop VR");

            //if (!XRSettings.enabled) return;

            //if (targetDevice == DeviceHTCVive || targetDevice == MockHTCVive)
            //{
            //    // VRモードカメラから通常モードカメラに切り替える
            //    vrCamera.SetActive(false);
            //    normalCamera.SetActive(true);

            //    // コントローラを無効化する
            //    rightController.SetActive(false);
            //    leftController.SetActive(false);
            //    gamepad.SetActive(false);

            //    // テレポートを無効化する
            //    TeleportController.Instance.Disable();
            //}

            //// VRメニュー画面を閉じる
            //VRMenuController.Instance.CloseMenu();
            //// メニューモードをノーマルに変更する
            //ThumbnailComposer.Instance.ResetVRThumbnailMode();

            //if (targetDevice == DeviceCardboard)
            //{
            //    // VR用スクリーンタッチパネルを無効にする
            //    touchPanel.gameObject.SetActive(false);
            //}

            XRSettings.enabled = false;
            Logging.Debug("4.XRSettings : " + XRSettings.enabled);

            //// 太陽光シミュレーションを一旦OFFにする
            //SolarOperation.Instance.SwitchOnOff(false);
            //// カラーコーディネートを一旦OFFにする
            //ColorCoordinate.Instance.SwitchOnOff(false);
            //// カラーチェンジを一旦OFFにする
            //ColorChange.Instance.SwitchOnOff(false);
            //// コントローラをONにする(ただし表示は設定状況に従う)
            //JoyStickSwitch.Instance.JoyStickStatusChange(true);
            
            // 通常モードに戻る
            StartCoroutine(LoadVRDevice(DeviceNone));

            // ARセッションをとめる
            Logging.Debug("Stop ARSession");
            ARSubsystemManager.StopSubsystems();
            Logging.Debug("Tracking is now: " + ARSubsystemManager.systemState.ToString());

            touchPanel.gameObject.SetActive(false);
            ToolShowModeManager.Instance.SetToolShowMode(ToolShowMode.Show);
        }

        /// <summary>
        /// カメラ向き修正
        /// </summary>
        private void LookAtFront()
        {
            var camera = Camera.main;
            // カメラのX軸周りの回転角
            var degree = camera.transform.localEulerAngles.x;
            var radian = MathLib.Deg360To180(degree) * Mathf.Deg2Rad;

            // カメラをX軸周りの回転角分だけ逆向きに回転させ、カメラの視線を水平にする
            camera.transform.RotateAround(camera.transform.position, camera.transform.right, -degree);
        }
    }
}
