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

                var worksheet = workbook.Worksheet(1); // 1 = first sheet
                var range = worksheet.RangeUsed(); // Gets the used range of cells

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
                int number = 0;
                //range.RowCount()
                //try
                //{
                //    number = int.Parse(Console.ReadLine());

                //}
                //catch
                //{
                //    number = range.RowCount();
                //}
                for (int row = 2; row <= range.RowCount(); row++)
                {
                    var cellValueName = worksheet.Cell(row, cellValueNameNum).GetValue<string>();
                    var cellValuePdf = worksheet.Cell(row, cellValuePdfNum).GetValue<string>();
                    var cellValuereportHtml = worksheet.Cell(row, cellValuereportHtmlNum).GetValue<string>();
                    this.downloaders.Add(new Downloader(cellValueName, cellValuePdf, cellValuereportHtml));
                }
            }

            var downloadTasks = downloaders.Select(d => d.Download()).ToList();
            while (!downloadTasks.All(t => t.IsCompleted))
            {
                Console.Clear();
                int numberComplete = 0;
                for (int i = 0; i < downloaders.Count; i++)
                {
                    if (!downloaders[i].IsDownloading)
                    {
                        numberComplete++;
                        continue;
                    }
                }
                Console.WriteLine(numberComplete + "/" + downloaders.Count + " are done!");
                await Task.Delay(1000);

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
                workbook.SaveAs("..\\..\\..\\list_of_Downloads.xlsx");

                int downLoadesCounter = 0;
                for (int row = 2; row <= downloaders.Count+1; row++)
                {

                    if (downloaders[downLoadesCounter].Status)
                    {
                        worksheet.Cell(row, cellValueNameNum).Value = downloaders[downLoadesCounter].Name;
                        worksheet.Cell(row, cellValuePdfdownload).Value = "Yes";
                        if (downloaders[downLoadesCounter].LinkChoice)
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
                        worksheet.Cell(row, cellValueNameNum).Value = downloaders[downLoadesCounter].Name;
                        worksheet.Cell(row, cellValuePdfdownload).Value = "No";
                        worksheet.Cell(row, cellValueLinkUsed).Value = "N/A";
                    }
                    downLoadesCounter++;

                    if (row % 100 == 0)
                    {
                        workbook.Save();
                    }
                }
                workbook.Save();
                Console.Clear();
                Console.WriteLine("Work done");
            }
        }
    }

}
