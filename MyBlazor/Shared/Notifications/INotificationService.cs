
namespace MyBlazor.Shared.Notifications
{
	public interface INotificationService
	{
		public event Action<NotificationSwimmer> OnNotificationPopupReceived;
		public event Action<NotificationSwimmer> OnNotificationSwimmerReceived;

		public void SendNotificationSwimmer(string title, string description, NotificationType type = NotificationType.Info);
		public void SendNotificationPopup(string title, string description, NotificationType type = NotificationType.Info);
	}
}
