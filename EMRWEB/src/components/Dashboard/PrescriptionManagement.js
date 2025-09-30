import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import {
  Box,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Grid,
  Paper,
  Typography,
  TextField,
  Chip,
  IconButton,
  Alert,
  CircularProgress,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  InputAdornment,
  Autocomplete,
  Card,
  CardContent,
  Divider,
} from '@mui/material';
import {
  Add,
  Edit,
  Block,
  Autorenew,
  Search,
  Medication,
  LocalPharmacy,
  Warning,
  CheckCircle,
  Print,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { prescriptionAPI, medicationAPI, patientAPI, providerAPI } from '../../services/api';
import { printPrescription } from '../../utils/printDocument';

const PrescriptionManagement = () => {
  const [prescriptions, setPrescriptions] = useState([]);
  const [medications, setMedications] = useState([]);
  const [patients, setPatients] = useState([]);
  const [providers, setProviders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  const { control, handleSubmit, reset, watch, formState: { errors } } = useForm();

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [medsRes, patientsRes, providersRes] = await Promise.all([
        medicationAPI.getAll(),
        patientAPI.getAll(),
        providerAPI.getAll(),
      ]);
      setMedications(medsRes.data);
      setPatients(patientsRes.data);
      setProviders(providersRes.data);
    } catch (err) {
      setError('Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const fetchPrescriptionsByPatient = async (patientId) => {
    try {
      const response = await prescriptionAPI.getByPatient(patientId);
      setPrescriptions(response.data);
    } catch (err) {
      setError('Failed to load prescriptions');
    }
  };

  const onSubmit = async (data) => {
    try {
      await prescriptionAPI.create({
        ...data,
        startDate: new Date().toISOString(),
        status: 'Active',
      });
      if (selectedPatient) {
        fetchPrescriptionsByPatient(selectedPatient.id);
      }
      setOpenDialog(false);
      reset();
    } catch (err) {
      setError('Failed to create prescription');
    }
  };

  const handleDiscontinue = async (id) => {
    if (window.confirm('Discontinue this prescription?')) {
      try {
        await prescriptionAPI.discontinue(id);
        if (selectedPatient) {
          fetchPrescriptionsByPatient(selectedPatient.id);
        }
      } catch (err) {
        setError('Failed to discontinue prescription');
      }
    }
  };

  const handleRefill = async (id) => {
    try {
      await prescriptionAPI.refill(id);
      if (selectedPatient) {
        fetchPrescriptionsByPatient(selectedPatient.id);
      }
    } catch (err) {
      setError('Failed to refill prescription');
    }
  };

  const getStatusColor = (status) => {
    const colors = {
      Active: 'success',
      Discontinued: 'error',
      Completed: 'default',
      OnHold: 'warning',
    };
    return colors[status] || 'default';
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
      <motion.div initial={{ opacity: 0, y: -20 }} animate={{ opacity: 1, y: 0 }}>
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
          <Box>
            <Typography variant="h4" fontWeight="bold" gutterBottom>
              Prescription Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage patient medications and prescriptions
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => setOpenDialog(true)}
            disabled={!selectedPatient}
            sx={{
              background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            }}
          >
            New Prescription
          </Button>
        </Box>
      </motion.div>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      <Grid container spacing={3}>
        {/* Patient Selection */}
        <Grid item xs={12} md={4}>
          <Paper elevation={3} sx={{ p: 3, height: 'calc(100vh - 250px)', overflow: 'auto' }}>
            <Typography variant="h6" fontWeight="bold" gutterBottom>
              Select Patient
            </Typography>
            <TextField
              fullWidth
              placeholder="Search patients..."
              variant="outlined"
              size="small"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              sx={{ mb: 2 }}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Search />
                  </InputAdornment>
                ),
              }}
            />
            {patients
              .filter(
                (p) =>
                  p.firstName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
                  p.lastName?.toLowerCase().includes(searchTerm.toLowerCase())
              )
              .map((patient) => (
                <Card
                  key={patient.id}
                  sx={{
                    mb: 1,
                    cursor: 'pointer',
                    border:
                      selectedPatient?.id === patient.id
                        ? '2px solid #667eea'
                        : '1px solid #e0e0e0',
                    '&:hover': {
                      boxShadow: 3,
                      borderColor: '#667eea',
                    },
                  }}
                  onClick={() => {
                    setSelectedPatient(patient);
                    fetchPrescriptionsByPatient(patient.id);
                  }}
                >
                  <CardContent>
                    <Typography variant="body1" fontWeight="bold">
                      {patient.firstName} {patient.lastName}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      DOB: {new Date(patient.dateOfBirth).toLocaleDateString()}
                    </Typography>
                    <br />
                    <Typography variant="caption" color="text.secondary">
                      MRN: {patient.mrn || 'N/A'}
                    </Typography>
                  </CardContent>
                </Card>
              ))}
          </Paper>
        </Grid>

        {/* Prescriptions List */}
        <Grid item xs={12} md={8}>
          {selectedPatient ? (
            <Paper elevation={3} sx={{ p: 3 }}>
              <Box display="flex" alignItems="center" mb={3}>
                <Medication sx={{ fontSize: 40, color: '#667eea', mr: 2 }} />
                <Box>
                  <Typography variant="h6" fontWeight="bold">
                    Prescriptions for {selectedPatient.firstName} {selectedPatient.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Active medications and prescription history
                  </Typography>
                </Box>
              </Box>

              <TableContainer>
                <Table>
                  <TableHead sx={{ bgcolor: '#f5f5f5' }}>
                    <TableRow>
                      <TableCell>Medication</TableCell>
                      <TableCell>Dosage</TableCell>
                      <TableCell>Frequency</TableCell>
                      <TableCell>Refills</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell align="right">Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {prescriptions.length > 0 ? (
                      prescriptions.map((prescription) => (
                        <TableRow
                          key={prescription.id}
                          hover
                          sx={{ '&:hover': { bgcolor: '#f9f9f9' } }}
                        >
                          <TableCell>
                            <Box display="flex" alignItems="center">
                              <LocalPharmacy sx={{ color: '#667eea', mr: 1 }} />
                              <Box>
                                <Typography variant="body2" fontWeight="bold">
                                  {prescription.medicationName}
                                </Typography>
                                <Typography variant="caption" color="text.secondary">
                                  Route: {prescription.route}
                                </Typography>
                              </Box>
                            </Box>
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2" fontWeight={600}>
                              {prescription.dosage}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2">{prescription.frequency}</Typography>
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={`${prescription.refills} left`}
                              size="small"
                              color={prescription.refills > 0 ? 'primary' : 'warning'}
                            />
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={prescription.status}
                              size="small"
                              color={getStatusColor(prescription.status)}
                            />
                          </TableCell>
                          <TableCell align="right">
                            <Tooltip title="Print Prescription">
                              <IconButton
                                onClick={() => {
                                  const provider = providers.find(p => p.id === prescription.providerId);
                                  printPrescription(prescription, selectedPatient, provider || {});
                                }}
                                color="secondary"
                              >
                                <Print />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title="Refill">
                              <IconButton
                                onClick={() => handleRefill(prescription.id)}
                                color="primary"
                                disabled={prescription.status !== 'Active'}
                              >
                                <Autorenew />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title="Discontinue">
                              <IconButton
                                onClick={() => handleDiscontinue(prescription.id)}
                                color="error"
                                disabled={prescription.status !== 'Active'}
                              >
                                <Block />
                              </IconButton>
                            </Tooltip>
                          </TableCell>
                        </TableRow>
                      ))
                    ) : (
                      <TableRow>
                        <TableCell colSpan={6} align="center">
                          <Typography color="text.secondary">
                            No prescriptions found for this patient
                          </Typography>
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            </Paper>
          ) : (
            <Paper
              elevation={3}
              sx={{
                p: 5,
                textAlign: 'center',
                height: 'calc(100vh - 250px)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
              }}
            >
              <Box>
                <Medication sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
                <Typography variant="h6" color="text.secondary">
                  Select a patient to view prescriptions
                </Typography>
              </Box>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Create Prescription Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="md" fullWidth>
        <DialogTitle>New Prescription</DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Alert severity="info" icon={<Warning />} sx={{ mb: 3 }}>
              Prescribing for: <strong>{selectedPatient?.firstName} {selectedPatient?.lastName}</strong>
            </Alert>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Controller
                  name="medicationId"
                  control={control}
                  rules={{ required: 'Medication is required' }}
                  render={({ field }) => (
                    <Autocomplete
                      {...field}
                      options={medications}
                      getOptionLabel={(option) => option.medicationName || ''}
                      onChange={(e, value) => field.onChange(value?.id)}
                      renderInput={(params) => (
                        <TextField
                          {...params}
                          label="Medication"
                          error={!!errors.medicationId}
                          helperText={errors.medicationId?.message}
                        />
                      )}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="dosage"
                  control={control}
                  rules={{ required: 'Dosage is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Dosage"
                      placeholder="e.g., 500mg"
                      error={!!errors.dosage}
                      helperText={errors.dosage?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="frequency"
                  control={control}
                  rules={{ required: 'Frequency is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Frequency"
                      SelectProps={{ native: true }}
                      error={!!errors.frequency}
                      helperText={errors.frequency?.message}
                    >
                      <option value="">Select Frequency</option>
                      <option value="Once Daily">Once Daily</option>
                      <option value="Twice Daily">Twice Daily</option>
                      <option value="Three Times Daily">Three Times Daily</option>
                      <option value="Four Times Daily">Four Times Daily</option>
                      <option value="Every 4 Hours">Every 4 Hours</option>
                      <option value="Every 6 Hours">Every 6 Hours</option>
                      <option value="Every 8 Hours">Every 8 Hours</option>
                      <option value="As Needed">As Needed</option>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="route"
                  control={control}
                  rules={{ required: 'Route is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Route"
                      SelectProps={{ native: true }}
                      error={!!errors.route}
                      helperText={errors.route?.message}
                    >
                      <option value="">Select Route</option>
                      <option value="Oral">Oral</option>
                      <option value="IV">Intravenous (IV)</option>
                      <option value="IM">Intramuscular (IM)</option>
                      <option value="Subcutaneous">Subcutaneous</option>
                      <option value="Topical">Topical</option>
                      <option value="Inhalation">Inhalation</option>
                      <option value="Rectal">Rectal</option>
                      <option value="Ophthalmic">Ophthalmic</option>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="quantity"
                  control={control}
                  rules={{ required: 'Quantity is required', min: 1 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Quantity"
                      error={!!errors.quantity}
                      helperText={errors.quantity?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="refills"
                  control={control}
                  rules={{ required: 'Refills is required', min: 0 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Number of Refills"
                      error={!!errors.refills}
                      helperText={errors.refills?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="providerId"
                  control={control}
                  rules={{ required: 'Provider is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Prescribing Provider"
                      SelectProps={{ native: true }}
                      error={!!errors.providerId}
                      helperText={errors.providerId?.message}
                    >
                      <option value="">Select Provider</option>
                      {providers.map((provider) => (
                        <option key={provider.id} value={provider.id}>
                          Dr. {provider.firstName} {provider.lastName}
                        </option>
                      ))}
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name="instructions"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={3}
                      label="Instructions for Patient"
                      placeholder="e.g., Take with food, avoid alcohol"
                    />
                  )}
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
            <Button
              type="submit"
              variant="contained"
              sx={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
              }}
            >
              Create Prescription
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default PrescriptionManagement;