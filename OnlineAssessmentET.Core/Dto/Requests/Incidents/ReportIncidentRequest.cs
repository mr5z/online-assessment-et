namespace OnlineAssessmentET.Core.Dto.Requests.Incidents;

internal record ReportIncidentRequest
{
	public required Severity Severity { get; init; }

	public required string Title { get; init; }

	public string? Description { get; init; }
}
