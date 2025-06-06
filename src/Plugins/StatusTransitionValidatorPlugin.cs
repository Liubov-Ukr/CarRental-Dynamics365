using Microsoft.Xrm.Sdk;
using SeventPlugins.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeventPlugins.Plugins
{
    public class StatusTransitionValidatorPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracing?.Trace("StatusTransitionValidatorPlugin started.");

            var (target, _, context) = PluginHelper.GetEntityServiceContext<SEvent_Rent>(serviceProvider);

            SEvent_Rent preImage = null;
            if (context.PreEntityImages.TryGetValue("PreImage", out var image))
            {
                preImage = image.ToEntity<SEvent_Rent>();
                tracing?.Trace("PreImage successfully retrieved.");
            }
            else
            {
                tracing?.Trace("PreImage not provided.");
            }

            try
            {
                StatusTransitionValidator.Validate(target, preImage, tracing);
                tracing?.Trace("Status transition validation passed.");
            }
            catch (InvalidPluginExecutionException ex)
            {
                tracing?.Trace("Validation failed: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                tracing?.Trace("Unexpected error: " + ex.ToString());
                throw new InvalidPluginExecutionException("An unexpected error occurred during status validation.");
            }

            tracing?.Trace("StatusTransitionValidatorPlugin finished.");
        }
    }

}
