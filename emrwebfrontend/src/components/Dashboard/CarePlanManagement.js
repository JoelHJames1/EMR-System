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
  Tooltip,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  Divider,
  MenuItem,
  Accordion,
  AccordionSummary,
  AccordionDetails,
} from '@mui/material';
import {
  Add,
  Edit,
  Delete,
  ExpandMore,
  CheckCircle,
  Assignment,
  PlayArrow,
  Task,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { carePlanAPI, patientAPI, providerAPI } from '../../services/api';

const CarePlanManagement = () => {
  const [carePlans, setCarePlans] = useState([]);
  const [patients, setPatients] = useState([]);
  const [providers, setProviders] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingPlan, setEditingPlan] = useState(null);

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

  const fetchCarePlansByPatient = async (patientId) => {
    try {
      const response = await carePlanAPI.getByPatient(patientId);
      setCarePlans(response.data);
    } catch (err) {
      setError('Failed to load care plans');
    }
  };

  const onSubmit = async (data) => {
    try {
      const carePlanData = {
        ...data,
        patientId: selectedPatient.id,
        createdDate: new Date().toISOString(),
        status: data.status || 'Active',
      };

      if (editingPlan) {
        await carePlanAPI.update(editingPlan.id, carePlanData);
      } else {
        await carePlanAPI.create(carePlanData);
      }

      fetchCarePlansByPatient(selectedPatient.id);
      handleCloseDialog();
    } catch (err) {
      setError(`Failed to ${editingPlan ? 'update' : 'create'} care plan`);
    }
  };

  const handleEdit = (plan) => {
    setEditingPlan(plan);
    setValue('title', plan.title);
    setValue('description', plan.description);
    setValue('intent', plan.intent);
    setValue('category', plan.category);
    setValue('status', plan.status);
    setValue('period', plan.period);
    setValue('careTeamLead', plan.careTeamLead);
    setValue('goals', plan.goals);
    setValue('activitiesSummary', plan.activitiesSummary);
    setOpenDialog(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this care plan?')) {
      try {
        await carePlanAPI.delete(id);
        fetchCarePlansByPatient(selectedPatient.id);
      } catch (err) {
        setError('Failed to delete care plan');
      }
    }
  };

  const handleActivate = async (id) => {
    try {
      await carePlanAPI.activate(id);
      fetchCarePlansByPatient(selectedPatient.id);
    } catch (err) {
      setError('Failed to activate care plan');
    }
  };

  const handleComplete = async (id) => {
    try {
      await carePlanAPI.complete(id);
      fetchCarePlansByPatient(selectedPatient.id);
    } catch (err) {
      setError('Failed to complete care plan');
    }
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingPlan(null);
    reset();
  };

  const getStatusColor = (status) => {
    const colors = {
      Draft: 'default',
      Active: 'success',
      'On Hold': 'warning',
      Completed: 'info',
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
              Care Plan Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Create and manage patient treatment plans and care coordination
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
            New Care Plan
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
                  fetchCarePlansByPatient(patient.id);
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

        {/* Care Plans */}
        <Grid item xs={12} md={8}>
          {selectedPatient ? (
            <Box>
              <Paper elevation={3} sx={{ p: 3, mb: 2 }}>
                <Box display="flex" alignItems="center" mb={2}>
                  <Assignment sx={{ fontSize: 40, color: '#667eea', mr: 2 }} />
                  <Box>
                    <Typography variant="h6" fontWeight="bold">
                      Care Plans for {selectedPatient.firstName} {selectedPatient.lastName}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      Treatment plans and care coordination
                    </Typography>
                  </Box>
                </Box>
              </Paper>

              {carePlans.length > 0 ? (
                carePlans.map((plan) => (
                  <Accordion key={plan.id} sx={{ mb: 2 }}>
                    <AccordionSummary
                      expandIcon={<ExpandMore />}
                      sx={{
                        background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
                      }}
                    >
                      <Box display="flex" alignItems="center" width="100%" mr={2}>
                        <Box flexGrow={1}>
                          <Typography variant="h6" fontWeight="bold">
                            {plan.title}
                          </Typography>
                          <Typography variant="caption" color="text.secondary">
                            Created: {new Date(plan.createdDate).toLocaleDateString()}
                            {plan.period && ` • Duration: ${plan.period}`}
                          </Typography>
                        </Box>
                        <Chip
                          label={plan.status}
                          color={getStatusColor(plan.status)}
                          size="small"
                          sx={{ mr: 2 }}
                        />
                      </Box>
                    </AccordionSummary>
                    <AccordionDetails>
                      <Grid container spacing={2}>
                        <Grid item xs={12}>
                          <Typography variant="body1" paragraph>
                            {plan.description}
                          </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6}>
                          <Typography variant="caption" color="text.secondary">
                            Category
                          </Typography>
                          <Typography variant="body2" fontWeight="bold">
                            {plan.category || 'General'}
                          </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6}>
                          <Typography variant="caption" color="text.secondary">
                            Intent
                          </Typography>
                          <Typography variant="body2" fontWeight="bold">
                            {plan.intent || 'Plan'}
                          </Typography>
                        </Grid>

                        {plan.careTeamLead && (
                          <Grid item xs={12} sm={6}>
                            <Typography variant="caption" color="text.secondary">
                              Care Team Lead
                            </Typography>
                            <Typography variant="body2" fontWeight="bold">
                              {plan.careTeamLead}
                            </Typography>
                          </Grid>
                        )}

                        {plan.goals && (
                          <Grid item xs={12}>
                            <Typography variant="caption" color="text.secondary">
                              Goals
                            </Typography>
                            <Typography variant="body2">
                              {plan.goals}
                            </Typography>
                          </Grid>
                        )}

                        {plan.activitiesSummary && (
                          <Grid item xs={12}>
                            <Typography variant="caption" color="text.secondary">
                              Activities
                            </Typography>
                            <Typography variant="body2">
                              {plan.activitiesSummary}
                            </Typography>
                          </Grid>
                        )}

                        <Grid item xs={12}>
                          <Divider sx={{ my: 2 }} />
                          <Box display="flex" gap={1} justifyContent="flex-end">
                            {plan.status === 'Draft' && (
                              <Button
                                startIcon={<PlayArrow />}
                                size="small"
                                onClick={() => handleActivate(plan.id)}
                                color="success"
                              >
                                Activate
                              </Button>
                            )}
                            {plan.status === 'Active' && (
                              <Button
                                startIcon={<CheckCircle />}
                                size="small"
                                onClick={() => handleComplete(plan.id)}
                                color="info"
                              >
                                Complete
                              </Button>
                            )}
                            <Button
                              startIcon={<Edit />}
                              size="small"
                              onClick={() => handleEdit(plan)}
                            >
                              Edit
                            </Button>
                            <Button
                              startIcon={<Delete />}
                              size="small"
                              onClick={() => handleDelete(plan.id)}
                              color="error"
                            >
                              Delete
                            </Button>
                          </Box>
                        </Grid>
                      </Grid>
                    </AccordionDetails>
                  </Accordion>
                ))
              ) : (
                <Paper elevation={3} sx={{ p: 5, textAlign: 'center' }}>
                  <Task sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
                  <Typography variant="h6" color="text.secondary">
                    No care plans found
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Create a new care plan to start coordinating care
                  </Typography>
                </Paper>
              )}
            </Box>
          ) : (
            <Paper elevation={3} sx={{ p: 5, textAlign: 'center', minHeight: 400 }}>
              <Assignment sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select a patient to view care plans
              </Typography>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Add/Edit Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingPlan ? 'Edit Care Plan' : 'Create New Care Plan'}
        </DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Controller
                  name="title"
                  control={control}
                  rules={{ required: 'Title is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Care Plan Title"
                      placeholder="e.g., Diabetes Management Plan"
                      error={!!errors.title}
                      helperText={errors.title?.message}
                    />
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
                      multiline
                      rows={3}
                      label="Description"
                      placeholder="Detailed care plan description..."
                      error={!!errors.description}
                      helperText={errors.description?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <Controller
                  name="category"
                  control={control}
                  defaultValue="General"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Category"
                    >
                      <MenuItem value="General">General</MenuItem>
                      <MenuItem value="Chronic Disease">Chronic Disease</MenuItem>
                      <MenuItem value="Acute Care">Acute Care</MenuItem>
                      <MenuItem value="Preventive">Preventive</MenuItem>
                      <MenuItem value="Palliative">Palliative</MenuItem>
                      <MenuItem value="Rehabilitation">Rehabilitation</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <Controller
                  name="intent"
                  control={control}
                  defaultValue="Plan"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Intent"
                    >
                      <MenuItem value="Proposal">Proposal</MenuItem>
                      <MenuItem value="Plan">Plan</MenuItem>
                      <MenuItem value="Order">Order</MenuItem>
                      <MenuItem value="Option">Option</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <Controller
                  name="status"
                  control={control}
                  defaultValue="Draft"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Status"
                    >
                      <MenuItem value="Draft">Draft</MenuItem>
                      <MenuItem value="Active">Active</MenuItem>
                      <MenuItem value="On Hold">On Hold</MenuItem>
                      <MenuItem value="Completed">Completed</MenuItem>
                      <MenuItem value="Cancelled">Cancelled</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="period"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Duration/Period"
                      placeholder="e.g., 3 months, 1 year"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="careTeamLead"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Care Team Lead"
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

              <Grid item xs={12}>
                <Controller
                  name="goals"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={2}
                      label="Goals"
                      placeholder="Treatment goals and expected outcomes..."
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12}>
                <Controller
                  name="activitiesSummary"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={3}
                      label="Activities & Interventions"
                      placeholder="Planned activities, interventions, and action items..."
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
              {editingPlan ? 'Update' : 'Create'} Care Plan
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default CarePlanManagement;