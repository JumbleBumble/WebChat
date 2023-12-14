namespace WebChat.Models
{
    public class GroupMessage
    {
        public int Id { get; set; }
        public string User { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
