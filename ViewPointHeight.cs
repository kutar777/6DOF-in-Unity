using Assets.Scripts.Core;
using Assets.Scripts.Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// 視点高さを表すクラス
    /// </summary>
    public class ViewPointHeight : MonoBehaviour
    {
        [SerializeField]
        private float fadeDuration = 0.2f;

        private Text text;
        private float fadeStartTime;

        void Awake()
        {
            text = GetComponent<Text>();
            ToolShowModeManager.OnSetToolShowMode += OnSetToolShowMode;
        }

        void OnDestroy()
        {
            ToolShowModeManager.OnSetToolShowMode -= OnSetToolShowMode;
        }

        void Update()
        {
            // 視点高さを取得し表示する
            var information = CurrentSelfPosition.ViewPointHeight(Camera.main.transform.position);
            gameObject.GetComponent<Text>().text = information;

            var isShow = ToolShowModeManager.Instance.Mode == ToolShowMode.Show;
            var rate = Mathf.Min(1f, (Time.realtimeSinceStartup - fadeStartTime) / fadeDuration);
            var alpha = fadeStartTime == 0f ? 0f : isShow ? rate : (1f - rate);
            text.SetColorA(alpha);
        }

        private void OnSetToolShowMode(ToolShowMode mode_new, ToolShowMode mode_old)
        {
            fadeStartTime = Time.realtimeSinceStartup;
        }
    }
}
