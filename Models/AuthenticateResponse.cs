using System.Collections.Generic;

namespace API.Models
{
    public class AuthenticateResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string Token { get; set; }
        public User User { get; set; }
        public List<string> Roles { get; set; }
        
        public AuthenticateResponse(User user, string token)
        {
            Status = 200;
            Message = "Login Successful";
            Id = user.Id;
            User = user;
            Token = token;
        }
    }
}