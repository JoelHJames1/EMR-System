# 🏥 EMR System - Enterprise Electronic Medical Records

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE.txt)
[![HL7 FHIR](https://img.shields.io/badge/HL7-FHIR%20Compliant-orange)](https://www.hl7.org/fhir/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com/JoelHJames1/EMR-System)

## 📋 Overview

A **comprehensive, HL7 FHIR-compliant** Electronic Medical Records (EMR) system built with .NET 8, designed to digitize and streamline all aspects of healthcare operations. This enterprise-grade solution covers the complete spectrum of clinical, administrative, and financial workflows in modern healthcare facilities.

### 🎯 Key Features

- ✅ **HL7 FHIR Standard Compliance** - Aligned with international healthcare data standards
- 🔐 **Enterprise Security** - JWT authentication with role-based access control (RBAC)
- 🏥 **Complete Clinical Workflow** - From patient registration to discharge
- 💊 **Medication Management** - Prescription tracking with DEA controlled substance support
- 🧪 **Laboratory Integration** - Orders, results, and LOINC coding
- 💰 **Billing & Insurance** - Claims processing with CPT/ICD-10 coding
- 📊 **Clinical Documentation** - SOAP notes, diagnoses, procedures
- 📱 **RESTful API** - Modern API architecture with Swagger documentation
- 🔄 **Interoperability Ready** - Designed for integration with external systems

## 🏗️ Architecture

### Technology Stack

```
Frontend:  React 18.2 + Material-UI 5 + Framer Motion
Backend:   ASP.NET Core 8.0 Web API
ORM:       Entity Framework Core 8.0.11
Database:  SQL Server 2019+
Auth:      JWT Bearer Token with Refresh Token
Logging:   NLog
API Docs:  Swagger/OpenAPI 3.0
Charts:    Recharts 3.2.1
Forms:     React Hook Form 7.63.0
```

### Layered Architecture

```
┌─────────────────────────────────┐
│    Presentation Layer           │  React Frontend / Swagger UI
├─────────────────────────────────┤
│    API Layer                    │  Controllers (RESTful)
├─────────────────────────────────┤
│    Business Logic Layer         │  Services & Domain Logic
├─────────────────────────────────┤
│    Data Access Layer            │  Repositories + EF Core
├─────────────────────────────────┤
│    Database Layer               │  SQL Server
└─────────────────────────────────┘
```

## 📊 Database Schema

### HL7 FHIR Core Resources (27 Entities)

#### 👥 **Patient Management**
- `Patient` - Complete demographics, contacts, insurance
- `FamilyHistory` - Hereditary conditions and family medical history
- `Allergy` - Drug/food/environmental allergies with severity
- `Immunization` - Vaccination records with CVX codes

#### 🏥 **Clinical Workflow**
- `Encounter` - Patient-provider interactions (visits, admissions)
- `Diagnosis` - ICD-10/ICD-11 coded conditions
- `Procedure` - CPT coded surgical/diagnostic procedures
- `Observation` - LOINC coded clinical measurements
- `ClinicalNote` - SOAP format documentation
- `VitalSign` - Temperature, BP, heart rate, oxygen saturation
- `CarePlan` - Treatment plans and care coordination
- `CarePlanActivity` - Individual care plan tasks

#### 💊 **Medication Management**
- `Medication` - Drug database with NDC codes
- `Prescription` - Dosage, frequency, refills
- DEA Schedule support for controlled substances

#### 🧪 **Laboratory & Diagnostics**
- `LabOrder` - Test requisitions with priority
- `LabResult` - Results with reference ranges and flags
- LOINC code support

#### 📅 **Scheduling & Referrals**
- `Appointment` - Scheduling with status workflow
- `Referral` - Specialist referrals and consults
- `Location` - Rooms, wards, facilities
- `Department` - Hospital departments and specialties

#### 💰 **Billing & Financial**
- `Billing` - Invoicing and payment tracking
- `BillingItem` - Line items with CPT/ICD codes
- `Insurance` - Policy information and claims

#### 👨‍⚕️ **Provider & Administration**
- `Provider` - Physicians, nurses, specialists
- `User` - System users with role assignments
- `Document` - Clinical documents and reports
- `MedicalRecord` - Legacy medical records

### Medical Coding Standards Support

| Standard | Purpose | Implementation |
|----------|---------|----------------|
| **ICD-10/11** | Diagnosis coding | `Diagnosis.ICDCode` |
| **CPT** | Procedure coding | `Procedure.CPTCode`, `BillingItem.CPTCode` |
| **NDC** | Medication identification | `Medication.NDC` |
| **LOINC** | Lab observations | `Observation.ObservationCode` |
| **CVX** | Vaccine codes | `Immunization.CVXCode` |
| **SNOMED CT** | Clinical terminology | `Procedure.SNOMEDCode` |
| **DEA** | Controlled substances | `Medication.DEASchedule` |

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or Full)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Node.js 18+](https://nodejs.org/) (for frontend)

### Installation

#### 1. Clone the Repository

```bash
git clone https://github.com/JoelHJames1/EMR-System.git
cd EMR-System
```

#### 2. Configure Database Connection

Edit `EMRWebAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=EMRSystemDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**For LocalDB (Development):**
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EMRSystemDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

#### 3. Update JWT Configuration (Required for Security)

⚠️ **Important**: Change the JWT secret key in production!

```json
{
  "Jwt": {
    "Key": "YOUR-SECURE-SECRET-KEY-AT-LEAST-32-CHARACTERS-LONG",
    "Issuer": "EMRSystem",
    "Audience": "EMRSystemUsers",
    "ExpireHours": "24"
  }
}
```

#### 4. Restore NuGet Packages

```bash
cd EMRWebAPI
dotnet restore
```

#### 5. Create Database and Apply Migrations

```bash
# Create initial migration (if not exists)
dotnet ef migrations add InitialCreate --project ../EMRDataLayer --startup-project .

# Apply migrations to database
dotnet ef database update --project ../EMRDataLayer --startup-project .
```

#### 6. Run the Application

```bash
dotnet run
```

**API URL**: `https://localhost:7099`
**Swagger UI**: `https://localhost:7099/swagger`

### Frontend Setup

```bash
cd emrwebfrontend
npm install
npm start
```

**Frontend URL**: `http://localhost:3000`

The frontend will automatically proxy API requests to `https://localhost:7099`.

## 🔐 Authentication & Authorization

### Default Roles

The system implements 6 predefined healthcare roles with granular permissions. For detailed information about each role including responsibilities, workflows, and permission matrix, see **[ROLES.md](ROLES.md)**.

| Role | Permissions | Use Case |
|------|-------------|----------|
| **Administrator** | Full system access | System configuration, user management |
| **Doctor** | Clinical records, prescriptions, orders | Physicians, specialists |
| **Nurse** | Patient care, vitals, medication admin | Nursing staff |
| **Receptionist** | Appointments, patient registration | Front desk |
| **Lab Technician** | Lab orders and results | Laboratory staff |
| **Billing Staff** | Billing, insurance, payments | Financial department |

### Authentication Flow

1. **Register User** (Admin only):
```http
POST /api/Auth/register
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@hospital.com",
  "phoneNumber": "+1234567890",
  "password": "SecureP@ss123",
  "confirmPassword": "SecureP@ss123",
  "roles": ["Doctor"]
}
```

2. **Login**:
```http
POST /api/Auth/login
{
  "email": "john.doe@hospital.com",
  "password": "SecureP@ss123"
}
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "user": {
    "id": "user-guid",
    "email": "john.doe@hospital.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["Doctor"]
  }
}
```

3. **Use Token in Requests**:
```http
GET /api/Auth/profile
Authorization: Bearer {your-jwt-token}
```

## 📚 API Documentation

### Swagger UI

Navigate to `https://localhost:7099/swagger` for interactive API documentation.

### Core API Endpoints

#### Authentication
- `POST /api/Auth/register` - Register new user (Admin)
- `POST /api/Auth/login` - User login
- `POST /api/Auth/refresh-token` - Refresh access token
- `GET /api/Auth/profile` - Get current user profile
- `POST /api/Auth/logout` - Logout user

#### Patient Management
- `GET /api/Patient` - List all patients
- `GET /api/Patient/{id}` - Get patient details
- `POST /api/Patient` - Register new patient
- `PUT /api/Patient/{id}` - Update patient
- `DELETE /api/Patient/{id}` - Deactivate patient

#### Clinical Workflow
- `POST /api/Encounter` - Create encounter
- `POST /api/Diagnosis` - Add diagnosis
- `POST /api/Procedure` - Schedule/record procedure
- `POST /api/Observation` - Record observation
- `POST /api/ClinicalNote` - Create SOAP note

#### Medications
- `GET /api/Medication` - List medications
- `POST /api/Prescription` - Create prescription
- `GET /api/Prescription/patient/{id}` - Patient prescriptions

#### Laboratory
- `POST /api/LabOrder` - Create lab order
- `POST /api/LabResult` - Enter lab results
- `GET /api/LabOrder/patient/{id}` - Patient lab orders

#### Billing
- `POST /api/Billing` - Create invoice
- `POST /api/Insurance` - Add insurance
- `GET /api/Billing/patient/{id}` - Patient billing history

*See [TECHNICAL_DOCUMENTATION.md](TECHNICAL_DOCUMENTATION.md) for complete API specifications*

## 🔒 Security Features

### 1. **Authentication & Authorization**
- ✅ JWT Bearer Token authentication
- ✅ Role-based access control (RBAC)
- ✅ Policy-based authorization
- ✅ Refresh token mechanism

### 2. **Password Security**
- ✅ Minimum 8 characters
- ✅ Requires uppercase, lowercase, digit, special character
- ✅ PBKDF2 hashing (ASP.NET Core Identity)
- ✅ Account lockout after 5 failed attempts

### 3. **Data Protection**
- ✅ HTTPS enforcement
- ✅ SQL injection prevention (parameterized queries)
- ✅ Input validation
- ✅ CORS configuration

### 4. **Audit Trail**
- ✅ CreatedBy/ModifiedBy tracking
- ✅ Timestamp tracking
- ✅ NLog logging
- ✅ User action tracking

### 5. **HIPAA Compliance Considerations**
- ✅ Encrypted data transmission (HTTPS/TLS)
- ✅ Access controls and user authentication
- ✅ Audit logging
- ✅ Data backup capabilities
- ⚠️ **Note**: Full HIPAA compliance requires additional infrastructure controls

## 📈 Database Migrations

### Create New Migration
```bash
dotnet ef migrations add MigrationName --project EMRDataLayer --startup-project EMRWebAPI
```

### Apply Migrations
```bash
dotnet ef database update --project EMRDataLayer --startup-project EMRWebAPI
```

### Rollback Migration
```bash
dotnet ef database update PreviousMigrationName --project EMRDataLayer --startup-project EMRWebAPI
```

### Remove Last Migration
```bash
dotnet ef migrations remove --project EMRDataLayer --startup-project EMRWebAPI
```

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test
```

### API Testing
Use Swagger UI (`/swagger`) or tools like:
- [Postman](https://www.postman.com/)
- [Insomnia](https://insomnia.rest/)
- [REST Client (VS Code)](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)

## 📦 Deployment

### Development
```bash
dotnet run --environment Development
```

### Production

1. **Publish Application**:
```bash
dotnet publish -c Release -o ./publish
```

2. **Configure Production Settings**:
   - Update `appsettings.Production.json`
   - Set secure JWT key via environment variables
   - Configure production database connection
   - Enable HTTPS/TLS certificates

3. **Deploy to**:
   - Azure App Service
   - AWS Elastic Beanstalk
   - Docker containers
   - IIS (Windows Server)
   - Linux with Nginx/Apache

### Docker Deployment (Optional)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EMRWebAPI/EMRWebAPI.csproj", "EMRWebAPI/"]
COPY ["EMRDataLayer/EMRDataLayer.csproj", "EMRDataLayer/"]
RUN dotnet restore "EMRWebAPI/EMRWebAPI.csproj"
COPY . .
WORKDIR "/src/EMRWebAPI"
RUN dotnet build "EMRWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EMRWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EMRWebAPI.dll"]
```

## 🗂️ Project Structure

```
EMR-System/
├── 📁 EMRDataLayer/              # Data Access Layer
│   ├── 📁 Model/                 # Entity Models (27 entities)
│   │   ├── Patient.cs
│   │   ├── Encounter.cs
│   │   ├── Diagnosis.cs
│   │   ├── Procedure.cs
│   │   ├── Observation.cs
│   │   ├── ClinicalNote.cs
│   │   ├── Prescription.cs
│   │   ├── LabOrder.cs
│   │   ├── Billing.cs
│   │   └── ... (18 more)
│   ├── 📁 DataContext/
│   │   └── EMRDbContext.cs       # EF Core DbContext
│   ├── 📁 Repository/            # Repository Pattern
│   │   ├── IRepository/
│   │   └── Repository.cs
│   └── 📁 Migrations/            # EF Migrations
│
├── 📁 EMRWebAPI/                 # API Layer
│   ├── 📁 Controllers/           # API Controllers
│   │   ├── AuthController.cs
│   │   ├── UserController.cs
│   │   └── ...
│   ├── 📁 Services/              # Business Logic
│   │   ├── JwtService.cs
│   │   ├── UserService.cs
│   │   └── IServices/
│   ├── 📁 Model/                 # DTOs
│   │   ├── LoginDTO.cs
│   │   ├── RegisterDto.cs
│   │   └── ...
│   ├── 📁 AutoMapper/            # Object Mapping
│   ├── Program.cs                # Application Entry
│   ├── appsettings.json          # Configuration
│   └── appsettings.Development.json
│
├── 📁 emrwebfrontend/            # React Frontend
│   ├── 📁 src/
│   │   ├── 📁 components/        # React Components
│   │   │   ├── Login.js          # Modern login with animations
│   │   │   ├── DashboardLayout.js # Main layout with navigation
│   │   │   ├── UserContext.js    # Auth context
│   │   │   └── 📁 Dashboard/     # Feature modules
│   │   │       ├── EnhancedDashboard.js       # Analytics & statistics
│   │   │       ├── PatientManagement.js       # Patient CRUD
│   │   │       ├── AppointmentManagement.js   # Scheduling
│   │   │       ├── EncounterManagement.js     # Clinical visits
│   │   │       ├── PrescriptionManagement.js  # Medications
│   │   │       ├── LabOrderManagement.js      # Lab tests
│   │   │       ├── VitalsManagement.js        # Vital signs
│   │   │       ├── AllergyImmunizationManagement.js # Allergies/Vaccines
│   │   │       └── BillingManagement.js       # Invoices/Payments
│   │   ├── 📁 services/          # API Services
│   │   │   └── api.js            # Axios API layer (all endpoints)
│   │   ├── 📁 utils/             # Utilities
│   │   │   └── printDocument.js  # Document printing
│   │   └── App.js                # Main app with routing
│   ├── 📁 public/
│   └── package.json
│
├── 📄 README.md                  # This file
├── 📄 TECHNICAL_DOCUMENTATION.md # Detailed technical docs
├── 📄 ROLES.md                   # Healthcare roles documentation
├── 📄 LICENSE.txt                # MIT License
└── 📄 .gitignore
```

## 🎨 Frontend Components

### 1. **EnhancedDashboard** - Analytics & Real-Time Statistics
**File**: `emrwebfrontend/src/components/Dashboard/EnhancedDashboard.js`

**Features**:
- Real-time statistics cards with trend indicators (patients, appointments, prescriptions, lab orders)
- Interactive charts: Area chart (30-day appointments), Pie chart (lab order status)
- Financial overview with collection rate calculation
- Provider workload leaderboard (top 5 providers)
- Recent activity feed (48-hour window)

**Backend Integration**:
```javascript
dashboardAPI.getStatistics()          → GET /api/Dashboard/statistics
dashboardAPI.getAppointmentStats(30)  → GET /api/Dashboard/appointments?days=30
dashboardAPI.getLabStats()            → GET /api/Dashboard/lab-statistics
dashboardAPI.getBillingSummary(30)    → GET /api/Dashboard/billing-summary?days=30
dashboardAPI.getProviderWorkload(30)  → GET /api/Dashboard/provider-workload?days=30
dashboardAPI.getActivity(48)          → GET /api/Dashboard/recent-activity?hours=48
```

**Controllers Used**: `DashboardController.cs`

---

### 2. **PatientManagement** - Patient CRUD Operations
**File**: `emrwebfrontend/src/components/Dashboard/PatientManagement.js`

**Features**:
- Patient list with search functionality
- Create/Edit patient demographics (name, DOB, gender, contact, address, insurance)
- View patient details with tabs (Demographics, Allergies, Immunizations)
- Delete/deactivate patients
- Form validation with react-hook-form

**Backend Integration**:
```javascript
patientAPI.getAll()         → GET /api/Patient
patientAPI.getById(id)      → GET /api/Patient/{id}
patientAPI.create(data)     → POST /api/Patient
patientAPI.update(id, data) → PUT /api/Patient/{id}
patientAPI.delete(id)       → DELETE /api/Patient/{id}
```

**Controllers Used**: `PatientController.cs`
**Entities**: `Patient`, `Allergy`, `Immunization`

---

### 3. **AppointmentManagement** - Scheduling System
**File**: `emrwebfrontend/src/components/Dashboard/AppointmentManagement.js`

**Features**:
- Calendar view with appointment listing
- Create appointments (patient, provider, date/time, type, reason)
- Status workflow: Scheduled → Confirmed → CheckedIn → Completed/Cancelled
- Patient and provider dropdowns
- Appointment types: Consultation, Follow-up, Procedure, Lab Work, Imaging

**Backend Integration**:
```javascript
appointmentAPI.getAll()           → GET /api/Appointment
appointmentAPI.create(data)       → POST /api/Appointment
appointmentAPI.updateStatus(id)   → PUT /api/Appointment/{id}/status
appointmentAPI.cancel(id)         → PUT /api/Appointment/{id}/cancel
```

**Controllers Used**: `AppointmentController.cs`
**Entities**: `Appointment`, `Patient`, `Provider`

---

### 4. **EncounterManagement** - Clinical Visit Tracking
**File**: `emrwebfrontend/src/components/Dashboard/EncounterManagement.js`

**Features**:
- Patient selection sidebar
- Create encounters (type, provider, reason for visit)
- Encounter types: Outpatient, Inpatient, Emergency, Virtual, Home Health
- View encounter details with tabs (Overview, Clinical Notes, Vitals)
- Complete encounter workflow (InProgress → Finished)
- Clinical notes display (SOAP format)

**Backend Integration**:
```javascript
encounterAPI.getByPatient(patientId)  → GET /api/Encounter/patient/{patientId}
encounterAPI.create(data)             → POST /api/Encounter
encounterAPI.getById(id)              → GET /api/Encounter/{id}
encounterAPI.complete(id)             → PUT /api/Encounter/{id}/complete
clinicalNoteAPI.getByEncounter(id)    → GET /api/ClinicalNote/encounter/{id}
```

**Controllers Used**: `EncounterController.cs`, `ClinicalNoteController.cs`
**Entities**: `Encounter`, `ClinicalNote`, `Patient`, `Provider`

---

### 5. **PrescriptionManagement** - Medication Management
**File**: `emrwebfrontend/src/components/Dashboard/PrescriptionManagement.js`

**Features**:
- Patient selection with prescription history
- Create prescriptions (medication, dosage, frequency, route, duration, refills)
- Medication search/autocomplete
- Refill management
- Print prescription button (professional document generation)
- Status tracking: Active, Completed, Cancelled

**Backend Integration**:
```javascript
prescriptionAPI.getByPatient(patientId) → GET /api/Prescription/patient/{patientId}
prescriptionAPI.create(data)            → POST /api/Prescription
prescriptionAPI.refill(id)              → POST /api/Prescription/{id}/refill
medicationAPI.getAll()                  → GET /api/Medication
```

**Printing**: `printPrescription(prescription, patient, provider)` generates HTML-to-PDF document

**Controllers Used**: `PrescriptionController.cs`, `MedicationController.cs`
**Entities**: `Prescription`, `Medication`, `Patient`, `Provider`

---

### 6. **LabOrderManagement** - Laboratory Test Management
**File**: `emrwebfrontend/src/components/Dashboard/LabOrderManagement.js`

**Features**:
- Patient selection with lab order history
- Create lab orders (test type, priority, specimen, instructions)
- Common lab tests: CBC, BMP, CMP, Lipid Panel, Liver Panel, TSH, HbA1c, Urinalysis
- Priority levels: STAT, Urgent, Routine
- Status workflow: Ordered → In Progress → Completed
- Enter lab results with values, units, reference ranges, flags (Normal/Abnormal)
- LOINC code support

**Backend Integration**:
```javascript
labOrderAPI.getByPatient(patientId) → GET /api/LabOrder/patient/{patientId}
labOrderAPI.create(data)            → POST /api/LabOrder
labOrderAPI.updateStatus(id, status)→ PUT /api/LabOrder/{id}/status
labOrderAPI.addResult(orderId, data)→ POST /api/LabResult
```

**Controllers Used**: `LabOrderController.cs`, `LabResultController.cs`
**Entities**: `LabOrder`, `LabResult`, `Patient`, `Provider`

---

### 7. **VitalsManagement** - Vital Signs Recording
**File**: `emrwebfrontend/src/components/Dashboard/VitalsManagement.js`

**Features**:
- Patient selection with vitals history
- Latest vitals display cards (Blood Pressure, Heart Rate, Temperature, O2 Saturation)
- Record vital signs form with validation:
  - Blood Pressure (Systolic/Diastolic in mmHg)
  - Heart Rate (bpm)
  - Temperature (°F)
  - Respiratory Rate (/min)
  - Oxygen Saturation (%)
  - Weight (lbs)
  - Height (inches)
- History table with all measurements

**Backend Integration**:
```javascript
observationAPI.getByPatient(patientId) → GET /api/Observation/patient/{patientId}
observationAPI.create(data)            → POST /api/Observation
```

**Data Structure**: Stored as `Observation` with `components` array
```javascript
components: [
  { name: 'Blood Pressure', value: '120/80', unit: 'mmHg' },
  { name: 'Heart Rate', value: '72', unit: 'bpm' },
  // ... more vitals
]
```

**Controllers Used**: `ObservationController.cs`
**Entities**: `Observation`, `Patient`

---

### 8. **AllergyImmunizationManagement** - Allergy & Vaccine Tracking
**File**: `emrwebfrontend/src/components/Dashboard/AllergyImmunizationManagement.js`

**Features**:
- Tabbed interface (Allergies / Immunizations)
- **Allergies**:
  - Add allergies (allergen, type, severity, reaction, notes)
  - Allergy types: Drug/Medication, Food, Environmental, Other
  - Severity levels: Mild, Moderate, Severe, Critical
  - Visual severity badges with color coding
  - Deactivate allergies
- **Immunizations**:
  - Add vaccine records (name, CVX code, dose number, lot number, manufacturer, administered by)
  - History display with all vaccine details

**Backend Integration**:
```javascript
allergyAPI.getByPatient(patientId)      → GET /api/Allergy/patient/{patientId}
allergyAPI.create(data)                 → POST /api/Allergy
allergyAPI.deactivate(id)               → PUT /api/Allergy/{id}/deactivate
immunizationAPI.getByPatient(patientId) → GET /api/Immunization/patient/{patientId}
immunizationAPI.create(data)            → POST /api/Immunization
```

**Controllers Used**: `AllergyController.cs`, `ImmunizationController.cs`
**Entities**: `Allergy`, `Immunization`, `Patient`

---

### 9. **BillingManagement** - Financial & Insurance Management
**File**: `emrwebfrontend/src/components/Dashboard/BillingManagement.js`

**Features**:
- Patient selection with billing history
- Create invoices (service description, amount, CPT/ICD codes, notes)
- Invoice table showing:
  - Invoice number (INV-{id})
  - Date, Total amount, Paid amount, Balance
  - Status (Pending, Paid, Overdue)
- Record payments:
  - Payment amount with balance validation
  - Payment methods: Cash, Credit Card, Debit Card, Insurance, Check
- Print invoice functionality
- Automatic due date calculation (30 days)

**Backend Integration**:
```javascript
billingAPI.getByPatient(patientId)         → GET /api/Billing/patient/{patientId}
billingAPI.create(data)                    → POST /api/Billing
billingAPI.recordPayment(billingId, data)  → POST /api/Billing/{id}/payment
```

**Payment Processing**:
```javascript
{
  amount: parseFloat(data.amount),
  paymentMethod: data.paymentMethod,
  paymentDate: new Date().toISOString()
}
```

**Controllers Used**: `BillingController.cs`, `InsuranceController.cs`
**Entities**: `Billing`, `BillingItem`, `Insurance`, `Patient`

---

### 10. **Login Component** - Modern Authentication
**File**: `emrwebfrontend/src/components/Login.js`

**Features**:
- Modern UI with Framer Motion animations
- Password strength validator with visual progress bar
- Real-time password strength calculation:
  - 25% - Length ≥ 8 characters
  - 25% - Mixed case (uppercase + lowercase)
  - 25% - Contains digits
  - 25% - Contains special characters
- Form validation with react-hook-form
- JWT token storage in localStorage
- Auto-redirect to dashboard on successful login

**Backend Integration**:
```javascript
authAPI.login(credentials) → POST /api/Auth/login
```

**Response**:
```json
{
  "token": "jwt-token-string",
  "refreshToken": "refresh-token-string",
  "user": {
    "id": "user-guid",
    "email": "user@email.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["Doctor"]
  }
}
```

**Controllers Used**: `AuthController.cs`

---

### 11. **API Service Layer** - Centralized API Management
**File**: `emrwebfrontend/src/services/api.js`

**Features**:
- Axios instance with JWT token interceptor
- All API endpoints mapped to backend controllers
- Automatic token injection in request headers
- 401 error handling (auto-redirect to login)
- Response interceptor for error handling

**API Modules**:
```javascript
authAPI              → /api/Auth/*
patientAPI           → /api/Patient/*
appointmentAPI       → /api/Appointment/*
encounterAPI         → /api/Encounter/*
prescriptionAPI      → /api/Prescription/*
labOrderAPI          → /api/LabOrder/*
observationAPI       → /api/Observation/*
allergyAPI           → /api/Allergy/*
immunizationAPI      → /api/Immunization/*
billingAPI           → /api/Billing/*
dashboardAPI         → /api/Dashboard/*
providerAPI          → /api/Provider/*
medicationAPI        → /api/Medication/*
clinicalNoteAPI      → /api/ClinicalNote/*
diagnosisAPI         → /api/Diagnosis/*
procedureAPI         → /api/Procedure/*
insuranceAPI         → /api/Insurance/*
```

**Token Management**:
```javascript
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${JSON.parse(token)}`;
  }
  return config;
});
```

---

### 12. **Document Printing Utility**
**File**: `emrwebfrontend/src/utils/printDocument.js`

**Features**:
- Professional HTML-to-PDF document generation
- Three document types:

**1. Prescription**:
```javascript
printPrescription(prescription, patient, provider)
```
- Provider information (name, NPI, DEA, contact)
- Patient demographics
- Prescription details (medication, dosage, frequency, route, duration, refills)
- Signature line and date
- Controlled substance warnings (if applicable)

**2. Lab Order**:
```javascript
printLabOrder(labOrder, patient, provider)
```
- Provider information
- Patient demographics
- Lab test details (test type, LOINC code, priority, specimen)
- Special instructions
- Collection date and signature

**3. Patient Summary**:
```javascript
printPatientSummary(patient, encounters, allergies, prescriptions)
```
- Patient demographics
- Active allergies
- Current medications
- Recent encounters
- Comprehensive patient overview

**Styling**: Print-optimized CSS with proper page breaks, professional fonts, and medical document formatting

---

## 🔗 Backend-Frontend Integration Architecture

### Data Flow

```
┌─────────────────┐
│  React Frontend │
│  (Port 3000)    │
└────────┬────────┘
         │
         │ Axios HTTP Request (JWT Token)
         │
         ▼
