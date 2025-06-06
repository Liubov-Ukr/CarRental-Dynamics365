using Microsoft.Xrm.Sdk;
using SeventPlugins;
using System.Collections.Generic;
using System.Linq;

public static class StatusTransitionValidator
{
    private static readonly Dictionary<SEvent_Rent_StatusCode, SEvent_Rent_StatusCode[]> AllowedTransitions =
    new Dictionary<SEvent_Rent_StatusCode, SEvent_Rent_StatusCode[]>
    {
        {
            SEvent_Rent_StatusCode.Created,
            new SEvent_Rent_StatusCode[]
            {
                SEvent_Rent_StatusCode.Confirmed,
                SEvent_Rent_StatusCode.Renting,
                SEvent_Rent_StatusCode.Canceled
            }
        },
        {
            SEvent_Rent_StatusCode.Confirmed,
            new SEvent_Rent_StatusCode[]
            {
                SEvent_Rent_StatusCode.Renting,
                SEvent_Rent_StatusCode.Canceled
            }
        },
        {
            SEvent_Rent_StatusCode.Renting,
            new SEvent_Rent_StatusCode[]
            {
                SEvent_Rent_StatusCode.Returned
            }
        }
    };


    public static void Validate(SEvent_Rent target, SEvent_Rent preImage, ITracingService tracing)
    {
        tracing?.Trace("Starting status transition validation...");

        var newStatusValue = target.StatusCode ?? preImage?.StatusCode;
        var oldStatusValue = preImage?.StatusCode;

        if (!newStatusValue.HasValue)
        {
            tracing?.Trace("New status is missing in both Target and PreImage.");
            throw new InvalidPluginExecutionException("New status is not specified.");
        }

        if (!oldStatusValue.HasValue)
        {
            tracing?.Trace("Old status is missing in PreImage. Skipping transition validation.");
            return;
        }

        var newStatus = (SEvent_Rent_StatusCode)newStatusValue.Value;
        var oldStatus = (SEvent_Rent_StatusCode)oldStatusValue.Value;

        tracing?.Trace($"Status transition: {oldStatus} → {newStatus}");

        if (newStatus == oldStatus)
        {
            tracing?.Trace("No status change detected. Skipping validation.");
            return;
        }

        if (!AllowedTransitions.TryGetValue(oldStatus, out var allowed) || !allowed.Contains(newStatus))
        {
            tracing?.Trace($"❌ Invalid status transition: {oldStatus} → {newStatus}");
            throw new InvalidPluginExecutionException(
                $"Status transition from '{oldStatus}' to '{newStatus}' is not allowed.");
        }

        tracing?.Trace("✅ Valid status transition.");
    }

}
