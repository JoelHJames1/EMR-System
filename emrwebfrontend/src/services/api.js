import axios from 'axios';

const API_BASE_URL = 'https://localhost:7099/api';

// Create axios instance with default config
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add auth token to requests
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${JSON.parse(token)}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Handle token expiration
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Authentication
export const authAPI = {
  login: (email, password) => api.post('/Auth/login', { email, password }),
  register: (data) => api.post('/Auth/register', data),
  logout: () => api.post('/Auth/logout'),
  getProfile: () => api.get('/Auth/profile'),
  getAllUsers: () => api.get('/User'),
  updateUser: (id, data) => api.put(`/User/${id}`, data),
  deleteUser: (id) => api.delete(`/User/${id}`),
};

// Dashboard
export const dashboardAPI = {
  getStatistics: () => api.get('/Dashboard/statistics'),
  getActivity: (hours = 24) => api.get(`/Dashboard/activity?hours=${hours}`),
  getAppointmentStats: (days = 7) => api.get(`/Dashboard/appointments/stats?days=${days}`),
  getProviderWorkload: (days = 30) => api.get(`/Dashboard/providers/workload?days=${days}`),
  getLabStats: () => api.get('/Dashboard/lab/stats'),
  getBillingSummary: (days = 30) => api.get(`/Dashboard/billing/summary?days=${days}`),
};

// Patient Management
export const patientAPI = {
  getAll: (page = 1, pageSize = 50, search = '') =>
    api.get(`/Patient?page=${page}&pageSize=${pageSize}&search=${search}`),
  getById: (id) => api.get(`/Patient/${id}`),
  create: (data) => api.post('/Patient', data),
  update: (id, data) => api.put(`/Patient/${id}`, data),
  delete: (id) => api.delete(`/Patient/${id}`),
  getAllergies: (id) => api.get(`/Patient/${id}/allergies`),
  getVitals: (id) => api.get(`/Patient/${id}/vitals`),
};

// Encounter Management
export const encounterAPI = {
  create: (data) => api.post('/Encounter', data),
  getByPatient: (patientId) => api.get(`/Encounter/patient/${patientId}`),
  getById: (id) => api.get(`/Encounter/${id}`),
  update: (id, data) => api.put(`/Encounter/${id}`, data),
  complete: (id) => api.post(`/Encounter/${id}/complete`),
};

// Diagnosis
export const diagnosisAPI = {
  create: (data) => api.post('/Diagnosis', data),
  getByEncounter: (encounterId) => api.get(`/Diagnosis/encounter/${encounterId}`),
  getByPatient: (patientId) => api.get(`/Diagnosis/patient/${patientId}`),
  update: (id, data) => api.put(`/Diagnosis/${id}`, data),
  delete: (id) => api.delete(`/Diagnosis/${id}`),
};

// Prescriptions
export const prescriptionAPI = {
  create: (data) => api.post('/Prescription', data),
  getByPatient: (patientId) => api.get(`/Prescription/patient/${patientId}`),
  getById: (id) => api.get(`/Prescription/${id}`),
  update: (id, data) => api.put(`/Prescription/${id}`, data),
  discontinue: (id) => api.post(`/Prescription/${id}/discontinue`),
  refill: (id) => api.post(`/Prescription/${id}/refill`),
};

// Medications
export const medicationAPI = {
  getAll: (page = 1, pageSize = 50, search = '') =>
    api.get(`/Medication?page=${page}&pageSize=${pageSize}&search=${search}`),
  search: (query) => api.get(`/Medication/search?query=${query}`),
  create: (data) => api.post('/Medication', data),
  update: (id, data) => api.put(`/Medication/${id}`, data),
  delete: (id) => api.delete(`/Medication/${id}`),
};

// Lab Orders
export const labOrderAPI = {
  create: (data) => api.post('/LabOrder', data),
  getByPatient: (patientId) => api.get(`/LabOrder/patient/${patientId}`),
  getById: (id) => api.get(`/LabOrder/${id}`),
  updateStatus: (id, status) => api.put(`/LabOrder/${id}/status`, { status }),
  addResult: (orderId, data) => api.post(`/LabOrder/${orderId}/result`, data),
};

