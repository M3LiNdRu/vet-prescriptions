import { useEffect, useState } from 'react'
import { getVetProfile, saveVetProfile, type VetProfileRequest } from './api'

export function VetProfilePage() {
  const [name, setName] = useState('')
  const [licenceNumber, setLicenceNumber] = useState('')
  const [clinicName, setClinicName] = useState('')
  const [address, setAddress] = useState('')
  const [phone, setPhone] = useState('')
  const [email, setEmail] = useState('')

  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    getVetProfile()
      .then((profile) => {
        setName(profile.name)
        setLicenceNumber(profile.licenceNumber)
        setClinicName(profile.clinicName)
        setAddress(profile.address)
        setPhone(profile.phone)
        setEmail(profile.email)
      })
      .catch(() => {
        // 404 or network error — leave form empty
      })
      .finally(() => setLoading(false))
  }, [])

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSuccess(false)
    setSubmitting(true)

    const req: VetProfileRequest = {
      name, licenceNumber, clinicName, address, phone, email,
    }

    try {
      await saveVetProfile(req)
      setSuccess(true)
    } catch {
      setError('Error saving profile. Please try again.')
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) {
    return (
      <div className="p-4 max-w-lg mx-auto">
        <p role="status" className="text-gray-500">Loading profile…</p>
      </div>
    )
  }

  return (
    <form onSubmit={handleSubmit} className="p-4 max-w-lg mx-auto space-y-4">
      <h2 className="text-xl font-bold">My Profile</h2>

      <div>
        <label htmlFor="profileName" className="block text-sm font-medium mb-1">Name</label>
        <input
          id="profileName"
          type="text"
          required
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="w-full border rounded-lg px-3 py-2"
        />
      </div>

      <div>
        <label htmlFor="profileLicence" className="block text-sm font-medium mb-1">Licence Number</label>
        <input
          id="profileLicence"
          type="text"
          required
          value={licenceNumber}
          onChange={(e) => setLicenceNumber(e.target.value)}
          className="w-full border rounded-lg px-3 py-2"
        />
      </div>

      <div>
        <label htmlFor="profileClinic" className="block text-sm font-medium mb-1">Clinic Name</label>
        <input
          id="profileClinic"
          type="text"
          required
          value={clinicName}
          onChange={(e) => setClinicName(e.target.value)}
          className="w-full border rounded-lg px-3 py-2"
        />
      </div>

      <div>
        <label htmlFor="profileAddress" className="block text-sm font-medium mb-1">Address</label>
        <input
          id="profileAddress"
          type="text"
          required
          value={address}
          onChange={(e) => setAddress(e.target.value)}
          className="w-full border rounded-lg px-3 py-2"
        />
      </div>

      <div>
        <label htmlFor="profilePhone" className="block text-sm font-medium mb-1">Phone</label>
        <input
          id="profilePhone"
          type="tel"
          required
          value={phone}
          onChange={(e) => setPhone(e.target.value)}
          className="w-full border rounded-lg px-3 py-2"
        />
      </div>

      <div>
        <label htmlFor="profileEmail" className="block text-sm font-medium mb-1">Email</label>
        <input
          id="profileEmail"
          type="email"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full border rounded-lg px-3 py-2"
        />
      </div>

      {success && (
        <p role="status" className="text-green-700 text-sm font-medium">
          Profile saved successfully.
        </p>
      )}

      {error && (
        <p role="alert" className="text-red-600 text-sm">{error}</p>
      )}

      <button
        type="submit"
        disabled={submitting}
        className="w-full bg-blue-600 text-white py-3 px-4 rounded-lg font-medium disabled:opacity-50"
      >
        {submitting ? 'Saving…' : 'Save Profile'}
      </button>
    </form>
  )
}
