namespace Domain.Events
{
    public record CourseAssignedEvent(
        Guid StudentId,
        string StudentEmail,
        string MatricNumber,
        List<Guid> CourseIds,
        int CourseCount,
        DateTime AssignedAt);
}
