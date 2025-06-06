using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeventPlugins.Validators
{
    public class CheckPaymentBeforeRenting
    {
        public static void Validate(SEvent_Rent rentRecord, SEvent_Rent preImage, ITracingService tracing)
        {
            if (!rentRecord.StatusCode.HasValue)
            {
                tracing.Trace("StatusCode is not set.");
                return;
            }

            var state = rentRecord.StatusCode.Value;
            tracing.Trace($"StatusCode: {state}");

            if (state == SEvent_Rent_StatusCode.Renting)
            {
                bool? paid = rentRecord.SEvent_Paid;

                if (!paid.HasValue && preImage != null)
                {
                    tracing.Trace("Field Paid not found in Target. Trying to take from PreImage.");
                    paid = preImage.SEvent_Paid;
                }

                tracing.Trace($"Value Paid: {paid}");

                if (!paid.HasValue || paid.Value == false)
                {
                    throw new InvalidPluginExecutionException("Car rent is not yet paid. Car cannot be rented.");
                }
            }
        }

    }
}
