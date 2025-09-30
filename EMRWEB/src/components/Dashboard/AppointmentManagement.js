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
} from '@mui/material';
import {
  Add,
  Event,
  Cancel,
  CheckCircle,
  Schedule,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { appointmentAPI, patientAPI, providerAPI } from '../../services/api';
import { format, addDays, startOfWeek } from 'date-fns';

const AppointmentManagement = () => {
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [selectedDate, setSelectedDate] = useState(new Date());
  const [patients, setPatients] = useState([]);
  const [providers, setProviders] = useState([]);

  const { control, handleSubmit, reset, formState: { errors } } = useForm();

  useEffect(() => {
    fetchData();
  }, [selectedDate]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [patientsRes, providersRes] = await Promise.all([
        patientAPI.getAll(),
        providerAPI.getAll(),
      ]);
      setPatients(patientsRes.data);
      setProviders(providersRes.data);
    } catch (err) {
      setError('Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data) => {
    try {
      await appointmentAPI.create({
        ...data,
        appointmentDate: new Date(data.appointmentDate).toISOString(),
        status: 'Scheduled',
      });
      fetchData();
      setOpenDialog(false);
      reset();
    } catch (err) {
      setError('Failed to create appointment');
    }
  };

  const handleCancelAppointment = async (id) => {
    if (window.confirm('Cancel this appointment?')) {
      try {
        await appointmentAPI.cancel(id, 'Cancelled by user');
        fetchData();
      } catch (err) {
        setError('Failed to cancel appointment');
      }
    }
  };

  const getStatusColor = (status) => {
    const colors = {
      Scheduled: 'primary',
      Completed: 'success',
      Cancelled: 'error',
      NoShow: 'warning',
      InProgress: 'info',
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
              Appointment Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Schedule and manage patient appointments
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => setOpenDialog(true)}
            sx={{
              background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            }}
          >
            New Appointment
          </Button>
        </Box>
      </motion.div>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      {/* Calendar Week View */}
      <Grid container spacing={2}>
        {[0, 1, 2, 3, 4, 5, 6].map((dayOffset) => {
          const day = addDays(startOfWeek(selectedDate), dayOffset);
          return (
            <Grid item xs={12} md={1.7} key={dayOffset}>
              <Paper
                elevation={3}
                sx={{
                  p: 2,
                  minHeight: '400px',
                  background: format(day, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd')
                    ? 'linear-gradient(135deg, #667eea15 0%, #764ba215 100%)'
                    : 'white',
                }}
              >
                <Typography variant="h6" fontWeight="bold" align="center" gutterBottom>
                  {format(day, 'EEE')}
                </Typography>
                <Typography variant="body2" align="center" color="text.secondary" mb={2}>
                  {format(day, 'MMM d')}
                </Typography>

                {/* Placeholder for appointments */}
                <Box>
                  {[1, 2, 3].map((apt) => (
                    <Paper
                      key={apt}
                      sx={{
                        p: 1.5,
                        mb: 1,
                        background: '#f5f5f5',
                        borderLeft: '4px solid #667eea',
                        cursor: 'pointer',
                        '&:hover': { background: '#e0e0e0' },
                      }}
                    >
                      <Typography variant="caption" fontWeight="bold">
                        9:00 AM
                      </Typography>
                      <Typography variant="body2" noWrap>
                        Patient Name
                      </Typography>
                      <Chip label="Scheduled" size="small" color="primary" sx={{ mt: 0.5 }} />
                    </Paper>
                  ))}
                </Box>
              </Paper>
            </Grid>
          );
        })}
      </Grid>

      {/* Create Appointment Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="md" fullWidth>
        <DialogTitle>Schedule New Appointment</DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="patientId"
                  control={control}
                  rules={{ required: 'Patient is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Patient"
                      SelectProps={{ native: true }}
                      error={!!errors.patientId}
                      helperText={errors.patientId?.message}
                    >
                      <option value="">Select Patient</option>
                      {patients.map((patient) => (
                        <option key={patient.id} value={patient.id}>
                          {patient.firstName} {patient.lastName}
                        </option>
                      ))}
                    </TextField>
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
                      label="Provider"
                      SelectProps={{ native: true }}
                      error={!!errors.providerId}
                      helperText={errors.providerId?.message}
                    >
                      <option value="">Select Provider</option>
                      {providers.map((provider) => (
                        <option key={provider.id} value={provider.id}>
                          Dr. {provider.firstName} {provider.lastName} - {provider.specialization}
                        </option>
                      ))}
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="appointmentDate"
                  control={control}
                  rules={{ required: 'Date is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Date & Time"
                      type="datetime-local"
                      InputLabelProps={{ shrink: true }}
                      error={!!errors.appointmentDate}
                      helperText={errors.appointmentDate?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="appointmentType"
                  control={control}
                  rules={{ required: 'Type is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Appointment Type"
                      SelectProps={{ native: true }}
                      error={!!errors.appointmentType}
                      helperText={errors.appointmentType?.message}
                    >
                      <option value="">Select Type</option>
                      <option value="Consultation">Consultation</option>
                      <option value="Follow-up">Follow-up</option>
                      <option value="Annual Checkup">Annual Checkup</option>
                      <option value="Urgent Care">Urgent Care</option>
                      <option value="Procedure">Procedure</option>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name="reasonForVisit"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={3}
                      label="Reason for Visit"
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
              Schedule
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default AppointmentManagement;