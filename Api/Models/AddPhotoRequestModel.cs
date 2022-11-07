namespace Api.Models
{
	public class AddPhotoRequestModel
	{
		public MetadataModel Photo { get; set; } = null!;
		public Guid PostId { get; set; }
	}
}