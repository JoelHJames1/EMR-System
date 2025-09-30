# EMR System - Role-Based Access Control (RBAC) Documentation

## Overview

The EMR System implements a comprehensive **Role-Based Access Control (RBAC)** system based on real-world healthcare facility operations. This document outlines all roles, their permissions, responsibilities, and typical use cases in a clinical environment.

---

## 🎭 Role Hierarchy

```
┌─────────────────────────────────────────────────────────┐
│                    Administrator                        │
│              (Full System Access)                       │
└─────────────────────────────────────────────────────────┘
                          │
        ┌─────────────────┼─────────────────┬──────────────────┐
        │                 │                 │                  │
┌───────▼────────┐ ┌─────▼──────┐ ┌───────▼────────┐ ┌──────▼──────┐
│     Doctor     │ │    Nurse   │ │  Receptionist  │ │   Billing   │
│   (Clinical)   │ │ (Clinical) │ │ (Front Desk)   │ │   Staff     │
└────────────────┘ └────────────┘ └────────────────┘ └─────────────┘
                          │
                  ┌───────▼────────┐
                  │ Lab Technician │
                  │  (Laboratory)  │
                  └────────────────┘
```

---

## 👥 Role Definitions

### 1. 👨‍💼 Administrator

**Database Role Name**: `Administrator`
**Normalized Name**: `ADMINISTRATOR`
**Role ID**: `1`

#### Description
System administrators have complete access to all functionality. They manage system configuration, user accounts, and have oversight of all clinical and administrative operations.

#### Primary Responsibilities
- User account creation and management
- Role assignment to users
- System configuration and settings
- Provider credential management
- Access to all patient records
- System monitoring and reporting
- Security and compliance oversight
- Database management
- Audit log review

#### API Access Permissions

| Module | Permissions |
|--------|-------------|
| **User Management** | Full CRUD (Create, Read, Update, Delete) |
| **Patient Management** | Full access to all patient records |
| **Provider Management** | Create, update, deactivate providers |
| **Appointments** | View, create, modify, cancel all appointments |
| **Clinical Records** | View all encounters, diagnoses, notes |
| **Prescriptions** | View all prescriptions |
| **Lab Orders** | View all lab orders and results |
| **Billing** | Full access to billing and insurance |
| **Analytics** | Full access to dashboards and reports |
| **System Settings** | Configure system-wide settings |

#### Typical Users
- IT Administrator
- Chief Medical Information Officer (CMIO)
- System Administrator
- Compliance Officer

#### Security Considerations
- Should be assigned to minimal personnel
- All actions are logged for audit trail
- Requires strongest password policies
- May require multi-factor authentication (MFA)

---

### 2. 👨‍⚕️ Doctor

**Database Role Name**: `Doctor`
**Normalized Name**: `DOCTOR`
**Role ID**: `2`

#### Description
Physicians and doctors have comprehensive clinical access to diagnose, prescribe, order tests, and document patient care. This is the primary clinical role in the EMR system.

#### Primary Responsibilities
- Patient examination and assessment
- Clinical documentation (SOAP notes)
- Diagnosis entry with ICD-10/11 coding
- Prescription writing (including controlled substances)
- Laboratory test ordering
- Procedure ordering and documentation
- Referral to specialists
- Care plan development
- Electronic signature on clinical notes
- Patient discharge

#### API Access Permissions

| Module | Endpoint Pattern | Access |
|--------|------------------|--------|
| **Patients** | `/api/Patient/**` | Full CRUD |
| **Encounters** | `/api/Encounter/**` | Full CRUD |
| **Diagnoses** | `/api/Diagnosis/**` | Full CRUD |
| **Clinical Notes** | `/api/ClinicalNote/**` | Full CRUD + Sign |
| **Prescriptions** | `/api/Prescription/**` | Create, Update |
| **Medications** | `/api/Prescription/medications` | Read |
| **Lab Orders** | `/api/LabOrder/**` | Create, Read, Update |
| **Procedures** | `/api/Procedure/**` | Full CRUD |
| **Care Plans** | `/api/CarePlan/**` | Full CRUD |
| **Referrals** | `/api/Referral/**` | Create, Read, Update |
| **Observations** | `/api/Observation/**` | Full CRUD |
| **Vital Signs** | `/api/Observation/vitals` | Read, Create |
| **Allergies** | `/api/Allergy/**` | Full CRUD |
| **Immunizations** | `/api/Immunization/**` | Full CRUD |
| **Appointments** | `/api/Appointment/**` | Full CRUD |
| **Provider** | `/api/Provider/{id}` | Read own profile |
| **Billing** | `/api/Billing/**` | Read only |
| **Dashboard** | `/api/Dashboard/**` | Read analytics |

