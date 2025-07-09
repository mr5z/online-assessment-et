using OnlineAssessmentET.Core.Dto.Requests.Incidents;
using OnlineAssessmentET.Core.Dto.Responses.Incidents;
using OnlineAssessmentET.Core.Patterns;
using IResult = OnlineAssessmentET.Core.Patterns.IResult;

namespace OnlineAssessmentET.Services;

internal interface IIncidentService
{
	Task<IResult> ReportIncident(ReportIncidentRequest request, CancellationToken cancellation = default);

	Task<Result<FindIncidentResponse[]>> FindIncidents(FindIncidentRequest request, CancellationToken cancellation = default);
}