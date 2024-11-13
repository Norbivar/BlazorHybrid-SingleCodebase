namespace MyBlazor.Notifications
{
    public class NotificationPopup : NotificationBase
    {
        public enum PopupButtonID
        {
            Ok = 1,
            Cancel = 2,
        }

        public List<PopupButtonID> Buttons { get; set; } = new List<PopupButtonID> { PopupButtonID.Ok };

        public event Action? OnClickedOK;
        public event Action? OnClickedCancel;

        private TaskCompletionSource<PopupButtonID> _popupCompletionSource;

        public NotificationPopup()
        {
            _popupCompletionSource = new TaskCompletionSource<PopupButtonID>();
        }

        public void ClickOK()
        {
            OnClickedOK?.Invoke();
            _popupCompletionSource?.TrySetResult(PopupButtonID.Ok);
        }
        public void ClickCancel()
        {
            OnClickedCancel?.Invoke();
            _popupCompletionSource?.TrySetResult(PopupButtonID.Cancel);
        }

        public Task<PopupButtonID> WaitForClose()
        {
            return _popupCompletionSource.Task;
        }
    }
}
