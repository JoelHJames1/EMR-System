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
  Alert,
  CircularProgress,
  Card,
  CardContent,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material';
import {
  Add,
  FitnessCenter,
  Favorite,
  Thermostat,
  MonitorWeight,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { observationAPI, patientAPI } from '../../services/api';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

const VitalsManagement = () => {
  const [vitals, setVitals] = useState([]);
  const [patients, setPatients] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);

  const { control, handleSubmit, reset, formState: { errors } } = useForm();

  useEffect(() => {
    fetchPatients();
  }, []);

  const fetchPatients = async () => {
    try {
      setLoading(true);
      const response = await patientAPI.getAll();
      setPatients(response.data);
    } catch (err) {
      setError('Failed to load patients');
    } finally {
      setLoading(false);
    }
  };

  const fetchVitals = async (patientId) => {
    try {
      const response = await observationAPI.getByPatient(patientId);
      setVitals(response.data);
    } catch (err) {
      setError('Failed to load vitals');
    }
  };

  const onSubmit = async (data) => {
    try {
      await observationAPI.create({
        patientId: selectedPatient.id,
        observationType: 'Vital Signs',
        observationDate: new Date().toISOString(),
        status: 'Final',
        components: [
          { name: 'Blood Pressure', value: `${data.systolic}/${data.diastolic}`, unit: 'mmHg' },
          { name: 'Heart Rate', value: data.heartRate, unit: 'bpm' },
          { name: 'Temperature', value: data.temperature, unit: '°F' },
          { name: 'Respiratory Rate', value: data.respiratoryRate, unit: '/min' },
          { name: 'Oxygen Saturation', value: data.oxygenSaturation, unit: '%' },
          { name: 'Weight', value: data.weight, unit: 'lbs' },
          { name: 'Height', value: data.height, unit: 'in' },
        ],
      });
      fetchVitals(selectedPatient.id);
      setOpenDialog(false);
      reset();
    } catch (err) {
      setError('Failed to record vitals');
    }
  };

  const getLatestVital = (type) => {
    if (vitals.length === 0) return 'N/A';
    const latest = vitals[0];
    const component = latest.components?.find((c) => c.name === type);
    return component ? `${component.value} ${component.unit}` : 'N/A';
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
              Vital Signs & Observations
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Track patient vital signs and measurements
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
            Record Vitals
          </Button>
        </Box>
      </motion.div>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      <Grid container spacing={3}>
        <Grid item xs={12} md={4}>
          <Paper elevation={3} sx={{ p: 3, maxHeight: 'calc(100vh - 250px)', overflow: 'auto' }}>
            <Typography variant="h6" fontWeight="bold" gutterBottom>
              Select Patient
            </Typography>
            {patients.map((patient) => (
              <Card
                key={patient.id}
                sx={{
                  mb: 1,
                  cursor: 'pointer',
                  border:
                    selectedPatient?.id === patient.id
                      ? '2px solid #667eea'
                      : '1px solid #e0e0e0',
                  '&:hover': { boxShadow: 3 },
                }}
                onClick={() => {
                  setSelectedPatient(patient);
                  fetchVitals(patient.id);
                }}
              >
                <CardContent>
                  <Typography variant="body1" fontWeight="bold">
                    {patient.firstName} {patient.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    DOB: {new Date(patient.dateOfBirth).toLocaleDateString()}
                  </Typography>
                </CardContent>
              </Card>
            ))}
          </Paper>
        </Grid>

        <Grid item xs={12} md={8}>
          {selectedPatient ? (
            <Box>
              {/* Latest Vitals Cards */}
              <Grid container spacing={2} mb={3}>
                <Grid item xs={6} sm={3}>
                  <Card elevation={3}>
                    <CardContent>
                      <Favorite sx={{ color: '#f44336', fontSize: 30, mb: 1 }} />
                      <Typography variant="caption" color="text.secondary">
                        Blood Pressure
                      </Typography>
                      <Typography variant="h6" fontWeight="bold">
                        {getLatestVital('Blood Pressure')}
                      </Typography>
                    </CardContent>
                  </Card>
                </Grid>
                <Grid item xs={6} sm={3}>
                  <Card elevation={3}>
                    <CardContent>
                      <FitnessCenter sx={{ color: '#667eea', fontSize: 30, mb: 1 }} />
                      <Typography variant="caption" color="text.secondary">
                        Heart Rate
                      </Typography>
                      <Typography variant="h6" fontWeight="bold">
                        {getLatestVital('Heart Rate')}
                      </Typography>
                    </CardContent>
                  </Card>
                </Grid>
                <Grid item xs={6} sm={3}>
                  <Card elevation={3}>
                    <CardContent>
                      <Thermostat sx={{ color: '#ff9800', fontSize: 30, mb: 1 }} />
                      <Typography variant="caption" color="text.secondary">
                        Temperature
                      </Typography>
                      <Typography variant="h6" fontWeight="bold">
                        {getLatestVital('Temperature')}
                      </Typography>
                    </CardContent>
                  </Card>
                </Grid>
                <Grid item xs={6} sm={3}>
                  <Card elevation={3}>
                    <CardContent>
                      <MonitorWeight sx={{ color: '#4caf50', fontSize: 30, mb: 1 }} />
                      <Typography variant="caption" color="text.secondary">
                        O2 Saturation
                      </Typography>
                      <Typography variant="h6" fontWeight="bold">
                        {getLatestVital('Oxygen Saturation')}
                      </Typography>
                    </CardContent>
                  </Card>
                </Grid>
              </Grid>

              {/* Vitals History Table */}
              <Paper elevation={3} sx={{ p: 3 }}>
                <Typography variant="h6" fontWeight="bold" gutterBottom>
                  Vitals History
                </Typography>
                <TableContainer>
                  <Table>
                    <TableHead sx={{ bgcolor: '#f5f5f5' }}>
                      <TableRow>
                        <TableCell>Date</TableCell>
                        <TableCell>BP</TableCell>
                        <TableCell>HR</TableCell>
                        <TableCell>Temp</TableCell>
                        <TableCell>O2</TableCell>
                        <TableCell>Weight</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {vitals.length > 0 ? (
                        vitals.map((vital, index) => (
                          <TableRow key={index} hover>
                            <TableCell>
                              {new Date(vital.observationDate).toLocaleString()}
                            </TableCell>
                            <TableCell>
                              {vital.components?.find((c) => c.name === 'Blood Pressure')?.value || 'N/A'}
                            </TableCell>
                            <TableCell>
                              {vital.components?.find((c) => c.name === 'Heart Rate')?.value || 'N/A'}
                            </TableCell>
                            <TableCell>
                              {vital.components?.find((c) => c.name === 'Temperature')?.value || 'N/A'}
                            </TableCell>
                            <TableCell>
                              {vital.components?.find((c) => c.name === 'Oxygen Saturation')?.value || 'N/A'}
                            </TableCell>
                            <TableCell>
                              {vital.components?.find((c) => c.name === 'Weight')?.value || 'N/A'}
                            </TableCell>
                          </TableRow>
                        ))
                      ) : (
                        <TableRow>
                          <TableCell colSpan={6} align="center">
                            <Typography color="text.secondary">
                              No vitals recorded
                            </Typography>
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>
              </Paper>
            </Box>
          ) : (
            <Paper elevation={3} sx={{ p: 5, textAlign: 'center', minHeight: 400 }}>
              <FitnessCenter sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select a patient to view vitals
              </Typography>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Record Vitals Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="md" fullWidth>
        <DialogTitle>Record Vital Signs</DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={6}>
                <Controller
                  name="systolic"
                  control={control}
                  rules={{ required: 'Required', min: 50, max: 250 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Systolic BP (mmHg)"
                      error={!!errors.systolic}
                      helperText={errors.systolic?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6}>
                <Controller
                  name="diastolic"
                  control={control}
                  rules={{ required: 'Required', min: 30, max: 150 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Diastolic BP (mmHg)"
                      error={!!errors.diastolic}
                      helperText={errors.diastolic?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6}>
                <Controller
                  name="heartRate"
                  control={control}
                  rules={{ required: 'Required', min: 30, max: 200 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Heart Rate (bpm)"
                      error={!!errors.heartRate}
                      helperText={errors.heartRate?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6}>
                <Controller
                  name="temperature"
                  control={control}
                  rules={{ required: 'Required', min: 95, max: 106 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Temperature (°F)"
                      error={!!errors.temperature}
                      helperText={errors.temperature?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6}>
                <Controller
                  name="respiratoryRate"
                  control={control}
                  rules={{ required: 'Required', min: 8, max: 40 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Respiratory Rate (/min)"
                      error={!!errors.respiratoryRate}
                      helperText={errors.respiratoryRate?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6}>
                <Controller
                  name="oxygenSaturation"
                  control={control}
                  rules={{ required: 'Required', min: 70, max: 100 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="O2 Saturation (%)"
                      error={!!errors.oxygenSaturation}
                      helperText={errors.oxygenSaturation?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6}>
                <Controller
                  name="weight"
                  control={control}
                  rules={{ required: 'Required', min: 0 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Weight (lbs)"
                      error={!!errors.weight}
                      helperText={errors.weight?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6}>
                <Controller
                  name="height"
                  control={control}
                  rules={{ required: 'Required', min: 0 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Height (inches)"
                      error={!!errors.height}
                      helperText={errors.height?.message}
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
              Record Vitals
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default VitalsManagement;