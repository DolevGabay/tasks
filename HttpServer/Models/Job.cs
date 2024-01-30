public class Job
// Represents a task has "job" in the code because "task" is a reserved keyword in C#
{
    public string JobUuid { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public int Urgency { get; set; }
    public string Status { get; set; }
    public DateTime Date { get; set; }
    public DateTime EndingDate { get; set; }  
    public string UserInCharge { get; set; }  // Username of the user in charge of the job

    public Job(string subject, string description, int urgency, string status, DateTime date, DateTime endingDate, string userInCharge)
    {
        JobUuid = Guid.NewGuid().ToString();
        Subject = subject;
        Description = description;
        Urgency = urgency;
        Status = status;
        Date = date;
        EndingDate = endingDate;
        UserInCharge = userInCharge;
    }

    public void editJob(string subject, string description, int urgency, string status, DateTime date, DateTime endingDate, string userInCharge)
    {
        Subject = subject;
        Description = description;
        Urgency = urgency;
        Status = status;
        Date = date;
        EndingDate = endingDate;
        UserInCharge = userInCharge;
    }
}
