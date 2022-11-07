using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
	public class CreateCommentModel
	{
		[Required]
		public string Text { get; set; }

		[Required]
		public Guid PostId { get; set; }

		public CreateCommentModel(string text, Guid postId)
		{
			Text = text;
			PostId = postId;
		}
	}
}