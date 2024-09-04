namespace EventPlanner.Models;

public record NewEvent(string EventName, string Description, /*int OrganizerID, */ String Date, string EventDeadline, bool Status);