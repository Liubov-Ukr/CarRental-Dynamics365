using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeventPlugins.Helpers
{
    public static class PluginHelper
    {
        public static (TEntity, IOrganizationService, IPluginExecutionContext) GetEntityServiceContext<TEntity>(IServiceProvider serviceProvider)
      where TEntity : Entity
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);

            if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity targetEntity))
                throw new InvalidPluginExecutionException("Target entity is missing or invalid.");

            return (targetEntity.ToEntity<TEntity>(), service, context);
        }
    }
}
