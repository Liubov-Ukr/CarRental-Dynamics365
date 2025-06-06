using Microsoft.Xrm.Sdk;
using SeventPlugins.Helpers;
using SeventPlugins.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeventPlugins.Plugins
{
    public class DateValidatorPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
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
                DateValidator.Validate(target, preImage, tracing);
            }
            catch (InvalidPluginExecutionException ex)
            {
                tracing?.Trace("Validation error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                tracing?.Trace("Unexpected error: " + ex);
                throw new InvalidPluginExecutionException("An unexpected error occurred during date validation.");
            }
        }
    }

}
