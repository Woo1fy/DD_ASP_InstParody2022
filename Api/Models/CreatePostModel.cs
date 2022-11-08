using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
	public class CreatePostModel
	{
		public string Header { get; set; }
		public string Text { get; set; }
		
		public CreatePostModel(string header, string text)
		{
			Header = header;
			Text = text;
		}
	}
}