#### Clinical Workflows
1. **New Patient Visit**
   - Review patient demographics
   - Check allergies and current medications
   - Record vital signs
   - Document SOAP note
   - Enter diagnoses with ICD codes
   - Order labs if needed
   - Write prescriptions
   - Schedule follow-up

2. **Follow-up Visit**
   - Review previous encounters
   - Check lab results
   - Update care plan
   - Adjust medications
   - Document progress

3. **Hospital Admission**
   - Create encounter
   - Enter admission diagnosis
   - Order initial labs and procedures
   - Write admission orders
   - Document H&P (History & Physical)

#### Controlled Substance Prescribing
- DEA number verification required
- System logs all controlled substance prescriptions
- Automatic alerts for Schedule II-V medications
- Refill limitations enforced

#### Typical Users
- Primary Care Physicians
- Specialists (Cardiologists, Surgeons, etc.)
- Emergency Room Physicians
- Hospitalists
- Medical Residents (supervised)

---

### 3. 👩‍⚕️ Nurse

**Database Role Name**: `Nurse`
**Normalized Name**: `NURSE`
**Role ID**: `3`

#### Description
Nurses provide direct patient care, document observations, administer medications, and assist physicians. They have clinical access but with some restrictions compared to doctors.

#### Primary Responsibilities
- Vital signs measurement and recording
- Medication administration
- Patient triage
- Observation documentation
- Allergy documentation
- Immunization administration
- Specimen collection for lab tests
- Patient education
- Care plan execution
- Wound care documentation

#### API Access Permissions

| Module | Endpoint Pattern | Access |
|--------|------------------|--------|
| **Patients** | `/api/Patient/**` | Read, Update demographics |
| **Encounters** | `/api/Encounter/**` | Read, Create, Update |
| **Diagnoses** | `/api/Diagnosis/**` | Read only |
| **Clinical Notes** | `/api/ClinicalNote/**` | Create, Read, Update (own notes) |
| **Prescriptions** | `/api/Prescription/{id}/status` | Update status (administer) |
| **Lab Orders** | `/api/LabOrder/**` | Read, Update status |
| **Procedures** | `/api/Procedure/**` | Read, Update status |
| **Observations** | `/api/Observation/**` | Full CRUD |
| **Vital Signs** | `/api/Observation/vitals` | Full CRUD |
| **Allergies** | `/api/Allergy/**` | Full CRUD |
| **Immunizations** | `/api/Immunization/**` | Full CRUD |
| **Appointments** | `/api/Appointment/**` | Full CRUD |
| **Care Plans** | `/api/CarePlan/activities` | Update activity status |
| **Provider** | `/api/Provider/**` | Read only |

#### Cannot Access
- ❌ Write prescriptions
- ❌ Order diagnostic procedures
- ❌ Make diagnoses
- ❌ Sign clinical notes as final
- ❌ Create referrals
- ❌ Billing operations

#### Nursing Workflows
1. **Patient Intake**
   - Record vital signs
   - Document allergies
   - Update medication list
   - Triage assessment
   - Room assignment

2. **Medication Administration**
   - Verify prescription
   - Check allergies
   - Administer medication
   - Update prescription status
   - Document administration

3. **Vital Signs Monitoring**
   - Temperature, BP, HR, RR, O2 Sat
   - Automatic BMI calculation
   - Trend analysis
   - Alert for abnormal values

#### Typical Users
- Registered Nurses (RN)
- Licensed Practical Nurses (LPN)
- Nurse Practitioners (NP) - may have expanded access
- Clinical Nurse Specialists
- Triage Nurses

---

### 4. 👔 Receptionist

**Database Role Name**: `Receptionist`
**Normalized Name**: `RECEPTIONIST`
**Role ID**: `4`

#### Description
Front desk staff responsible for patient registration, appointment scheduling, and basic demographic management. They are the first point of contact for patients.

