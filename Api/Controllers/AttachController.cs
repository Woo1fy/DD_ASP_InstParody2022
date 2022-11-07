﻿using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class AttachController : CommonController
	{
		[HttpPost]
		public static async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
		{
			var res = new List<MetadataModel>();
			foreach (var file in files)
			{
				res.Add(await UploadFile(file));
			}
			return res;
		}

		private static async Task<MetadataModel> UploadFile(IFormFile file)
		{
			var tempPath = Path.GetTempPath();
			var meta = new MetadataModel
			{
				TempId = Guid.NewGuid(),
				Name = file.FileName,
				MimeType = file.ContentType,
				Size = file.Length,
			};

			var newPath = Path.Combine(tempPath, meta.TempId.ToString());

			var fileinfo = new FileInfo(newPath);
			if (fileinfo.Exists)
			{
				throw new ArgumentException("file exist");
			}
			else
			{
				if (fileinfo.Directory == null)
				{
					throw new ArgumentNullException(nameof(file), "temp is null");
				}
				else if (!fileinfo.Directory.Exists)
				{
					fileinfo.Directory?.Create();
				}

				using (var stream = System.IO.File.Create(newPath))
				{
					await file.CopyToAsync(stream);
				}

				return meta;
			}
		}
	}
}