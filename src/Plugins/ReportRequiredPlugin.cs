using Microsoft.Xrm.Sdk;
using SeventPlugins.Helpers;
using SeventPlugins.Validators;
using SeventPlugins;
using System;

public class ReportRequiredPlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

        var (target, _, context) = PluginHelper.GetEntityServiceContext<SEvent_Rent>(serviceProvider);

        var preImage = context.PreEntityImages?.Contains("PreImage") == true
            ? context.PreEntityImages["PreImage"].ToEntity<SEvent_Rent>()
            : null;

        try
        {
            CheckReportRequired.Validate(target);
        }
        catch (InvalidPluginExecutionException ex)
        {
            tracing?.Trace("Validation failed: " + ex.Message);
            throw;
        }
    }
}
