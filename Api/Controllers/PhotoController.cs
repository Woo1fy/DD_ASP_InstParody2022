using Api.Models;
using Api.Services;
using Common;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Api.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class PhotoController : CommonController
	{
		private readonly PostService postService;
		private readonly AttachService attachService;

		public PhotoController(PostService postService, AttachService attachService)
		{
			this.postService = postService;
			this.attachService = attachService;
		}

		[HttpPost]
		[Authorize]
		public async Task AddPhotoToPost(Guid postId, MetadataModel model)
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

				await postService.AddPhotoToPost(postId, GetCurrentUser(), model, path);
			}
		}

		[HttpGet]
		public async Task<string> ShowPicture(Guid pictureId, Guid postId)
		{
			var attach = await attachService.GetPictureFromPost(pictureId, postId);

			return null;
		}
	}
}