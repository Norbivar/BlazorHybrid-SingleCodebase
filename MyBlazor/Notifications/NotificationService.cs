using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazor.Notifications
{
	public class NotificationService : INotificationService
	{
		public event Action<NotificationPopup>? OnNotificationPopupReceived;
		public event Action<NotificationSwimmer>? OnNotificationSwimmerReceived;

		public void SendNotificationPopup(NotificationPopup popup)
		{
			if (OnNotificationPopupReceived is not null)
			{
				lock (OnNotificationPopupReceived)
				{
					OnNotificationPopupReceived.Invoke(popup);
				}
			}
		}

		public void SendNotificationPopup(string title, string description, NotificationType type = NotificationType.Info)
		{
			var notification = new NotificationPopup
			{
				Title = title,
				Description = description,
				Type = type
			};

			SendNotificationPopup(notification);
		}

		public void SendNotificationSwimmer(string title, string description, NotificationType type = NotificationType.Info)
		{
			var notification = new NotificationSwimmer
			{
				Title = title,
				Description = description,
				Type = type
			};

			if (OnNotificationSwimmerReceived is not null)
			{
				lock (OnNotificationSwimmerReceived)
				{
					OnNotificationSwimmerReceived.Invoke(notification);
				}
			}
		}

		public void SendNotificationSwimmer(NotificationSwimmer swimmer)
		{
			if (OnNotificationSwimmerReceived is not null)
			{
				lock (OnNotificationSwimmerReceived)
				{
					OnNotificationSwimmerReceived.Invoke(swimmer);
				}
			}
		}
	}
}
