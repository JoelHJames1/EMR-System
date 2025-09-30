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
  Card,
  CardContent,
  MenuItem,
} from '@mui/material';
import {
  Add,
  Edit,
  Delete,
  LocalHospital,
  Warning,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { diagnosisAPI, patientAPI, encounterAPI, providerAPI } from '../../services/api';

const DiagnosisManagement = () => {
  const [diagnoses, setDiagnoses] = useState([]);
  const [patients, setPatients] = useState([]);
  const [encounters, setEncounters] = useState([]);
  const [providers, setProviders] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingDiagnosis, setEditingDiagnosis] = useState(null);

  const { control, handleSubmit, reset, setValue, formState: { errors } } = useForm();

  useEffect(() => {
    fetchData();
  }, []);

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

  const fetchDiagnosesByPatient = async (patientId) => {
    try {
      const [diagnosesRes, encountersRes] = await Promise.all([
        diagnosisAPI.getByPatient(patientId),
        encounterAPI.getByPatient(patientId),
      ]);
      setDiagnoses(diagnosesRes.data);
      setEncounters(encountersRes.data);
    } catch (err) {
      setError('Failed to load diagnoses');
    }
  };

  const onSubmit = async (data) => {
    try {
      const diagnosisData = {
        ...data,
        patientId: selectedPatient.id,
        diagnosisDate: new Date().toISOString(),
        isActive: true,
      };

      if (editingDiagnosis) {
        await diagnosisAPI.update(editingDiagnosis.id, diagnosisData);
      } else {
        await diagnosisAPI.create(diagnosisData);
      }

      fetchDiagnosesByPatient(selectedPatient.id);
      handleCloseDialog();
    } catch (err) {
      setError(`Failed to ${editingDiagnosis ? 'update' : 'create'} diagnosis`);
    }
  };

  const handleEdit = (diagnosis) => {
    setEditingDiagnosis(diagnosis);
    setValue('icdCode', diagnosis.icdCode);
    setValue('description', diagnosis.description);
    setValue('diagnosisType', diagnosis.diagnosisType);
    setValue('severity', diagnosis.severity);
    setValue('status', diagnosis.status);
    setValue('encounterId', diagnosis.encounterId);
    setValue('providerId', diagnosis.providerId);
    setValue('notes', diagnosis.notes);
    setOpenDialog(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this diagnosis?')) {
      try {
        await diagnosisAPI.delete(id);
        fetchDiagnosesByPatient(selectedPatient.id);
      } catch (err) {
        setError('Failed to delete diagnosis');
      }
    }
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingDiagnosis(null);
    reset();
  };

  const getSeverityColor = (severity) => {
    const colors = {
      Mild: 'success',
      Moderate: 'warning',
      Severe: 'error',
      Critical: 'error',
    };
    return colors[severity] || 'default';
  };

  const getStatusColor = (status) => {
    const colors = {
      Active: 'error',
      Resolved: 'success',
      Chronic: 'warning',
      Recurrent: 'info',
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
              Diagnosis Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage patient diagnoses with ICD-10/11 codes
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
            Add Diagnosis
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
                  fetchDiagnosesByPatient(patient.id);
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

        {/* Diagnoses Table */}
        <Grid item xs={12} md={8}>
          {selectedPatient ? (
            <Paper elevation={3} sx={{ p: 3 }}>
              <Box display="flex" alignItems="center" mb={3}>
                <LocalHospital sx={{ fontSize: 40, color: '#667eea', mr: 2 }} />
                <Box>
                  <Typography variant="h6" fontWeight="bold">
                    Diagnoses for {selectedPatient.firstName} {selectedPatient.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    ICD-10/11 coded diagnoses
                  </Typography>
                </Box>
              </Box>

              <TableContainer>
                <Table>
                  <TableHead sx={{ bgcolor: '#f5f5f5' }}>
                    <TableRow>
                      <TableCell>ICD Code</TableCell>
                      <TableCell>Description</TableCell>
                      <TableCell>Type</TableCell>
                      <TableCell>Severity</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Date</TableCell>
                      <TableCell align="right">Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {diagnoses.length > 0 ? (
                      diagnoses.map((diagnosis) => (
                        <TableRow key={diagnosis.id} hover>
                          <TableCell>
                            <Typography variant="body2" fontWeight="bold" color="primary">
                              {diagnosis.icdCode}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2">
                              {diagnosis.description}
                            </Typography>
                            {diagnosis.notes && (
                              <Typography variant="caption" color="text.secondary" display="block">
                                {diagnosis.notes}
                              </Typography>
                            )}
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={diagnosis.diagnosisType || 'Primary'}
                              size="small"
                              variant="outlined"
                            />
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={diagnosis.severity || 'N/A'}
                              size="small"
                              color={getSeverityColor(diagnosis.severity)}
                            />
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={diagnosis.status || 'Active'}
                              size="small"
                              color={getStatusColor(diagnosis.status)}
                            />
                          </TableCell>
                          <TableCell>
                            {new Date(diagnosis.diagnosisDate).toLocaleDateString()}
                          </TableCell>
                          <TableCell align="right">
                            <Tooltip title="Edit">
                              <IconButton
                                onClick={() => handleEdit(diagnosis)}
                                color="primary"
                                size="small"
                              >
                                <Edit />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title="Delete">
                              <IconButton
                                onClick={() => handleDelete(diagnosis.id)}
                                color="error"
                                size="small"
                              >
                                <Delete />
                              </IconButton>
                            </Tooltip>
                          </TableCell>
                        </TableRow>
                      ))
                    ) : (
                      <TableRow>
                        <TableCell colSpan={7} align="center">
                          <Typography color="text.secondary">
                            No diagnoses found
                          </Typography>
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            </Paper>
          ) : (
            <Paper elevation={3} sx={{ p: 5, textAlign: 'center', minHeight: 400 }}>
              <Warning sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select a patient to view diagnoses
              </Typography>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Add/Edit Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingDiagnosis ? 'Edit Diagnosis' : 'Add New Diagnosis'}
        </DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={4}>
                <Controller
                  name="icdCode"
                  control={control}
                  rules={{ required: 'ICD code is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="ICD-10/11 Code"
                      placeholder="e.g., E11.9"
                      error={!!errors.icdCode}
                      helperText={errors.icdCode?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <Controller
                  name="diagnosisType"
                  control={control}
                  defaultValue="Primary"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Diagnosis Type"
                    >
                      <MenuItem value="Primary">Primary</MenuItem>
                      <MenuItem value="Secondary">Secondary</MenuItem>
                      <MenuItem value="Differential">Differential</MenuItem>
                      <MenuItem value="Provisional">Provisional</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <Controller
                  name="severity"
                  control={control}
                  defaultValue="Moderate"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Severity"
                    >
                      <MenuItem value="Mild">Mild</MenuItem>
                      <MenuItem value="Moderate">Moderate</MenuItem>
                      <MenuItem value="Severe">Severe</MenuItem>
                      <MenuItem value="Critical">Critical</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name="description"
                  control={control}
                  rules={{ required: 'Description is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Description"
                      placeholder="e.g., Type 2 diabetes mellitus without complications"
                      error={!!errors.description}
                      helperText={errors.description?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="status"
                  control={control}
                  defaultValue="Active"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Status"
                    >
                      <MenuItem value="Active">Active</MenuItem>
                      <MenuItem value="Resolved">Resolved</MenuItem>
                      <MenuItem value="Chronic">Chronic</MenuItem>
                      <MenuItem value="Recurrent">Recurrent</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="encounterId"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Associated Encounter"
                    >
                      <MenuItem value="">None</MenuItem>
                      {encounters.map((encounter) => (
                        <MenuItem key={encounter.id} value={encounter.id}>
                          {encounter.encounterNumber || `ENC-${encounter.id}`} - {new Date(encounter.startDate).toLocaleDateString()}
                        </MenuItem>
                      ))}
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name="providerId"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Diagnosing Provider"
                    >
                      <MenuItem value="">Select Provider</MenuItem>
                      {providers.map((provider) => (
                        <MenuItem key={provider.id} value={provider.id}>
                          Dr. {provider.firstName} {provider.lastName} - {provider.specialization}
                        </MenuItem>
                      ))}
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name="notes"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={3}
                      label="Clinical Notes"
                      placeholder="Additional clinical information..."
                    />
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
              {editingDiagnosis ? 'Update' : 'Add'} Diagnosis
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default DiagnosisManagement;