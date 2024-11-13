using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazor.Notifications
{
	public enum NotificationType
	{
		Info,
		Error
	}
	public class NotificationBase
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

		public string Icon { get; set; } = "oi-info";
		public required string Title { get; set; }
		public required string Description { get; set; }
		public NotificationType Type { get; set; } = NotificationType.Info;
	}
}
