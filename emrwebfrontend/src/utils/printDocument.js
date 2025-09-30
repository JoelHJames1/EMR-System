// Document printing and generation utilities for EMR system

export const printPrescription = (prescription, patient, provider) => {
  const printWindow = window.open('', '_blank');

  const html = `
    <!DOCTYPE html>
    <html>
    <head>
      <title>Prescription - ${patient.firstName} ${patient.lastName}</title>
      <style>
        @media print {
          @page {
            size: letter;
            margin: 0.5in;
          }
          body {
            -webkit-print-color-adjust: exact;
            print-color-adjust: exact;
          }
        }
        body {
          font-family: 'Arial', sans-serif;
          padding: 20px;
          max-width: 8.5in;
          margin: 0 auto;
        }
        .header {
          border-bottom: 3px solid #667eea;
          padding-bottom: 15px;
          margin-bottom: 20px;
        }
        .clinic-name {
          font-size: 24px;
          font-weight: bold;
          color: #667eea;
          margin-bottom: 5px;
        }
        .clinic-info {
          font-size: 12px;
          color: #666;
        }
        .rx-symbol {
          font-size: 48px;
          font-weight: bold;
          color: #667eea;
          text-align: center;
          margin: 20px 0;
        }
        .section {
          margin-bottom: 20px;
          padding: 15px;
          border: 1px solid #e0e0e0;
          border-radius: 8px;
        }
        .section-title {
          font-weight: bold;
          color: #333;
          margin-bottom: 10px;
          font-size: 14px;
          text-transform: uppercase;
        }
        .field {
          margin: 8px 0;
          font-size: 13px;
        }
        .field-label {
          font-weight: bold;
          color: #666;
          display: inline-block;
          width: 150px;
        }
        .field-value {
          color: #000;
        }
        .medication-box {
          background: linear-gradient(135deg, #667eea15 0%, #764ba215 100%);
          padding: 20px;
          border-left: 5px solid #667eea;
          margin: 20px 0;
        }
        .medication-name {
          font-size: 20px;
          font-weight: bold;
          color: #667eea;
          margin-bottom: 10px;
        }
        .instructions {
          background: #f9f9f9;
          padding: 15px;
          border-radius: 8px;
          margin: 15px 0;
          border-left: 4px solid #43e97b;
        }
        .instructions-title {
          font-weight: bold;
          color: #333;
          margin-bottom: 10px;
        }
        .footer {
          margin-top: 40px;
          border-top: 2px solid #e0e0e0;
          padding-top: 20px;
        }
        .signature-line {
          border-bottom: 1px solid #000;
          width: 300px;
          margin: 30px 0 5px 0;
        }
        .signature-label {
          font-size: 11px;
          color: #666;
        }
        .warning-box {
          background: #fff3cd;
          border: 1px solid #ffc107;
          padding: 10px;
          border-radius: 5px;
          margin: 15px 0;
          font-size: 11px;
        }
        .print-date {
          text-align: right;
          font-size: 11px;
          color: #666;
          margin-top: 20px;
        }
      </style>
    </head>
    <body>
      <div class="header">
        <div class="clinic-name">Healthcare Medical Center</div>
        <div class="clinic-info">
          123 Medical Drive, Suite 100 | City, State 12345<br>
          Phone: (555) 123-4567 | Fax: (555) 123-4568<br>
          DEA#: XX1234567 | NPI#: 1234567890
        </div>
      </div>

      <div class="rx-symbol">℞</div>

      <div class="section">
        <div class="section-title">Patient Information</div>
        <div class="field">
          <span class="field-label">Patient Name:</span>
          <span class="field-value">${patient.firstName} ${patient.lastName}</span>
        </div>
        <div class="field">
          <span class="field-label">Date of Birth:</span>
          <span class="field-value">${new Date(patient.dateOfBirth).toLocaleDateString()}</span>
        </div>
        <div class="field">
          <span class="field-label">Medical Record #:</span>
          <span class="field-value">${patient.mrn || 'N/A'}</span>
        </div>
        <div class="field">
          <span class="field-label">Address:</span>
          <span class="field-value">${patient.address || 'On file'}</span>
        </div>
        <div class="field">
          <span class="field-label">Phone:</span>
          <span class="field-value">${patient.phoneNumber}</span>
        </div>
      </div>

      <div class="medication-box">
        <div class="medication-name">${prescription.medicationName}</div>
        <div class="field">
          <span class="field-label">Dosage:</span>
          <span class="field-value">${prescription.dosage}</span>
        </div>
        <div class="field">
          <span class="field-label">Route:</span>
          <span class="field-value">${prescription.route}</span>
        </div>
        <div class="field">
          <span class="field-label">Frequency:</span>
          <span class="field-value">${prescription.frequency}</span>
        </div>
        <div class="field">
          <span class="field-label">Quantity:</span>
          <span class="field-value">${prescription.quantity}</span>
        </div>
        <div class="field">
          <span class="field-label">Refills:</span>
          <span class="field-value">${prescription.refills}</span>
        </div>
      </div>

      ${prescription.instructions ? `
        <div class="instructions">
          <div class="instructions-title">Instructions for Patient:</div>
          ${prescription.instructions}
        </div>
      ` : ''}

      <div class="warning-box">
        ⚠️ This prescription is valid for ${prescription.refills > 0 ? '1 year' : '6 months'} from date of issue.
        Do not share this medication with others. Keep out of reach of children.
      </div>

      <div class="footer">
        <div class="section">
          <div class="section-title">Prescriber Information</div>
          <div class="field">
            <span class="field-label">Provider Name:</span>
            <span class="field-value">Dr. ${provider.firstName} ${provider.lastName}</span>
          </div>
          <div class="field">
            <span class="field-label">Specialization:</span>
            <span class="field-value">${provider.specialization || 'General Medicine'}</span>
          </div>
          <div class="field">
            <span class="field-label">License Number:</span>
            <span class="field-value">${provider.licenseNumber || 'On file'}</span>
          </div>
          <div class="field">
            <span class="field-label">Phone:</span>
            <span class="field-value">${provider.phoneNumber || provider.officePhone || '(555) 123-4567'}</span>
          </div>
        </div>

        <div class="signature-line"></div>
        <div class="signature-label">Provider Signature</div>

        <div class="print-date">
          Printed on: ${new Date().toLocaleString()}<br>
          Prescription Date: ${new Date(prescription.startDate || new Date()).toLocaleDateString()}
        </div>
      </div>
    </body>
    </html>
  `;

  printWindow.document.write(html);
  printWindow.document.close();
  printWindow.focus();

  setTimeout(() => {
    printWindow.print();
  }, 250);
};