#### Primary Responsibilities
- Patient registration and check-in
- Appointment scheduling
- Insurance information collection
- Demographics updates
- Appointment reminders
- Waiting room management
- Basic insurance verification
- Medical records request handling

#### API Access Permissions

| Module | Endpoint Pattern | Access |
|--------|------------------|--------|
| **Patients** | `/api/Patient/**` | Create, Read, Update demographics |
| **Appointments** | `/api/Appointment/**` | Full CRUD |
| **Providers** | `/api/Provider/**` | Read schedules |
| **Encounters** | `/api/Encounter/**` | Create (check-in) |
| **Insurance** | `/api/Insurance/**` | Read, Create, Update, Verify |
| **Referrals** | `/api/Referral/**` | Read, Update status |

#### Cannot Access
- ❌ Clinical records (diagnoses, notes)
- ❌ Prescriptions
- ❌ Lab results
- ❌ Procedure details
- ❌ Billing amounts (can see status)
- ❌ Medical history
- ❌ Vital signs

#### Front Desk Workflows
1. **New Patient Registration**
   - Collect demographics
   - Scan insurance cards
   - Verify insurance eligibility
   - Schedule first appointment
   - Collect co-payment

2. **Patient Check-In**
   - Verify identity
   - Update demographics if changed
   - Confirm insurance
   - Collect payment
   - Create encounter (check-in)
   - Notify clinical staff

3. **Appointment Management**
   - Schedule appointments
   - Check provider availability
   - Detect scheduling conflicts
   - Send reminders
   - Manage cancellations
   - Handle walk-ins

#### Typical Users
- Front Desk Receptionist
- Medical Secretary
- Patient Services Representative
- Scheduling Coordinator

---

### 5. 🔬 Lab Technician

**Database Role Name**: `Lab Technician`
**Normalized Name**: `LAB TECHNICIAN`
**Role ID**: `5`

#### Description
Laboratory personnel responsible for processing lab orders, collecting specimens, performing tests, and entering results with LOINC coding.

#### Primary Responsibilities
- Lab order processing
- Specimen collection
- Lab test execution
- Result entry with LOINC codes
- Quality control
- Reference range validation
- Critical value flagging
- Equipment maintenance logs

#### API Access Permissions

| Module | Endpoint Pattern | Access |
|--------|------------------|--------|
| **Lab Orders** | `/api/LabOrder/**` | Full CRUD |
| **Lab Results** | `/api/LabOrder/{id}/results` | Create, Update |
| **Patients** | `/api/Patient/{id}` | Read demographics only |
| **Providers** | `/api/Provider/**` | Read (for routing) |
| **Dashboard** | `/api/Dashboard/lab/stats` | Read statistics |

#### Cannot Access
- ❌ Full patient medical history
- ❌ Prescriptions
- ❌ Billing information
- ❌ Encounters
- ❌ Clinical notes

#### Lab Workflows
1. **Order Processing**
   - Review pending orders
   - Prioritize by urgency (STAT, Urgent, Routine)
   - Collect specimens
   - Label and track samples
   - Update order status

2. **Result Entry**
   - Enter test results
   - Include LOINC codes
   - Specify units and reference ranges
   - Flag abnormal values (High, Low, Critical)
   - Add technician comments
   - Mark order as completed

3. **Quality Assurance**
   - Run control tests
   - Calibrate equipment
   - Document quality metrics
   - Flag results needing review

#### LOINC Code Usage
All lab results must include LOINC (Logical Observation Identifiers Names and Codes) for standardization:
- `2339-0` - Glucose (blood)
- `2571-8` - Triglyceride (blood)
- `718-7` - Hemoglobin (blood)
- And 70,000+ other codes

#### Typical Users
- Medical Laboratory Technician (MLT)
- Medical Laboratory Scientist (MLS)
- Clinical Laboratory Technologist
- Phlebotomist (limited access)

---

### 6. 💰 Billing Staff

**Database Role Name**: `Billing Staff`
**Normalized Name**: `BILLING STAFF`
**Role ID**: `6`

#### Description
Financial personnel responsible for billing, insurance claims, payment processing, and revenue cycle management.

