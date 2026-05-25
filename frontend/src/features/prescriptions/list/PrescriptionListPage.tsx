import { useEffect, useState } from 'react'
import { listPrescriptions, type PrescriptionSummary } from './api'

interface Props {
  onSelect: (id: string) => void
}

export function PrescriptionListPage({ onSelect }: Props) {
  const [prescriptions, setPrescriptions] = useState<PrescriptionSummary[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    listPrescriptions()
      .then(setPrescriptions)
      .catch(() => setError('Error loading prescriptions.'))
      .finally(() => setLoading(false))
  }, [])

  if (loading) {
    return <p className="p-4 text-gray-500">Loading…</p>
  }

  if (error) {
    return <p role="alert" className="p-4 text-red-600">{error}</p>
  }

  if (prescriptions.length === 0) {
    return <p className="p-4 text-gray-500">No prescriptions yet.</p>
  }

  return (
    <ul className="divide-y divide-gray-100">
      {prescriptions.map((p) => (
        <li key={p.id}>
          <button
            type="button"
            onClick={() => onSelect(p.id)}
            className="w-full text-left px-4 py-3 hover:bg-gray-50"
          >
            <div className="flex justify-between items-center">
              <span className="font-medium text-gray-900">{p.prescriptionNumber}</span>
              <span className="text-sm text-gray-500">{p.date}</span>
            </div>
            <div className="text-sm text-gray-600 mt-0.5">
              {p.patientName} — {p.vetName}
            </div>
          </button>
        </li>
      ))}
    </ul>
  )
}
