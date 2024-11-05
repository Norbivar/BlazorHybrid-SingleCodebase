namespace MyBlazor.Server.Services
{
	public interface ISessionTrackerService
	{
		public ResultT<string> CreateNewSession(int ownerId);
		public Session? GetSession(int ownerId);
	}
}