#### Primary Responsibilities
- Invoice generation with CPT/ICD codes
- Insurance claim submission
- Payment processing
- Outstanding balance tracking
- Insurance verification
- Pre-authorization requests
- Denial management
- Financial reporting

#### API Access Permissions

| Module | Endpoint Pattern | Access |
|--------|------------------|--------|
| **Billing** | `/api/Billing/**` | Full CRUD |
| **Insurance** | `/api/Insurance/**` | Full CRUD |
| **Patients** | `/api/Patient/{id}` | Read demographics and insurance |
| **Encounters** | `/api/Encounter/**` | Read for billing codes |
| **Diagnoses** | `/api/Diagnosis/**` | Read for ICD codes |
| **Procedures** | `/api/Procedure/**` | Read for CPT codes |
| **Dashboard** | `/api/Dashboard/billing/summary` | Read financial reports |

#### Cannot Access
- ❌ Clinical notes (except for coding)
- ❌ Lab results
- ❌ Prescriptions
- ❌ Vital signs
- ❌ Medical history details

#### Billing Workflows
1. **Invoice Creation**
   - Retrieve encounter details
   - Extract CPT codes from procedures
   - Extract ICD codes from diagnoses
   - Calculate totals
   - Determine insurance coverage
   - Calculate patient responsibility

2. **Insurance Claim**
   - Verify insurance eligibility
   - Prepare claim with codes
   - Submit electronically
   - Track claim status
   - Handle denials
   - Resubmit if needed

3. **Payment Processing**
   - Record payments
   - Apply to invoices
   - Update balances
   - Generate receipts
   - Track outstanding amounts

#### Medical Coding
Billing staff must understand:
- **CPT Codes**: Current Procedural Terminology (99213, 45378, etc.)
- **ICD-10 Codes**: International Classification of Diseases (E11.9, I10, etc.)
- **Modifiers**: Additional code details
- **HCPCS**: Healthcare Common Procedure Coding System

#### Typical Users
- Medical Biller
- Medical Coder
- Revenue Cycle Specialist
- Claims Specialist
- Patient Account Representative

---

## 🔐 Role Assignment Process

### How Roles Are Assigned

1. **Administrator Creates User**
   ```http
   POST /api/Auth/register
   {
     "firstName": "John",
     "lastName": "Smith",
     "email": "john.smith@hospital.com",
     "password": "SecureP@ss123",
     "roles": ["Doctor"]
   }
   ```

2. **Multiple Roles**
   A user can have multiple roles if needed:
   ```json
   {
     "roles": ["Doctor", "Administrator"]
   }
   ```

3. **Provider Link**
   For clinical roles, link to Provider record:
   ```json
   {
     "userId": "user-guid",
     "firstName": "John",
     "lastName": "Smith",
     "specialization": "Cardiology",
     "licenseNumber": "MD123456",
     "npi": "1234567890"
   }
   ```

---

## 🛡️ Security Features

### Password Requirements (All Roles)
- Minimum 8 characters
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 digit
- At least 1 special character

### Account Lockout
- 5 failed login attempts
- 15-minute lockout period
- Automatic unlock after cooldown

### Audit Trail
All actions are logged with:
- User ID
- Timestamp
- Action performed
- Resource affected
- IP address

---

## 📋 Permission Matrix

### Complete Access Control Matrix

| Feature | Admin | Doctor | Nurse | Receptionist | Lab Tech | Billing |
|---------|:-----:|:------:|:-----:|:------------:|:--------:|:-------:|
| **User Management** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Patient Registration** | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| **Patient Demographics** | ✅ | ✅ | ✅ | ✅ | 📖 | 📖 |
| **Appointments** | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| **Encounters** | ✅ | ✅ | ✅ | ✅* | ❌ | 📖 |
| **Clinical Notes** | ✅ | ✅ | ✏️ | ❌ | ❌ | ❌ |
| **Sign Notes** | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Diagnoses (ICD)** | ✅ | ✅ | 📖 | ❌ | ❌ | 📖 |
| **Prescriptions** | ✅ | ✅ | 📖* | ❌ | ❌ | ❌ |
| **Controlled Substances** | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Lab Orders** | ✅ | ✅ | 📖 | ❌ | ✅ | ❌ |
| **Lab Results** | ✅ | ✅ | 📖 | ❌ | ✅ | ❌ |
| **Procedures (CPT)** | ✅ | ✅ | 📖 | ❌ | ❌ | 📖 |
| **Vital Signs** | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| **Allergies** | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| **Immunizations** | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| **Care Plans** | ✅ | ✅ | ✏️ | ❌ | ❌ | ❌ |
| **Referrals** | ✅ | ✅ | 📖 | ✏️ | ❌ | ❌ |
| **Insurance** | ✅ | 📖 | ❌ | ✏️ | ❌ | ✅ |
| **Billing** | ✅ | 📖 | ❌ | ❌ | ❌ | ✅ |
| **Payments** | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ |
| **Provider Management** | ✅ | 📖 | 📖 | 📖 | ❌ | ❌ |
| **Dashboard/Analytics** | ✅ | ✅ | ✅ | ❌ | ✏️ | ✏️ |

