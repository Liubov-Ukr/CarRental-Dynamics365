# SEVENT – Power Platform Car Rental Ecosystem

SEVENT is an evolving portfolio project built with Microsoft Dynamics 365 and Power Platform to model an end-to-end car rental process.

The project originally started as a Dynamics 365 development exercise using C# plug-ins and JavaScript. It has since evolved into a broader Power Platform solution that includes Dataverse, Model-driven Apps, Power Automate, Power Pages, AI-assisted document processing, and Application Lifecycle Management with Azure DevOps.

The goal of the project is to build practical experience with both functional solution design and technical development using realistic business scenarios.

---

## Technology Stack

- Microsoft Dataverse
- Model-driven Power Apps
- Power Automate
- Power Pages
- Microsoft Copilot Studio
- Azure AI Document Intelligence
- C# Dataverse plug-ins
- JavaScript web resources
- Microsoft Teams integration
- Azure DevOps Boards
- Azure Repos / Git
- Power Platform CLI (PAC CLI)

---

## Project Status

SEVENT is developed iteratively and tested as the solution evolves.

Documentation reviewed on **2026-10-02**, incorporating development work discussed after the **2026-09-03** repository baseline. These notes distinguish observed results from configuration work and pending verification. They do not assert that the existing solution ZIP contains the later environment changes.

### Implemented and Demonstrated

- Dataverse data model and relationships
- Model-driven rental management application
- JavaScript pricing and form validation
- C# server-side business validation
- Rental status lifecycle
- Payment tracking and validation
- Power Automate notifications
- Microsoft Teams integration
- Managed DEV-to-TEST solution deployment
- Azure DevOps Boards
- Azure Repos source control using PAC CLI

### Progress after 2026-09-03

| Area | Recorded progress | Remaining work |
| --- | --- | --- |
| Power Pages reservation creation | Creation of the reservation record in Dataverse confirmed in development on September 18. | Verify the full submission-to-confirmation journey. |
| Confirmation/status polling | Processing Status flow mapping developed; JavaScript polling via the Power Pages Web API designed to refresh asynchronous results. | Successful automatic refresh without a manual reload is not yet confirmed. |
| Driver licence extraction | Azure AI Document Intelligence v4 extraction tested; switching input from a Dataverse Image column to a File column produced correct name, date of birth and expiry on tested samples. | Broader extraction evaluation and end-to-end validation/manual-review tests. |
| Approval flow | September work records a redesign around `Start and wait for an approval`, including an alternate-approver fallback. | Approval, rejection, timeout and fallback behaviour still need recorded end-to-end verification. |

See [Power Pages and approvals](docs/power-pages-and-approvals.md) and [driver licence validation](docs/driver-licence-validation.md) for evidence boundaries and follow-up checks.

### In Progress

- Rental Operations Agent with Copilot Studio
- AI-assisted driver licence validation: broader sample testing and end-to-end review/approval verification
- Additional Power Automate flows
- Business Process Flow and lifecycle refinements
- Power Pages: confirm automatic status polling and complete end-to-end portal testing
- DEV-to-TEST deployment pipeline
- Mobile Pickup & Return experience

---

## Solution Overview

SEVENT is designed to support the main stages of a car rental lifecycle:

**Reservation → Validation → Confirmation → Pickup → Renting → Return → Payment**

The solution covers customer management, vehicle availability, pricing, rental validation, approvals, payments, pickup and return operations, and customer self-service.

Some capabilities are complete, while others are actively being developed and tested.

---

## Dataverse Data Model

Main tables include:

- Contact
- Rent
- Car
- Car Class
- Insurance Option
- Payment
- Car Transfer Report / Vehicle Inspection
- Branch

Dataverse relationships connect customer, vehicle, reservation, payment, and rental-operation information.

---

## Model-driven Application

The internal SEVENT application supports rental employees and managers.

Key capabilities include:

- Create and manage reservations
- Select vehicle class and vehicle
- Calculate rental duration and estimated price
- Manage pickup and return locations
- Track rental lifecycle using Status Reason
- Validate required rental information
- Create pickup and return reports
- Track payments
- Support manager approval scenarios

### Rental Management

![SEVENT Rental Form](docs/screenshots/rent-form.png)

---

## JavaScript

JavaScript is used for client-side form behaviour, calculations, and immediate user feedback.

Examples include:

- Filtering vehicles by selected Car Class
- Reservation date validation
- Rental-day calculation
- Price calculation
- Conditional field behaviour
- Form notifications and validation

JavaScript examples are available in the [`scripts`](./scripts) folder.

---

## C# Plug-ins

The project includes Dataverse plug-ins for server-side business validation.

Examples include:

- Status transition validation
- Required-field validation
- Payment validation before rental lifecycle transitions
- Vehicle and rental business rules
- Pickup and return validation

The plug-in source code is available in the [`src`](./src) folder.

### Server-side Business Validation

For example, the rental cannot move to the **Renting** status when required payment conditions are not satisfied.

![Payment Validation](docs/screenshots/payment-validation.png)

---

## Rental Lifecycle

Status Reason transitions are used to control valid rental lifecycle changes.

Example lifecycle:

**Created → Ready for Confirmation → Confirmed → Renting → Returned**

