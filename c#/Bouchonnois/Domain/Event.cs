namespace Bouchonnois.Domain;

public record Event(DateTime Date, string Message)
{
    public override string ToString()
    {
        return $"{Date:HH:mm} - {Message}";
    }
}