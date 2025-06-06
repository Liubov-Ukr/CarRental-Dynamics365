using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;

namespace SeventPlugins.Plugins
{
    public static class RentLimitValidator
    {
        public static void Validate(IOrganizationService service, SEvent_Rent target, SEvent_Rent preImage, ITracingService tracing)
        {
            tracing?.Trace("Starting rent limit validation...");

            var status = target.StatusCode ?? preImage?.StatusCode;

            if (!status.HasValue)
            {
                tracing?.Trace("StatusCode is missing. Skipping validation.");
                return;
            }

            if (status.Value != SEvent_Rent_StatusCode.Renting)
            {
                tracing?.Trace("Status is not 'Renting'. Skipping validation.");
                return;
            }

            Guid? customerId = target.SEvent_Customer?.Id ?? preImage?.SEvent_Customer?.Id;
            tracing?.Trace($"Resolved CustomerId: {customerId}");

            if (customerId == null || customerId == Guid.Empty)
            {
                tracing?.Trace("CustomerId is missing.");
                throw new InvalidPluginExecutionException("CustomerId is required to validate rent limit.");
            }

            int rentingCount = GetRentingCount(service, customerId.Value, tracing);
            tracing?.Trace($"Customer has {rentingCount} active rents in 'Renting' status.");

            if (rentingCount >= 10)
            {
                tracing?.Trace("Renting limit exceeded.");
                throw new InvalidPluginExecutionException("Customer already has 10 or more active rents in status 'Renting'.");
            }

            tracing?.Trace("Rent limit validation passed.");
        }

        private static int GetRentingCount(IOrganizationService service, Guid customerId, ITracingService tracing)
        {
            string fetchXml = $@"
            <fetch aggregate='true'>
              <entity name='sevent_rent'>
                <attribute name='sevent_rentid' alias='rentcount' aggregate='count' />
                <filter>
                  <condition attribute='sevent_customer' operator='eq' value='{customerId}' />
                  <condition attribute='statuscode' operator='eq' value='{(int)SEvent_Rent_StatusCode.Renting}' />
                </filter>
              </entity>
            </fetch>";

            try
            {
                var result = service.RetrieveMultiple(new FetchExpression(fetchXml));
                if (result.Entities.Count > 0 &&
                    result.Entities[0].Attributes.TryGetValue("rentcount", out var aliased) &&
                    aliased is AliasedValue av && av.Value is int count)
                {
                    return count;
                }
            }
            catch (Exception ex)
            {
                tracing?.Trace("Error during rent count fetch: " + ex);
                throw new InvalidPluginExecutionException("Failed to validate rent limit due to a data retrieval error.");
            }

            return 0;
        }
    }

}
