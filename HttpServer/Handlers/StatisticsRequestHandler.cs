
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

class StatisticsRequestHandler
{
    public static async Task HandleBestUsersByPeriod(HttpListenerContext context)
    // Handle best-users-by-period request
    {
        try
        {
            string startDateString = context.Request.QueryString["start-date"];
            string endDateString = context.Request.QueryString["end-date"];
            DateTime startDate = DateTime.Parse(startDateString);
            DateTime endDate = DateTime.Parse(endDateString);

            List<Job> allJobs = Data.GetAllJobs();

            Dictionary<string, int> usersJobs = new Dictionary<string, int>();

            foreach (Job job in allJobs)
            {
                if (job.EndingDate >= startDate && job.EndingDate <= endDate)
                {
                    if (usersJobs.ContainsKey(job.UserInCharge))
                    {
                        usersJobs[job.UserInCharge]++;
                    }
                    else
                    {
                        usersJobs.Add(job.UserInCharge, 1);
                    }
                }
            }

            var sortedUsersJobs = usersJobs.OrderByDescending(x => x.Value).ToList();

            // Extract the usernames into an array based on the sorted order
            string[] sortedUsers = sortedUsersJobs.Select(x => x.Key).ToArray();
            Console.WriteLine($"Sorted users: {string.Join(", ", sortedUsers)}");

            Controller.SendResponseToClient(context.Response, JsonSerializer.Serialize(sortedUsers), HttpStatusCode.OK);
        }
        catch (JsonException)
        {
            Controller.SendResponseToClient(context.Response, "false", HttpStatusCode.BadRequest); // JSON parsing error
        }
    }
}