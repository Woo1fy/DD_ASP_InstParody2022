using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class UserController : CommonController
	{
		private readonly UserService userService;
		private readonly AttachService attachService;

		public UserController(UserService userService, AttachService attachService)
		{
			this.userService = userService;
			this.attachService = attachService;
		}

		[HttpPost]
		public async Task CreateUser(CreateUserModel model)
		{
			if (await userService.CheckUserExist(model.Email))
				throw new ArgumentException("user is exist");
			await userService.CreateUser(model);
		}

		[HttpPost]
		[Authorize]
		public async Task AddAvatarToUser(MetadataModel model)
		{
			var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
			if (!tempFi.Exists)
				throw new FileNotFoundException("file not found");
			else
			{
				var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", model.TempId.ToString());
				var destFi = new FileInfo(path);
				if (destFi.Directory != null && !destFi.Directory.Exists)
					destFi.Directory.Create();

				System.IO.File.Copy(tempFi.FullName, path, true);

				await attachService.AddAvatarToUser(base.GetCurrentUser(), model, path);
			}
		}

		[HttpGet]
		public async Task<FileResult> GetUserAvatar(Guid userId)
		{
			var attach = await attachService.GetUserAvatar(userId);

			return File(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType);
		}

		[HttpGet]
		public async Task<FileResult> DownloadAvatar(Guid userId)
		{
			var attach = await attachService.GetUserAvatar(userId);

			HttpContext.Response.ContentType = attach.MimeType;
			FileContentResult result = new(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType)
			{
				FileDownloadName = attach.Name
			};

			return result;
		}

		[HttpGet]
		[Authorize]
		public async Task<List<UserModel>> GetUsers()
			=> await userService.GetUsers();

		[HttpGet]
		[Authorize]
		public new async Task<UserModel> GetCurrentUser()
			=> await userService.GetUser(base.GetCurrentUser());
	}
}