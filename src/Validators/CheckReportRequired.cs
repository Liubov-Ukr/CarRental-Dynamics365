using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeventPlugins.Validators
{
    public class CheckReportRequired
    {
        public static void Validate(SEvent_Rent rentRecord)
        {
            
            if (rentRecord.StatusCode.HasValue)
            {
                var status = rentRecord.StatusCode.Value;

                if (status == SEvent_Rent_StatusCode.Renting &&
                    (rentRecord.SEvent_PickupReport == null || rentRecord.SEvent_PickupReport.Id == Guid.Empty))
                {
                    throw new InvalidPluginExecutionException("Pickup report is required for status Renting");
                }

                if (status == SEvent_Rent_StatusCode.Returned &&
                    (rentRecord.SEvent_ReturnReport == null || rentRecord.SEvent_ReturnReport.Id == Guid.Empty))
                {
                    throw new InvalidPluginExecutionException("Return report is required for status Returned");
                }
            }
        }

    }
}
