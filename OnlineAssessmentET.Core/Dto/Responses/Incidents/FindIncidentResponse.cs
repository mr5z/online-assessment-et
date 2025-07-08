namespace OnlineAssessmentET.Core.Dto.Responses.Incidents;

internal class FindIncidentResponse
{
	public required Severity Severity { get; init; }

	public required string Title { get; init; }

	public string? Description { get; init; }

	public DateTimeOffset CreatedAt { get; init; }
}
