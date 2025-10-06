using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PDF_downloader
{
    class Coordinator
    {
        public async Task test()
        {
            string url = "http://cdn12.a1.net/m/resources/media/pdf/A1-Umwelterkl-rung-2016-2017.pdf"; // Replace with your actual PDF URL
            string filePath = "sample.pdf";

            try
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                Console.WriteLine("PDF downloaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading PDF: {ex.Message}");
            }
        }
    }
}
