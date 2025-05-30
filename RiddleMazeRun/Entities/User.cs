namespace RiddleMazeRun.Entities;

public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public int CompletedLevels { get; set; }
    public int[,,] CustomLevels { get; set; }
}
