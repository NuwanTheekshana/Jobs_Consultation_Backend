namespace Jobs_Consultation_Backend.Models
{
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResult
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Permission { get; set; }
        public int Status { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }

    }
}
