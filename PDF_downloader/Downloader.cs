using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_downloader
{
    internal class Downloader
    {
        private string name;
        private string pdfFurLink;
        private string reportHtmlAddress;
        private bool isDownloading = true;
        private bool status = true;
        private bool linkchoice = true;

        public Downloader(string name, string pdfFurLink, string reportHtmlAddress)
        {
            this.name = name;
            this.pdfFurLink = pdfFurLink;
            this.reportHtmlAddress = reportHtmlAddress;
        }

        public string Name { get => name; set => name = value; }
        public bool Status { get => status; set => status = value; }
        public bool LinkChoice { get => linkchoice; set => linkchoice = value; }
        public bool IsDownloading { get => isDownloading; set => isDownloading = value; }

        public async Task Download()
        {
            string filePath = "C:\\Users\\SPAC-O-2\\Desktop\\TestDownload\\" + Name + ".pdf";
            try
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(this.pdfFurLink);
                response.EnsureSuccessStatusCode();

                if (response.Content.Headers.ContentType?.MediaType == "application/pdf")
                {
                    byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(filePath, pdfBytes);
                }
                else { this.linkchoice = false; }
            }
            catch (Exception ex)
            {
                this.linkchoice = false;
            }

            
            if (!this.linkchoice)
            {
                try
                {
                    using HttpClient client = new HttpClient();
                    using HttpResponseMessage response = await client.GetAsync(this.reportHtmlAddress);
                    response.EnsureSuccessStatusCode();

                    if (response.Content.Headers.ContentType?.MediaType == "application/pdf")
                    {
                        byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                        await File.WriteAllBytesAsync(filePath, pdfBytes);
                    }

                }
                catch (Exception ex)
                {
                    status = false;
                    //Console.WriteLine($"Error downloading PDF: {ex.Message}");
                }
            }
            isDownloading = false;
        }

        private bool IsPdfEmpty(string filePath)
        {
            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] header = new byte[5]; // %PDF- = 5 bytes
                    stream.Read(header, 0, 5);
                    string headerString = System.Text.Encoding.ASCII.GetString(header);

                    if (!headerString.StartsWith("%PDF"))
                    {
                        Console.WriteLine("Invalid PDF header — deleting file.");
                        File.Delete(filePath);
                        return false;
                    }
                }

                Console.WriteLine("File is a valid PDF.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                File.Delete(filePath);
            }
            return true;
        }
    }
}
