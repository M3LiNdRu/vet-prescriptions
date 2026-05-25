import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { PrescriptionListPage } from '../PrescriptionListPage'
import * as api from '../api'

vi.mock('../api')

const mockedList = vi.mocked(api.listPrescriptions)

const sampleList = [
  { id: 'id1', prescriptionNumber: 'RX-2026-0002', date: '2026-05-17', patientName: 'Rex', vetName: 'Dr. Joan' },
  { id: 'id2', prescriptionNumber: 'RX-2026-0001', date: '2026-05-16', patientName: 'Luna', vetName: 'Dr. Joan' },
]

describe('PrescriptionListPage', () => {
  beforeEach(() => vi.resetAllMocks())

  it('shows loading then renders list', async () => {
    mockedList.mockResolvedValue(sampleList)
    render(<PrescriptionListPage onSelect={() => {}} />)

    expect(screen.getByText('Loading…')).toBeInTheDocument()
    await waitFor(() =>
      expect(screen.getByText('RX-2026-0002')).toBeInTheDocument(),
    )
    expect(screen.getByText('RX-2026-0001')).toBeInTheDocument()
  })

  it('shows empty state when no prescriptions', async () => {
    mockedList.mockResolvedValue([])
    render(<PrescriptionListPage onSelect={() => {}} />)

    await waitFor(() =>
      expect(screen.getByText('No prescriptions yet.')).toBeInTheDocument(),
    )
  })

  it('shows error when API fails', async () => {
    mockedList.mockRejectedValue(new Error('Network error'))
    render(<PrescriptionListPage onSelect={() => {}} />)

    await waitFor(() =>
      expect(screen.getByRole('alert')).toHaveTextContent('Error loading prescriptions'),
    )
  })

  it('calls onSelect with correct id when item clicked', async () => {
    mockedList.mockResolvedValue(sampleList)
    const onSelect = vi.fn()
    const user = userEvent.setup()

    render(<PrescriptionListPage onSelect={onSelect} />)
    await waitFor(() => screen.getByText('RX-2026-0002'))

    await user.click(screen.getByText('RX-2026-0002'))
    expect(onSelect).toHaveBeenCalledWith('id1')
  })
})
