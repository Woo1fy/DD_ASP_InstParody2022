namespace Api.Models
{
	public class PostModel
	{
		public Guid Id { get; set; }

		public string? Text { get; set; }
		public Guid AuthorId { get; set; }

		public PostModel()
		{
		}

		public PostModel(Guid id, string text, Guid authorId)
		{
			Id = id;
			Text = text;
			AuthorId = authorId;
		}
	}
}