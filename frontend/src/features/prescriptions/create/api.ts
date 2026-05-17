import apiClient from '../../../shared/api-client'

export interface VetRequest {
  name: string
  licenceNumber: string
  clinicName: string
  address: string
  phone: string
  email: string
}

export interface OwnerRequest {
  name: string
  address: string
  phone: string
  cifDni: string
}

export interface PatientRequest {
  animalName: string
  species: string
  breed: string
}

export interface PrescriptionItemRequest {
  drugName: string
  quantity: string
  pharmaceuticalForm: string
  dosageRegimen: string
  withdrawalPeriod?: string
}

export interface CreatePrescriptionRequest {
  vet: VetRequest
  owner: OwnerRequest
  patient: PatientRequest
  items: PrescriptionItemRequest[]
  warnings?: string
  isOffLabel: boolean
  isAntimicrobialSpecialUse: boolean
}

export interface CreatePrescriptionResponse {
  id: string
  prescriptionNumber: string
  date: string
}

export interface PdfResponse {
  url: string
  prescriptionNumber: string
}

export async function createPrescription(
  req: CreatePrescriptionRequest,
): Promise<CreatePrescriptionResponse> {
  const { data } = await apiClient.post<CreatePrescriptionResponse>(
    '/prescriptions',
    req,
  )
  return data
}

export async function generatePdf(id: string): Promise<PdfResponse> {
  const { data } = await apiClient.post<PdfResponse>(
    `/prescriptions/${id}/pdf`,
  )
  return data
}

export async function getPdfUrl(id: string): Promise<PdfResponse> {
  const { data } = await apiClient.get<PdfResponse>(
    `/prescriptions/${id}/pdf`,
  )
  return data
}
