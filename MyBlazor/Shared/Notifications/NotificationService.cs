using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazor.Shared.Notifications
{
	public class NotificationService : INotificationService
	{
		public event Action<NotificationSwimmer> OnNotificationPopupReceived;
		public event Action<NotificationSwimmer> OnNotificationSwimmerReceived;

		public void SendNotificationPopup(string title, string description, NotificationType type = NotificationType.Info)
		{
			// TODO
		}

		public void SendNotificationSwimmer(string title, string description, NotificationType type = NotificationType.Info)
		{
			var notification = new NotificationSwimmer
			{
				Title = title,
				Description = description,
				Type = type
			};

			lock (OnNotificationSwimmerReceived)
			{
				OnNotificationSwimmerReceived?.Invoke(notification);
			}
		}
	}
}
