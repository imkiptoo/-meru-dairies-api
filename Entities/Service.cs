using System;
using System.Collections.Generic;

#nullable disable

namespace API.Entities
{
    public partial class Service
    {
        public string Name { get; set; }
        public string MainCompanyName { get; set; }
        public string MainCompanySyncEnabled { get; set; }
        public string Hash { get; set; }
    }
}
