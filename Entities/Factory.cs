using System;
using System.Collections.Generic;

#nullable disable

namespace API.Entities
{
    public partial class Factory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Hash { get; set; }
    }
}
