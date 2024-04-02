using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic; 

class Controller
{
    private static readonly Dictionary<string, Func<HttpListenerContext, Task>> RequestMappings = new Dictionary<string, Func<HttpListenerContext, Task>>(StringComparer.OrdinalIgnoreCase)
    {
        // POST Handlers
        {"POST:/signup", PostRequestHandler.HandleSignup},
        {"POST:/add-task", PostRequestHandler.HandleAddTask},
        {"POST:/remove-task", PostRequestHandler.HandleRemoveTask},
        {"POST:/edit-task", PostRequestHandler.HandleEditTask},
        {"POST:/delete-user", PostRequestHandler.HandleDeleteUser},

        // GET Handlers
        {"GET:/login", context => Task.Run(() => GetRequestHandler.HandleLoginGet(context))},
        {"GET:/sort-old-to-new", context => Task.Run(() => GetRequestHandler.HandleSortOldToNew(context))},
        {"GET:/open-tasks", context => Task.Run(() => GetRequestHandler.HandleGetOpenTasks(context))},
        {"GET:/user-exist", context => Task.Run(() => GetRequestHandler.HandleUserExist(context))},
        {"GET:/this-week-tasks", context => Task.Run(() => GetRequestHandler.HandleThisWeekTasks(context))},
        {"GET:/get-tasks", context => Task.Run(() => GetRequestHandler.HandleGetTasks(context))},
        {"GET:/closed-tasks", context => Task.Run(() => GetRequestHandler.HandleClosedTasks(context))},
        {"GET:/statistics-get-best-users-by-period", context => Task.Run(() => StatisticsRequestHandler.HandleBestUsersByPeriod(context))},
    };

    public static async Task ProcessRequest(HttpListenerContext context)
    {
        if (context.Request.HttpMethod == "OPTIONS")
        {
            SetupCORS(context);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.Close();
            return;
        }

        SetupCORS(context);

        string method = context.Request.HttpMethod;
        string path = context.Request.Url.AbsolutePath;
        string key = $"{method}:{path}";

        if (RequestMappings.TryGetValue(key, out var handler))
        {
            await handler(context);
        }
        else
        {
            SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest);
        }
    }

    private static void SetupCORS(HttpListenerContext context)
    // Setup CORS headers
    {
        context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
        context.Response.AppendHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
        context.Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type");
    }

    public static async Task SendResponseToClient(HttpListenerResponse response, string message, HttpStatusCode statusCode)
    {
        response.ContentType = "text/plain";
        response.StatusCode = (int)statusCode;

        Stream output = response.OutputStream;
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        await output.WriteAsync(buffer, 0, buffer.Length);
        output.Close();
    }
}
