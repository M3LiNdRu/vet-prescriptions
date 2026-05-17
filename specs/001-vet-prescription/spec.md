# Feature Specification: Veterinary Prescription Generator

**Feature Branch**: `001-vet-prescription`
**Created**: 2026-05-17
**Status**: Draft

## User Scenarios & Testing

### User Story 1 — Create and Print a Prescription (Priority: P1)

A veterinarian fills in a prescription form with patient and drug details and generates a printable PDF document to hand to the owner at a pharmacy.

**Why this priority**: This is the entire core value of the tool. Everything else is secondary.

**Independent Test**: Can be tested by filling the form with sample data and verifying a valid PDF is produced with all fields correctly rendered.

**Acceptance Scenarios**:

1. **Given** the vet opens the tool, **When** they fill in all required fields and click "Generate", **Then** a PDF is produced with the correct prescription layout.
2. **Given** a required field is missing, **When** the vet tries to generate, **Then** the tool shows a validation error and does not produce a PDF.
3. **Given** a valid prescription is generated, **When** the vet opens the PDF, **Then** all fields are legible, correctly formatted, and ready to print.

---

### User Story 2 — Save and Reuse Vet Profile (Priority: P2)

The veterinarian's details (name, licence number, clinic, address, signature) are saved and pre-filled on every new prescription.

**Why this priority**: Avoids re-entering the same data repeatedly; saves time in daily practice.

**Independent Test**: Fill vet profile once, close and reopen the tool, verify fields are pre-filled.

**Acceptance Scenarios**:

1. **Given** the vet enters their profile data, **When** they save it, **Then** it persists across sessions.
2. **Given** a saved profile exists, **When** a new prescription is opened, **Then** vet fields are pre-filled automatically.

---

### User Story 3 — Prescription History (Priority: P3)

Previously issued prescriptions are listed and can be viewed or reprinted.

**Why this priority**: Useful for record-keeping but not essential for MVP.

**Independent Test**: Generate 3 prescriptions, open history view, verify all are listed and each can be reopened as PDF.

**Acceptance Scenarios**:

1. **Given** prescriptions have been generated, **When** the vet opens the history, **Then** all past prescriptions are listed with date and patient name.
2. **Given** a past prescription is selected, **When** the vet clicks "Reprint", **Then** the original PDF is regenerated.

---

### Edge Cases

- What happens when the drug name contains special characters?
- How does the system handle very long patient names or addresses that overflow the PDF layout?
- What if the vet profile file is corrupted or missing?

## Requirements

### Functional Requirements

All mandatory fields are defined by **Article 105.5 of Regulation (EU) 2019/6 of the European Parliament and of the Council** (veterinary medicinal products). Article 112 of the same regulation governs off-label (cascada terapéutica) prescriptions for non-food-producing animals.

- **FR-001**: System MUST collect vet details: full name, contact details (address, phone, email), and professional licence number — per Art. 105.5(d).
- **FR-002**: System MUST collect owner details: full name and contact details — per Art. 105.5(b).
- **FR-003**: System MUST collect animal/patient identification: species, breed, age, weight, and individual or group identification — per Art. 105.5(a).
- **FR-004**: System MUST collect for each prescribed item: drug name and active principle(s), pharmaceutical form, concentration, quantity/number of units, and dosage regimen — per Art. 105.5(f)(g)(h)(i).
- **FR-005**: System MUST include: date of issue and vet signature area (blank line for physical signature + typed vet name) — per Art. 105.5(c)(e).
- **FR-006**: System MUST support indicating whether the prescription is issued under off-label use (Art. 112/113/114 cascade) or antimicrobial prophylaxis/metaphylaxis (Art. 107.3/4) — per Art. 105.5(l)(m).
- **FR-007**: System MUST support a withdrawal period field for food-producing animal species (zero or otherwise) — per Art. 105.5(j). For non-food-producing animals this field is not required.
- **FR-008**: System MUST support a free-text warnings field for correct/prudent use — per Art. 105.5(k).
- **FR-009**: System MUST validate all Art. 105.5 mandatory fields before generating the PDF.
- **FR-010**: System MUST generate a PDF with a unique prescription ID and all legally required fields clearly laid out.
- **FR-011**: System MUST allow the vet profile to be saved and auto-loaded on startup (name, contact details, licence number).
- **FR-012**: Generated PDFs MUST be downloadable from the browser.

### Key Entities

- **Vet**: name, licence_number, clinic_name, address, phone, email
- **Owner**: name, address, phone, cif_dni
- **Patient**: animal_name, species, breed
- **PrescriptionItem**: drug_name, quantity, pharmaceutical_form, dosage_regimen, withdrawal_period
- **Prescription**: id, date, vet, owner, patient, items[], warnings, is_off_label, is_antimicrobial_special_use

## Success Criteria

- **SC-001**: A complete prescription PDF is generated in under 3 seconds on a standard desktop.
- **SC-002**: All prescription fields appear correctly on the printed document with no truncation.
- **SC-003**: The tool works fully offline with no network calls required.
- **SC-004**: Vet profile persists correctly across application restarts.

## Assumptions

- Target platform is any device with a modern internet browser (laptop, desktop, mobile phone, tablet). The UI must be mobile-first responsive.
- Regulatory format follows Spanish veterinary prescription standards (jurisdiction: Catalonia).
- No electronic signature or digital certification is required for MVP — a typed name with blank space for physical signature is sufficient.
- Single-vet use (no multi-user or login system needed for MVP).
- The generated PDF is downloaded to the device on both desktop and mobile browsers.
