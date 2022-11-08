namespace DAL.Entities
{
	public class Post
	{
		public Guid Id { get; set; }
		public string Header { get; set; } = null!;
		public string Text { get; set; } = null!;
		public virtual User? Author { get; set; }
		public virtual ICollection<Photo>? Photos { get; set; }

		public virtual ICollection<Comment>? Comments { get; set; }
	}
}