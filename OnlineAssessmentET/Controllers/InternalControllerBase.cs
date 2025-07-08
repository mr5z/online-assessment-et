﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace OnlineAssessmentET.Controllers;

// These are just to make internal controller possible.
// Adhering to "principle of least privilege"

internal class InternalControllerBase : ControllerBase;

internal class CustomControllerFeatureProvider : ControllerFeatureProvider
{
	protected override bool IsController(TypeInfo typeInfo)
	{
		var isCustomController = !typeInfo.IsAbstract && typeof(InternalControllerBase).IsAssignableFrom(typeInfo);
		return isCustomController || base.IsController(typeInfo);
	}
}

internal static class InternalControllersExtension
{
	public static IMvcBuilder EnableInternalControllers(this IMvcBuilder builder)
	{
		builder.ConfigureApplicationPartManager(manager =>
		{
			manager.FeatureProviders.Add(new CustomControllerFeatureProvider());
		});

		return builder;
	}
}