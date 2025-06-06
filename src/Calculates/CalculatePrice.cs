using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace SeventPlugins.Calculates
{
    public static class CalculatePrice
    {
        public static void Calculate(IOrganizationService service, SEvent_Rent target, SEvent_Rent preImage, ITracingService tracing)
        {
            tracing?.Trace("Start calculating the rental cost...");

            var pickupDate = target.SEvent_ReservedPickup
                ?? preImage?.SEvent_ReservedPickup;
            var returnDate = target.SEvent_ReservedHandover
                ?? preImage?.SEvent_ReservedHandover;

            if (!pickupDate.HasValue || !returnDate.HasValue)
            {
                tracing?.Trace("One of the dates is missing - calculation is not possible.");
                return;
            }

            var carClassRef = target.SEvent_CarClass ?? preImage?.SEvent_CarClass;

            if (carClassRef == null || carClassRef.Id == Guid.Empty)
                return;

            var carClassEntity = service.Retrieve("sevent_carclass", carClassRef.Id, new ColumnSet("sevent_price"));
          
            if (!carClassEntity.Contains("sevent_price"))
                throw new InvalidPluginExecutionException("Car class price is not set.");

            decimal pricePerDay = carClassEntity.GetAttributeValue<Money>("sevent_price")?.Value ?? 0m;

            if (pricePerDay <= 0)
                return;

           int totalDays = (returnDate.Value.Date - pickupDate.Value.Date).Days +1;
           
          //  if (totalDays < 1) totalDays = 1;

            decimal totalPrice = pricePerDay * totalDays;

            var returnLocation = target.SEvent_ReturnLocation ?? preImage?.SEvent_ReturnLocation;
            var pickupLocation = target.SEvent_PickupLocation ?? preImage?.SEvent_PickupLocation;
           
            if (returnLocation.HasValue && returnLocation.Value != SEvent_PickupLocation.Office)
                totalPrice += 100;

            if ( pickupLocation.HasValue && pickupLocation.Value != SEvent_PickupLocation.Office)
                totalPrice += 100;

            target.SEvent_Price = new Money(totalPrice);

            tracing?.Trace($"Pickup: {pickupDate}, Return: {returnDate}, Days: {totalDays}, Price/day: {pricePerDay}, Final: {totalPrice}");
        }

    }

}




