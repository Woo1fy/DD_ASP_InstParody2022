namespace DAL.Entities
{
	public class User
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = "empty";
		public string Email { get; set; } = "empty";
		public string PasswordHash { get; set; } = "empty";
		public DateTimeOffset BirthDay { get; set; }
		public Guid? AvatarId { get; set; }

		public virtual Avatar? Avatar { get; set; }
		public virtual ICollection<UserSession>? Sessions { get; set; }
		public virtual ICollection<Post>? Posts { get; set; }

		public virtual ICollection<Comment>? Comments { get; set; }
	}
}