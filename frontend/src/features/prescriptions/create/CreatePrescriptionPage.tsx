import { CreatePrescriptionForm } from './CreatePrescriptionForm'

export function CreatePrescriptionPage() {
  return (
    <main className="min-h-screen bg-gray-50">
      <header className="bg-white border-b px-4 py-3">
        <h1 className="text-xl font-bold text-gray-900">VetPrescription</h1>
      </header>
      <CreatePrescriptionForm />
    </main>
  )
}
