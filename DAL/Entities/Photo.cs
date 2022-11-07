namespace DAL.Entities
{
	public class Photo : Attach
	{
		public virtual Post? Post { get; set; }
	}
}