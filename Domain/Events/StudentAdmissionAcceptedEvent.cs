namespace Domain.Events
{
    public record StudentAdmissionAcceptedEvent(
        Guid StudentId,
        string Email,
        string FirstName,
        string LastName,
        string MatricNumber,
        DateTime AcceptedAt);
}
