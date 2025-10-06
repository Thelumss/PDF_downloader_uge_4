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
        private string pDFURlLink;
        private string reportHtmlAddress;
        private bool status = true;
        private bool linkchoice = true;

        public Downloader(string name, string pDFURlLink, string reportHtmlAddress)
        {
            this.Name = name;
            this.pDFURlLink = pDFURlLink;
            this.reportHtmlAddress = reportHtmlAddress;
        }

        public string Name { get => name; set => name = value; }
        public bool Status { get => status; set => status = value; }
        public bool Linkchoice { get => linkchoice; set => linkchoice = value; }
        public async Task download()
        {
            string filePath = "C:\\Users\\SPAC-O-2\\Desktop\\" + Name + ".pdf";
            bool firstTry = true;
            try
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(this.pDFURlLink);
                response.EnsureSuccessStatusCode();

                byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(filePath, pdfBytes);
            }
            catch (Exception ex)
            {
                firstTry = false;
                this.linkchoice = false;
            }

            if (!firstTry)
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
        }
    }
}
