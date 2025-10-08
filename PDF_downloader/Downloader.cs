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
        public bool Linkchoice { get => linkchoice; set => linkchoice = value; }
        public bool IsDownloading { get => isDownloading; set => isDownloading = value; }

        public async Task download()
        {
            string filePath = "C:\\Users\\SPAC-O-2\\Desktop\\TestDowload\\" + Name + ".pdf";
            try
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(this.pdfFurLink);
                response.EnsureSuccessStatusCode();

                byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(filePath, pdfBytes);
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

                    byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(filePath, pdfBytes);
                }
                catch (Exception ex)
                {
                    status = false;
                    //Console.WriteLine($"Error downloading PDF: {ex.Message}");
                }
            }
            isDownloading = false;
        }
    }
}
