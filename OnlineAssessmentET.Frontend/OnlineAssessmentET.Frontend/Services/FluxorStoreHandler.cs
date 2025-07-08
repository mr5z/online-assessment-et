using Fluxor;
using Fluxor.Persist.Storage;

namespace OnlineAssessmentET.Frontend.Services;

internal class FluxorStoreHandler : IStoreHandler
{
	// TODO replace with distributed cache such as Redis
	private readonly Dictionary<string, object> _store = [];

	Task<object> IStoreHandler.GetState(IFeature feature)
	{
		if (_store.TryGetValue(feature.GetName(), out var value))
		{
			return Task.FromResult(value);
		}

		return Task.FromResult(feature.GetState());
	}

	Task IStoreHandler.SetState(IFeature feature)
	{
		_store[feature.GetName()] = feature.GetState();

		return Task.CompletedTask;
	}
}
