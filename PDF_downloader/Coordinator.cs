using ClosedXML.Excel;

namespace PDF_downloader
{
    class Coordinator
    {
        private string excelFilePath = "..\\..\\..\\GRI_2017_2020.xlsx";
        private List<Downloader> downloaders = new List<Downloader>();
        public async Task Coordinating()
        {
            Console.Clear();
            Console.WriteLine("Processing excel file");
            using (var workbook = new XLWorkbook(excelFilePath))
            {

                var worksheet = workbook.Worksheet(1);
                var range = worksheet.RangeUsed();

                int cellValueNameNum = 1;
                int cellValuePdfNum = 1;
                int cellValuereportHtmlNum = 1;

                for (int col = 1; col <= range.ColumnCount(); col++)
                {
                    var cellValue = worksheet.Cell(1, col).GetValue<string>();
                    switch (cellValue)
                    {
                        case ("BRnum"):
                            cellValueNameNum = col;
                            break;
                        case ("Pdf_URL"):
                            cellValuePdfNum = col;
                            break;
                        case ("Report Html Address"):
                            cellValuereportHtmlNum = col;
                            break;
                        default:
                            break;
                    }
                }

                Console.Clear();
                Console.WriteLine("beginning downloads");

                for (int row = 2; row <= range.RowCount(); row++)
                {
                    var cellValueName = worksheet.Cell(row, cellValueNameNum).GetValue<string>();
                    var cellValuePdf = worksheet.Cell(row, cellValuePdfNum).GetValue<string>();
                    var cellValuereportHtml = worksheet.Cell(row, cellValuereportHtmlNum).GetValue<string>();
                    this.downloaders.Add(new Downloader(cellValueName, cellValuePdf, cellValuereportHtml));
                }
            }

            
            const int batchSize = 100;
            for (int i = 0; i < downloaders.Count; i += batchSize)
            {

                var batch = downloaders.Skip(i).Take(batchSize).ToList();
                var tasks = batch.Select(d => d.Download()).ToList();
                await Task.WhenAll(tasks);

                Console.Clear();
                Console.WriteLine($"Completed {Math.Min(i + batchSize, downloaders.Count)}/{downloaders.Count}");
            }


            Console.Clear();
            Console.WriteLine("Processing excel file");
            using (var workbook = new XLWorkbook())
            {
                int cellValueNameNum = 1;
                int cellValuePdfdownload = 2;
                int cellValueLinkUsed = 3;

                var worksheet = workbook.Worksheets.Add("MySheet");

                worksheet.Cell(1, cellValueNameNum).Value = "BRnum";
                worksheet.Cell(1, cellValuePdfdownload).Value = "pdf_downloaded";
                worksheet.Cell(1, cellValueLinkUsed).Value = "Link_Used";

                for (int i = 0; i < downloaders.Count; i++)
                {
                    int row = i + 2;
                    var d = downloaders[i];

                    if (d.Status)
                    {
                        worksheet.Cell(row, cellValueNameNum).Value = d.Name;
                        worksheet.Cell(row, cellValuePdfdownload).Value = "Yes";
                        if (d.LinkChoice)
                        {
                            worksheet.Cell(row, cellValueLinkUsed).Value = "used Pdf_URL";
                        }
                        else
                        {
                            worksheet.Cell(row, cellValueLinkUsed).Value = "used Report Html Address";
                        }
                    }
                    else
                    {
                        worksheet.Cell(row, cellValueNameNum).Value = downloaders[i].Name;
                        worksheet.Cell(row, cellValuePdfdownload).Value = "No";
                        worksheet.Cell(row, cellValueLinkUsed).Value = "N/A";
                    }



                }
                workbook.SaveAs("..\\..\\..\\list_of_Downloads.xlsx");
            }
                Console.Clear();
                Console.WriteLine("Work done");
        }
    }

}
