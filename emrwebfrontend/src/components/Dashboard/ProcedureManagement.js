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
  MedicalServices,
  CheckCircle,
  Schedule,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { procedureAPI, patientAPI, encounterAPI, providerAPI } from '../../services/api';

const ProcedureManagement = () => {
  const [procedures, setProcedures] = useState([]);
  const [patients, setPatients] = useState([]);
  const [encounters, setEncounters] = useState([]);
  const [providers, setProviders] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingProcedure, setEditingProcedure] = useState(null);

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

  const fetchProceduresByPatient = async (patientId) => {
    try {
      const [proceduresRes, encountersRes] = await Promise.all([
        procedureAPI.getByPatient(patientId),
        encounterAPI.getByPatient(patientId),
      ]);
      setProcedures(proceduresRes.data);
      setEncounters(encountersRes.data);
    } catch (err) {
      setError('Failed to load procedures');
    }
  };

  const onSubmit = async (data) => {
    try {
      const procedureData = {
        ...data,
        patientId: selectedPatient.id,
      };

      if (editingProcedure) {
        await procedureAPI.update(editingProcedure.id, procedureData);
      } else {
        await procedureAPI.create(procedureData);
      }

      fetchProceduresByPatient(selectedPatient.id);
      handleCloseDialog();
    } catch (err) {
      setError(`Failed to ${editingProcedure ? 'update' : 'create'} procedure`);
    }
  };

  const handleEdit = (procedure) => {
    setEditingProcedure(procedure);
    setValue('procedureName', procedure.procedureName);
    setValue('cptCode', procedure.cptCode);
    setValue('snomedCode', procedure.snomedCode);
    setValue('procedureDate', procedure.procedureDate?.split('T')[0]);
    setValue('status', procedure.status);
    setValue('category', procedure.category);
    setValue('urgency', procedure.urgency);
    setValue('encounterId', procedure.encounterId);
    setValue('performedBy', procedure.performedBy);
    setValue('location', procedure.location);
    setValue('duration', procedure.duration);
    setValue('notes', procedure.notes);
    setValue('outcome', procedure.outcome);
    setOpenDialog(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this procedure?')) {
      try {
        await procedureAPI.delete(id);
        fetchProceduresByPatient(selectedPatient.id);
      } catch (err) {
        setError('Failed to delete procedure');
      }
    }
  };

  const handleComplete = async (id) => {
    try {
      await procedureAPI.complete(id);
      fetchProceduresByPatient(selectedPatient.id);
    } catch (err) {
      setError('Failed to complete procedure');
    }
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingProcedure(null);
    reset();
  };

  const getStatusColor = (status) => {
    const colors = {
      Scheduled: 'info',
      'In Progress': 'warning',
      Completed: 'success',
      Cancelled: 'error',
    };
    return colors[status] || 'default';
  };

  const getUrgencyColor = (urgency) => {
    const colors = {
      Routine: 'success',
      Urgent: 'warning',
      Emergency: 'error',
    };
    return colors[urgency] || 'default';
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
              Procedure Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Schedule and track surgical/diagnostic procedures with CPT codes
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
            Schedule Procedure
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
                  fetchProceduresByPatient(patient.id);
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

        {/* Procedures Table */}
        <Grid item xs={12} md={8}>
          {selectedPatient ? (
            <Paper elevation={3} sx={{ p: 3 }}>
              <Box display="flex" alignItems="center" mb={3}>
                <MedicalServices sx={{ fontSize: 40, color: '#667eea', mr: 2 }} />
                <Box>
                  <Typography variant="h6" fontWeight="bold">
                    Procedures for {selectedPatient.firstName} {selectedPatient.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Surgical and diagnostic procedures
                  </Typography>
                </Box>
              </Box>

              <TableContainer>
                <Table>
                  <TableHead sx={{ bgcolor: '#f5f5f5' }}>
                    <TableRow>
                      <TableCell>CPT Code</TableCell>
                      <TableCell>Procedure</TableCell>
                      <TableCell>Date</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Urgency</TableCell>
                      <TableCell>Provider</TableCell>
                      <TableCell align="right">Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {procedures.length > 0 ? (
                      procedures.map((procedure) => (
                        <TableRow key={procedure.id} hover>
                          <TableCell>
                            <Typography variant="body2" fontWeight="bold" color="primary">
                              {procedure.cptCode || 'N/A'}
                            </Typography>
                            {procedure.snomedCode && (
                              <Typography variant="caption" color="text.secondary">
                                SNOMED: {procedure.snomedCode}
                              </Typography>
                            )}
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2" fontWeight="bold">
                              {procedure.procedureName}
                            </Typography>
                            {procedure.category && (
                              <Chip
                                label={procedure.category}
                                size="small"
                                variant="outlined"
                                sx={{ mt: 0.5 }}
                              />
                            )}
                          </TableCell>
                          <TableCell>
                            {new Date(procedure.procedureDate).toLocaleString()}
                            {procedure.duration && (
                              <Typography variant="caption" display="block" color="text.secondary">
                                {procedure.duration} min
                              </Typography>
                            )}
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={procedure.status}
                              size="small"
                              color={getStatusColor(procedure.status)}
                            />
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={procedure.urgency || 'Routine'}
                              size="small"
                              color={getUrgencyColor(procedure.urgency)}
                            />
                          </TableCell>
                          <TableCell>
                            <Typography variant="caption">
                              {procedure.performedBy || 'Not assigned'}
                            </Typography>
                          </TableCell>
                          <TableCell align="right">
                            {procedure.status !== 'Completed' && (
                              <Tooltip title="Mark Complete">
                                <IconButton
                                  onClick={() => handleComplete(procedure.id)}
                                  color="success"
                                  size="small"
                                >
                                  <CheckCircle />
                                </IconButton>
                              </Tooltip>
                            )}
                            <Tooltip title="Edit">
                              <IconButton
                                onClick={() => handleEdit(procedure)}
                                color="primary"
                                size="small"
                              >
                                <Edit />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title="Delete">
                              <IconButton
                                onClick={() => handleDelete(procedure.id)}
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
                            No procedures found
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
              <Schedule sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select a patient to view procedures
              </Typography>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Add/Edit Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingProcedure ? 'Edit Procedure' : 'Schedule New Procedure'}
        </DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="procedureName"
                  control={control}
                  rules={{ required: 'Procedure name is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Procedure Name"
                      placeholder="e.g., Appendectomy"
                      error={!!errors.procedureName}
                      helperText={errors.procedureName?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={3}>
                <Controller
                  name="cptCode"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="CPT Code"
                      placeholder="e.g., 44950"
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={3}>
                <Controller
                  name="snomedCode"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="SNOMED Code"
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="category"
                  control={control}
                  defaultValue="Surgical"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Category"
                    >
                      <MenuItem value="Surgical">Surgical</MenuItem>
                      <MenuItem value="Diagnostic">Diagnostic</MenuItem>
                      <MenuItem value="Therapeutic">Therapeutic</MenuItem>
                      <MenuItem value="Preventive">Preventive</MenuItem>
                      <MenuItem value="Rehabilitative">Rehabilitative</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="urgency"
                  control={control}
                  defaultValue="Routine"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Urgency"
                    >
                      <MenuItem value="Routine">Routine</MenuItem>
                      <MenuItem value="Urgent">Urgent</MenuItem>
                      <MenuItem value="Emergency">Emergency</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="procedureDate"
                  control={control}
                  rules={{ required: 'Date is required' }}
                  defaultValue={new Date().toISOString().split('T')[0]}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="datetime-local"
                      label="Procedure Date & Time"
                      InputLabelProps={{ shrink: true }}
                      error={!!errors.procedureDate}
                      helperText={errors.procedureDate?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="status"
                  control={control}
                  defaultValue="Scheduled"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Status"
                    >
                      <MenuItem value="Scheduled">Scheduled</MenuItem>
                      <MenuItem value="In Progress">In Progress</MenuItem>
                      <MenuItem value="Completed">Completed</MenuItem>
                      <MenuItem value="Cancelled">Cancelled</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="performedBy"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Performing Provider"
                    >
                      <MenuItem value="">Select Provider</MenuItem>
                      {providers.map((provider) => (
                        <MenuItem key={provider.id} value={`Dr. ${provider.firstName} ${provider.lastName}`}>
                          Dr. {provider.firstName} {provider.lastName} - {provider.specialization}
                        </MenuItem>
                      ))}
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={3}>
                <Controller
                  name="duration"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Duration (min)"
                      placeholder="60"
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={3}>
                <Controller
                  name="location"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Location"
                      placeholder="OR-1"
                    />
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
                  name="notes"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={2}
                      label="Pre-Procedure Notes"
                      placeholder="Preparation instructions, special requirements..."
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name="outcome"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={2}
                      label="Outcome/Results"
                      placeholder="Post-procedure notes, complications, findings..."
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
              {editingProcedure ? 'Update' : 'Schedule'} Procedure
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default ProcedureManagement;