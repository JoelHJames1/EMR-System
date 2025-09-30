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
Frontend:  React.js + Bootstrap
Backend:   ASP.NET Core 8.0 Web API
ORM:       Entity Framework Core 8.0
Database:  SQL Server 2019+
Auth:      JWT Bearer Token
Logging:   NLog
API Docs:  Swagger/OpenAPI 3.0
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

### Frontend Setup (Optional)

```bash
cd emrwebfrontend
npm install
npm start
```

**Frontend URL**: `http://localhost:3000`

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
│   ├── src/
│   ├── public/
│   └── package.json
│
├── 📄 README.md                  # This file
├── 📄 TECHNICAL_DOCUMENTATION.md # Detailed technical docs
├── 📄 LICENSE.txt                # MIT License
└── 📄 .gitignore
```

## 🌟 Roadmap & Future Enhancements

### Phase 1 (Current)
- ✅ Core EMR functionality
- ✅ HL7 FHIR compliance
- ✅ JWT authentication
- ✅ RESTful API

### Phase 2 (Planned)
- [ ] Patient Portal (self-service)
- [ ] Mobile applications (iOS/Android)
- [ ] Telemedicine integration
- [ ] E-prescribing integration

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
- **Lines of Code**: 15,000+ (Backend + Frontend)
- **API Endpoints**: 50+ RESTful endpoints
- **Security Roles**: 6 predefined roles
- **Medical Standards**: ICD-10, CPT, NDC, LOINC, CVX, SNOMED CT

---

<div align="center">

**Made with ❤️ for Healthcare**

[![GitHub Stars](https://img.shields.io/github/stars/JoelHJames1/EMR-System?style=social)](https://github.com/JoelHJames1/EMR-System)
[![GitHub Forks](https://img.shields.io/github/forks/JoelHJames1/EMR-System?style=social)](https://github.com/JoelHJames1/EMR-System)

</div>