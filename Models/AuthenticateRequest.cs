using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
    
    public class AuthenticateRequestPayload
    {
        [Required]
        public string Payload { get; set; }
    }
}