// Appointments
export const appointmentAPI = {
  create: (data) => api.post('/Appointment', data),
  getByPatient: (patientId) => api.get(`/Appointment/patient/${patientId}`),
  getByProvider: (providerId, date) =>
    api.get(`/Appointment/provider/${providerId}?date=${date}`),
  update: (id, data) => api.put(`/Appointment/${id}`, data),
  cancel: (id, reason) => api.post(`/Appointment/${id}/cancel`, { reason }),
  getAvailableSlots: (providerId, date) =>
    api.get(`/Appointment/available-slots?providerId=${providerId}&date=${date}`),
};

// Clinical Notes
export const clinicalNoteAPI = {
  create: (data) => api.post('/ClinicalNote', data),
  getByEncounter: (encounterId) => api.get(`/ClinicalNote/encounter/${encounterId}`),
  sign: (id) => api.post(`/ClinicalNote/${id}/sign`),
  addAddendum: (id, text) => api.post(`/ClinicalNote/${id}/addendum`, { addendumText: text }),
};

// Procedures
export const procedureAPI = {
  create: (data) => api.post('/Procedure', data),
  getByEncounter: (encounterId) => api.get(`/Procedure/encounter/${encounterId}`),
  getByPatient: (patientId) => api.get(`/Procedure/patient/${patientId}`),
  update: (id, data) => api.put(`/Procedure/${id}`, data),
  delete: (id) => api.delete(`/Procedure/${id}`),
  complete: (id) => api.post(`/Procedure/${id}/complete`),
};

// Billing
export const billingAPI = {
  create: (data) => api.post('/Billing', data),
  getByPatient: (patientId) => api.get(`/Billing/patient/${patientId}`),
  getById: (id) => api.get(`/Billing/${id}`),
  recordPayment: (id, data) => api.post(`/Billing/${id}/payment`, data),
};

// Insurance
export const insuranceAPI = {
  getByPatient: (patientId) => api.get(`/Insurance/patient/${patientId}`),
  create: (data) => api.post('/Insurance', data),
  update: (id, data) => api.put(`/Insurance/${id}`, data),
  verify: (id) => api.get(`/Insurance/${id}/verify`),
  deactivate: (id) => api.delete(`/Insurance/${id}`),
};

// Allergies
export const allergyAPI = {
  getByPatient: (patientId) => api.get(`/Allergy/patient/${patientId}`),
  create: (data) => api.post('/Allergy', data),
  update: (id, data) => api.put(`/Allergy/${id}`, data),
  deactivate: (id) => api.delete(`/Allergy/${id}`),
};

// Immunizations
export const immunizationAPI = {
  getByPatient: (patientId) => api.get(`/Immunization/patient/${patientId}`),
  create: (data) => api.post('/Immunization', data),
  update: (id, data) => api.put(`/Immunization/${id}`, data),
  getHistory: (patientId) => api.get(`/Immunization/patient/${patientId}/history`),
};

// Providers
export const providerAPI = {
  getAll: (page = 1, pageSize = 50, specialization = '') =>
    api.get(`/Provider?page=${page}&pageSize=${pageSize}&specialization=${specialization}`),
  getById: (id) => api.get(`/Provider/${id}`),
  create: (data) => api.post('/Provider', data),
  update: (id, data) => api.put(`/Provider/${id}`, data),
  delete: (id) => api.delete(`/Provider/${id}`),
  getSchedule: (id, date) => api.get(`/Provider/${id}/schedule?date=${date}`),
};

// Observations (Vitals)
export const observationAPI = {
  create: (data) => api.post('/Observation', data),
  getByPatient: (patientId) => api.get(`/Observation/patient/${patientId}`),
  getByEncounter: (encounterId) => api.get(`/Observation/encounter/${encounterId}`),
};

// Care Plans
export const carePlanAPI = {
  create: (data) => api.post('/CarePlan', data),
  getByPatient: (patientId) => api.get(`/CarePlan/patient/${patientId}`),
  update: (id, data) => api.put(`/CarePlan/${id}`, data),
  delete: (id) => api.delete(`/CarePlan/${id}`),
  activate: (id) => api.post(`/CarePlan/${id}/activate`),
  complete: (id) => api.post(`/CarePlan/${id}/complete`),
  getActivities: (id) => api.get(`/CarePlan/${id}/activities`),
};

// Referrals
export const referralAPI = {
  create: (data) => api.post('/Referral', data),
  getByPatient: (patientId) => api.get(`/Referral/patient/${patientId}`),
  update: (id, data) => api.put(`/Referral/${id}`, data),
  delete: (id) => api.delete(`/Referral/${id}`),
  approve: (id) => api.post(`/Referral/${id}/approve`),
  complete: (id) => api.post(`/Referral/${id}/complete`),
};

export default api;