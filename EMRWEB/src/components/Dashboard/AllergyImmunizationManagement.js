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
  Card,
  CardContent,
  Tabs,
  Tab,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  Tooltip,
} from '@mui/material';
import {
  Add,
  Warning,
  Vaccines,
  Delete,
  Edit,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { allergyAPI, immunizationAPI, patientAPI } from '../../services/api';

const AllergyImmunizationManagement = () => {
  const [patients, setPatients] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [allergies, setAllergies] = useState([]);
  const [immunizations, setImmunizations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [tabValue, setTabValue] = useState(0);
  const [openDialog, setOpenDialog] = useState(false);
  const [dialogType, setDialogType] = useState('');

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

  const fetchAllergiesAndImmunizations = async (patientId) => {
    try {
      const [allergiesRes, immunizationsRes] = await Promise.all([
        allergyAPI.getByPatient(patientId),
        immunizationAPI.getByPatient(patientId),
      ]);
      setAllergies(allergiesRes.data);
      setImmunizations(immunizationsRes.data);
    } catch (err) {
      setError('Failed to load data');
    }
  };

  const onSubmit = async (data) => {
    try {
      if (dialogType === 'allergy') {
        await allergyAPI.create({
          ...data,
          patientId: selectedPatient.id,
          identifiedDate: new Date().toISOString(),
          isActive: true,
        });
        await fetchAllergiesAndImmunizations(selectedPatient.id);
      } else {
        await immunizationAPI.create({
          ...data,
          patientId: selectedPatient.id,
          administeredDate: new Date().toISOString(),
        });
        await fetchAllergiesAndImmunizations(selectedPatient.id);
      }
      setOpenDialog(false);
      reset();
    } catch (err) {
      setError(`Failed to add ${dialogType}`);
    }
  };

  const handleDeleteAllergy = async (id) => {
    if (window.confirm('Remove this allergy?')) {
      try {
        await allergyAPI.deactivate(id);
        await fetchAllergiesAndImmunizations(selectedPatient.id);
      } catch (err) {
        setError('Failed to remove allergy');
      }
    }
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
              Allergies & Immunizations
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage patient allergies and vaccination records
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => {
              setDialogType(tabValue === 0 ? 'allergy' : 'immunization');
              setOpenDialog(true);
            }}
            disabled={!selectedPatient}
            sx={{
              background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            }}
          >
            Add {tabValue === 0 ? 'Allergy' : 'Immunization'}
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
                  fetchAllergiesAndImmunizations(patient.id);
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
            <Paper elevation={3} sx={{ p: 3 }}>
              <Tabs value={tabValue} onChange={(e, v) => setTabValue(v)} sx={{ mb: 3 }}>
                <Tab label="Allergies" icon={<Warning />} />
                <Tab label="Immunizations" icon={<Vaccines />} />
              </Tabs>

              {tabValue === 0 ? (
                <Box>
                  <Typography variant="h6" fontWeight="bold" gutterBottom>
                    Known Allergies
                  </Typography>
                  {allergies.length > 0 ? (
                    <List>
                      {allergies.map((allergy) => (
                        <Paper
                          key={allergy.id}
                          elevation={2}
                          sx={{
                            mb: 2,
                            p: 2,
                            borderLeft: '5px solid #f44336',
                            background: 'linear-gradient(135deg, #fff 0%, #ffebee 100%)',
                          }}
                        >
                          <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                            <Box flexGrow={1}>
                              <Typography variant="h6" color="error" gutterBottom>
                                {allergy.allergen}
                              </Typography>
                              <Box display="flex" gap={1} mb={1}>
                                <Chip
                                  label={allergy.severity}
                                  size="small"
                                  color={getSeverityColor(allergy.severity)}
                                />
                                <Chip
                                  label={allergy.allergyType || 'Drug'}
                                  size="small"
                                  variant="outlined"
                                />
                              </Box>
                              <Typography variant="body2" color="text.secondary">
                                <strong>Reaction:</strong> {allergy.reaction}
                              </Typography>
                              {allergy.notes && (
                                <Typography variant="body2" color="text.secondary" mt={1}>
                                  <strong>Notes:</strong> {allergy.notes}
                                </Typography>
                              )}
                              <Typography variant="caption" color="text.secondary" display="block" mt={1}>
                                Identified: {new Date(allergy.identifiedDate).toLocaleDateString()}
                              </Typography>
                            </Box>
                            <Tooltip title="Remove Allergy">
                              <IconButton
                                onClick={() => handleDeleteAllergy(allergy.id)}
                                color="error"
                                size="small"
                              >
                                <Delete />
                              </IconButton>
                            </Tooltip>
                          </Box>
                        </Paper>
                      ))}
                    </List>
                  ) : (
                    <Alert severity="success" icon={<Warning />}>
                      No known allergies recorded for this patient
                    </Alert>
                  )}
                </Box>
              ) : (
                <Box>
                  <Typography variant="h6" fontWeight="bold" gutterBottom>
                    Immunization History
                  </Typography>
                  {immunizations.length > 0 ? (
                    <List>
                      {immunizations.map((immunization) => (
                        <Paper
                          key={immunization.id}
                          elevation={2}
                          sx={{
                            mb: 2,
                            p: 2,
                            borderLeft: '5px solid #4caf50',
                          }}
                        >
                          <Typography variant="h6" color="primary" gutterBottom>
                            {immunization.vaccineName}
                          </Typography>
                          <Grid container spacing={2}>
                            <Grid item xs={6}>
                              <Typography variant="body2" color="text.secondary">
                                <strong>Date:</strong>{' '}
                                {new Date(immunization.administeredDate).toLocaleDateString()}
                              </Typography>
                            </Grid>
                            <Grid item xs={6}>
                              <Typography variant="body2" color="text.secondary">
                                <strong>Dose:</strong> {immunization.doseNumber || 1}
                              </Typography>
                            </Grid>
                            {immunization.cvxCode && (
                              <Grid item xs={6}>
                                <Typography variant="body2" color="text.secondary">
                                  <strong>CVX Code:</strong> {immunization.cvxCode}
                                </Typography>
                              </Grid>
                            )}
                            {immunization.lotNumber && (
                              <Grid item xs={6}>
                                <Typography variant="body2" color="text.secondary">
                                  <strong>Lot #:</strong> {immunization.lotNumber}
                                </Typography>
                              </Grid>
                            )}
                            {immunization.administeredBy && (
                              <Grid item xs={12}>
                                <Typography variant="body2" color="text.secondary">
                                  <strong>Administered by:</strong> {immunization.administeredBy}
                                </Typography>
                              </Grid>
                            )}
                          </Grid>
                        </Paper>
                      ))}
                    </List>
                  ) : (
                    <Alert severity="info">
                      No immunization records found for this patient
                    </Alert>
                  )}
                </Box>
              )}
            </Paper>
          ) : (
            <Paper elevation={3} sx={{ p: 5, textAlign: 'center', minHeight: 400 }}>
              <Warning sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select a patient to view allergies and immunizations
              </Typography>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Add Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>
          Add {dialogType === 'allergy' ? 'Allergy' : 'Immunization'}
        </DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            {dialogType === 'allergy' ? (
              <Grid container spacing={2}>
                <Grid item xs={12}>
                  <Controller
                    name="allergen"
                    control={control}
                    rules={{ required: 'Allergen is required' }}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        label="Allergen"
                        placeholder="e.g., Penicillin, Peanuts"
                        error={!!errors.allergen}
                        helperText={errors.allergen?.message}
                      />
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name="allergyType"
                    control={control}
                    rules={{ required: 'Type is required' }}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        select
                        label="Allergy Type"
                        SelectProps={{ native: true }}
                        error={!!errors.allergyType}
                        helperText={errors.allergyType?.message}
                      >
                        <option value="">Select Type</option>
                        <option value="Drug">Drug/Medication</option>
                        <option value="Food">Food</option>
                        <option value="Environmental">Environmental</option>
                        <option value="Other">Other</option>
                      </TextField>
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name="severity"
                    control={control}
                    rules={{ required: 'Severity is required' }}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        select
                        label="Severity"
                        SelectProps={{ native: true }}
                        error={!!errors.severity}
                        helperText={errors.severity?.message}
                      >
                        <option value="">Select Severity</option>
                        <option value="Mild">Mild</option>
                        <option value="Moderate">Moderate</option>
                        <option value="Severe">Severe</option>
                        <option value="Critical">Critical</option>
                      </TextField>
                    )}
                  />
                </Grid>
                <Grid item xs={12}>
                  <Controller
                    name="reaction"
                    control={control}
                    rules={{ required: 'Reaction is required' }}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        label="Reaction"
                        placeholder="e.g., Hives, Anaphylaxis"
                        error={!!errors.reaction}
                        helperText={errors.reaction?.message}
                      />
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
                        label="Additional Notes"
                      />
                    )}
                  />
                </Grid>
              </Grid>
            ) : (
              <Grid container spacing={2}>
                <Grid item xs={12}>
                  <Controller
                    name="vaccineName"
                    control={control}
                    rules={{ required: 'Vaccine name is required' }}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        label="Vaccine Name"
                        placeholder="e.g., COVID-19, Influenza"
                        error={!!errors.vaccineName}
                        helperText={errors.vaccineName?.message}
                      />
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name="cvxCode"
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} fullWidth label="CVX Code" />
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name="doseNumber"
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} fullWidth type="number" label="Dose Number" />
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name="lotNumber"
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} fullWidth label="Lot Number" />
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name="manufacturer"
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} fullWidth label="Manufacturer" />
                    )}
                  />
                </Grid>
                <Grid item xs={12}>
                  <Controller
                    name="administeredBy"
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} fullWidth label="Administered By" />
                    )}
                  />
                </Grid>
              </Grid>
            )}
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
              Add {dialogType === 'allergy' ? 'Allergy' : 'Immunization'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default AllergyImmunizationManagement;