export const printLabOrder = (labOrder, patient, provider) => {
  const printWindow = window.open('', '_blank');

  const html = `
    <!DOCTYPE html>
    <html>
    <head>
      <title>Lab Order - ${patient.firstName} ${patient.lastName}</title>
      <style>
        @media print {
          @page { size: letter; margin: 0.5in; }
          body { -webkit-print-color-adjust: exact; print-color-adjust: exact; }
        }
        body {
          font-family: 'Arial', sans-serif;
          padding: 20px;
          max-width: 8.5in;
          margin: 0 auto;
        }
        .header {
          border-bottom: 3px solid #4facfe;
          padding-bottom: 15px;
          margin-bottom: 20px;
        }
        .clinic-name {
          font-size: 24px;
          font-weight: bold;
          color: #4facfe;
        }
        .section {
          margin: 20px 0;
          padding: 15px;
          border: 1px solid #e0e0e0;
          border-radius: 8px;
        }
        .section-title {
          font-weight: bold;
          color: #333;
          margin-bottom: 10px;
          font-size: 14px;
          text-transform: uppercase;
        }
        .test-box {
          background: linear-gradient(135deg, #4facfe15 0%, #00f2fe15 100%);
          padding: 20px;
          border-left: 5px solid #4facfe;
          margin: 20px 0;
        }
        .test-name {
          font-size: 20px;
          font-weight: bold;
          color: #4facfe;
        }
        .priority-badge {
          display: inline-block;
          padding: 5px 15px;
          border-radius: 20px;
          font-weight: bold;
          margin-top: 10px;
        }
        .priority-stat { background: #f44336; color: white; }
        .priority-urgent { background: #ff9800; color: white; }
        .priority-routine { background: #4caf50; color: white; }
        .field {
          margin: 8px 0;
          font-size: 13px;
        }
        .field-label {
          font-weight: bold;
          color: #666;
          display: inline-block;
          width: 150px;
        }
      </style>
    </head>
    <body>
      <div class="header">
        <div class="clinic-name">Healthcare Laboratory Services</div>
        <div>123 Medical Drive | Phone: (555) 123-4567</div>
      </div>

      <div class="section">
        <div class="section-title">Patient Information</div>
        <div class="field">
          <span class="field-label">Patient Name:</span>
          ${patient.firstName} ${patient.lastName}
        </div>
        <div class="field">
          <span class="field-label">Date of Birth:</span>
          ${new Date(patient.dateOfBirth).toLocaleDateString()}
        </div>
        <div class="field">
          <span class="field-label">MRN:</span>
          ${patient.mrn || 'N/A'}
        </div>
      </div>

      <div class="test-box">
        <div class="test-name">${labOrder.testName}</div>
        <div class="field">
          <span class="field-label">Test Code:</span>
          ${labOrder.testCode}
        </div>
        <div class="priority-badge priority-${labOrder.priority.toLowerCase()}">
          ${labOrder.priority} Priority
        </div>
      </div>

      ${labOrder.clinicalInfo ? `
        <div class="section">
          <div class="section-title">Clinical Information</div>
          ${labOrder.clinicalInfo}
        </div>
      ` : ''}

      <div class="section">
        <div class="section-title">Ordering Provider</div>
        <div class="field">
          <span class="field-label">Provider:</span>
          Dr. ${provider.firstName} ${provider.lastName}
        </div>
        <div class="field">
          <span class="field-label">Order Date:</span>
          ${new Date(labOrder.orderDate).toLocaleString()}
        </div>
      </div>

      <div style="margin-top: 40px; text-align: right; font-size: 11px; color: #666;">
        Printed: ${new Date().toLocaleString()}
      </div>
    </body>
    </html>
  `;

  printWindow.document.write(html);
  printWindow.document.close();
  printWindow.focus();

  setTimeout(() => {
    printWindow.print();
  }, 250);
};

