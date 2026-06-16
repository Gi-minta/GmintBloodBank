export interface User {
  id: string;
  username: string;
  email: string;
  roleId: string;
  roleName: string;
  isActive: boolean;
  lastLoginAt?: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface Donor {
  id: string;
  donorCode: string;
  firstName: string;
  lastName: string;
  identification: string;
  dateOfBirth: string;
  bloodTypeId: string;
  bloodTypeCode: string;
  genderId: string;
  genderName: string;
  isEligible: boolean;
  lastDonationDate?: string;
  email?: string;
  phone?: string;
  createdAt: string;
}

export interface CreateDonorRequest {
  firstName: string;
  lastName: string;
  identification: string;
  dateOfBirth: string;
  bloodTypeId: string;
  genderId: string;
  email?: string;
  phone?: string;
}

export interface UpdateDonorRequest extends CreateDonorRequest {
  id: string;
}

export interface Donation {
  id: string;
  donationCode: string;
  donorId: string;
  donorName: string;
  bloodBankId: string;
  bloodBankName: string;
  statusId: string;
  statusCode: string;
  donationDate: string;
  volumeML: number;
  collectionBagCode: string;
  createdAt: string;
}

export interface CreateDonationRequest {
  donorId: string;
  bloodBankId: string;
  donationDate: string;
  volumeML: number;
  collectionBagCode: string;
}

export interface DonationAppointment {
  id: string;
  donorId: string;
  donorName: string;
  appointmentDate: string;
  status: string;
  bloodBankId: string;
  bloodBankName: string;
  notes?: string;
}

export interface CreateAppointmentRequest {
  donorId: string;
  appointmentDate: string;
  bloodBankId: string;
  notes?: string;
}

export interface DonationEvaluation {
  id: string;
  donationId: string;
  donationCode: string;
  evaluatedBy: string;
  isEligible: boolean;
  hemoglobinLevel: number;
  bloodPressure: string;
  notes?: string;
  evaluationDate: string;
}

export interface RegisterEvaluationRequest {
  donationId: string;
  evaluatedBy: string;
  isEligible: boolean;
  hemoglobinLevel: number;
  bloodPressure: string;
  notes?: string;
}

export interface BloodUnit {
  id: string;
  unitCode: string;
  qrCode: string;
  donationId: string;
  bloodTypeId: string;
  bloodTypeCode: string;
  componentId: string;
  componentName: string;
  statusId: string;
  statusCode: string;
  volumeML: number;
  collectionDate: string;
  expirationDate: string;
  isReleased: boolean;
}

export interface RegisterBloodUnitRequest {
  donationId: string;
  bloodTypeId: string;
  componentId: string;
  volumeML: number;
  collectionDate: string;
  expirationDate: string;
}

export interface BloodScreening {
  id: string;
  bloodUnitId: string;
  screeningType: string;
  result: string;
  performedBy: string;
  screeningDate: string;
  notes?: string;
}

export interface RegisterScreeningRequest {
  bloodUnitId: string;
  screeningType: string;
  result: string;
  performedBy: string;
  screeningDate: string;
  notes?: string;
}

export interface InventoryItem {
  bloodTypeCode: string;
  bloodTypeDescription: string;
  available: number;
  quarantined: number;
  reserved: number;
  total: number;
}

export interface InventoryMovement {
  id: string;
  bloodUnitId: string;
  unitCode: string;
  movementType: string;
  fromLocation: string;
  toLocation: string;
  performedBy: string;
  movementDate: string;
  notes?: string;
}

export interface RegisterMovementRequest {
  bloodUnitId: string;
  movementType: string;
  fromLocation?: string;
  toLocation?: string;
  notes?: string;
}

export interface StockSummary {
  bloodType: string;
  totalUnits: number;
  availableUnits: number;
  expiringIn30Days: number;
}

export interface DonationsReport {
  period: string;
  totalDonations: number;
  eligible: number;
  deferred: number;
  averageVolume: number;
}

export interface Tenant {
  id: string;
  code: string;
  name: string;
  connectionString?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateTenantRequest {
  code: string;
  name: string;
  connectionString?: string;
}

export interface LicenseStatus {
  tenantId: string;
  tenantName: string;
  planName: string;
  licenseKey: string;
  startDate: string;
  expirationDate: string;
  isActive: boolean;
  daysRemaining: number;
}

export interface FeatureFlag {
  id: string;
  key: string;
  description: string;
  scope: string;
  isEnabled: boolean;
}

export interface ExpiringUnit {
  id: string;
  unitCode: string;
  bloodTypeCode: string;
  expirationDate: string;
  daysRemaining: number;
  location: string;
}
