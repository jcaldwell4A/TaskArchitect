namespace TaskArchitect.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public User User { get; set; }
    }
}
