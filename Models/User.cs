using System;
using System.Collections.Generic;

#nullable disable

namespace API.Models
{
    public partial class User
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string OneTimePassword { get; set; }
        public string ResetKey { get; set; }
        public string AccessToken { get; set; }
        public string AccountStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
