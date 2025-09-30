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
  Tabs,
  Tab,
  List,
  ListItem,
  ListItemText,
  Divider,
} from '@mui/material';
import {
  Add,
  LocalHospital,
  Edit,
  CheckCircle,
  Assignment,
  MedicalServices,
  Science,
  Medication,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import {
  encounterAPI,
  patientAPI,
  providerAPI,
  diagnosisAPI,
  clinicalNoteAPI,
  observationAPI,
} from '../../services/api';

const EncounterManagement = () => {
  const [encounters, setEncounters] = useState([]);
  const [patients, setPatients] = useState([]);
  const [providers, setProviders] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [selectedEncounter, setSelectedEncounter] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [viewDialog, setViewDialog] = useState(false);
  const [tabValue, setTabValue] = useState(0);
  const [encounterDetails, setEncounterDetails] = useState(null);

  const { control, handleSubmit, reset, formState: { errors } } = useForm();

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

  const fetchEncountersByPatient = async (patientId) => {
    try {
      const response = await encounterAPI.getByPatient(patientId);
      setEncounters(response.data);
    } catch (err) {
      setError('Failed to load encounters');
    }
  };

  const fetchEncounterDetails = async (encounterId) => {
    try {
      const [encounterRes, notesRes] = await Promise.all([
        encounterAPI.getById(encounterId),
        clinicalNoteAPI.getByEncounter(encounterId),
      ]);
      setEncounterDetails({
        ...encounterRes.data,
        notes: notesRes.data,
      });
    } catch (err) {
      setError('Failed to load encounter details');
    }
  };

  const onSubmit = async (data) => {
    try {
      await encounterAPI.create({
        ...data,
        patientId: selectedPatient.id,
        startDate: new Date().toISOString(),
        status: 'InProgress',
      });
      fetchEncountersByPatient(selectedPatient.id);
      setOpenDialog(false);
      reset();
    } catch (err) {
      setError('Failed to create encounter');
    }
  };

  const handleCompleteEncounter = async (id) => {
    try {
      await encounterAPI.complete(id);
      if (selectedPatient) {
        fetchEncountersByPatient(selectedPatient.id);
      }
    } catch (err) {
      setError('Failed to complete encounter');
    }
  };

  const handleViewEncounter = async (encounter) => {
    setSelectedEncounter(encounter);
    await fetchEncounterDetails(encounter.id);
    setViewDialog(true);
  };

  const getStatusColor = (status) => {
    const colors = {
      Planned: 'info',
      InProgress: 'warning',
      Finished: 'success',
      Cancelled: 'error',
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
              Clinical Encounters
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage patient visits and clinical encounters
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
            New Encounter
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
                  fetchEncountersByPatient(patient.id);
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

        {/* Encounters Table */}
        <Grid item xs={12} md={8}>
          {selectedPatient ? (
            <Paper elevation={3} sx={{ p: 3 }}>
              <Box display="flex" alignItems="center" mb={3}>
                <LocalHospital sx={{ fontSize: 40, color: '#667eea', mr: 2 }} />
                <Box>
                  <Typography variant="h6" fontWeight="bold">
                    Encounters for {selectedPatient.firstName} {selectedPatient.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Clinical visit history and active encounters
                  </Typography>
                </Box>
              </Box>

              <TableContainer>
                <Table>
                  <TableHead sx={{ bgcolor: '#f5f5f5' }}>
                    <TableRow>
                      <TableCell>Encounter #</TableCell>
                      <TableCell>Type</TableCell>
                      <TableCell>Provider</TableCell>
                      <TableCell>Date</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell align="right">Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {encounters.length > 0 ? (
                      encounters.map((encounter) => (
                        <TableRow key={encounter.id} hover>
                          <TableCell>
                            <Typography variant="body2" fontWeight="bold">
                              {encounter.encounterNumber || `ENC-${encounter.id}`}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={encounter.encounterClass || 'Outpatient'}
                              size="small"
                              variant="outlined"
                            />
                          </TableCell>
                          <TableCell>
                            {encounter.providerName || 'Not Assigned'}
                          </TableCell>
                          <TableCell>
                            {new Date(encounter.startDate).toLocaleString()}
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={encounter.status}
                              size="small"
                              color={getStatusColor(encounter.status)}
                            />
                          </TableCell>
                          <TableCell align="right">
                            <Tooltip title="View Details">
                              <IconButton
                                onClick={() => handleViewEncounter(encounter)}
                                color="info"
                              >
                                <Assignment />
                              </IconButton>
                            </Tooltip>
                            {encounter.status === 'InProgress' && (
                              <Tooltip title="Complete Encounter">
                                <IconButton
                                  onClick={() => handleCompleteEncounter(encounter.id)}
                                  color="success"
                                >
                                  <CheckCircle />
                                </IconButton>
                              </Tooltip>
                            )}
                          </TableCell>
                        </TableRow>
                      ))
                    ) : (
                      <TableRow>
                        <TableCell colSpan={6} align="center">
                          <Typography color="text.secondary">
                            No encounters found
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
              <LocalHospital sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select a patient to view encounters
              </Typography>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Create Encounter Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>New Clinical Encounter</DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Controller
                  name="encounterClass"
                  control={control}
                  rules={{ required: 'Encounter type is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Encounter Type"
                      SelectProps={{ native: true }}
                      error={!!errors.encounterClass}
                      helperText={errors.encounterClass?.message}
                    >
                      <option value="">Select Type</option>
                      <option value="Outpatient">Outpatient Visit</option>
                      <option value="Inpatient">Inpatient Admission</option>
                      <option value="Emergency">Emergency Visit</option>
                      <option value="Virtual">Telehealth/Virtual</option>
                      <option value="Home Health">Home Health Visit</option>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12}>
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
              <Grid item xs={12}>
                <Controller
                  name="reasonForVisit"
                  control={control}
                  rules={{ required: 'Reason is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={3}
                      label="Reason for Visit"
                      placeholder="Chief complaint and reason for visit"
                      error={!!errors.reasonForVisit}
                      helperText={errors.reasonForVisit?.message}
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
              Start Encounter
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* View Encounter Details Dialog */}
      <Dialog
        open={viewDialog}
        onClose={() => setViewDialog(false)}
        maxWidth="lg"
        fullWidth
      >
        <DialogTitle>
          Encounter Details - {selectedEncounter?.encounterNumber || `ENC-${selectedEncounter?.id}`}
        </DialogTitle>
        <DialogContent>
          {encounterDetails && (
            <Box>
              <Tabs value={tabValue} onChange={(e, v) => setTabValue(v)} sx={{ mb: 3 }}>
                <Tab label="Overview" icon={<Assignment />} />
                <Tab label="Clinical Notes" icon={<MedicalServices />} />
                <Tab label="Vitals" icon={<Science />} />
              </Tabs>

              {tabValue === 0 && (
                <Grid container spacing={2}>
                  <Grid item xs={6}>
                    <Typography variant="body2" color="text.secondary">
                      Encounter Type
                    </Typography>
                    <Typography variant="body1" fontWeight="bold">
                      {encounterDetails.encounterClass}
                    </Typography>
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant="body2" color="text.secondary">
                      Status
                    </Typography>
                    <Chip
                      label={encounterDetails.status}
                      color={getStatusColor(encounterDetails.status)}
                    />
                  </Grid>
                  <Grid item xs={12}>
                    <Typography variant="body2" color="text.secondary">
                      Reason for Visit
                    </Typography>
                    <Typography variant="body1" fontWeight="bold">
                      {encounterDetails.reasonForVisit || 'N/A'}
                    </Typography>
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant="body2" color="text.secondary">
                      Start Date
                    </Typography>
                    <Typography variant="body1">
                      {new Date(encounterDetails.startDate).toLocaleString()}
                    </Typography>
                  </Grid>
                  {encounterDetails.endDate && (
                    <Grid item xs={6}>
                      <Typography variant="body2" color="text.secondary">
                        End Date
                      </Typography>
                      <Typography variant="body1">
                        {new Date(encounterDetails.endDate).toLocaleString()}
                      </Typography>
                    </Grid>
                  )}
                </Grid>
              )}

              {tabValue === 1 && (
                <Box>
                  {encounterDetails.notes?.length > 0 ? (
                    encounterDetails.notes.map((note, index) => (
                      <Paper key={index} sx={{ p: 2, mb: 2 }}>
                        <Typography variant="body2" fontWeight="bold" gutterBottom>
                          {note.noteType} - {new Date(note.createdDate).toLocaleString()}
                        </Typography>
                        {note.subjective && (
                          <Box mb={1}>
                            <Typography variant="caption" color="text.secondary">
                              Subjective:
                            </Typography>
                            <Typography variant="body2">{note.subjective}</Typography>
                          </Box>
                        )}
                        {note.objective && (
                          <Box mb={1}>
                            <Typography variant="caption" color="text.secondary">
                              Objective:
                            </Typography>
                            <Typography variant="body2">{note.objective}</Typography>
                          </Box>
                        )}
                        {note.assessment && (
                          <Box mb={1}>
                            <Typography variant="caption" color="text.secondary">
                              Assessment:
                            </Typography>
                            <Typography variant="body2">{note.assessment}</Typography>
                          </Box>
                        )}
                        {note.plan && (
                          <Box>
                            <Typography variant="caption" color="text.secondary">
                              Plan:
                            </Typography>
                            <Typography variant="body2">{note.plan}</Typography>
                          </Box>
                        )}
                        {note.isSigned && (
                          <Chip
                            label="Signed"
                            size="small"
                            color="success"
                            icon={<CheckCircle />}
                            sx={{ mt: 1 }}
                          />
                        )}
                      </Paper>
                    ))
                  ) : (
                    <Typography color="text.secondary">No clinical notes recorded</Typography>
                  )}
                </Box>
              )}

              {tabValue === 2 && (
                <Typography color="text.secondary">Vitals tracking coming soon...</Typography>
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

export default EncounterManagement;