┌─────────────────┐
│  ASP.NET Core   │
│  Web API        │
│  (Port 7099)    │
└────────┬────────┘
         │
         │ JWT Validation → Role-Based Authorization
         │
         ▼
┌─────────────────┐
│  Controllers    │
│  (18 endpoints) │
└────────┬────────┘
         │
         │ Business Logic & Validation
         │
         ▼
┌─────────────────┐
│  Repositories   │
│  (EF Core)      │
└────────┬────────┘
         │
         │ LINQ Queries
         │
         ▼
┌─────────────────┐
│  SQL Server     │
│  Database       │
│  (27 tables)    │
└─────────────────┘
```

### Authentication Flow

1. **Login**: User submits credentials → `AuthController.Login()`
2. **Token Generation**: JWT token with user claims + refresh token
3. **Token Storage**: Frontend stores token in localStorage
4. **Request Interceptor**: Axios adds token to all requests
5. **Token Validation**: Backend validates JWT on each request
6. **Authorization**: Role-based policies check permissions
7. **Response**: Data returned or 401 Unauthorized

### Component-to-Controller Mapping

| Frontend Component | Backend Controller(s) | Entities Used |
|-------------------|----------------------|---------------|
| EnhancedDashboard | DashboardController | All entities (statistics) |
| PatientManagement | PatientController | Patient, Allergy, Immunization |
| AppointmentManagement | AppointmentController | Appointment, Patient, Provider |
| EncounterManagement | EncounterController, ClinicalNoteController | Encounter, ClinicalNote, Patient, Provider |
| PrescriptionManagement | PrescriptionController, MedicationController | Prescription, Medication, Patient, Provider |
| LabOrderManagement | LabOrderController, LabResultController | LabOrder, LabResult, Patient, Provider |
| VitalsManagement | ObservationController | Observation, Patient |
| AllergyImmunizationManagement | AllergyController, ImmunizationController | Allergy, Immunization, Patient |
| BillingManagement | BillingController, InsuranceController | Billing, BillingItem, Insurance, Patient |
| Login | AuthController | User, AspNetRoles, AspNetUserRoles |

---

## 🌟 Roadmap & Future Enhancements

### Phase 1 (Current - ✅ COMPLETED)
- ✅ Core EMR functionality with 9 modules
- ✅ HL7 FHIR compliance (27 entities)
- ✅ JWT authentication with refresh tokens
- ✅ RESTful API (100+ endpoints)
- ✅ Modern React frontend with Material-UI
- ✅ Real-time dashboard with analytics
- ✅ Document printing (prescriptions, lab orders, patient summaries)

### Phase 2 (Planned)
- [ ] Patient Portal (self-service)
- [ ] Mobile applications (iOS/Android)
- [ ] Telemedicine integration
- [ ] E-prescribing integration (SureScripts)
- [ ] SignalR for real-time notifications

### Phase 3 (Future)
- [ ] AI-powered diagnostic assistance
- [ ] Predictive analytics
- [ ] DICOM viewer (radiology)
- [ ] Advanced reporting & dashboards
- [ ] Multi-language support

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## 📞 Support & Contact

- **GitHub Issues**: [Report bugs or request features](https://github.com/JoelHJames1/EMR-System/issues)
- **Email**: support@emrsystem.com
- **Documentation**: [Technical Documentation](TECHNICAL_DOCUMENTATION.md)

## 🙏 Acknowledgments

- [HL7 FHIR](https://www.hl7.org/fhir/) - Healthcare data standards
- [ASP.NET Core](https://dotnet.microsoft.com/) - Backend framework
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - ORM
- [JWT](https://jwt.io/) - Authentication
- [Swagger](https://swagger.io/) - API documentation

## 📊 Statistics

- **Total Entities**: 27 HL7 FHIR-compliant models
- **Lines of Code**: 20,000+ (Backend + Frontend)
- **API Endpoints**: 100+ RESTful endpoints
- **Frontend Modules**: 9 complete feature modules
- **Security Roles**: 6 predefined healthcare roles
- **Medical Standards**: ICD-10/11, CPT, NDC, LOINC, CVX, SNOMED CT, DEA
- **Controllers**: 18 fully-implemented API controllers
- **Frontend Components**: 12 major components with real API integration

---

<div align="center">

**Made with ❤️ for Healthcare**

[![GitHub Stars](https://img.shields.io/github/stars/JoelHJames1/EMR-System?style=social)](https://github.com/JoelHJames1/EMR-System)
[![GitHub Forks](https://img.shields.io/github/forks/JoelHJames1/EMR-System?style=social)](https://github.com/JoelHJames1/EMR-System)

</div>