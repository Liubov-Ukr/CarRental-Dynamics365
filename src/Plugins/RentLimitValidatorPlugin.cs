using Microsoft.Xrm.Sdk;
using SeventPlugins.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeventPlugins.Plugins
{
    public class RentLimitValidatorPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var (target, service, context) = PluginHelper.GetEntityServiceContext<SEvent_Rent>(serviceProvider);

            tracing?.Trace("RentLimitValidatorPlugin started.");

            SEvent_Rent preImage = null;

            if (context.PreEntityImages.TryGetValue("PreImage", out var preEntity))
            {
                preImage = preEntity.ToEntity<SEvent_Rent>();
                tracing?.Trace("PreImage successfully retrieved.");
            }
            else
            {
                tracing?.Trace("PreImage not provided.");
            }

            try
            {
                RentLimitValidator.Validate(service, target, preImage, tracing);
            }
            catch (InvalidPluginExecutionException ex)
            {
                tracing?.Trace("Validation failed: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                tracing?.Trace("Unexpected error: " + ex);
                throw new InvalidPluginExecutionException("An unexpected error occurred during rent limit validation.");
            }

            tracing?.Trace("RentLimitValidatorPlugin finished.");
        }
    }

}
