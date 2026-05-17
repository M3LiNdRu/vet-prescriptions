import { useState } from 'react'
import {
  createPrescription,
  generatePdf,
  type CreatePrescriptionRequest,
  type CreatePrescriptionResponse,
  type PdfResponse,
  type PrescriptionItemRequest,
} from './api'

const emptyItem = (): PrescriptionItemRequest => ({
  drugName: '',
  quantity: '',
  pharmaceuticalForm: '',
  dosageRegimen: '',
  withdrawalPeriod: '',
})

interface SuccessState {
  prescription: CreatePrescriptionResponse
  pdf: PdfResponse | null
  pdfLoading: boolean
  pdfError: string | null
}

export function CreatePrescriptionForm() {
  const [vetName, setVetName] = useState('')
  const [vetLicence, setVetLicence] = useState('')
  const [vetClinic, setVetClinic] = useState('')
  const [vetAddress, setVetAddress] = useState('')
  const [vetPhone, setVetPhone] = useState('')
  const [vetEmail, setVetEmail] = useState('')

  const [ownerName, setOwnerName] = useState('')
  const [ownerAddress, setOwnerAddress] = useState('')
  const [ownerPhone, setOwnerPhone] = useState('')
  const [ownerCifDni, setOwnerCifDni] = useState('')

  const [animalName, setAnimalName] = useState('')
  const [species, setSpecies] = useState('')
  const [breed, setBreed] = useState('')

  const [items, setItems] = useState<PrescriptionItemRequest[]>([emptyItem()])
  const [warnings, setWarnings] = useState('')
  const [isOffLabel, setIsOffLabel] = useState(false)
  const [isAntimicrobial, setIsAntimicrobial] = useState(false)

  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<SuccessState | null>(null)

  function updateItem(
    index: number,
    field: keyof PrescriptionItemRequest,
    value: string,
  ) {
    setItems((prev) =>
      prev.map((item, i) => (i === index ? { ...item, [field]: value } : item)),
    )
  }

  function addItem() {
    setItems((prev) => [...prev, emptyItem()])
  }

  function removeItem(index: number) {
    setItems((prev) => prev.filter((_, i) => i !== index))
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)

    const req: CreatePrescriptionRequest = {
      vet: {
        name: vetName,
        licenceNumber: vetLicence,
        clinicName: vetClinic,
        address: vetAddress,
        phone: vetPhone,
        email: vetEmail,
      },
      owner: {
        name: ownerName,
        address: ownerAddress,
        phone: ownerPhone,
        cifDni: ownerCifDni,
      },
      patient: { animalName, species, breed },
      items: items.map((it) => ({
        ...it,
        withdrawalPeriod: it.withdrawalPeriod || undefined,
      })),
      warnings: warnings || undefined,
      isOffLabel,
      isAntimicrobialSpecialUse: isAntimicrobial,
    }

    try {
      const prescription = await createPrescription(req)
      setSuccess({ prescription, pdf: null, pdfLoading: false, pdfError: null })
    } catch {
      setError('Error creating prescription. Please try again.')
    } finally {
      setSubmitting(false)
    }
  }

  async function handleGeneratePdf() {
    if (!success) return
    setSuccess((prev) => prev && { ...prev, pdfLoading: true, pdfError: null })

    try {
      const pdf = await generatePdf(success.prescription.id)
      setSuccess((prev) => prev && { ...prev, pdf, pdfLoading: false })
    } catch {
      setSuccess((prev) =>
        prev && { ...prev, pdfLoading: false, pdfError: 'Error generating PDF. Please try again.' },
      )
    }
  }

  if (success) {
    return (
      <div className="p-4 max-w-lg mx-auto">
        <div className="bg-green-50 border border-green-200 rounded-lg p-4 mb-4">
          <h2 className="text-lg font-semibold text-green-800 mb-1">
            Prescription created
          </h2>
          <p className="text-green-700">
            Number: <strong>{success.prescription.prescriptionNumber}</strong>
          </p>
          <p className="text-sm text-green-600">Date: {success.prescription.date}</p>
        </div>

        {!success.pdf && (
          <button
            type="button"
            onClick={handleGeneratePdf}
            disabled={success.pdfLoading}
            className="w-full bg-blue-600 text-white py-3 px-4 rounded-lg font-medium disabled:opacity-50"
          >
            {success.pdfLoading ? 'Generating PDF…' : 'Generate PDF'}
          </button>
        )}

        {success.pdfError && (
          <p role="alert" className="mt-2 text-red-600 text-sm">
            {success.pdfError}
          </p>
        )}

        {success.pdf && (
          <a
            href={success.pdf.url}
            target="_blank"
            rel="noopener noreferrer"
            className="block w-full text-center bg-green-600 text-white py-3 px-4 rounded-lg font-medium mt-2"
          >
            Open PDF
          </a>
        )}
      </div>
    )
  }

  return (
    <form onSubmit={handleSubmit} className="p-4 max-w-lg mx-auto space-y-6">
      <h1 className="text-2xl font-bold">New Prescription</h1>

      {/* Vet */}
      <fieldset className="space-y-3">
        <legend className="text-lg font-semibold">Veterinarian</legend>
        <div>
          <label htmlFor="vetName" className="block text-sm font-medium mb-1">
            Name
          </label>
          <input
            id="vetName"
            type="text"
            required
            value={vetName}
            onChange={(e) => setVetName(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="vetLicence" className="block text-sm font-medium mb-1">
            Licence Number
          </label>
          <input
            id="vetLicence"
            type="text"
            required
            value={vetLicence}
            onChange={(e) => setVetLicence(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="vetClinic" className="block text-sm font-medium mb-1">
            Clinic Name
          </label>
          <input
            id="vetClinic"
            type="text"
            required
            value={vetClinic}
            onChange={(e) => setVetClinic(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="vetAddress" className="block text-sm font-medium mb-1">
            Address
          </label>
          <input
            id="vetAddress"
            type="text"
            required
            value={vetAddress}
            onChange={(e) => setVetAddress(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="vetPhone" className="block text-sm font-medium mb-1">
            Phone
          </label>
          <input
            id="vetPhone"
            type="tel"
            required
            value={vetPhone}
            onChange={(e) => setVetPhone(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="vetEmail" className="block text-sm font-medium mb-1">
            Email
          </label>
          <input
            id="vetEmail"
            type="email"
            required
            value={vetEmail}
            onChange={(e) => setVetEmail(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
      </fieldset>

      {/* Owner */}
      <fieldset className="space-y-3">
        <legend className="text-lg font-semibold">Owner</legend>
        <div>
          <label htmlFor="ownerName" className="block text-sm font-medium mb-1">
            Name
          </label>
          <input
            id="ownerName"
            type="text"
            required
            value={ownerName}
            onChange={(e) => setOwnerName(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="ownerAddress" className="block text-sm font-medium mb-1">
            Address
          </label>
          <input
            id="ownerAddress"
            type="text"
            required
            value={ownerAddress}
            onChange={(e) => setOwnerAddress(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="ownerPhone" className="block text-sm font-medium mb-1">
            Phone
          </label>
          <input
            id="ownerPhone"
            type="tel"
            required
            value={ownerPhone}
            onChange={(e) => setOwnerPhone(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="ownerCifDni" className="block text-sm font-medium mb-1">
            CIF/DNI
          </label>
          <input
            id="ownerCifDni"
            type="text"
            required
            value={ownerCifDni}
            onChange={(e) => setOwnerCifDni(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
      </fieldset>

      {/* Patient */}
      <fieldset className="space-y-3">
        <legend className="text-lg font-semibold">Patient</legend>
        <div>
          <label htmlFor="animalName" className="block text-sm font-medium mb-1">
            Animal Name
          </label>
          <input
            id="animalName"
            type="text"
            required
            value={animalName}
            onChange={(e) => setAnimalName(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="species" className="block text-sm font-medium mb-1">
            Species
          </label>
          <input
            id="species"
            type="text"
            required
            value={species}
            onChange={(e) => setSpecies(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label htmlFor="breed" className="block text-sm font-medium mb-1">
            Breed
          </label>
          <input
            id="breed"
            type="text"
            required
            value={breed}
            onChange={(e) => setBreed(e.target.value)}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
      </fieldset>

      {/* Items */}
      <fieldset className="space-y-4">
        <legend className="text-lg font-semibold">Prescription Items</legend>
        {items.map((item, index) => (
          <div key={index} className="border rounded-lg p-3 space-y-3">
            <div className="flex justify-between items-center">
              <span className="font-medium text-sm">Item {index + 1}</span>
              {items.length > 1 && (
                <button
                  type="button"
                  onClick={() => removeItem(index)}
                  className="text-red-500 text-sm"
                  aria-label={`Remove item ${index + 1}`}
                >
                  Remove
                </button>
              )}
            </div>
            <div>
              <label
                htmlFor={`drugName-${index}`}
                className="block text-sm font-medium mb-1"
              >
                Drug Name
              </label>
              <input
                id={`drugName-${index}`}
                type="text"
                required
                value={item.drugName}
                onChange={(e) => updateItem(index, 'drugName', e.target.value)}
                className="w-full border rounded-lg px-3 py-2"
              />
            </div>
            <div>
              <label
                htmlFor={`quantity-${index}`}
                className="block text-sm font-medium mb-1"
              >
                Quantity
              </label>
              <input
                id={`quantity-${index}`}
                type="text"
                required
                value={item.quantity}
                onChange={(e) => updateItem(index, 'quantity', e.target.value)}
                className="w-full border rounded-lg px-3 py-2"
              />
            </div>
            <div>
              <label
                htmlFor={`pharmaForm-${index}`}
                className="block text-sm font-medium mb-1"
              >
                Pharmaceutical Form
              </label>
              <input
                id={`pharmaForm-${index}`}
                type="text"
                required
                value={item.pharmaceuticalForm}
                onChange={(e) =>
                  updateItem(index, 'pharmaceuticalForm', e.target.value)
                }
                className="w-full border rounded-lg px-3 py-2"
              />
            </div>
            <div>
              <label
                htmlFor={`dosage-${index}`}
                className="block text-sm font-medium mb-1"
              >
                Dosage Regimen
              </label>
              <input
                id={`dosage-${index}`}
                type="text"
                required
                value={item.dosageRegimen}
                onChange={(e) =>
                  updateItem(index, 'dosageRegimen', e.target.value)
                }
                className="w-full border rounded-lg px-3 py-2"
              />
            </div>
            <div>
              <label
                htmlFor={`withdrawal-${index}`}
                className="block text-sm font-medium mb-1"
              >
                Withdrawal Period (optional)
              </label>
              <input
                id={`withdrawal-${index}`}
                type="text"
                value={item.withdrawalPeriod ?? ''}
                onChange={(e) =>
                  updateItem(index, 'withdrawalPeriod', e.target.value)
                }
                className="w-full border rounded-lg px-3 py-2"
              />
            </div>
          </div>
        ))}
        <button
          type="button"
          onClick={addItem}
          className="w-full border-2 border-dashed border-gray-300 text-gray-600 py-2 rounded-lg text-sm"
        >
          + Add Item
        </button>
      </fieldset>

      {/* Warnings */}
      <div>
        <label htmlFor="warnings" className="block text-sm font-medium mb-1">
          Warnings (optional)
        </label>
        <textarea
          id="warnings"
          value={warnings}
          onChange={(e) => setWarnings(e.target.value)}
          rows={3}
          className="w-full border rounded-lg px-3 py-2"
        />
      </div>

      {/* Flags */}
      <div className="space-y-2">
        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={isOffLabel}
            onChange={(e) => setIsOffLabel(e.target.checked)}
          />
          <span className="text-sm">Off-label use (Art. 112)</span>
        </label>
        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={isAntimicrobial}
            onChange={(e) => setIsAntimicrobial(e.target.checked)}
          />
          <span className="text-sm">Antimicrobial special use</span>
        </label>
      </div>

      {error && (
        <p role="alert" className="text-red-600 text-sm">
          {error}
        </p>
      )}

      <button
        type="submit"
        disabled={submitting}
        className="w-full bg-blue-600 text-white py-3 px-4 rounded-lg font-medium disabled:opacity-50"
      >
        {submitting ? 'Creating…' : 'Create Prescription'}
      </button>
    </form>
  )
}
