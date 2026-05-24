import apiClient from '../../../shared/api-client'

export interface VetProfileRequest {
  name: string
  licenceNumber: string
  clinicName: string
  address: string
  phone: string
  email: string
}

export interface VetProfileResponse {
  name: string
  licenceNumber: string
  clinicName: string
  address: string
  phone: string
  email: string
}

export async function saveVetProfile(req: VetProfileRequest): Promise<VetProfileResponse> {
  const { data } = await apiClient.post<VetProfileResponse>('/vets/profile', req)
  return data
}

export async function getVetProfile(): Promise<VetProfileResponse> {
  const { data } = await apiClient.get<VetProfileResponse>('/vets/profile')
  return data
}
