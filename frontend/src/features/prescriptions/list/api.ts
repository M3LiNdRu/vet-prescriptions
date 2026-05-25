import apiClient from '../../../shared/api-client'

export interface PrescriptionSummary {
  id: string
  prescriptionNumber: string
  date: string
  patientName: string
  vetName: string
}

export async function listPrescriptions(): Promise<PrescriptionSummary[]> {
  const { data } = await apiClient.get<PrescriptionSummary[]>('/prescriptions')
  return data
}
