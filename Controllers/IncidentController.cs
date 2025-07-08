using Microsoft.AspNetCore.Mvc;
using OnlineAssessmentET.Core.Dto.Requests.Incidents;
using OnlineAssessmentET.Core.Patterns;
using OnlineAssessmentET.Services;

namespace OnlineAssessmentET.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
internal class IncidentController(
	ILogger<IncidentController> logger,
	IIncidentService incidentService) : InternalControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Incident(ReportIncidentRequest request)
	{
		try
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
		catch (Exception ex)
		{
			const string error = "An error occurred while trying to report an incident. (Title: {title})";
			logger.LogError(ex, error, request.Title);
			throw;
		}
	}

	[HttpGet]
	public async Task<IActionResult> Incident([FromQuery] FindIncidentRequest request)
	{
		try
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
		catch (Exception ex)
		{
			const string error = "An error occurred while trying to find an incident. (SearchTerm: {SearchTerm})";
			logger.LogError(ex, error, request.SearchTerm);
			throw;
		}
	}
}
