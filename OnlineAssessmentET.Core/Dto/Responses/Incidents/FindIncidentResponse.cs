namespace OnlineAssessmentET.Core.Dto.Responses.Incidents;

internal record FindIncidentResponse
{
	public required int Id { get; init; }

	public required Severity Severity { get; init; }

	public required string Title { get; init; }

	public string? Description { get; init; }

	public DateTimeOffset CreatedAt { get; init; }
}
