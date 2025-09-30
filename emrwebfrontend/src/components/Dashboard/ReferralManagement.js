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
  Send,
  CheckCircle,
  Cancel,
  PersonSearch,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { referralAPI, patientAPI, providerAPI } from '../../services/api';

const ReferralManagement = () => {
  const [referrals, setReferrals] = useState([]);
  const [patients, setPatients] = useState([]);
  const [providers, setProviders] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingReferral, setEditingReferral] = useState(null);

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

  const fetchReferralsByPatient = async (patientId) => {
    try {
      const response = await referralAPI.getByPatient(patientId);
      setReferrals(response.data);
    } catch (err) {
      setError('Failed to load referrals');
    }
  };

  const onSubmit = async (data) => {
    try {
      const referralData = {
        ...data,
        patientId: selectedPatient.id,
        referralDate: new Date().toISOString(),
        status: data.status || 'Pending',
      };

      if (editingReferral) {
        await referralAPI.update(editingReferral.id, referralData);
      } else {
        await referralAPI.create(referralData);
      }

      fetchReferralsByPatient(selectedPatient.id);
      handleCloseDialog();
    } catch (err) {
      setError(`Failed to ${editingReferral ? 'update' : 'create'} referral`);
    }
  };

  const handleEdit = (referral) => {
    setEditingReferral(referral);
    setValue('referringProviderId', referral.referringProviderId);
    setValue('referredToProviderId', referral.referredToProviderId);
    setValue('specialty', referral.specialty);
    setValue('reason', referral.reason);
    setValue('priority', referral.priority);
    setValue('status', referral.status);
    setValue('notes', referral.notes);
    setOpenDialog(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this referral?')) {
      try {
        await referralAPI.delete(id);
        fetchReferralsByPatient(selectedPatient.id);
      } catch (err) {
        setError('Failed to delete referral');
      }
    }
  };

  const handleApprove = async (id) => {
    try {
      await referralAPI.approve(id);
      fetchReferralsByPatient(selectedPatient.id);
    } catch (err) {
      setError('Failed to approve referral');
    }
  };

  const handleComplete = async (id) => {
    try {
      await referralAPI.complete(id);
      fetchReferralsByPatient(selectedPatient.id);
    } catch (err) {
      setError('Failed to complete referral');
    }
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingReferral(null);
    reset();
  };

  const getStatusColor = (status) => {
    const colors = {
      Pending: 'warning',
      Approved: 'info',
      Scheduled: 'success',
      Completed: 'default',
      Cancelled: 'error',
    };
    return colors[status] || 'default';
  };

  const getPriorityColor = (priority) => {
    const colors = {
      Routine: 'success',
      Urgent: 'warning',
      Emergency: 'error',
    };
    return colors[priority] || 'default';
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
              Referral Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage patient referrals to specialists and consultants
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
            New Referral
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
                  fetchReferralsByPatient(patient.id);
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

        {/* Referrals Table */}
        <Grid item xs={12} md={8}>
          {selectedPatient ? (
            <Paper elevation={3} sx={{ p: 3 }}>
              <Box display="flex" alignItems="center" mb={3}>
                <PersonSearch sx={{ fontSize: 40, color: '#667eea', mr: 2 }} />
                <Box>
                  <Typography variant="h6" fontWeight="bold">
                    Referrals for {selectedPatient.firstName} {selectedPatient.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Specialist referrals and consultations
                  </Typography>
                </Box>
              </Box>

              <TableContainer>
                <Table>
                  <TableHead sx={{ bgcolor: '#f5f5f5' }}>
                    <TableRow>
                      <TableCell>Date</TableCell>
                      <TableCell>Specialty</TableCell>
                      <TableCell>Referred To</TableCell>
                      <TableCell>Reason</TableCell>
                      <TableCell>Priority</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell align="right">Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {referrals.length > 0 ? (
                      referrals.map((referral) => (
                        <TableRow key={referral.id} hover>
                          <TableCell>
                            {new Date(referral.referralDate).toLocaleDateString()}
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2" fontWeight="bold" color="primary">
                              {referral.specialty}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2">
                              {referral.referredToProviderName || 'Not assigned'}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="caption" color="text.secondary">
                              {referral.reason?.substring(0, 50)}
                              {referral.reason?.length > 50 && '...'}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={referral.priority || 'Routine'}
                              size="small"
                              color={getPriorityColor(referral.priority)}
                            />
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={referral.status}
                              size="small"
                              color={getStatusColor(referral.status)}
                            />
                          </TableCell>
                          <TableCell align="right">
                            {referral.status === 'Pending' && (
                              <Tooltip title="Approve">
                                <IconButton
                                  onClick={() => handleApprove(referral.id)}
                                  color="success"
                                  size="small"
                                >
                                  <CheckCircle />
                                </IconButton>
                              </Tooltip>
                            )}
                            {(referral.status === 'Approved' || referral.status === 'Scheduled') && (
                              <Tooltip title="Complete">
                                <IconButton
                                  onClick={() => handleComplete(referral.id)}
                                  color="info"
                                  size="small"
                                >
                                  <CheckCircle />
                                </IconButton>
                              </Tooltip>
                            )}
                            <Tooltip title="Edit">
                              <IconButton
                                onClick={() => handleEdit(referral)}
                                color="primary"
                                size="small"
                              >
                                <Edit />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title="Delete">
                              <IconButton
                                onClick={() => handleDelete(referral.id)}
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
                            No referrals found
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
              <Send sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select a patient to view referrals
              </Typography>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Add/Edit Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingReferral ? 'Edit Referral' : 'Create New Referral'}
        </DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="referringProviderId"
                  control={control}
                  rules={{ required: 'Referring provider is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Referring Provider"
                      error={!!errors.referringProviderId}
                      helperText={errors.referringProviderId?.message}
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

              <Grid item xs={12} sm={6}>
                <Controller
                  name="specialty"
                  control={control}
                  rules={{ required: 'Specialty is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Specialty Required"
                      error={!!errors.specialty}
                      helperText={errors.specialty?.message}
                    >
                      <MenuItem value="Cardiology">Cardiology</MenuItem>
                      <MenuItem value="Neurology">Neurology</MenuItem>
                      <MenuItem value="Orthopedics">Orthopedics</MenuItem>
                      <MenuItem value="Dermatology">Dermatology</MenuItem>
                      <MenuItem value="Ophthalmology">Ophthalmology</MenuItem>
                      <MenuItem value="Psychiatry">Psychiatry</MenuItem>
                      <MenuItem value="Surgery">Surgery</MenuItem>
                      <MenuItem value="Oncology">Oncology</MenuItem>
                      <MenuItem value="Endocrinology">Endocrinology</MenuItem>
                      <MenuItem value="Gastroenterology">Gastroenterology</MenuItem>
                      <MenuItem value="Pulmonology">Pulmonology</MenuItem>
                      <MenuItem value="Rheumatology">Rheumatology</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12}>
                <Controller
                  name="referredToProviderId"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Referred To (Specialist)"
                    >
                      <MenuItem value="">Not Assigned Yet</MenuItem>
                      {providers
                        .filter((p) => p.specialization === control._formValues.specialty)
                        .map((provider) => (
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
                  name="reason"
                  control={control}
                  rules={{ required: 'Reason is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={3}
                      label="Reason for Referral"
                      placeholder="Clinical indication and reason for consultation..."
                      error={!!errors.reason}
                      helperText={errors.reason?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <Controller
                  name="priority"
                  control={control}
                  defaultValue="Routine"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Priority"
                    >
                      <MenuItem value="Routine">Routine</MenuItem>
                      <MenuItem value="Urgent">Urgent</MenuItem>
                      <MenuItem value="Emergency">Emergency</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <Controller
                  name="status"
                  control={control}
                  defaultValue="Pending"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Status"
                    >
                      <MenuItem value="Pending">Pending</MenuItem>
                      <MenuItem value="Approved">Approved</MenuItem>
                      <MenuItem value="Scheduled">Scheduled</MenuItem>
                      <MenuItem value="Completed">Completed</MenuItem>
                      <MenuItem value="Cancelled">Cancelled</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <Controller
                  name="expirationDate"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="date"
                      label="Expiration Date"
                      InputLabelProps={{ shrink: true }}
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
                      placeholder="Additional clinical information, test results, etc..."
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
              {editingReferral ? 'Update' : 'Create'} Referral
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default ReferralManagement;