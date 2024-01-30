public class User
{
    public string Username { get; set; } // unique
    public string Password { get; set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
