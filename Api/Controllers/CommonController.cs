using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	public abstract class CommonController : ControllerBase
	{
		protected Guid GetCurrentUser()
		{
			var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
				throw new ArgumentException("You are not authorized!");
			return userId;
		}
	}
}