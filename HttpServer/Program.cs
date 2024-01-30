using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        await HttpServer.Start(8080);
    }
}
