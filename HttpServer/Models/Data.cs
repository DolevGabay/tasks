using System.Collections.Generic;

public class Data
// This class is used to store the data in memory
{
    private static List<User> userList = new List<User>();
    private static List<Job> jobList = new List<Job>();

    public static void AddUser(User user)
    {
        userList.Add(user);
        Console.Write("User added!");
    }

    public static List<User> GetAllUsers()
    {
        return userList;
    }

    public static bool ValidateUser(string username, string password)
    {
        foreach (User user in userList)
        {
            if (user.Username == username && user.Password == password)
            {
                return true;
            }
        }
        return false;
    }

    public static bool userExists(string username)
    {
        foreach (User user in userList)
        {
            if (user.Username == username)
            {
                return true;
            }
        }
        return false;
    }

    public static bool deleteUser(string username)
    {
        foreach (User user in userList)
        {
            if (user.Username == username)
            {
                userList.Remove(user);
                return true;
            }
        }
        return false;
    }

    public static void AddJob(Job job)
    {
        Console.WriteLine($"Job added: {job.Subject}");
        jobList.Add(job);
        Console.Write("Job added!");
    }

    public static List<Job> GetAllJobs()
    {
        return jobList;
    }

    public static bool RemoveJob(string jobUuid)
    {
        foreach (Job job in jobList)
        {
            if (job.JobUuid == jobUuid)
            {
                jobList.Remove(job);
                Console.WriteLine($"Job removed: {job.Subject}");
                return true;
            }
        }
        return false;
    }

    public static Job GetJob(string jobUuid)
    {
        foreach (Job job in jobList)
        {
            if (job.JobUuid == jobUuid)
            {
                return job;
            }
        }
        return null;
    }
}
