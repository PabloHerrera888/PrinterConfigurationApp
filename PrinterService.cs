using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Management;
using PdfiumViewer;

public class PrinterService
{
    public void PrintPdf(string pdfPath, string printerIp)
    {
        try
        {
            // Verificar si el archivo PDF existe
            if (!File.Exists(pdfPath))
            {
                Console.WriteLine($"El archivo no existe: {pdfPath}");
                return;
            }

            // Obtener el nombre de la impresora por IP
            string printerName = GetPrinterNameByIp(printerIp);
            if (string.IsNullOrEmpty(printerName))
            {
                Console.WriteLine($"No se encontró una impresora válida en la IP: {printerIp}");
                return;
            }

            Console.WriteLine($"Imprimiendo en la impresora: {printerName}");

            // Cargar el PDF usando PdfiumViewer
            using (var pdfDocument = PdfDocument.Load(pdfPath))
            {
                using (PrintDocument printDoc = new PrintDocument())
                {
                    printDoc.PrinterSettings.PrinterName = printerName;

                    // Configuración de la página tamaño carta
                    printDoc.DefaultPageSettings.PaperSize = new PaperSize("Carta", 850, 1100);
                    printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                    printDoc.PrintPage += (sender, e) =>
                    {
                        // Definir dimensiones exactas en cm
                        const float targetWidthCm = 13f; 
                        const float targetHeightCm = 15f;
                        const float dpi = 600;            
                        const float cmToInch = 2.54f;

                        // Convertir dimensiones de cm a píxeles (para el renderizado)
                        int renderWidthPx = (int)(targetWidthCm / cmToInch * dpi);
                        int renderHeightPx = (int)(targetHeightCm / cmToInch * dpi);

                        // Renderizar la primera página del PDF con dimensiones calculadas
                        using (var pageImage = pdfDocument.Render(0, renderWidthPx, renderHeightPx, PdfRenderFlags.CorrectFromDpi))
                        {
                            // Calcular dimensiones en puntos para imprimir
                            float targetWidthPoints = targetWidthCm * 72 / cmToInch;
                            float targetHeightPoints = targetHeightCm * 72 / cmToInch;

                            // Dibujar la imagen en la esquina superior izquierda
                            e.Graphics.DrawImage(
                                pageImage,
                                new RectangleF(0, 0, targetWidthPoints, targetHeightPoints)
                            );
                        }
                    };

                    // Imprimir el documento
                    printDoc.Print();
                    Console.WriteLine("El PDF se imprimió correctamente.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al imprimir el PDF: {ex.Message}");
        }
    }



    private string GetPrinterNameByIp(string printerIp)
    {
        try
        {
            if (printerIp == "127.0.0.1")
            {
                return "PDFCreator"; //Default
            }

            // Conectar al servicio WMI para buscar impresoras
            string query = "SELECT * FROM Win32_Printer";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject printer in searcher.Get())
                {
                    // Obtener las propiedades relevantes de la impresora
                    string printerName = printer["Name"]?.ToString();
                    string portName = printer["PortName"]?.ToString();

                    // Verificar si el puerto coincide con la IP proporcionada
                    if (!string.IsNullOrEmpty(portName) && portName.Contains(printerIp))
                    {
                        return printerName;
                    }
                }
            }

            Console.WriteLine($"No se encontró una impresora con la IP: {printerIp}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al buscar la impresora por IP: {ex.Message}");
            return null;
        }
    }
}

