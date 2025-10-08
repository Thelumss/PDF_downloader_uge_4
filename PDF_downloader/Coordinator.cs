using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PDF_downloader
{
    class Coordinator
    {
        private string exelFilePath = "..\\..\\..\\GRI_2017_2020.xlsx";
        private List<Downloader> downloaders = new List<Downloader>();
        public async Task Coordinating()
        {
            Console.Clear();
            Console.WriteLine("Prosesing exel file");
            using (var workbook = new XLWorkbook(exelFilePath))
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
                Console.WriteLine("begining downloads");
                //range.RowCount()
                for (int row = 2; row <= 30; row++)
                {
                    var cellValueName = worksheet.Cell(row, cellValueNameNum).GetValue<string>();
                    var cellValuePdf = worksheet.Cell(row, cellValuePdfNum).GetValue<string>();
                    var cellValuereportHtml = worksheet.Cell(row, cellValuereportHtmlNum).GetValue<string>();
                    this.downloaders.Add(new Downloader(cellValueName, cellValuePdf, cellValuereportHtml));
                }
            }

            var downloadTasks = downloaders.Select(d => d.download()).ToList();
            while (!downloadTasks.All(t => t.IsCompleted))
            {
                Console.Clear();
                int numberComplet = 0;
                for (int i = 0; i < downloaders.Count; i++)
                {
                    if (!downloaders[i].IsDownloading)
                    {
                        numberComplet++;
                        continue;
                    }
                }
                Console.WriteLine(numberComplet + "/" + downloaders.Count + " are done!");
                await Task.Delay(1000);

            }
            Console.Clear();
            for (int i = 0; i < downloaders.Count; i++)
            {
                if (downloaders[i].IsDownloading)
                {
                    Console.WriteLine(downloaders[i].Name + " is still downloading!");
                    continue;
                }
                if (!downloaders[i].Status)
                {
                    Console.WriteLine(downloaders[i].Name + " PDF did not download!");
                    continue;
                }

                if (downloaders[i].Linkchoice)
                {
                    Console.WriteLine(downloaders[i].Name + " PDF downloaded successfully used Pdf_URL");
                }
                else
                {
                    Console.WriteLine(downloaders[i].Name + " PDF downloaded successfully used Report Html Address");
                }
            }
            exelFilePath = "..\\..\\..\\Metadata2006_2016.xlsx";
            using (var workbook = new XLWorkbook(exelFilePath))
            {
                int cellValueNameNum = 1;
                int cellValuePdfdownload = 1;

                var worksheet = workbook.Worksheet(1);
                var range = worksheet.RangeUsed();

                for (int col = 1; col <= range.ColumnCount(); col++)
                {
                    var cellValue = worksheet.Cell(1, col).GetValue<string>();
                    switch (cellValue)
                    {
                        case ("BRnum"):
                            cellValueNameNum = col;
                            break;
                        case ("pdf_downloaded"):
                            cellValuePdfdownload = col;
                            break;
                        default:
                            break;
                    }
                }

                int downLoadescounter = 0;
                for (int row = range.RowCount() + 1; row <= range.RowCount() + downloaders.Count; row++)
                {

                    if (downloaders[downLoadescounter].Status)
                    {
                        worksheet.Cell(row, cellValueNameNum).Value = downloaders[downLoadescounter].Name;
                        worksheet.Cell(row, cellValuePdfdownload).Value = "Yes";
                    }
                    else
                    {
                        worksheet.Cell(row, cellValueNameNum).Value = downloaders[downLoadescounter].Name;
                        worksheet.Cell(row, cellValuePdfdownload).Value = "No";
                    }
                    downLoadescounter++;
                }
                workbook.Save();
            }
        }
    }

}
