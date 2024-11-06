
namespace MyBlazor.SharedUI.Pages.Models
{
	public class NotificationModel : Object
	{
		public required string Title { get; set; }
		public required string Description { get; set; }

		public bool Display { get; set; } = true;

		public System.Threading.Timer ShouldDisappearHandler { get; set; }
	}
}
