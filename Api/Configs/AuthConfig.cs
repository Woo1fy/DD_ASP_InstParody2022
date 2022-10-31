using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Configs
{
    public class AuthConfig
    {
        public const string Position = "auth";
        public string? Issuer { get; set; } = default;
        public string? Audience { get; set; } = default;
		public string? Key { get; set; } = default;
		public int LifeTime { get; set; }
        public SymmetricSecurityKey SymmetricSecurityKey()
            => new(Encoding.UTF8.GetBytes(Key));
    } 
}
