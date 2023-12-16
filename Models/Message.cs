using System.ComponentModel.DataAnnotations;
namespace WebChat.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string User { get; set; } = null!;
        [Required]
        public string Text { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
