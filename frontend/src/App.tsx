import { useState } from 'react'
import { CreatePrescriptionPage } from './features/prescriptions/create/CreatePrescriptionPage'
import { VetProfilePage } from './features/vets/profile/VetProfilePage'

type Tab = 'prescription' | 'profile'

function App() {
  const [tab, setTab] = useState<Tab>('prescription')

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white border-b px-4 py-3">
        <h1 className="text-xl font-bold text-gray-900 mb-3">VetPrescription</h1>
        <nav className="flex gap-2">
          <button
            type="button"
            onClick={() => setTab('prescription')}
            className={`px-4 py-1.5 rounded-full text-sm font-medium ${
              tab === 'prescription'
                ? 'bg-blue-600 text-white'
                : 'bg-gray-100 text-gray-600'
            }`}
          >
            New Prescription
          </button>
          <button
            type="button"
            onClick={() => setTab('profile')}
            className={`px-4 py-1.5 rounded-full text-sm font-medium ${
              tab === 'profile'
                ? 'bg-blue-600 text-white'
                : 'bg-gray-100 text-gray-600'
            }`}
          >
            My Profile
          </button>
        </nav>
      </header>

      <main>
        {tab === 'prescription' && <CreatePrescriptionPage />}
        {tab === 'profile' && <VetProfilePage />}
      </main>
    </div>
  )
}

export default App
