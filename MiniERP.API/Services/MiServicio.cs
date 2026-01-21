using Microsoft.Extensions.Options;
using MiniERP.Core.Settings;

namespace MiniERP.API.Services
{
    public class MiServicio
    {
        private readonly JwtSettings _jwtSettings;

        public MiServicio(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        // Ejemplo de método que usa JwtSettings
        public string ObtenerIssuer()
        {
            return _jwtSettings.Issuer;
        }
    }
}
