import { useState } from 'react'
import { CreatePrescriptionPage } from './features/prescriptions/create/CreatePrescriptionPage'
import { PrescriptionListPage } from './features/prescriptions/list/PrescriptionListPage'
import { VetProfilePage } from './features/vets/profile/VetProfilePage'

type Tab = 'prescription' | 'history' | 'profile'

function App() {
  const [tab, setTab] = useState<Tab>('prescription')
  function handleSelectPrescription(id: string) {
    // TODO Phase 5: navigate to detail view
    alert(`Prescription ${id} selected — detail view coming in Phase 5`)
  }

  const navBtn = (t: Tab, label: string) => (
    <button
      type="button"
      onClick={() => setTab(t)}
      className={`px-4 py-1.5 rounded-full text-sm font-medium ${
        tab === t ? 'bg-blue-600 text-white' : 'bg-gray-100 text-gray-600'
      }`}
    >
      {label}
    </button>
  )

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white border-b px-4 py-3">
        <h1 className="text-xl font-bold text-gray-900 mb-3">VetPrescription</h1>
        <nav className="flex gap-2">
          {navBtn('prescription', 'New Prescription')}
          {navBtn('history', 'History')}
          {navBtn('profile', 'My Profile')}
        </nav>
      </header>

      <main>
        {tab === 'prescription' && <CreatePrescriptionPage />}
        {tab === 'history' && (
          <PrescriptionListPage onSelect={handleSelectPrescription} />
        )}
        {tab === 'profile' && <VetProfilePage />}
      </main>
    </div>
  )
}

export default App
