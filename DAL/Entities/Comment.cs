namespace DAL.Entities
{
	public class Comment
	{
		public Guid Id { get; set; }
		public string Text { get; set; } = null!;
		public DateTimeOffset CreationDate = DateTimeOffset.Now.UtcDateTime;
		public virtual Post? Post { get; set; }
		public virtual User? Author { get; set; }
	}
}