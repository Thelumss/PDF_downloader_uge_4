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
        private readonly string exelFilePath = "C:\\Users\\SPAC-O-2\\Desktop\\Git\\PDF_downloader\\PDF_downloader\\GRI_2017_2020.xlsx";
        private List<Downloader> downloaders = new List<Downloader>();
        public async Task Coordinating()
        {
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
                
                //range.RowCount()
                for (int row = 2; row <= 10; row++)
                {
                    var cellValueName = worksheet.Cell(row, cellValueNameNum).GetValue<string>();
                    var cellValuePdf = worksheet.Cell(row, cellValuePdfNum).GetValue<string>();
                    var cellValuereportHtml = worksheet.Cell(row, cellValuereportHtmlNum).GetValue<string>();
                    downloaders.Add(new Downloader(cellValueName,cellValuePdf, cellValuereportHtml));
                }

                var downloadTasks = downloaders.Select(d => d.download()).ToList();
                await Task.WhenAll(downloadTasks);

                for (int i = 0; i < downloadTasks.Count; i++) 
                {

                }

                //name 1
                //pdf 38
                //report html 39
            }
        }
    }
}
