import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import {
  Box,
  Button,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Chip,
  Typography,
  InputAdornment,
  Grid,
  Avatar,
  Tooltip,
  Alert,
  CircularProgress,
  Tabs,
  Tab,
} from '@mui/material';
import {
  Add,
  Edit,
  Delete,
  Search,
  Visibility,
  Person,
  LocalHospital,
  MedicalServices,
  Assignment,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { patientAPI, allergyAPI, immunizationAPI } from '../../services/api';

const PatientManagement = () => {
  const [patients, setPatients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [viewDialog, setViewDialog] = useState(false);
  const [tabValue, setTabValue] = useState(0);
  const [patientDetails, setPatientDetails] = useState(null);

  const { control, handleSubmit, reset, formState: { errors } } = useForm();

  useEffect(() => {
    fetchPatients();
  }, [searchTerm]);

  const fetchPatients = async () => {
    try {
      setLoading(true);
      const response = await patientAPI.getAll(1, 50, searchTerm);
      setPatients(response.data);
    } catch (err) {
      setError('Failed to load patients');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (patient = null) => {
    setSelectedPatient(patient);
    if (patient) {
      reset(patient);
    } else {
      reset({
        firstName: '',
        lastName: '',
        dateOfBirth: '',
        gender: '',
        email: '',
        phoneNumber: '',
        ssn: '',
        bloodType: '',
        maritalStatus: '',
        emergencyContactName: '',
        emergencyContactPhone: '',
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedPatient(null);
  };

  const handleViewPatient = async (patient) => {
    try {
      const [patientRes, allergiesRes, immunizationsRes] = await Promise.all([
        patientAPI.getById(patient.id),
        allergyAPI.getByPatient(patient.id),
        immunizationAPI.getByPatient(patient.id),
      ]);

      setPatientDetails({
        ...patientRes.data,
        allergies: allergiesRes.data,
        immunizations: immunizationsRes.data,
      });
      setViewDialog(true);
    } catch (err) {
      setError('Failed to load patient details');
    }
  };

  const onSubmit = async (data) => {
    try {
      if (selectedPatient) {
        await patientAPI.update(selectedPatient.id, data);
      } else {
        await patientAPI.create(data);
      }
      fetchPatients();
      handleCloseDialog();
    } catch (err) {
      setError('Failed to save patient');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this patient?')) {
      try {
        await patientAPI.delete(id);
        fetchPatients();
      } catch (err) {
        setError('Failed to delete patient');
      }
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress size={60} />
      </Box>
    );
  }

  return (
    <Box>
      <motion.div
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
      >
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
          <Box>
            <Typography variant="h4" fontWeight="bold" gutterBottom>
              Patient Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage patient records and medical information
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => handleOpenDialog()}
            sx={{
              background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
              '&:hover': {
                background: 'linear-gradient(135deg, #764ba2 0%, #667eea 100%)',
              },
            }}
          >
            Add Patient
          </Button>
        </Box>
      </motion.div>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      {/* Search Bar */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.1 }}
      >
        <Paper elevation={3} sx={{ p: 2, mb: 3 }}>
          <TextField
            fullWidth
            placeholder="Search patients by name, email, or phone..."
            variant="outlined"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <Search />
                </InputAdornment>
              ),
            }}
          />
        </Paper>
      </motion.div>

      {/* Patients Table */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.2 }}
      >
        <TableContainer component={Paper} elevation={3}>
          <Table>
            <TableHead sx={{ bgcolor: '#f5f5f5' }}>
              <TableRow>
                <TableCell>Patient</TableCell>
                <TableCell>Date of Birth</TableCell>
                <TableCell>Gender</TableCell>
                <TableCell>Contact</TableCell>
                <TableCell>Blood Type</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {patients.map((patient) => (
                <TableRow
                  key={patient.id}
                  hover
                  sx={{ '&:hover': { bgcolor: '#f9f9f9' } }}
                >
                  <TableCell>
                    <Box display="flex" alignItems="center">
                      <Avatar sx={{ mr: 2, bgcolor: '#667eea' }}>
                        {patient.firstName?.[0]}{patient.lastName?.[0]}
                      </Avatar>
                      <Box>
                        <Typography variant="body1" fontWeight="bold">
                          {patient.firstName} {patient.lastName}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          MRN: {patient.mrn || 'N/A'}
                        </Typography>
                      </Box>
                    </Box>
                  </TableCell>
                  <TableCell>
                    {patient.dateOfBirth
                      ? new Date(patient.dateOfBirth).toLocaleDateString()
                      : 'N/A'}
                  </TableCell>
                  <TableCell>{patient.gender || 'N/A'}</TableCell>
                  <TableCell>
                    <Typography variant="body2">{patient.email}</Typography>
                    <Typography variant="caption" color="text.secondary">
                      {patient.phoneNumber}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip label={patient.bloodType || 'Unknown'} size="small" color="error" variant="outlined" />
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={patient.isActive ? 'Active' : 'Inactive'}
                      size="small"
                      color={patient.isActive ? 'success' : 'default'}
                    />
                  </TableCell>
                  <TableCell align="right">
                    <Tooltip title="View Details">
                      <IconButton onClick={() => handleViewPatient(patient)} color="info">
                        <Visibility />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Edit">
                      <IconButton onClick={() => handleOpenDialog(patient)} color="primary">
                        <Edit />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Delete">
                      <IconButton onClick={() => handleDelete(patient.id)} color="error">
                        <Delete />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </motion.div>

      {/* Add/Edit Patient Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {selectedPatient ? 'Edit Patient' : 'Add New Patient'}
        </DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="firstName"
                  control={control}
                  rules={{ required: 'First name is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="First Name"
                      error={!!errors.firstName}
                      helperText={errors.firstName?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="lastName"
                  control={control}
                  rules={{ required: 'Last name is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Last Name"
                      error={!!errors.lastName}
                      helperText={errors.lastName?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="dateOfBirth"
                  control={control}
                  rules={{ required: 'Date of birth is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Date of Birth"
                      type="date"
                      InputLabelProps={{ shrink: true }}
                      error={!!errors.dateOfBirth}
                      helperText={errors.dateOfBirth?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="gender"
                  control={control}
                  rules={{ required: 'Gender is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Gender"
                      SelectProps={{ native: true }}
                      error={!!errors.gender}
                      helperText={errors.gender?.message}
                    >
                      <option value="">Select Gender</option>
                      <option value="Male">Male</option>
                      <option value="Female">Female</option>
                      <option value="Other">Other</option>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="email"
                  control={control}
                  rules={{
                    required: 'Email is required',
                    pattern: {
                      value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                      message: 'Invalid email',
                    },
                  }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Email"
                      error={!!errors.email}
                      helperText={errors.email?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="phoneNumber"
                  control={control}
                  rules={{ required: 'Phone number is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Phone Number"
                      error={!!errors.phoneNumber}
                      helperText={errors.phoneNumber?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="ssn"
                  control={control}
                  render={({ field }) => (
                    <TextField {...field} fullWidth label="SSN" />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="bloodType"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Blood Type"
                      SelectProps={{ native: true }}
                    >
                      <option value="">Select Blood Type</option>
                      <option value="A+">A+</option>
                      <option value="A-">A-</option>
                      <option value="B+">B+</option>
                      <option value="B-">B-</option>
                      <option value="AB+">AB+</option>
                      <option value="AB-">AB-</option>
                      <option value="O+">O+</option>
                      <option value="O-">O-</option>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="maritalStatus"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Marital Status"
                      SelectProps={{ native: true }}
                    >
                      <option value="">Select Status</option>
                      <option value="Single">Single</option>
                      <option value="Married">Married</option>
                      <option value="Divorced">Divorced</option>
                      <option value="Widowed">Widowed</option>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="emergencyContactName"
                  control={control}
                  render={({ field }) => (
                    <TextField {...field} fullWidth label="Emergency Contact Name" />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="emergencyContactPhone"
                  control={control}
                  render={({ field }) => (
                    <TextField {...field} fullWidth label="Emergency Contact Phone" />
                  )}
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog}>Cancel</Button>
            <Button
              type="submit"
              variant="contained"
              sx={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
              }}
            >
              {selectedPatient ? 'Update' : 'Create'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* View Patient Details Dialog */}
      <Dialog
        open={viewDialog}
        onClose={() => setViewDialog(false)}
        maxWidth="lg"
        fullWidth
      >
        <DialogTitle>Patient Details</DialogTitle>
        <DialogContent>
          {patientDetails && (
            <Box>
              <Tabs value={tabValue} onChange={(e, v) => setTabValue(v)}>
                <Tab label="Demographics" icon={<Person />} />
                <Tab label="Allergies" icon={<LocalHospital />} />
                <Tab label="Immunizations" icon={<MedicalServices />} />
              </Tabs>

              {tabValue === 0 && (
                <Box mt={3}>
                  <Grid container spacing={2}>
                    <Grid item xs={6}>
                      <Typography variant="body2" color="text.secondary">Name</Typography>
                      <Typography variant="body1" fontWeight="bold">
                        {patientDetails.firstName} {patientDetails.lastName}
                      </Typography>
                    </Grid>
                    <Grid item xs={6}>
                      <Typography variant="body2" color="text.secondary">DOB</Typography>
                      <Typography variant="body1" fontWeight="bold">
                        {new Date(patientDetails.dateOfBirth).toLocaleDateString()}
                      </Typography>
                    </Grid>
                    <Grid item xs={6}>
                      <Typography variant="body2" color="text.secondary">Gender</Typography>
                      <Typography variant="body1" fontWeight="bold">
                        {patientDetails.gender}
                      </Typography>
                    </Grid>
                    <Grid item xs={6}>
                      <Typography variant="body2" color="text.secondary">Blood Type</Typography>
                      <Typography variant="body1" fontWeight="bold">
                        {patientDetails.bloodType}
                      </Typography>
                    </Grid>
                  </Grid>
                </Box>
              )}

              {tabValue === 1 && (
                <Box mt={3}>
                  {patientDetails.allergies?.length > 0 ? (
                    patientDetails.allergies.map((allergy, index) => (
                      <Paper key={index} sx={{ p: 2, mb: 2 }}>
                        <Typography variant="body1" fontWeight="bold">
                          {allergy.allergen}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                          Severity: {allergy.severity} | Reaction: {allergy.reaction}
                        </Typography>
                      </Paper>
                    ))
                  ) : (
                    <Typography>No allergies recorded</Typography>
                  )}
                </Box>
              )}

              {tabValue === 2 && (
                <Box mt={3}>
                  {patientDetails.immunizations?.length > 0 ? (
                    patientDetails.immunizations.map((immunization, index) => (
                      <Paper key={index} sx={{ p: 2, mb: 2 }}>
                        <Typography variant="body1" fontWeight="bold">
                          {immunization.vaccineName}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                          Date: {new Date(immunization.administeredDate).toLocaleDateString()} |
                          Dose: {immunization.doseNumber}
                        </Typography>
                      </Paper>
                    ))
                  ) : (
                    <Typography>No immunizations recorded</Typography>
                  )}
                </Box>
              )}
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setViewDialog(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default PatientManagement;