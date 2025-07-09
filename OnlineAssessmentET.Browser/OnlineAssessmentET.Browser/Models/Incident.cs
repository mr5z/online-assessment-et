namespace OnlineAssessmentET.Browser.Models;

internal record Incident
{
	public int Id { get; init; }

	public Severity Severity { get; init; }

	public required string Title { get; init; }

	public string? Description { get; init; }
}
