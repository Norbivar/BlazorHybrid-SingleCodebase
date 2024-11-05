namespace MyBlazor.Server.Services
{
	using System.Text;
	using System;
	using SessionID = string;

	public class SessionTrackerService : ISessionTrackerService
	{
		private static readonly Random Random = new Random();
		private static readonly string SessionIDChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		private short SessionIDLength = 64;
		private short SessionIDGenerationRetries = 10;
		private Dictionary<SessionID, Session> Sessions = new Dictionary<SessionID, Session>();

		private SessionID GenerateNewSessionID()
		{
			var result = new StringBuilder(SessionIDLength);

			for (int i = 0; i < SessionIDLength; i++)
			{
				int index = Random.Next(SessionIDChars.Length);
				result.Append(SessionIDChars[index]);
			}

			return result.ToString();
		}


		public ResultT<SessionID> CreateNewSession(int ownerId)
		{
			lock(Sessions)
			{
				for (int i = 0; i < SessionIDGenerationRetries; i++)
				{
					var newID = GenerateNewSessionID();
					if (!Sessions.ContainsKey(newID))
					{
						Sessions.Add(newID, new Session {
							Sid = newID,
							OwnerUser = ownerId
						});

						return ResultT<SessionID>.Success(newID);
					}
				}
			}

			return ResultT<SessionID>.Failure(Error.Failure("Failed", "Failed to create new session!"));
		}

		public Session? GetSession(int ownerId)
		{
			return null;
		}
	}
}
