namespace PDF_downloader
{
    internal class Downloader
    {
        public string Name { get; set; }
        public string PdfUrl { get; set; }
        public string ReportHtmlUrl { get; set; }
        public bool IsDownloading { get; private set; }
        public bool Status { get; private set; }
        public bool LinkChoice { get; private set; }

        public Downloader(string name, string pdfUrl, string reportHtmlUrl,string filePath)
        {
            Name = name;
            PdfUrl = pdfUrl;
            ReportHtmlUrl = reportHtmlUrl;
            IsDownloading = false;
            Status = false;
            LinkChoice = false;
        }

        public async Task Download()
        {
            IsDownloading = true;
            string filePath = $"C:\\Users\\SPAC-O-2\\Desktop\\TestDownload\\{Name}.pdf";

            try
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(PdfUrl);
                response.EnsureSuccessStatusCode();

                if (response.Content.Headers.ContentType?.MediaType == "application/pdf")
                {
                    byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(filePath, pdfBytes);

                    Status = true;
                    LinkChoice = true;
                    IsDownloading = false;
                    return;   
                }
            }
            catch
            {
                
            }

            
            try
            {
                if (!string.IsNullOrWhiteSpace(ReportHtmlUrl))
                {
                    using HttpClient client = new HttpClient();
                    using HttpResponseMessage response = await client.GetAsync(ReportHtmlUrl);
                    response.EnsureSuccessStatusCode();

                    if (response.Content.Headers.ContentType?.MediaType == "application/pdf")
                    {
                        byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                        await File.WriteAllBytesAsync(filePath, pdfBytes);

                        Status = true;
                        LinkChoice = false;
                    }
                    else
                    {
                        Status = false;
                    }
                }
                else
                {
                    Status = false;
                }
            }
            catch
            {
                Status = false;
            }
            finally
            {
                IsDownloading = false;
            }
        }
    }
}
