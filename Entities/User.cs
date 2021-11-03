using System;
using System.Collections.Generic;

#nullable disable

namespace API.Entities
{
    public partial class User
    {
        public string Username { get; set; }
        public string Passphrase { get; set; }
        public string PassphraseType { get; set; }
        public int FactoryId { get; set; }
        public string UserStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Hash { get; set; }
        public string UserType { get; set; }
    }
}
