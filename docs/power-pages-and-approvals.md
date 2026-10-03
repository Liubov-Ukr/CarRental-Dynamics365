# Power Pages confirmation and approvals

Documentation reviewed: 2026-10-02. Scope: development work after the 2026-09-03 GitHub baseline.

## Evidence and current status

These notes summarize project development discussions; they are not a fresh inspection of the live Power Platform environment. The existing screenshots predate the work described here. No updated portal export or flow definition is added by this documentation change.

| Work item | Recorded result | Status boundary |
| --- | --- | --- |
| Multi-step reservation submission | The reservation record was confirmed to exist in Dataverse on September 18. | Record creation is confirmed; full journey acceptance is not. |
| Confirmation page | Calculated data became visible after a manual reload. | Automatic refresh success has not been confirmed. |
| Processing Status | September 18 flow review recorded mappings for processing, completed and review outcomes. | Configuration progress, not proof of every branch executing successfully. |
| JavaScript status polling | Polling the reservation through the Power Pages Web API was proposed to display asynchronous results. | Final deployed script and a successful no-reload test are not available in this repository. |
| Approval redesign | September 22 project notes describe switching to `Start and wait for an approval` with an alternate-approver fallback. | Detailed flow export and response/fallback test evidence remain to be captured. |

## Confirmation/status contract

Processing Status is separate from rental Status Reason and from the business validation result. In particular, **Completed does not mean approved or valid**: an expired-licence result can finish processing.

| Status label | Intended meaning | Recorded mapping or remaining check |
| --- | --- | --- |
| Pending | Work has not started. | Proposed initial state; verify initialization. |
| Processing | Validation/calculation is running. | The valid-licence path remained Processing until later checks finished. |
| Completed | Processing has finished; display the business result. | Expired-licence and Ready for Confirmation branches were mapped to Completed. |
| Needs Review | A person must review the result. | Low-confidence extraction and contact mismatch were mapped here. |
| Failed | Technical processing could not finish. | Verify error paths set this state before termination. |

Numeric choice values and final schema/API names must be taken from the environment; they are not specified here.

The polling design is to read the created reservation, wait while processing is pending/running, then display the calculated values or the review/error result. A terminal processing state must not silently advance the rental lifecycle.

The September 18 flow review also recorded trigger Select columns limited to `sevent_reservedhandover,sevent_reservedpickup` to reduce self-triggering from status updates. This is a recorded configuration change, not a verified guarantee against duplicate runs.

## Approval redesign

The later design replaces notification-only handling with an approval decision using `Start and wait for an approval`. An alternate approver is part of the fallback design. Exact assignment rules, waiting period and escalation conditions are not documented as verified settings.

Do not describe approval delivery, fallback routing or automatic completion as tested merely because the action exists. Human review remains necessary for uncertain extraction and contact mismatches.

## Outstanding acceptance checks

All checks below are pending evidence; this documentation update does not mark them passed.

- Submit a reservation and confirm the correct Dataverse record is associated with the confirmation page.
- Confirm the page displays calculated values automatically without a manual reload.
- Exercise Completed, Needs Review and Failed outcomes; ensure Completed can display an invalid business result.
- Verify bounded polling, a useful timeout message and handling of Web API/network errors.
- Check web roles and table permissions allow access only to the intended customer's reservation.
- Confirm one expected run on creation, no extra run caused solely by status writes, and recalculation after relevant date changes.
- Exercise approval and rejection; verify the returned decision is applied to the correct record.
- Exercise timeout/unavailable-primary-approver fallback and ensure a delayed response cannot apply conflicting decisions.
- Capture sanitized portal/flow exports and test evidence, then update the related Azure DevOps work items.

The Word template for signing remains planned. These notes do not claim that document generation, signing or SharePoint hand-off is complete.

[Back to README](../README.md)
