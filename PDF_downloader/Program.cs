
namespace PDF_downloader
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("give the abslue path for the excel file with the links");
            string excelpath = Console.ReadLine();
            Console.WriteLine("give the abslue path for where the pdf should end up");
            string pdfdumpfile = Console.ReadLine();
            Coordinator coordinator = new Coordinator(excelpath, pdfdumpfile);

            await coordinator.Coordinating();
        }
    }
}
