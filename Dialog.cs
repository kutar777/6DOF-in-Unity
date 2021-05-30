using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class Dialog : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Button okButton;
    [SerializeField] Button cancelButton;

    [SerializeField] Result.Status myStatus = Result.Status.None;

    void Awake()
    {
        // DoAsync().Forget();
        okButton.onClick.AddListener(() => myStatus = Result.Status.Ok);
        cancelButton.onClick.AddListener(() => myStatus = Result.Status.Cancel);
    }

    private async UniTask DoAsync()
    {
        var token = this.GetCancellationTokenOnDestroy();
        var onOk = okButton.GetAsyncClickEventHandler(token);
        var onCancel = cancelButton.GetAsyncClickEventHandler(token);

        // while(true)
        // {
        await UniTask.WhenAny(onOk.OnClickAsync(), onCancel.OnClickAsync());

            // await onClick.OnClickAsync();
        Debug.Log($"Result State: {myStatus}");
        Hide();
        // }
    }


    public async UniTask<Result> Open()
    {
        Show();

        var token = this.GetCancellationTokenOnDestroy();
        var onOk = okButton.GetAsyncClickEventHandler(token);
        var onCancel = cancelButton.GetAsyncClickEventHandler(token);

        // while(true)
        // {
        await UniTask.WhenAny(onOk.OnClickAsync(), onCancel.OnClickAsync());

            // await onClick.OnClickAsync();
        Hide();

        return new Result { status = myStatus, message = inputField.text };
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public struct Result
    {
        public enum Status
        {
            None,
            Ok,
            Cancel,
        }
        public Status status;
        public string message;
    }
}
