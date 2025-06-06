using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeventPlugins.Validators
{
    public class CheckCarRequired
    {
        public static void Validate(SEvent_Rent rentRecord)
        {
            if (rentRecord.StatusCode.HasValue)
            {
                var newStatus = rentRecord.StatusCode.Value;
                if (newStatus == SEvent_Rent_StatusCode.Confirmed
                    || newStatus == SEvent_Rent_StatusCode.Renting
                    || newStatus == SEvent_Rent_StatusCode.Returned)
                {

                    if (rentRecord.SEvent_Car == null || rentRecord.SEvent_Car.Id == Guid.Empty)
                    {
                        throw new InvalidPluginExecutionException("Field Car is required for status Confirmed, Renting or Returned");
                    }
                }

            }
        }
    }
}
