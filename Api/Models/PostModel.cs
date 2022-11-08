using DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Models
{
	public class PostModel
	{
		public string Header { get; set; }
		public string Text { get; set; }
		public string Author { get; set; }
		public ICollection<PhotoModel> Photos { get; set; }
		public ICollection<CommentModel> Comments { get; set; }

		public PostModel()
		{

		}

		public PostModel(string header, string text, string author, ICollection<PhotoModel> photos, ICollection<CommentModel> comments)
		{
			Header = header;
			Text = text;
			Author = author;
			Photos = photos;
			Comments = comments;
		}
	}
}