namespace Api.Models
{
	public class CommentModel
	{
		public Guid Id { get; set; }

		public string? Text { get; set; }
		public Guid AuthorId { get; set; }
		public Guid PostId { get; set; }

		public CommentModel()
		{
		}

		public CommentModel(Guid id, string text, Guid authorId, Guid postId)
		{
			Id = id;
			Text = text;
			AuthorId = authorId;
			PostId = postId;
		}
	}
}