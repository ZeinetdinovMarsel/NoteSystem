namespace NoteSystem.Infrastructure;
public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int TokenExpiredHours { get; set; }
}