using Microsoft.Xrm.Sdk;
using SeventPlugins;
using System;

public static class SetActualDateHandler
{
    public static void HandleCreateOrUpdate(
        IOrganizationService service,
        SEvent_CarTransferReport report,
        SEvent_CarTransferReport preImage,
        ITracingService tracing)
    {
        tracing?.Trace("HandleCreateOrUpdate started");

        var type = report.SEvent_Type ?? preImage?.SEvent_Type;
        if (!type.HasValue)
        {
            tracing?.Trace("Type is missing. Skipping.");
            return;
        }

        var reportType = (SEvent_TypeCarTransferReport)type.Value;
        if (reportType != SEvent_TypeCarTransferReport.Pickup && reportType != SEvent_TypeCarTransferReport.Return)
        {
            tracing?.Trace($"Type {reportType} is unsupported. Skipping.");
            return;
        }

        var now = DateTime.UtcNow;
        tracing?.Trace($"Setting datetime: {now}");

        var reportUpdate = new SEvent_CarTransferReport { Id = report.Id };
        reportUpdate.SEvent_Date = now;
        service.Update(reportUpdate);
        tracing?.Trace("Report date updated.");

        var rentRef = report.SEvent_Rent;
        if (rentRef == null || rentRef.Id == Guid.Empty)
        {
            tracing?.Trace("Rent reference is missing. Skipping Rent update.");
            return;
        }

        var rentUpdate = new SEvent_Rent { Id = rentRef.Id };

        if (reportType == SEvent_TypeCarTransferReport.Pickup)
        {
            rentUpdate.SEvent_ActualPickup = now;
            rentUpdate.SEvent_PickupReport = new EntityReference(SEvent_CarTransferReport.EntityLogicalName, report.Id);
            tracing?.Trace("Updated Rent with actual pickup and pickup report.");
        }
        else if (reportType == SEvent_TypeCarTransferReport.Return)
        {
            rentUpdate.SEvent_ActualReturn = now;
            rentUpdate.SEvent_ReturnReport = new EntityReference(SEvent_CarTransferReport.EntityLogicalName, report.Id);
            tracing?.Trace("Updated Rent with actual return and return report.");
        }

        service.Update(rentUpdate);
        tracing?.Trace("Rent updated successfully.");
    }
}
