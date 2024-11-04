using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtLibrary
{
    public class JwtOptions
    {
        public const string SectionName = "JwtSettings";

        public string Secret { get; set; } = String.Empty;
        public string Issuer { get; set; } = String.Empty;
        public int TTLSeconds { get; set; } = 3600;
    }
}
