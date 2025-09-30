# Electronic Medical Records (EMR) System - Technical Documentation

## Table of Contents
1. [System Overview](#system-overview)
2. [Architecture](#architecture)
3. [Technology Stack](#technology-stack)
4. [Database Schema](#database-schema)
5. [Authentication & Authorization](#authentication--authorization)
6. [Core Modules](#core-modules)
7. [API Endpoints](#api-endpoints)
8. [Installation Guide](#installation-guide)
9. [Configuration](#configuration)
10. [Security Features](#security-features)

---

## System Overview

The EMR System is a comprehensive healthcare management application designed to digitize and streamline medical record keeping, patient management, appointment scheduling, medication tracking, laboratory management, and billing operations.

### Key Features
- **Patient Management**: Complete patient demographics, medical history, and health records
- **Appointment Scheduling**: Calendar-based appointment booking and management
- **Clinical Documentation**: Comprehensive medical records, progress notes, and assessments
- **Medication Management**: Prescription tracking, dosage management, and drug interactions
- **Laboratory Integration**: Lab orders, results tracking, and diagnostic reports
- **Billing & Insurance**: Invoice generation, insurance claims, and payment tracking
- **Role-Based Access Control**: Secure multi-role authentication system
- **Audit Trail**: Complete logging of all system activities

---

## Architecture

### System Architecture Pattern
The EMR System follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│      (React Frontend / Swagger UI)      │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│          API Layer (Controllers)        │
│        (RESTful Web API - .NET 8)       │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│       Business Logic Layer              │
│     (Services & Domain Logic)           │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│       Data Access Layer                 │
│  (Repositories & Entity Framework)      │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│          Database Layer                 │
│         (SQL Server)                    │
└─────────────────────────────────────────┘
```

### Project Structure

```
EMR-System/
├── EMRDataLayer/                    # Data Access Layer
│   ├── Model/                       # Entity Models
│   │   ├── Patient.cs
│   │   ├── Provider.cs
│   │   ├── Appointment.cs
│   │   ├── MedicalRecord.cs
│   │   ├── Prescription.cs
│   │   ├── Medication.cs
│   │   ├── LabOrder.cs
│   │   ├── LabResult.cs
│   │   ├── Billing.cs
│   │   ├── BillingItem.cs
│   │   ├── Insurance.cs
│   │   ├── Allergy.cs
│   │   ├── Immunization.cs
│   │   ├── VitalSign.cs
│   │   ├── User.cs
│   │   └── Address.cs
│   ├── DataContext/
│   │   └── EMRDbContext.cs          # Database Context
│   └── Repository/                  # Repository Pattern
│       ├── IRepository/             # Interfaces
│       └── Repository.cs            # Implementations
│
├── EMRWebAPI/                       # API Layer
│   ├── Controllers/                 # API Controllers
│   │   ├── AuthController.cs
│   │   ├── UserController.cs
│   │   └── [Other Controllers]
│   ├── Services/                    # Business Logic
│   │   ├── JwtService.cs
│   │   ├── UserService.cs
│   │   └── IServices/
│   ├── Model/                       # DTOs
│   │   ├── LoginDTO.cs
│   │   ├── RegisterDto.cs
│   │   ├── RefreshTokenDto.cs
│   │   └── [Other DTOs]
│   ├── AutoMapper/                  # Object Mapping
│   ├── Program.cs                   # Application Entry
│   └── appsettings.json            # Configuration
│
└── emrwebfrontend/                  # React Frontend
    └── [React Components]
```

---

## Technology Stack

### Backend
- **Framework**: .NET 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server (LocalDB for development)
- **Authentication**: JWT (JSON Web Tokens)
- **API Documentation**: Swagger/OpenAPI 3.0
- **Logging**: NLog

### Key NuGet Packages
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="AutoMapper" Version="13.0.1" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.0" />
```

### Frontend
- **Framework**: React.js
- **UI Library**: Bootstrap
- **HTTP Client**: Axios

---

## Database Schema

### Core Tables

#### 1. **AspNetUsers** (Identity Framework)
Extended with custom User model for authentication.

#### 2. **Patients**
```sql
Patients
├── Id (PK)
├── FirstName
├── LastName
├── MiddleName
├── DateOfBirth
├── Gender
├── SocialSecurityNumber
├── Email
├── PhoneNumber
├── EmergencyContact
├── EmergencyContactName
├── BloodType
├── MaritalStatus
├── Occupation
├── Employer
├── PreferredLanguage
├── Ethnicity
├── Race
├── IsActive
├── CreatedDate
├── ModifiedDate
├── CreatedBy
├── ModifiedBy
└── AddressId (FK)
```

#### 3. **Providers**
```sql
Providers
├── Id (PK)
├── FirstName
├── LastName
├── MiddleName
├── Specialization
├── LicenseNumber
├── NPI (National Provider Identifier)
├── DEA (Drug Enforcement Administration)
├── Email
├── PhoneNumber
├── IsActive
├── CreatedDate
├── ModifiedDate
└── UserId (FK)
```

#### 4. **Appointments**
```sql
Appointments
├── Id (PK)
├── PatientId (FK)
├── ProviderId (FK)
├── AppointmentDate
├── StartTime
├── EndTime
├── Status (Scheduled, Confirmed, InProgress, Completed, Cancelled, NoShow)
├── AppointmentType
├── ReasonForVisit
├── Notes
├── RoomNumber
├── CreatedDate
└── ModifiedDate
```

#### 5. **MedicalRecords**
```sql
MedicalRecords
├── Id (PK)
├── PatientId (FK)
├── ProviderId (FK)
├── VisitDate
├── ChiefComplaint
├── HistoryOfPresentIllness
├── PhysicalExamination
├── Assessment
├── Diagnosis
├── TreatmentPlan
├── ProgressNotes
├── RecordType
├── IsSigned
├── SignedDate
├── CreatedDate
└── ModifiedDate
```

#### 6. **Medications**
```sql
Medications
├── Id (PK)
├── Name
├── GenericName
├── BrandName
├── Category
├── Form (Tablet, Capsule, Liquid)
├── Strength
├── NDC (National Drug Code)
├── Description
├── SideEffects
├── Contraindications
├── IsControlledSubstance
├── DEASchedule
├── IsActive
├── CreatedDate
└── ModifiedDate
```

#### 7. **Prescriptions**
```sql
Prescriptions
├── Id (PK)
├── PatientId (FK)
├── ProviderId (FK)
├── MedicationId (FK)
├── Dosage
├── Frequency
├── Route (Oral, IV, IM)
├── Quantity
├── Refills
├── StartDate
├── EndDate
├── Instructions
├── Status (Active, Discontinued, Completed, OnHold)
├── IsGenericAllowed
├── CreatedDate
└── ModifiedDate
```

#### 8. **LabOrders**
```sql
LabOrders
├── Id (PK)
├── PatientId (FK)
├── ProviderId (FK)
├── TestName
├── TestCode
├── Category
├── Priority (Stat, Urgent, Routine)
├── Status (Ordered, InProgress, Completed, Cancelled)
├── OrderedDate
├── CollectedDate
├── CompletedDate
└── ClinicalNotes
```

#### 9. **LabResults**
```sql
LabResults
├── Id (PK)
├── LabOrderId (FK)
├── TestComponent
├── Value
├── Unit
├── ReferenceRange
├── Flag (Normal, High, Low, Critical)
├── ResultDate
├── Comments
├── PerformedBy
└── CreatedDate
```

#### 10. **Billings**
```sql
Billings
├── Id (PK)
├── PatientId (FK)
├── InvoiceNumber
├── InvoiceDate
├── DueDate
├── TotalAmount
├── PaidAmount
├── BalanceAmount
├── Status (Unpaid, Partial, Paid, Overdue)
├── PaymentMethod
├── PaymentDate
├── InsuranceId (FK)
├── InsuranceCoverage
├── PatientResponsibility
├── Notes
├── CreatedDate
└── ModifiedDate
```

#### 11. **BillingItems**
```sql
BillingItems
├── Id (PK)
├── BillingId (FK)
├── Description
├── CPTCode (Current Procedural Terminology)
├── ICDCode (International Classification of Diseases)
├── Quantity
├── UnitPrice
├── TotalPrice
└── CreatedDate
```

#### 12. **Insurances**
```sql
Insurances
├── Id (PK)
├── PatientId (FK)
├── InsuranceCompany
├── PolicyNumber
├── GroupNumber
├── PlanType (HMO, PPO, EPO, POS)
├── PolicyHolderName
├── PolicyHolderRelationship
├── PolicyHolderSSN
├── EffectiveDate
├── ExpirationDate
├── InsurancePhone
├── InsuranceAddress
├── IsPrimary
├── IsActive
├── CreatedDate
└── ModifiedDate
```

#### 13. **Allergies**
```sql
Allergies
├── Id (PK)
├── PatientId (FK)
├── Allergen
├── AllergyType (Drug, Food, Environmental)
├── Severity (Mild, Moderate, Severe, Life-threatening)
├── Reaction
├── OnsetDate
├── Notes
├── IsActive
├── CreatedDate
└── ModifiedDate
```

#### 14. **Immunizations**
```sql
Immunizations
├── Id (PK)
├── PatientId (FK)
├── VaccineName
├── CVXCode
├── AdministeredDate
├── AdministeredBy
├── Route
├── Site
├── LotNumber
├── Manufacturer
├── ExpirationDate
├── DoseNumber
├── Notes
├── CreatedDate
└── ModifiedDate
```

#### 15. **VitalSigns**
```sql
VitalSigns
├── Id (PK)
├── PatientId (FK)
├── MeasurementDate
├── Temperature
├── TemperatureUnit
├── SystolicBP
├── DiastolicBP
├── HeartRate
├── RespiratoryRate
├── OxygenSaturation
├── Height
├── HeightUnit
├── Weight
├── WeightUnit
├── BMI
├── MeasuredBy
├── Notes
└── CreatedDate
```

### Entity Relationships

```
Patient 1──────* Appointment
Patient 1──────* MedicalRecord
Patient 1──────* Prescription
Patient 1──────* LabOrder
Patient 1──────* Billing
Patient 1──────* Insurance
Patient 1──────* Allergy
Patient 1──────* Immunization
Patient 1──────* VitalSign

Provider 1─────* Appointment
Provider 1─────* MedicalRecord
Provider 1─────* Prescription
Provider 1─────* LabOrder

Medication 1───* Prescription

LabOrder 1─────* LabResult

Billing 1──────* BillingItem
Billing *──────1 Insurance

User 1─────────1 Provider
User *─────────1 Address
Patient *──────1 Address
```

---

## Authentication & Authorization

### JWT Token-Based Authentication

The system uses JSON Web Tokens (JWT) for stateless authentication:

#### Token Structure
```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "nameid": "user-guid",
    "email": "user@example.com",
    "FirstName": "John",
    "LastName": "Doe",
    "role": ["Doctor", "Administrator"],
    "exp": 1735689600
  }
}
```

#### Token Configuration
- **Algorithm**: HMAC-SHA256
- **Expiration**: 24 hours (configurable)
- **Refresh Token**: Supported for seamless re-authentication
- **Claims**: User ID, Email, Name, Roles

### Role-Based Access Control (RBAC)

#### Predefined Roles

1. **Administrator**
   - Full system access
   - User management
   - System configuration
   - All module access

2. **Doctor**
   - Patient management
   - Medical records (full access)
   - Prescriptions
   - Lab orders
   - Appointments

3. **Nurse**
   - Patient vitals
   - Appointments
   - Medical records (view/update)
   - Medication administration

4. **Receptionist**
   - Patient registration
   - Appointment scheduling
   - Basic patient information

5. **Lab Technician**
   - Lab orders
   - Lab results entry
   - Diagnostic reports

6. **Billing Staff**
   - Billing management
   - Insurance processing
   - Payment tracking
   - Invoice generation

### Authorization Policies

```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireRole("Administrator"));

    options.AddPolicy("DoctorOnly",
        policy => policy.RequireRole("Doctor", "Administrator"));

    options.AddPolicy("NurseOnly",
        policy => policy.RequireRole("Nurse", "Doctor", "Administrator"));

    options.AddPolicy("LabTechOnly",
        policy => policy.RequireRole("Lab Technician", "Doctor", "Administrator"));

    options.AddPolicy("BillingOnly",
        policy => policy.RequireRole("Billing Staff", "Administrator"));
});
```

---

## Core Modules

### 1. Patient Management Module
**Purpose**: Manage complete patient demographics, contacts, and personal information.

**Key Features**:
- Patient registration and profile management
- Demographics tracking
- Emergency contact information
- Medical history
- Insurance information linking
- Address management

### 2. Appointment Scheduling Module
**Purpose**: Manage patient appointments with healthcare providers.

**Key Features**:
- Appointment booking
- Calendar view
- Appointment status tracking
- Reminder notifications
- Conflict detection
- Room assignment

### 3. Clinical Documentation Module
**Purpose**: Comprehensive medical record keeping and clinical notes.

**Key Features**:
- SOAP notes (Subjective, Objective, Assessment, Plan)
- Chief complaints
- Physical examination records
- Diagnosis tracking
- Treatment plans
- Progress notes
- Electronic signature support

### 4. Medication Management Module
**Purpose**: Prescription and medication tracking.

**Key Features**:
- Medication database
- Prescription creation
- Dosage and frequency management
- Refill tracking
- Drug interaction alerts
- Controlled substance tracking (DEA schedule)

### 5. Laboratory & Diagnostics Module
**Purpose**: Lab test ordering and results management.

**Key Features**:
- Lab order creation
- Test catalog
- Result entry and tracking
- Critical value flagging
- Reference range validation
- Report generation

### 6. Billing & Insurance Module
**Purpose**: Financial management and insurance processing.

**Key Features**:
- Invoice generation
- CPT/ICD code support
- Insurance claim processing
- Payment tracking
- Statement generation
- Outstanding balance reports

### 7. Allergy & Immunization Tracking
**Purpose**: Patient safety through allergy and vaccination records.

**Key Features**:
- Allergy documentation
- Severity classification
- Reaction tracking
- Immunization records
- Vaccination schedules
- CVX code support

### 8. Vital Signs Monitoring
**Purpose**: Track patient vital signs and measurements.

**Key Features**:
- Temperature, BP, heart rate
- Height, weight, BMI
- Oxygen saturation
- Respiratory rate
- Historical trending
- Unit conversion

---

## API Endpoints

### Authentication Endpoints

#### POST /api/Auth/register
Register a new user (Admin only)
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890",
  "password": "SecureP@ss123",
  "confirmPassword": "SecureP@ss123",
  "roles": ["Doctor"]
}
```

#### POST /api/Auth/login
User login
```json
{
  "email": "john.doe@example.com",
  "password": "SecureP@ss123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "user": {
    "id": "user-guid",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["Doctor"]
  }
}
```

#### POST /api/Auth/refresh-token
Refresh expired access token
```json
{
  "accessToken": "expired-token",
  "refreshToken": "valid-refresh-token"
}
```

#### GET /api/Auth/profile
Get current user profile (Authenticated)

#### POST /api/Auth/logout
Logout user (Authenticated)

---

## Installation Guide

### Prerequisites
- **.NET 8.0 SDK** or later
- **SQL Server** (LocalDB, Express, or Full)
- **Visual Studio 2022** or **VS Code**
- **Node.js** (for frontend)

### Backend Setup

1. **Clone the Repository**
```bash
git clone https://github.com/JoelHJames1/EMR-System.git
cd EMR-System
```

2. **Restore NuGet Packages**
```bash
cd EMRWebAPI
dotnet restore
```

3. **Update Connection String**
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-SQL-Server-Connection-String"
  }
}
```

4. **Create Database**
```bash
dotnet ef database update --project ../EMRDataLayer
```

Or create migration if needed:
```bash
dotnet ef migrations add InitialCreate --project ../EMRDataLayer
dotnet ef database update --project ../EMRDataLayer
```

5. **Run the Application**
```bash
dotnet run
```

The API will be available at: `https://localhost:7099`

6. **Access Swagger UI**
Navigate to: `https://localhost:7099/swagger`

### Frontend Setup

1. **Navigate to Frontend Directory**
```bash
cd emrwebfrontend
```

2. **Install Dependencies**
```bash
npm install
```

3. **Start Development Server**
```bash
npm start
```

The frontend will be available at: `http://localhost:3000`

---

## Configuration

### appsettings.json Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EMRSystemDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "Your-Secret-Key-At-Least-32-Characters-Long",
    "Issuer": "EMRSystem",
    "Audience": "EMRSystemUsers",
    "ExpireHours": "24"
  }
}
```

### Environment-Specific Configuration

#### Development (appsettings.Development.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EMRSystemDB_Dev;..."
  }
}
```

#### Production
Use environment variables or Azure Key Vault for sensitive data.

---

## Security Features

### 1. Password Security
- Minimum 8 characters
- Requires uppercase, lowercase, digit, and special character
- Hashed using ASP.NET Core Identity (PBKDF2)
- No plain text storage

### 2. Account Lockout
- 5 failed login attempts
- 15-minute lockout period
- Prevents brute force attacks

### 3. JWT Token Security
- HMAC-SHA256 signing
- Short expiration (24 hours)
- Refresh token mechanism
- Secure token validation

### 4. HTTPS Enforcement
- All API calls over HTTPS
- Certificate validation
- TLS 1.2+ required

### 5. CORS Configuration
- Configurable origin whitelist
- Controlled in production
- Development mode: localhost allowed

### 6. SQL Injection Prevention
- Entity Framework parameterized queries
- No raw SQL execution
- Input validation

### 7. Data Validation
- Model validation attributes
- Required fields enforcement
- Email and phone format validation
- Custom business rule validation

### 8. Audit Trail
- NLog logging
- User action tracking
- Created/Modified timestamps
- Created/Modified by user ID

### 9. Role-Based Authorization
- Granular permission control
- Multi-role support
- Policy-based authorization
- Attribute-based access control

---

## Database Migrations

### Creating a New Migration
```bash
dotnet ef migrations add MigrationName --project EMRDataLayer --startup-project EMRWebAPI
```

### Applying Migrations
```bash
dotnet ef database update --project EMRDataLayer --startup-project EMRWebAPI
```

### Rolling Back Migrations
```bash
dotnet ef database update PreviousMigrationName --project EMRDataLayer --startup-project EMRWebAPI
```

### Removing Last Migration
```bash
dotnet ef migrations remove --project EMRDataLayer --startup-project EMRWebAPI
```

---

## Performance Considerations

### Database Optimization
- Indexed foreign keys
- Composite indexes on frequently queried columns
- Eager loading for related entities
- Pagination for large datasets

### Caching Strategy
- Response caching for read-heavy endpoints
- Distributed cache for multi-server deployments
- Cache invalidation on updates

### API Best Practices
- Asynchronous operations (async/await)
- DTOs to reduce payload size
- Compression for API responses
- Rate limiting for API protection

---

## Compliance & Standards

### HIPAA Compliance Considerations
- **PHI Protection**: Encrypted data at rest and in transit
- **Access Controls**: Role-based authentication
- **Audit Logs**: Complete activity tracking
- **Data Backup**: Regular automated backups

### HL7 Standards
- Ready for HL7 FHIR integration
- Standard medical codes (ICD, CPT, NDC, CVX)

### Interoperability
- RESTful API for third-party integration
- Standard JSON data formats
- OpenAPI/Swagger documentation

---

## Troubleshooting

### Common Issues

#### 1. Database Connection Failure
- Verify SQL Server is running
- Check connection string
- Ensure database exists
- Verify user permissions

#### 2. JWT Token Errors
- Check JWT key configuration
- Verify token expiration
- Ensure clock synchronization
- Validate issuer/audience

#### 3. Migration Errors
- Delete existing database
- Remove migration files
- Recreate migrations
- Check model configurations

#### 4. CORS Errors
- Verify CORS policy configuration
- Check allowed origins
- Ensure preflight requests handled
- Validate request headers

---

## Future Enhancements

1. **Patient Portal**: Self-service portal for patients
2. **Telemedicine**: Video consultation integration
3. **Mobile App**: iOS/Android applications
4. **AI Integration**: Diagnostic assistance and predictive analytics
5. **Electronic Prescribing**: E-prescribing integration
6. **Imaging Integration**: DICOM viewer for radiology
7. **Reporting Module**: Advanced analytics and dashboards
8. **Notifications**: SMS/Email alerts and reminders
9. **Multi-language Support**: Internationalization
10. **Cloud Deployment**: Azure/AWS hosting

---

## Support & Maintenance

### Version Information
- **Current Version**: 1.0.0
- **Framework**: .NET 8.0
- **Database**: SQL Server 2019+

### Contact Information
- **Email**: support@emrsystem.com
- **GitHub**: https://github.com/JoelHJames1/EMR-System

### License
MIT License - See LICENSE.txt for details

---

## Glossary

- **CPT**: Current Procedural Terminology
- **CVX**: Vaccine Administered Code
- **DEA**: Drug Enforcement Administration
- **EMR**: Electronic Medical Record
- **HL7**: Health Level Seven International
- **HIPAA**: Health Insurance Portability and Accountability Act
- **ICD**: International Classification of Diseases
- **JWT**: JSON Web Token
- **NDC**: National Drug Code
- **NPI**: National Provider Identifier
- **PHI**: Protected Health Information
- **RBAC**: Role-Based Access Control
- **SOAP**: Subjective, Objective, Assessment, Plan

---

**Document Version**: 1.0
**Last Updated**: 2024
**Author**: EMR System Development Team