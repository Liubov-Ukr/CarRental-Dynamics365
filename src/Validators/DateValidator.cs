using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeventPlugins
{
    public static class DateValidator
    {
        public static void Validate(SEvent_Rent target, SEvent_Rent preImage, ITracingService tracing)
        {
            tracing?.Trace("Starting date validation...");

            var pickupDate = target.Attributes.Contains("sevent_reservedpickup")
                ? target.SEvent_ReservedPickup
                : preImage?.SEvent_ReservedPickup;
            var handoverDate = target.Attributes.Contains("sevent_reservedhandover")
                ? target.SEvent_ReservedHandover
                : preImage?.SEvent_ReservedHandover;

            var actualPickup = target.Attributes.Contains("sevent_actualpickup")
                ? target.SEvent_ActualPickup
                : preImage?.SEvent_ActualPickup;
            var actualReturn = target.Attributes.Contains("sevent_actualreturn")
                ? target.SEvent_ActualReturn
                : preImage?.SEvent_ActualReturn;

            var today = DateTime.UtcNow.Date;

            if (pickupDate.HasValue || handoverDate.HasValue)
            {
                if (!pickupDate.HasValue)
                    throw new InvalidPluginExecutionException("Reserved Pickup Date must be provided.");
                if (!handoverDate.HasValue)
                    throw new InvalidPluginExecutionException("Reserved Return Date must be provided.");
                if (pickupDate.Value.Date < today)
                    throw new InvalidPluginExecutionException("Reserved Pickup Date cannot be in the past.");
                if (handoverDate.Value.Date < pickupDate.Value.Date)
                    throw new InvalidPluginExecutionException("Reserved Return Date cannot be earlier than Reserved Pickup Date.");
            }

            if (actualReturn.HasValue)
            {
                if (!actualPickup.HasValue)
                    throw new InvalidPluginExecutionException("Actual Pickup Date must be provided before setting Actual Return Date.");
                if (actualReturn.Value < actualPickup.Value)
                    throw new InvalidPluginExecutionException("Actual return date/time cannot be earlier than actual pickup date/time.");
            }

            tracing?.Trace("Date validation passed.");
        }

    }

}
