namespace MyBlazor.Shared.Notifications
{
    public class NotificationSwimmer : NotificationBase
    {
        public bool Display { get; set; } = true;
        public bool HoveredOver { get; set; } = false;
        public bool ManuallyClosed { get; set; } = false;
        public TimeSpan ShowFor { get; set; } = TimeSpan.FromSeconds(10);

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

        public Timer? ShouldDisappearHandler { get; set; }
    }
}