Cancellation and no-show scenarios are also supported.

![Rental Status Transitions](docs/screenshots/status-transitions.png)

---

## Power Automate

Power Automate is used throughout SEVENT for business logic, integrations, scheduled processing, notifications, approvals, and AI-assisted validation. The list includes work at different stages; document generation and related hand-off remain planned or unverified.

Automation scenarios include:

- Reservation validation and pricing calculations
- Driver age and licence validation
- Rental fee calculations
- Microsoft Teams and email notifications
- Manager approval workflows
- Scheduled upcoming-return processing
- Word template generation for signing (planned)
- SharePoint document storage (completion not verified in this update)
- Rental summary communication (completion not verified in this update)
- AI-assisted driver licence processing
- Error handling using Try/Catch-style scopes

### Rental Pricing and Validation

One of the main automation processes combines Dataverse data from the customer, vehicle, car class, and insurance records to calculate rental-related values and validate reservation conditions.

The flow handles values such as driver age, licence validity, rental days, location fees, insurance pricing, young-driver fees, reserved price, and estimated rental total.

![Reservation Calculation Flow](docs/screenshots/reservation-calculation-flow.png)

### AI-assisted Driver Licence Validation

The driver licence validation work connects Dataverse and Power Automate with Azure AI Document Intelligence. OCR, custom extraction and structured-field approaches were evaluated; this does not imply that every evaluated tool is part of the final flow.

Extraction from File-column input has produced correct values on tested samples. Confidence handling, business validation and human review remain an integration/testing workstream. See the [detailed validation notes](docs/driver-licence-validation.md).

The screenshot below is an earlier flow snapshot already present in the September 3 baseline, not evidence of the later changes.

![AI Licence Validation Flow](docs/screenshots/ai-licence-validation-flow.png)

### Scheduled Rental Operations

A scheduled Power Automate flow checks upcoming vehicle returns, retrieves relevant Dataverse records, prepares the information, and sends notifications.

This automation has also been tested through recurring scheduled executions.

![Daily Upcoming Returns](docs/screenshots/daily-upcoming-returns.png)

### Error Handling

Testing has also helped improve flow reliability.

For example, a document-processing flow originally failed when an expected file or image was unavailable. The flow was redesigned to use Try/Catch-style scopes so expected failures can be handled more safely instead of relying only on the happy path.

![Power Automate Error Handling](docs/screenshots/flow-error-handling.png)
---

## Power Pages

The customer-facing portal uses a multi-step reservation form connected to Dataverse. Reservation record creation was confirmed in development on September 18; the remaining issue was displaying the calculated information without reloading the page.

The confirmation work uses a separate **Processing Status** concept: **Pending**, **Processing**, **Completed**, **Needs Review** and **Failed**. Flow status mapping was developed, and JavaScript polling through the Power Pages Web API was proposed to refresh the result after asynchronous processing.

**Status:** Dataverse creation confirmed; successful automatic confirmation refresh is not yet verified. Processing completion does not mean the licence is valid or the rental is approved.

The approval redesign uses `Start and wait for an approval` with an alternate-approver fallback in the design. Complete response/fallback testing is not confirmed. See [Power Pages and approvals](docs/power-pages-and-approvals.md).

---

## AI-assisted Driver Licence Validation

Azure AI Document Intelligence v4 was evaluated for driver licence extraction, including the prebuilt ID-document driver-licence output (`idDocument.driverLicense`) and custom extraction experiments.

A practical input-quality issue was identified: the Dataverse Image-column input produced inaccurate results, while switching to a File column yielded correct name, date of birth and expiry on the tested documents (September 7–9).

The workflow design includes extracted-field checks, licence expiry and minimum-age validation, and human review for uncertain results or contact mismatches. Extraction success on a few samples is not a completed or production-validated licence-validation service.

**Status:** extraction prototype tested on samples; broader evaluation and end-to-end validation/approval tests remain open. See [driver licence validation](docs/driver-licence-validation.md).

---

## Rental Operations Agent

A Rental Operations Agent is being developed with Microsoft Copilot Studio.

The planned agent will help with scenarios such as:

- Reservation validation
- Rental status checks
- Missing-information detection
- Business-rule guidance
- Manager escalation
- Rental operation support

**Status: In progress**

---

## Azure DevOps Project Management

SEVENT development is organized in Azure DevOps.

The project backlog includes several functional and technical workstreams:

- Application Lifecycle Management
- Rental Operations Agent
- Driver Licence Validation
- Mobile Pickup & Return App
- Payments & Rental Lifecycle

![SEVENT Azure DevOps Features](docs/screenshots/azure-devops-features.png)

Azure Boards is also used to break work down into:

**Epic → Feature → User Story → Task / Bug**

### ALM Work Tracking

Solution preparation, managed deployment, source control, and future pipeline automation are tracked as individual work items.

![Azure DevOps ALM Board](docs/screenshots/azure-devops-alm-board.png)

---

## Application Lifecycle Management

SEVENT now uses separate development and testing environments.

Current workflow:

```text
DEV
 ↓
Unmanaged development solution
 ↓
Managed solution export
 ↓
SEVENT TEST
 ↓
Functional validation
```
