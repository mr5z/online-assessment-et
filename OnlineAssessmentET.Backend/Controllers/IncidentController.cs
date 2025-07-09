using Microsoft.AspNetCore.Mvc;
using OnlineAssessmentET.Core.Dto.Requests.Incidents;
using OnlineAssessmentET.Core.Patterns;
using OnlineAssessmentET.Services;

namespace OnlineAssessmentET.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
internal class IncidentController(IIncidentService incidentService) : InternalControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Incident(ReportIncidentRequest request)
	{
		var result = await incidentService.ReportIncident(request).ConfigureAwait(false);

		if (result.IsSuccess)
		{
			// Just returns Ok instead of CreatedAtAction for simplicity
			return Ok();
		}

		if (result.ErrorCode == ErrorCode.Duplicate)
		{
			// I think this is the response status code that suits this scenario
			return Conflict(result);
		}

		return BadRequest(result);
	}

	[HttpGet]
	public async Task<IActionResult> Incident([FromQuery] FindIncidentRequest request)
	{
		var result = await incidentService.FindIncidents(request).ConfigureAwait(false);

		if (result.TryGetValue(out var incidents))
		{
			return Ok(incidents);
		}

		if (result.ErrorCode == ErrorCode.NotFound)
		{
			return NotFound();
		}

		return BadRequest(result);
	}
}
