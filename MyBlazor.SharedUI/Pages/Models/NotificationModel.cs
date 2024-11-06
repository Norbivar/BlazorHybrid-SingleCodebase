
using MyBlazor.Shared.Notifications;

namespace MyBlazor.SharedUI.Pages.Models
{
	public class NotificationSwimmerModel : NotificationSwimmer
	{
		public static string NotificationTypeToIcon(NotificationType type)
		{
			switch (type)
			{
				case NotificationType.Info: return "oi-info";
				case NotificationType.Error: return "oi-warning";
				default: return "";
			}
		}

		public bool Display { get; set; } = true;
		public bool HoveredOver { get; set; } = false;
		public bool ManuallyClosed { get; set; } = false;

		public string Icon { get; set; } = "oi-info";

		public void EarlyDisappear()
		{
			if (ShouldDisappearHandler != null)
			{
				ShouldDisappearHandler.Change(TimeSpan.FromMilliseconds(1), Timeout.InfiniteTimeSpan);
			}
			else
			{
				Display = false;
			}
		}

		public System.Threading.Timer ShouldDisappearHandler { get; set; }
	}
}
