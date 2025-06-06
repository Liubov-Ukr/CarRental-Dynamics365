using Microsoft.Xrm.Sdk;
using SeventPlugins.Helpers;
using SeventPlugins;
using System;
namespace SeventPlugins.Plugins
{
    public class SetActualDateOnReportCreatePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracing?.Trace("SetActualDateOnReportCreatePlugin started");

            var (report, service, context) = PluginHelper.GetEntityServiceContext<SEvent_CarTransferReport>(serviceProvider);

            SEvent_CarTransferReport preImage = null;
            if (context.PreEntityImages.TryGetValue("PreImage", out var preEntity))
            {
                preImage = preEntity.ToEntity<SEvent_CarTransferReport>();
                tracing?.Trace("PreImage retrieved.");
            }

            try
            {
                SetActualDateHandler.HandleCreateOrUpdate(service, report, preImage, tracing);
            }
            catch (InvalidPluginExecutionException ex)
            {
                tracing?.Trace("Business validation failed: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                tracing?.Trace("Unexpected error: " + ex);
                throw new InvalidPluginExecutionException("Unexpected error during report processing.");
            }

            tracing?.Trace("Plugin finished.");
        }
    }
}