export const printPatientSummary = (patient, vitals = [], allergies = [], medications = []) => {
  const printWindow = window.open('', '_blank');

  const html = `
    <!DOCTYPE html>
    <html>
    <head>
      <title>Patient Summary - ${patient.firstName} ${patient.lastName}</title>
      <style>
        @media print {
          @page { size: letter; margin: 0.5in; }
          body { -webkit-print-color-adjust: exact; print-color-adjust: exact; }
        }
        body {
          font-family: 'Arial', sans-serif;
          padding: 20px;
          max-width: 8.5in;
          margin: 0 auto;
        }
        .header {
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          color: white;
          padding: 20px;
          border-radius: 8px;
          margin-bottom: 20px;
        }
        .patient-name {
          font-size: 24px;
          font-weight: bold;
        }
        .section {
          margin: 20px 0;
          padding: 15px;
          border: 1px solid #e0e0e0;
          border-radius: 8px;
        }
        .section-title {
          font-weight: bold;
          color: #667eea;
          font-size: 16px;
          margin-bottom: 15px;
          border-bottom: 2px solid #667eea;
          padding-bottom: 5px;
        }
        .field {
          margin: 8px 0;
          font-size: 13px;
        }
        .field-label {
          font-weight: bold;
          color: #666;
          display: inline-block;
          width: 150px;
        }
        .alert-box {
          background: #fff3cd;
          border-left: 4px solid #ffc107;
          padding: 15px;
          margin: 10px 0;
        }
        .allergy-item {
          background: #ffebee;
          border-left: 4px solid #f44336;
          padding: 10px;
          margin: 5px 0;
        }
        table {
          width: 100%;
          border-collapse: collapse;
          margin: 10px 0;
        }
        th, td {
          border: 1px solid #ddd;
          padding: 8px;
          text-align: left;
        }
        th {
          background-color: #667eea;
          color: white;
        }
      </style>
    </head>
    <body>
      <div class="header">
        <div class="patient-name">${patient.firstName} ${patient.lastName}</div>
        <div>DOB: ${new Date(patient.dateOfBirth).toLocaleDateString()} | MRN: ${patient.mrn || 'N/A'}</div>
      </div>

      <div class="section">
        <div class="section-title">Demographics</div>
        <div class="field">
          <span class="field-label">Gender:</span>
          ${patient.gender}
        </div>
        <div class="field">
          <span class="field-label">Blood Type:</span>
          ${patient.bloodType || 'Unknown'}
        </div>
        <div class="field">
          <span class="field-label">Phone:</span>
          ${patient.phoneNumber}
        </div>
        <div class="field">
          <span class="field-label">Email:</span>
          ${patient.email}
        </div>
        <div class="field">
          <span class="field-label">Emergency Contact:</span>
          ${patient.emergencyContactName || 'N/A'} - ${patient.emergencyContactPhone || 'N/A'}
        </div>
      </div>

      ${allergies.length > 0 ? `
        <div class="section">
          <div class="section-title">⚠️ Allergies</div>
          ${allergies.map(a => `
            <div class="allergy-item">
              <strong>${a.allergen}</strong> - ${a.severity}<br>
              <small>Reaction: ${a.reaction}</small>
            </div>
          `).join('')}
        </div>
      ` : ''}

      ${medications.length > 0 ? `
        <div class="section">
          <div class="section-title">Current Medications</div>
          <table>
            <tr>
              <th>Medication</th>
              <th>Dosage</th>
              <th>Frequency</th>
            </tr>
            ${medications.map(m => `
              <tr>
                <td>${m.medicationName}</td>
                <td>${m.dosage}</td>
                <td>${m.frequency}</td>
              </tr>
            `).join('')}
          </table>
        </div>
      ` : ''}

      <div style="margin-top: 40px; text-align: center; font-size: 11px; color: #666;">
        This document contains confidential patient health information.<br>
        Generated: ${new Date().toLocaleString()}
      </div>
    </body>
    </html>
  `;

  printWindow.document.write(html);
  printWindow.document.close();
  printWindow.focus();

  setTimeout(() => {
    printWindow.print();
  }, 250);
};

export default {
  printPrescription,
  printLabOrder,
  printPatientSummary,
};