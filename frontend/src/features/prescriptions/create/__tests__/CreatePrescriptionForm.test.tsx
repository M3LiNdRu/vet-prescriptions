import axios from 'axios'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { CreatePrescriptionForm } from '../CreatePrescriptionForm'
import * as api from '../api'

vi.mock('../api')

const mockedCreate = vi.mocked(api.createPrescription)
const mockedGenerate = vi.mocked(api.generatePdf)

function fillForm(user: ReturnType<typeof userEvent.setup>) {
  return async () => {
    await user.type(screen.getByLabelText('Name', { selector: '#vetName' }), 'Dr. Joan')
    await user.type(screen.getByLabelText('Licence Number'), 'CAT-1')
    await user.type(screen.getByLabelText('Clinic Name'), 'Clinic')
    await user.type(screen.getByLabelText('Address', { selector: '#vetAddress' }), 'Addr')
    await user.type(screen.getByLabelText('Phone', { selector: '#vetPhone' }), '+34600')
    await user.type(screen.getByLabelText('Email'), 'j@j.cat')
    await user.type(screen.getByLabelText('Name', { selector: '#ownerName' }), 'Owner')
    await user.type(screen.getByLabelText('Address', { selector: '#ownerAddress' }), 'Addr')
    await user.type(screen.getByLabelText('Phone', { selector: '#ownerPhone' }), '+34600')
    await user.type(screen.getByLabelText('CIF/DNI'), '12345678A')
    await user.type(screen.getByLabelText('Animal Name'), 'Rex')
    await user.type(screen.getByLabelText('Species'), 'Canis')
    await user.type(screen.getByLabelText('Breed'), 'Labrador')
    await user.type(screen.getByLabelText('Drug Name'), 'DrugA')
    await user.type(screen.getByLabelText('Quantity'), '1 box')
    await user.type(screen.getByLabelText('Pharmaceutical Form'), 'Tablets')
    await user.type(screen.getByLabelText('Dosage Regimen'), '1/day')
  }
}

describe('CreatePrescriptionForm', () => {
  beforeEach(() => {
    vi.resetAllMocks()
  })

  it('renders all sections', () => {
    render(<CreatePrescriptionForm />)
    expect(screen.getByText('Veterinarian')).toBeInTheDocument()
    expect(screen.getByText('Owner')).toBeInTheDocument()
    expect(screen.getByText('Patient')).toBeInTheDocument()
    expect(screen.getByText('Prescription Items')).toBeInTheDocument()
    expect(screen.getByLabelText('Warnings (optional)')).toBeInTheDocument()
    expect(screen.getByLabelText(/Off-label/)).toBeInTheDocument()
    expect(screen.getByLabelText(/Antimicrobial/)).toBeInTheDocument()
  })

  it('calls createPrescription on valid submit and shows prescription number', async () => {
    mockedCreate.mockResolvedValue({
      id: 'id1',
      prescriptionNumber: 'RX-2026-0001',
      date: '2026-05-17',
    })

    const user = userEvent.setup()
    render(<CreatePrescriptionForm />)
    await fillForm(user)()
    await user.click(screen.getByRole('button', { name: 'Create Prescription' }))

    await waitFor(() =>
      expect(screen.getByText('RX-2026-0001')).toBeInTheDocument(),
    )
    expect(mockedCreate).toHaveBeenCalledOnce()
  })

  it('shows error message on API failure', async () => {
    mockedCreate.mockRejectedValue(new Error('Network error'))

    const user = userEvent.setup()
    render(<CreatePrescriptionForm />)
    await fillForm(user)()
    await user.click(screen.getByRole('button', { name: 'Create Prescription' }))

    await waitFor(() =>
      expect(screen.getByRole('alert')).toHaveTextContent(
        'Error creating prescription',
      ),
    )
  })

  it('shows Generate PDF button after success and opens PDF link', async () => {
    mockedCreate.mockResolvedValue({
      id: 'id1',
      prescriptionNumber: 'RX-2026-0001',
      date: '2026-05-17',
    })
    mockedGenerate.mockResolvedValue({
      url: 'http://localhost:8080/pdfs/RX-2026-0001.pdf',
      prescriptionNumber: 'RX-2026-0001',
    })

    const user = userEvent.setup()
    render(<CreatePrescriptionForm />)
    await fillForm(user)()
    await user.click(screen.getByRole('button', { name: 'Create Prescription' }))
    await waitFor(() =>
      expect(screen.getByRole('button', { name: 'Generate PDF' })).toBeInTheDocument(),
    )

    await user.click(screen.getByRole('button', { name: 'Generate PDF' }))

    await waitFor(() =>
      expect(screen.getByRole('link', { name: 'Open PDF' })).toBeInTheDocument(),
    )
    expect(mockedGenerate).toHaveBeenCalledWith('id1')
  })

  it('shows field-level errors on 422 response', async () => {
    const axiosError = new axios.AxiosError('Validation failed')
    axiosError.response = {
      status: 422,
      data: { errors: { 'Vet.Name': ["'Name' must not be empty."] } },
      statusText: 'Unprocessable Entity',
      headers: {},
      config: {} as never,
    }
    mockedCreate.mockRejectedValue(axiosError)

    const user = userEvent.setup()
    render(<CreatePrescriptionForm />)
    await fillForm(user)()
    await user.click(screen.getByRole('button', { name: 'Create Prescription' }))

    await waitFor(() =>
      expect(screen.getByText("'Name' must not be empty.")).toBeInTheDocument(),
    )
    expect(screen.getByRole('alert')).toHaveTextContent('Please fix the validation errors')
  })

  it('shows error when PDF generation fails', async () => {
    mockedCreate.mockResolvedValue({
      id: 'id1',
      prescriptionNumber: 'RX-2026-0001',
      date: '2026-05-17',
    })
    mockedGenerate.mockRejectedValue(new Error('PDF error'))

    const user = userEvent.setup()
    render(<CreatePrescriptionForm />)
    await fillForm(user)()
    await user.click(screen.getByRole('button', { name: 'Create Prescription' }))
    await waitFor(() =>
      expect(screen.getByRole('button', { name: 'Generate PDF' })).toBeInTheDocument(),
    )

    await user.click(screen.getByRole('button', { name: 'Generate PDF' }))

    await waitFor(() =>
      expect(screen.getByRole('alert')).toHaveTextContent('Error generating PDF'),
    )
  })

  it('can add and remove prescription items', async () => {
    render(<CreatePrescriptionForm />)
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: '+ Add Item' }))
    expect(screen.getByText('Item 2')).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: 'Remove item 1' }))
    expect(screen.queryByText('Item 2')).not.toBeInTheDocument()
  })
})
