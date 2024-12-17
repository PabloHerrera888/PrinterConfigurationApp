using System;
using PrinterConfigurationApp.Services;
using static System.Net.Mime.MediaTypeNames;

namespace PrinterConfigurationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var printerService = new PrinterService();
            var validationService = new ValidationService();

            var app = new Application(printerService, validationService);

            if (args.Length < 2)
            {
                Console.WriteLine("Uso: PrinterConfigurationApp.exe <ruta_dinamica> <IP_impresora>");
                return;
            }

            // Pasar los argumentos al método Run
            app.Run(args[0], args[1]);
        }
    }
}
