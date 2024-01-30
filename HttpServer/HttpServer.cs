using System;
using System.Net;
using System.Threading.Tasks;

class HttpServer
{
    public static async Task Start(int port)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add($"http://+:{port}/");
        Console.WriteLine($"Listening on port {port}...");
        listener.Start();

        try
        {
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                await Controller.ProcessRequest(context);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            listener.Stop();
        }
    }
}
