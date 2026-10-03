# AI-assisted driver licence validation

Documentation reviewed: 2026-10-02. Scope: development work after the 2026-09-03 GitHub baseline.

## What was actually established

The project evaluated Azure AI Document Intelligence v4 for extracting driver licence fields, alongside OCR and custom extraction experiments. The prebuilt ID-document work used driver-licence output described as `idDocument.driverLicense`; this is an output document type, not a custom model identifier.

| Work | Recorded outcome | Limit |
| --- | --- | --- |
| Dataverse Image-column input | Inaccurate extraction was encountered across attempted approaches. | This describes the project samples, not a general benchmark of the services. |
| Switch to File-column input | September 7: correct name, date of birth and expiry were reported using full-quality file input with Azure v4. | Sample-level success only. |
| Comparison after the input change | September 9: correct OCR, custom-model fields and Azure structured fields were reported with File input. | No measured accuracy rate or representative evaluation dataset is documented. |
| Custom extraction training | Project notes record an initial five-image training experiment. | An exploratory model, not a production-quality claim; broader training/evaluation remains open. |
| Validation and human review | Work covers required extracted fields, expiry, age, confidence and contact matching. | Complete branch-by-branch execution and approval verification are not established by these notes. |

The key practical finding was input quality: moving from the Image column to a File column improved the observed extraction results. The repository does not include a new model export, training dataset, benchmark or updated solution package for this work.

## Validation design and review boundaries

The intended flow retrieves the uploaded file, extracts fields, checks whether the results are usable, and applies business rules. Confidence and missing-field handling must happen before extracted values are treated as reliable.

- Missing or unreliable date-of-birth/expiry extraction requires review or re-upload.
- A mismatch with the contact record requires human review.
- Licence validity must cover the required rental period.
- Later project notes simplify the age policy to a minimum age of 18 and remove the proposed over-70 check. The exported implementation and boundary tests still need reconciliation with this later policy.
- Low-confidence/contact-mismatch outcomes route to Needs Review in the recorded processing-status mapping.
- The approval redesign and alternate-approver fallback are described in [Power Pages and approvals](power-pages-and-approvals.md); their end-to-end success is not yet verified.

No final numeric confidence threshold is asserted here. A configured threshold, successful extraction on samples and approval completion are different milestones.

## Remaining work

- Expand the document sample set and evaluate on documents not used for training, including different image quality and layouts.
- Record field-level extraction results and the chosen confidence/review policy.
- Test missing fields, low confidence, contact mismatch, expired licence and licence validity through return.
- Test minimum-age boundaries and check that older business rules do not remain in the active flow.
- Verify the human-review decision, rejection and alternate-approver paths end to end.
- Verify every terminal processing path updates the confirmation status consistently.
- Export the updated configuration and capture sanitized run evidence before claiming the repository package reproduces these results.

These are pending validation tasks. The feature is a tested extraction prototype with ongoing validation and integration work, not a completed automated approval service.

[Back to README](../README.md)
