# Task A
![image](https://github.com/user-attachments/assets/faa43473-5b36-42cc-b2fe-cf0c524ba4f0)

# Task B
![image](https://github.com/user-attachments/assets/426840b1-e917-4d69-aee9-b39a6894a766)

#### Describe what could go wrong in rendering and how you'd fix it.
Blazor components rely heavily on state management, especially in Blazor Server projects where rendering depends on consistent server-side state. I used Fluxor, a Redux-style library, to centralize and manage state effectively.

In Blazor Server, state must be configured properly to avoid issues like UI resets or inconsistent rendering. This can be addressed by enabling distributed caching for scalability, but for this project, it only uses in-memory with a scoped lifetime.

Behind a load balancer, sticky sessions are required to keep users on the same server. This should avoid session-related rendering issues and works well with distributed caching.

For cases where component state should persist after a page reload, Fluxor store can easily be extended to save and restore state.

# Task C
![image](https://github.com/user-attachments/assets/75adca89-7c22-4a25-bdde-b9404c3a9453)

# Task D

```
internal enum Severity { Low, Medium, High }

internal class Incident
{
	public required Severity Severity { get; init; }
	public required string? Title { get; init; }
	public required DateTimeOffset DateReported { get; init; }

	float CalculateUrgency()
	{
		var now = DateTimeOffset.Now;
		var datePassed = now - DateReported;

		int severityWeight = Severity switch
		{
			Severity.Low => 1,
			Severity.Medium => 2,
			Severity.High => 3,
			_ => 0,
		};

		var hoursPassed = (float)datePassed.TotalHours;
		return severityWeight * (1 + hoursPassed / 24);
	}
}
```

### Validation
As for the validation, I would move it on a different layer, not within `CalculateUrgency()`.

### Sample output

| Severity | Hours Passed | Urgency |
|----------|--------------|---------|
| Low      | 2            | 0.99    |
| Medium   | 12           | 2.81    |
| High     | 72           | 11.7    |
| High     | 0            | 2.72    |
