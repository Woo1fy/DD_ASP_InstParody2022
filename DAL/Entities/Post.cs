namespace DAL.Entities
{
	public class Post
	{
		public Guid Id { get; set; }
		public string Text { get; set; } = null!;
		private DateTimeOffset CreationDate = DateTimeOffset.Now.UtcDateTime;
		public virtual User? Author { get; set; }
		public virtual ICollection<Photo>? Photo { get; set; }

		public virtual ICollection<Comment>? Comment { get; set; }
	}
}