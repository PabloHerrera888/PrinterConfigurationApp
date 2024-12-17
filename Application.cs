using PrinterConfigurationApp.Services;
using System;

public class Application
{
    private readonly PrinterService _printerService;
    private readonly ValidationService _validationService;

    public Application(PrinterService printerService, ValidationService validationService)
    {
        _printerService = printerService;
        _validationService = validationService;
    }

    public void Run(string filePath, string printerIp)
    {
        // Validar la ruta del archivo
        if (!_validationService.ValidatePath(filePath))
        {
            Console.WriteLine("La ruta especificada no es válida.");
            return;
        }

        // Validar la dirección IP
        if (!_validationService.ValidateIp(printerIp))
        {
            Console.WriteLine("La dirección IP especificada no es válida.");
            return;
        }

        // Imprimir el archivo PDF
        Console.WriteLine($"Imprimiendo el archivo PDF: {filePath}");
        _printerService.PrintPdf(filePath, printerIp);
    }
}
