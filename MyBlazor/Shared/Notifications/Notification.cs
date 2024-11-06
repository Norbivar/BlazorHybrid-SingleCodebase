using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazor.Shared.Notifications
{
	public enum NotificationType
	{
		Info,
		Error
	}
	public class NotificationSwimmer
	{

		public required string Title { get; set; }
		public required string Description { get; set; }
		public NotificationType Type { get; set; } = NotificationType.Info;

		public NotificationSwimmer ShallowCopy()
		{
			return (NotificationSwimmer)MemberwiseClone();
		}
	}
}
