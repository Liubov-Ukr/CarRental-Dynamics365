var sevent = sevent || {};

sevent.form = (function () {
  var formContext = null;
  var carFilterHandler = null;

  const STATE_CODES = {
    Created: 1,
    Confirmed: 239770001,
    Renting: 239770002,
    Returned: 2,
  };
  const PICKUP_RET_LOC_CODE = {
    Airport: 239770000,
    CityCenter: 239770001,
    Office: 239770002,
  };
  const TYPE_REPORT_CODE = {
    pickupReport: 239770000,
    returnReport: 239770001,
  };

  var onLoadForm = function (executionContext) {
    formContext = executionContext.getFormContext();
    filterCarByCarClass();
    calculateNumberOfDaysAndPrice();
    makeCarRequiredBasedOnStatus();

    addEvents();
  };

  var addEvents = function () {
    if (formContext.getAttribute("sevent_carclass")) {
      formContext
        .getAttribute("sevent_carclass")
        .addOnChange(onChangeCarClassField);
    }

    if (formContext.getAttribute("statuscode")) {
      formContext
        .getAttribute("statuscode")
        .addOnChange(onChangeStatusCodeField);
    }

    if (formContext.getAttribute("sevent_reservedhandover")) {
      formContext
        .getAttribute("sevent_reservedhandover")
        .addOnChange(onChangeHandoverDateField);
    }

    if (formContext.getAttribute("sevent_reservedpickup")) {
      formContext
        .getAttribute("sevent_reservedpickup")
        .addOnChange(onChangePickupDateField);
    }
    if (formContext.getAttribute("sevent_pickuplocation")) {
      formContext
        .getAttribute("sevent_pickuplocation")
        .addOnChange(onChangePickupReturnLocation);
    }
    if (formContext.getAttribute("sevent_returnlocation")) {
      formContext
        .getAttribute("sevent_returnlocation")
        .addOnChange(onChangePickupReturnLocation);
    }
    if (formContext.getAttribute("sevent_damages")) {
      formContext.getAttribute("sevent_damages").addOnChange(onChangeDamages);
    }
  };

  var onChangePickupReturnLocation = function () {
    calculateNumberOfDaysAndPrice();
  };

  var onChangeCarClassField = function () {
    formContext.getAttribute("sevent_carclass").setSubmitMode("always");
    filterCarByCarClass();
    calculateNumberOfDaysAndPrice();
  };

  var onChangeStatusCodeField = function () {
    warnIfRentingWithoutPaid();
    makeCarRequiredBasedOnStatus();
  };

  var onChangeHandoverDateField = function () {
    handoverDateValidate();
    calculateNumberOfDaysAndPrice();
  };

  var onChangePickupDateField = function () {
    pickupDateValidate();
    calculateNumberOfDaysAndPrice();
  };

  var onChangeDamages = function () {
    onDamageChange();
  };

  var filterCarByCarClass = function () {
    const classAttr = formContext.getAttribute("sevent_carclass");
    const carAttr = formContext.getAttribute("sevent_car");
    const carControl = formContext.getControl("sevent_car");

    if (!classAttr || !carAttr || !carControl) return;

    const carClass = classAttr.getValue();

    if (!carClass || carClass.length === 0) {
      carAttr.setValue(null);
      carControl.setDisabled(true);

      if (carFilterHandler) {
        carControl.removePreSearch(carFilterHandler);
        carFilterHandler = null;
      }

      return;
    }

    const carClassId = carClass[0].id;

    if (carFilterHandler) {
      carControl.removePreSearch(carFilterHandler);
    }

    carFilterHandler = function () {
      const filterXml = `<filter type='and'>
				<condition attribute='sevent_carclass' operator='eq' value='${carClassId}' />
			</filter>`;
      carControl.addCustomFilter(filterXml, "sevent_car");
    };

    carControl.addPreSearch(carFilterHandler);
    carControl.setDisabled(false);
  };

  var warnIfRentingWithoutPaid = function () {
    const statusAttr = formContext.getAttribute("statuscode");
    const paidAttr = formContext.getAttribute("sevent_paid");

    if (!statusAttr || !paidAttr) {
      console.log("Status or Paid field is missing on the form.");
      return;
    }

    const statusCode = statusAttr.getValue();
    const isPaid = paidAttr.getValue();

    if (!isPaid && statusCode == STATE_CODES.Renting) {
      Xrm.Navigation.openAlertDialog({
        text: "Car rent is not yet paid. Car cannot be rented.",
        title: "Warning",
      });
    }
  };

  let pickupDateValidate = function () {
    var control = formContext.getControl("sevent_reservedpickup");
    control.clearNotification("reservedPickupError");

    var reservedPickupAttr = formContext.getAttribute("sevent_reservedpickup");
    var reservedPickupValue = reservedPickupAttr?.getValue();

    function getDateOnly(date) {
      return date
        ? new Date(date.getFullYear(), date.getMonth(), date.getDate())
        : null;
    }

    var today = new Date();
    var todayDateOnly = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate()
    );
    var pickupDateOnly = getDateOnly(reservedPickupValue);

    if (pickupDateOnly && pickupDateOnly < todayDateOnly) {
      control.setNotification(
        "Reserved pickup date/time cannot be earlier than today.",
        "reservedPickupError"
      );
      reservedPickupAttr.setValue(null);
    }
  };

  var handoverDateValidate = function () {
    var control = formContext.getControl("sevent_reservedhandover");
    control.clearNotification("reservedHandOverError");

    var reservedHandoverAttr = formContext.getAttribute(
      "sevent_reservedhandover"
    );
    var reservedPickupAttr = formContext.getAttribute("sevent_reservedpickup");

    var reservedHandoverValue = reservedHandoverAttr?.getValue();
    var reservedPickupValue = reservedPickupAttr?.getValue();

    if (reservedHandoverValue && reservedPickupValue) {
      var handoverDate =
        reservedHandoverValue instanceof Date
          ? reservedHandoverValue
          : new Date(reservedHandoverValue);
      var pickupDate =
        reservedPickupValue instanceof Date
          ? reservedPickupValue
          : new Date(reservedPickupValue);

      if (handoverDate < pickupDate) {
        control.setNotification(
          "Reserved handover date/time cannot be earlier than reserved pickup date/time.",
          "reservedHandOverError"
        );
        reservedHandoverAttr.setValue(null);
      }
    }
  };

  var calculateDays = function (pickup, handover) {
    const daysField = formContext.getAttribute("sevent_numberofdays");
    if (!pickup || !handover) return null;

    function getDateOnly(date) {
      return date
        ? new Date(date.getFullYear(), date.getMonth(), date.getDate())
        : null;
    }
    const start = getDateOnly(
      pickup instanceof Date ? pickup : new Date(pickup)
    );
    const end = getDateOnly(
      handover instanceof Date ? handover : new Date(handover)
    );

    const diffMs = end - start;
    let days = Math.floor(diffMs / (1000 * 60 * 60 * 24)) + 1;
    if (days < 1) days = 1;
    if (daysField) daysField.setValue(days);
    return days;
  };

  var calculateNumberOfDaysAndPrice = function () {
    formContext.ui.clearFormNotification("price_hint");
    formContext.ui.clearFormNotification("price_actual_warning");

    const reservedPickup = formContext
      .getAttribute("sevent_reservedpickup")
      ?.getValue();
    const reservedHandover = formContext
      .getAttribute("sevent_reservedhandover")
      ?.getValue();
    const carClass = formContext.getAttribute("sevent_carclass")?.getValue();
    const daysField = formContext.getAttribute("sevent_numberofdays");
    const priceField = formContext.getAttribute("sevent_price");
    const pickupLoc = formContext
      .getAttribute("sevent_pickuplocation")
      ?.getValue();
    const returnLoc = formContext
      .getAttribute("sevent_returnlocation")
      ?.getValue();

    if (
      !reservedPickup ||
      !reservedHandover ||
      !carClass ||
      !pickupLoc ||
      !returnLoc
    ) {
      if (daysField) daysField.setValue(null);
      if (priceField) priceField.setValue(null);
      return;
    }

    const days = calculateDays(reservedPickup, reservedHandover);

    const carClassId = carClass[0].id;

    Xrm.WebApi.retrieveRecord(
      "sevent_carclass",
      carClassId,
      "?$select=sevent_price"
    ).then(
      function (result) {
        let total = result.sevent_price * days;
        if (pickupLoc !== PICKUP_RET_LOC_CODE.Office) total += 100;
        if (returnLoc !== PICKUP_RET_LOC_CODE.Office) total += 100;

        priceField.setValue(parseFloat(total.toFixed(2)));

        formContext.ui.setFormNotification(
          `Estimated price: €${total.toFixed(2)}`,
          "INFO",
          "price_hint"
        );
      },
      function (error) {
        console.error("Error retrieving CarClass price: ", error.message);
      }
    );
  };
  function openTransferQuickCreate(typeCode) {
    const formContext = Xrm.Page;

    if (formContext.data.getIsDirty()) {
      formContext.data.save().then(
        () => proceedToCreateReport(typeCode, formContext),
        () => {}
      );
    } else {
      proceedToCreateReport(typeCode, formContext);
    }
  }

  function proceedToCreateReport(typeCode, formContext) {
    const rentId = formContext.data.entity.getId();
    if (!rentId) return;

    const carLookup = formContext.getAttribute("sevent_car").getValue();
    if (!carLookup || carLookup.length === 0) return;

    const now = new Date().toISOString();
    const car = carLookup[0];
    const rentName =
      formContext.getAttribute("sevent_reservationname")?.getValue() ||
      "Reservation";

    const parameters = {
      sevent_type: typeCode,
      sevent_date: now,
      sevent_car: {
        id: car.id,
        name: car.name,
        entityType: car.entityType,
      },
      sevent_rent: {
        id: rentId,
        name: rentName,
        entityType: "sevent_rent",
      },
    };

    const entityFormOptions = {
      entityName: "sevent_cartransferreport",
      useQuickCreateForm: true,
    };

    Xrm.Navigation.openForm(entityFormOptions, parameters).then((result) => {
      if (result?.savedEntityReference) {
        const transferId = result.savedEntityReference.id;
        if (typeCode === TYPE_REPORT_CODE.pickupReport) {
          Xrm.Page.getAttribute("sevent_pickupreport").setValue(
            result.savedEntityReference
          );

          Xrm.Page.getAttribute("sevent_actualpickup").setValue(new Date());
        } else {
          Xrm.Page.getAttribute("sevent_returnreport").setValue(
            result.savedEntityReference
          );
          Xrm.Page.getAttribute("sevent_actualreturn").setValue(new Date());
        }
      }
    });
  }

  var onDamageChange = function () {
    var isDamaged = formContext.getAttribute("sevent_damages").getValue();
    var descField = formContext.getAttribute("sevent_damagedescription");
    if (descField) {
      descField.setRequiredLevel(isDamaged === 1 ? "required" : "none");
    }
  };

  function makeCarRequiredBasedOnStatus() {
    const statusField = formContext.getAttribute("statuscode");
    const carField = formContext.getAttribute("sevent_car");

    if (!statusField || !carField) {
      console.log("Missing fields on form.");
      return;
    }

    const status = statusField.getValue();

    const STATUS_CONFIRMED = STATE_CODES.Confirmed;
    const STATUS_RENTING = STATE_CODES.Renting;
    const STATUS_RETURNED = STATE_CODES.Returned;

    const shouldBeRequired = [
      STATUS_CONFIRMED,
      STATUS_RENTING,
      STATUS_RETURNED,
    ].includes(status);

    carField.setRequiredLevel(shouldBeRequired ? "required" : "none");
  }

  return {
    onLoadForm: onLoadForm,
    openTransferQuickCreate: openTransferQuickCreate,
  };
})();
