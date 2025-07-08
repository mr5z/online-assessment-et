using System.ComponentModel.DataAnnotations;

namespace OnlineAssessmentET.Persistence;

internal class Incident
{
	public const int TitleMaxLength = 100;

	public const int DescriptionMaxLength = 1_000;

	public int Id { get; set; }

	public Severity Severity { get; set; }

	[MaxLength(TitleMaxLength)]
	public string Title { get; set; } = default!;

	[MaxLength(DescriptionMaxLength)]
	public string? Description { get; set; }

	public DateTimeOffset CreatedAt { get; set; }
}
