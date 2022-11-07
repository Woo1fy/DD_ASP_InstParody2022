using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
	public class SessionService : IDisposable
	{
		private readonly DAL.DataContext context;

		public SessionService(DataContext context)
		{
			this.context = context;
		}

		public async Task<UserSession> GetSessionById(Guid id)
		{
			var session = await context.UserSessions.FirstOrDefaultAsync(x => x.Id == id);
			if (session == null)
			{
				throw new Exception("session is not found");
			}
			return session;
		}

		public async Task<UserSession> GetSessionByRefreshToken(Guid id)
		{
			var session = await context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id);
			if (session == null)
			{
				throw new Exception("session is not found");
			}
			return session;
		}

		#region IDisposable Methods

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			context.Dispose();
		}

		#endregion IDisposable Methods
	}
}