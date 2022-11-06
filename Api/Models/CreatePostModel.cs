using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class CreatePostModel
    {
        [Required]
        public string Text { get; set; }

        public CreatePostModel(string text)
        {
            Text = text;
        }
    }
}
