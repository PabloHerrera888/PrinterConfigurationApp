using System.IO;
using System.Net;

namespace PrinterConfigurationApp.Services
{
    public class ValidationService
    {
        // Valida si la ruta del archivo es válida
        public bool ValidatePath(string filePath)
        {
            return !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath);
        }

        // Valida si la dirección IP es válida
        public bool ValidateIp(string ipAddress)
        {
            return IPAddress.TryParse(ipAddress, out _);
        }
    }
}
