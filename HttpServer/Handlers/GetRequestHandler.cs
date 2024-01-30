
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

class GetRequestHandler
{
    public static void HandleLoginGet(HttpListenerContext context)
    // Handle login request
    {
        // Extract username and password from query parameters
        string username = context.Request.QueryString["user"];
        string password = context.Request.QueryString["password"];
        Console.WriteLine($"Received login request: {username} {password}");
        Console.WriteLine("All Users:");
        foreach (var user in Data.GetAllUsers())
        {
            Console.WriteLine($"Username: {user.Username}, Password: {user.Password}");
        }

        bool loginSuccessful = Data.ValidateUser(username, password);

        if (loginSuccessful)
        {
            Console.WriteLine($"User {username} logged in!");
            Controller.SendResponseToClient(context.Response, "true", HttpStatusCode.OK); // Successful operation

        }
        else
        {
            Console.WriteLine($"User {username} failed to log in!");
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // JSON parsing error
        }
    }

    public static void HandleSortOldToNew(HttpListenerContext context)
    // Handle sort-old-to-new request
    {
        try
        {
            Console.WriteLine($"Received sort-old-to-new request");
            List<Job> allJobs = Data.GetAllJobs();
            allJobs.Sort((x, y) => x.Date.CompareTo(y.Date));
            string jobsJson = JsonSerializer.Serialize(allJobs);
            Controller.SendResponseToClient(context.Response, jobsJson, HttpStatusCode.OK); // Successful operation

        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // JSON parsing error
        }
    }

    public static void HandleGetOpenTasks(HttpListenerContext context)
    // Handle get-open-tasks request
    {
        try
        {
            Console.WriteLine($"Received get-open-tasks request");
            List<Job> allJobs = Data.GetAllJobs();
            allJobs = allJobs.FindAll(job => job.Status.Equals("pending") || job.Status.Equals("in progress"));
            string jobsJson = JsonSerializer.Serialize(allJobs);
            Controller.SendResponseToClient(context.Response, jobsJson, HttpStatusCode.OK); // Successful operation
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // JSON parsing error
        }
    }

    public static async Task HandleGetTasks(HttpListenerContext context)
    // Handle get-tasks request
    {
        try
        {
            List<Job> allJobs = Data.GetAllJobs();
            string jobsJson = JsonSerializer.Serialize(allJobs);
            Controller.SendResponseToClient(context.Response, jobsJson, HttpStatusCode.OK); // Successful operation
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // JSON parsing error
        }

    }

    public static async Task HandleUserExist(HttpListenerContext context)
    // Handle user-exist request
    {
        try
        {
            string username = context.Request.QueryString["user"];
            Console.WriteLine($"Received user-exist request: {username}");
            Console.WriteLine("All Users:");
            foreach (var user in Data.GetAllUsers())
            {
                Console.WriteLine($"Username: {user.Username}, Password: {user.Password}");
            }
            if (Data.userExists(username))
            {
                Controller.SendResponseToClient(context.Response, "true", HttpStatusCode.OK); // Successful operation
                return;
            }
            Console.WriteLine($"User {username} does not exist");
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // JSON parsing error
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.InternalServerError); // JSON parsing error
        }
    }

    public static async Task HandleThisWeekTasks(HttpListenerContext context)
    // Handle this-week-tasks request
    {
        try
        {
            Console.WriteLine($"Received this-week-tasks request");
            List<Job> allJobs = Data.GetAllJobs();
            allJobs = allJobs.FindAll(job => job.EndingDate.CompareTo(DateTime.Now) >= 0 && job.EndingDate.CompareTo(DateTime.Now.AddDays(7)) <= 0);
            string jobsJson = JsonSerializer.Serialize(allJobs);
            Controller.SendResponseToClient(context.Response, jobsJson, HttpStatusCode.OK); // Successful operation
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.InternalServerError); // JSON parsing error
        }
    }

    public static async Task HandleClosedTasks(HttpListenerContext context)
    // Handle closed-tasks request
    {
        try
        {
            Console.WriteLine($"Received closed-tasks request");
            List<Job> allJobs = Data.GetAllJobs();
            allJobs = allJobs.FindAll(job => job.Status.Equals("closed") || job.Status.Equals("canceled"));
            string jobsJson = JsonSerializer.Serialize(allJobs);
            Controller.SendResponseToClient(context.Response, jobsJson, HttpStatusCode.OK); // Successful operation
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.InternalServerError); // JSON parsing error
        }
    }
}