**Legend:**
- ✅ Full Access (Create, Read, Update, Delete)
- ✏️ Limited Write (Can update specific fields)
- 📖 Read Only
- ❌ No Access
- * Special conditions apply

---

## 🔄 Role-Based Workflows

### Example: Medication Order Process

```
Doctor (Prescribes)
    ↓
    Creates Prescription with CPT/NDC codes
    ↓
Nurse (Administers)
    ↓
    Views prescription
    ↓
    Checks allergies
    ↓
    Administers medication
    ↓
    Updates status to "Administered"
    ↓
Billing Staff (Bills)
    ↓
    Views procedure codes
    ↓
    Creates invoice
    ↓
    Submits insurance claim
```

### Example: Lab Test Process

```
Doctor (Orders)
    ↓
    Creates Lab Order with LOINC code
    ↓
Nurse (Collects)
    ↓
    Updates status to "Collected"
    ↓
    Documents collection time
    ↓
Lab Technician (Processes)
    ↓
    Performs test
    ↓
    Enters results with reference ranges
    ↓
    Flags abnormal values
    ↓
    Completes order
    ↓
Doctor (Reviews)
    ↓
    Reviews results
    ↓
    Documents in clinical note
    ↓
    Adjusts treatment if needed
```

---

## 🚀 Implementation Example

### Checking User Role in Code

```csharp
// Check if user has Doctor role
[Authorize(Roles = "Doctor")]
public async Task<IActionResult> CreatePrescription([FromBody] Prescription rx)
{
    // Only doctors can execute this
}

// Multiple roles allowed
[Authorize(Roles = "Doctor,Nurse,Administrator")]
public async Task<IActionResult> GetPatient(int id)
{
    // Doctors, Nurses, and Admins can execute
}

// Policy-based authorization
[Authorize(Policy = "DoctorOnly")]
public async Task<IActionResult> SignNote(int noteId)
{
    // Uses custom policy defined in Program.cs
}
```

### Getting Current User's Roles

```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

// Check if user has specific role
if (User.IsInRole("Doctor"))
{
    // Doctor-specific logic
}
```

---

## 📊 Role Statistics

Based on typical healthcare facilities:

| Role | Typical % of Users | Average per 100 Patients |
|------|-------------------:|-------------------------:|
| Doctor | 15% | 3-5 physicians |
| Nurse | 35% | 8-12 nurses |
| Receptionist | 15% | 2-3 staff |
| Lab Technician | 10% | 1-2 techs |
| Billing Staff | 15% | 2-3 staff |
| Administrator | 10% | 1-2 admins |

---

## 🔧 Customization

### Adding Custom Roles

To add a new role, update `EMRDbContext.cs`:

```csharp
private void SeedRoles(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<IdentityRole>().HasData(
        // ... existing roles ...
        new IdentityRole
        {
            Id = "7",
            Name = "Pharmacist",
            NormalizedName = "PHARMACIST"
        }
    );
}
```

Then create migration and update database:
```bash
dotnet ef migrations add AddPharmacistRole
dotnet ef database update
```

---

## 📞 Support

For questions about role configuration:
- **Email**: support@emrsystem.com
- **Documentation**: [TECHNICAL_DOCUMENTATION.md](TECHNICAL_DOCUMENTATION.md)
- **GitHub Issues**: https://github.com/JoelHJames1/EMR-System/issues

---

**Document Version**: 1.0
**Last Updated**: 2024
**Maintained By**: EMR System Development Team