using Microsoft.Xrm.Sdk;
using SeventPlugins.Helpers;
using System;
using SeventPlugins.Calculates;

namespace SeventPlugins.Plugins
{
    public class CalculatePricePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var (rentRecord, service, context) = PluginHelper.GetEntityServiceContext<SEvent_Rent>(serviceProvider);

            SEvent_Rent preImage = null;

            if (context.PreEntityImages.TryGetValue("PreImage", out var preImageEntity))
            {
                preImage = preImageEntity.ToEntity<SEvent_Rent>();
                tracing?.Trace("PreImage found.");
            }
            else
            {
                tracing?.Trace("PreImage didn't find.");
            }

            try
            {
                CalculatePrice.Calculate(service, rentRecord, preImage, tracing);
            }
            catch (InvalidPluginExecutionException ex)
            {
                tracing?.Trace("Exception: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                tracing?.Trace("CalculatePrice Exception: " + ex.ToString());
                throw new InvalidPluginExecutionException("Exception during calculate price");
            }
        }
    }

}
