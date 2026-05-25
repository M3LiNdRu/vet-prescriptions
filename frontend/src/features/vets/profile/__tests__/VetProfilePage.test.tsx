import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { VetProfilePage } from '../VetProfilePage'
import * as api from '../api'

vi.mock('../api')

const mockedGet = vi.mocked(api.getVetProfile)
const mockedSave = vi.mocked(api.saveVetProfile)

const sampleProfile = {
  name: 'Dr. Joan',
  licenceNumber: 'CAT-1',
  clinicName: 'Clinic',
  address: 'Addr',
  phone: '+34600',
  email: 'j@j.cat',
}

describe('VetProfilePage', () => {
  beforeEach(() => {
    vi.resetAllMocks()
  })

  it('renders all fields', async () => {
    mockedGet.mockRejectedValue({ response: { status: 404 } })
    render(<VetProfilePage />)
    await waitFor(() => expect(screen.queryByRole('status', { name: /loading/i })).not.toBeInTheDocument())
    expect(screen.getByLabelText('Name')).toBeInTheDocument()
    expect(screen.getByLabelText('Licence Number')).toBeInTheDocument()
    expect(screen.getByLabelText('Clinic Name')).toBeInTheDocument()
    expect(screen.getByLabelText('Address')).toBeInTheDocument()
    expect(screen.getByLabelText('Phone')).toBeInTheDocument()
    expect(screen.getByLabelText('Email')).toBeInTheDocument()
  })

  it('pre-fills form when profile exists', async () => {
    mockedGet.mockResolvedValue(sampleProfile)
    render(<VetProfilePage />)

    await waitFor(() =>
      expect(screen.getByLabelText('Name')).toHaveValue('Dr. Joan'),
    )
    expect(screen.getByLabelText('Licence Number')).toHaveValue('CAT-1')
    expect(screen.getByLabelText('Email')).toHaveValue('j@j.cat')
  })

  it('shows empty form when no profile saved yet', async () => {
    mockedGet.mockRejectedValue({ response: { status: 404 } })
    render(<VetProfilePage />)

    await waitFor(() =>
      expect(screen.getByLabelText('Name')).toHaveValue(''),
    )
  })

  it('calls saveVetProfile on submit', async () => {
    mockedGet.mockRejectedValue({ response: { status: 404 } })
    mockedSave.mockResolvedValue(sampleProfile)

    const user = userEvent.setup()
    render(<VetProfilePage />)

    await waitFor(() => expect(screen.getByLabelText('Name')).toBeInTheDocument())
    await user.type(screen.getByLabelText('Name'), 'Dr. Joan')
    await user.type(screen.getByLabelText('Licence Number'), 'CAT-1')
    await user.type(screen.getByLabelText('Clinic Name'), 'Clinic')
    await user.type(screen.getByLabelText('Address'), 'Addr')
    await user.type(screen.getByLabelText('Phone'), '+34600')
    await user.type(screen.getByLabelText('Email'), 'j@j.cat')

    await user.click(screen.getByRole('button', { name: 'Save Profile' }))

    await waitFor(() => expect(mockedSave).toHaveBeenCalledOnce())
    expect(mockedSave).toHaveBeenCalledWith(sampleProfile)
  })

  it('shows success message after save', async () => {
    mockedGet.mockRejectedValue({ response: { status: 404 } })
    mockedSave.mockResolvedValue(sampleProfile)

    const user = userEvent.setup()
    render(<VetProfilePage />)

    await waitFor(() => expect(screen.getByLabelText('Name')).toBeInTheDocument())
    await user.type(screen.getByLabelText('Name'), 'Dr. Joan')
    await user.type(screen.getByLabelText('Licence Number'), 'CAT-1')
    await user.type(screen.getByLabelText('Clinic Name'), 'Clinic')
    await user.type(screen.getByLabelText('Address'), 'Addr')
    await user.type(screen.getByLabelText('Phone'), '+34600')
    await user.type(screen.getByLabelText('Email'), 'j@j.cat')

    await user.click(screen.getByRole('button', { name: 'Save Profile' }))

    await waitFor(() =>
      expect(screen.getByRole('status')).toHaveTextContent('Profile saved successfully'),
    )
  })
})
