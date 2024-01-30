using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

class PostRequestHandler
{
    public static async Task HandleSignup(HttpListenerContext context)
    // Handle signup request
    {
        try
        {
            using (Stream body = context.Request.InputStream)
            using (StreamReader reader = new StreamReader(body, Encoding.UTF8))
            {
                string jsonData = await reader.ReadToEndAsync();
                Console.WriteLine($"Received signup request: {jsonData}");
                JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
                JsonElement root = jsonDocument.RootElement;

                string username = root.GetProperty("user").GetString();
                string password = root.GetProperty("password").GetString();

                if (Data.userExists(username))
                {
                    Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.Conflict); // User already exists
                    return;
                }

                User newUser = new User(username, password);
                Console.WriteLine($"Username: {newUser.Username}, Password: {newUser.Password}");

                Data.AddUser(newUser);
                Controller.SendResponseToClient(context.Response, "true", HttpStatusCode.Created); // Signup successful
            }
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.InternalServerError); // JSON parsing error
        }
    }


    public static async Task HandleAddTask(HttpListenerContext context)
    // Handle add-task request
    {
        try
        {
            using (Stream body = context.Request.InputStream)
            using (StreamReader reader = new StreamReader(body, Encoding.UTF8))
            {
                string jsonData = await reader.ReadToEndAsync();
                Console.WriteLine($"Received add-task request: {jsonData}");
                JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
                JsonElement root = jsonDocument.RootElement;

                string urgency = root.GetProperty("urgency").GetString();
                int urgencyInt = int.Parse(urgency);
                Job newJob = new Job(
                    root.GetProperty("subject").GetString(),
                    root.GetProperty("description").GetString(),
                    urgencyInt,
                    root.GetProperty("status").GetString(),
                    root.GetProperty("date").GetDateTime(),
                    root.GetProperty("endingDate").GetDateTime(),
                    root.GetProperty("userInCharge").GetString()
                );

                Console.WriteLine(newJob);
                Data.AddJob(newJob);
                List<Job> allJobs = Data.GetAllJobs();
                string jobsJson = JsonSerializer.Serialize(allJobs);
                Controller.SendResponseToClient(context.Response, jobsJson, HttpStatusCode.Created); // Successful operation
            }
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // Bad request or JSON parsing error
        }
    }

    public static async Task HandleEditTask(HttpListenerContext context)
    // Handle edit-task request
    {
        try
        {
            using (Stream body = context.Request.InputStream)
            using (StreamReader reader = new StreamReader(body, Encoding.UTF8))
            {
                string jsonData = await reader.ReadToEndAsync();
                Console.WriteLine($"Received edit-task request: {jsonData}");
                JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
                JsonElement root = jsonDocument.RootElement;

                string editUuid = root.GetProperty("uuid").GetString();

                if (root.TryGetProperty("urgency", out JsonElement urgencyElement))
                {
                    int urgencyInt;
                    if (urgencyElement.ValueKind == JsonValueKind.String)
                    {
                        // Attempt to parse the string value to int
                        if (int.TryParse(urgencyElement.GetString(), out urgencyInt))
                        {
                            // Conversion successful
                        }
                        else
                        {
                            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // Invalid urgency format
                            return;
                        }
                    }
                    else if (urgencyElement.ValueKind == JsonValueKind.Number)
                    {
                        urgencyInt = urgencyElement.GetInt32();
                    }
                    else
                    {
                        // Handle unexpected urgency format
                        Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // Invalid urgency format
                        return;
                    }

                    Job jobToEdit = Data.GetJob(editUuid);
                    jobToEdit.editJob(
                        root.GetProperty("subject").GetString(),
                        root.GetProperty("description").GetString(),
                        urgencyInt,
                        root.GetProperty("status").GetString(),
                        root.GetProperty("date").GetDateTime(),
                        root.GetProperty("endingDate").GetDateTime(),
                        root.GetProperty("userInCharge").GetString()
                    );

                    List<Job> allJobs = Data.GetAllJobs();
                    string jobsJson = JsonSerializer.Serialize(allJobs);
                    Console.WriteLine(jobsJson);
                    Controller.SendResponseToClient(context.Response, jobsJson, HttpStatusCode.OK); // Successful operation
                }
                else
                {
                    Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest);
                }
            }
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // JSON parsing error
        }
    }

    public static async Task HandleRemoveTask(HttpListenerContext context)
    // Handle remove-task request
    {
        try
        {
            using (Stream body = context.Request.InputStream)
            using (StreamReader reader = new StreamReader(body, Encoding.UTF8))
            {
                string jsonData = await reader.ReadToEndAsync();
                Console.WriteLine($"Received remove-task request: {jsonData}");

                JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
                JsonElement root = jsonDocument.RootElement;

                string id = root.GetProperty("uuid").GetString();
                Data.RemoveJob(id);

                List<Job> allJobs = Data.GetAllJobs();
                string jobsJson = JsonSerializer.Serialize(allJobs);
                Controller.SendResponseToClient(context.Response, jobsJson, HttpStatusCode.OK); // Successful operation
            }
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // JSON parsing error
        }
    }

    public static async Task HandleDeleteUser(HttpListenerContext context)
    // Handle delete-user request
    {
        try
        {
            using (Stream body = context.Request.InputStream)
            using (StreamReader reader = new StreamReader(body, Encoding.UTF8))
            {
                string jsonData = await reader.ReadToEndAsync();
                Console.WriteLine($"Received delete-user request: {jsonData}");

                JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
                JsonElement root = jsonDocument.RootElement;

                string username = root.GetProperty("username").GetString();

                if (Data.userExists(username))
                {
                    
                    if (Data.deleteUser(username))
                    {
                        Controller.SendResponseToClient(context.Response, "true", HttpStatusCode.OK); // Successful operation
                    }
                    else
                    {
                        Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); 
                    }
                }
                else
                {
                    Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.NotFound); // User not found
                }
            }
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // JSON parsing error
        }
    }
}