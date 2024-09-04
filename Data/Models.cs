namespace EventPlanner.Models
{
    public record Event(int? EventID, string EventName, string Description, /*int OrganizerID, */ String Date, string EventDeadline, bool Status);
}