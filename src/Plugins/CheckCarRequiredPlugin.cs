using Microsoft.Xrm.Sdk;
using SeventPlugins.Helpers;
using SeventPlugins.Validators;
using System;

namespace SeventPlugins.Plugins
{
    public class CheckCarRequiredPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var (target, _, _) = PluginHelper.GetEntityServiceContext<SEvent_Rent>(serviceProvider);

            try
            {
                CheckCarRequired.Validate(target); 
            }
            catch (InvalidPluginExecutionException ex)
            {
                tracing?.Trace("Validation failed: " + ex.Message);
                throw;
            }
        }
      
    }
}
