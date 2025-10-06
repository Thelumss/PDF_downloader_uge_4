
namespace PDF_downloader
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Coordinator coordinator = new Coordinator();

            await coordinator.test();
        }
    }
}
