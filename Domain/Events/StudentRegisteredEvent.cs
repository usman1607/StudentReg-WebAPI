namespace Domain.Events
{
    public record StudentRegisteredEvent(
        Guid StudentId,
        string Email,
        string FirstName,
        string LastName,
        string MatricNumber,
        DateTime RegisteredAt);
}
