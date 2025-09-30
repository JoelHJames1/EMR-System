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
  Stepper,
  Step,
  StepLabel,
} from '@mui/material';
import {
  Add,
  Science,
  AccessTime,
  CheckCircle,
  PlayArrow,
  Description,
  BiotechOutlined,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { labOrderAPI, patientAPI, providerAPI } from '../../services/api';

const LabOrderManagement = () => {
  const [labOrders, setLabOrders] = useState([]);
  const [patients, setPatients] = useState([]);
  const [providers, setProviders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [openResultDialog, setOpenResultDialog] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [selectedPatient, setSelectedPatient] = useState(null);

  const { control, handleSubmit, reset, formState: { errors } } = useForm();
  const resultForm = useForm();

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

  const fetchLabOrdersByPatient = async (patientId) => {
    try {
      const response = await labOrderAPI.getByPatient(patientId);
      setLabOrders(response.data);
    } catch (err) {
      setError('Failed to load lab orders');
    }
  };

  const onSubmit = async (data) => {
    try {
      await labOrderAPI.create({
        ...data,
        patientId: selectedPatient.id,
        orderDate: new Date().toISOString(),
        status: 'Ordered',
      });
      fetchLabOrdersByPatient(selectedPatient.id);
      setOpenDialog(false);
      reset();
    } catch (err) {
      setError('Failed to create lab order');
    }
  };

  const handleStatusUpdate = async (orderId, newStatus) => {
    try {
      await labOrderAPI.updateStatus(orderId, newStatus);
      if (selectedPatient) {
        fetchLabOrdersByPatient(selectedPatient.id);
      }
    } catch (err) {
      setError('Failed to update status');
    }
  };

  const handleAddResult = async (data) => {
    try {
      await labOrderAPI.addResult(selectedOrder.id, data);
      await handleStatusUpdate(selectedOrder.id, 'Completed');
      setOpenResultDialog(false);
      resultForm.reset();
    } catch (err) {
      setError('Failed to add result');
    }
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case 'Ordered':
        return <AccessTime color="warning" />;
      case 'InProgress':
        return <PlayArrow color="info" />;
      case 'Completed':
        return <CheckCircle color="success" />;
      default:
        return <AccessTime />;
    }
  };

  const getStepStatus = (status) => {
    const steps = ['Ordered', 'InProgress', 'Completed'];
    return steps.indexOf(status);
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
              Lab Order Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Order and track laboratory tests
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
            New Lab Order
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
                  fetchLabOrdersByPatient(patient.id);
                }}
              >
                <CardContent>
                  <Typography variant="body1" fontWeight="bold">
                    {patient.firstName} {patient.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    MRN: {patient.mrn || 'N/A'}
                  </Typography>
                </CardContent>
              </Card>
            ))}
          </Paper>
        </Grid>

        {/* Lab Orders Table */}
        <Grid item xs={12} md={8}>
          {selectedPatient ? (
            <Paper elevation={3} sx={{ p: 3 }}>
              <Box display="flex" alignItems="center" mb={3}>
                <Science sx={{ fontSize: 40, color: '#667eea', mr: 2 }} />
                <Box>
                  <Typography variant="h6" fontWeight="bold">
                    Lab Orders for {selectedPatient.firstName} {selectedPatient.lastName}
                  </Typography>
                </Box>
              </Box>

              <TableContainer>
                <Table>
                  <TableHead sx={{ bgcolor: '#f5f5f5' }}>
                    <TableRow>
                      <TableCell>Test Name</TableCell>
                      <TableCell>Priority</TableCell>
                      <TableCell>Order Date</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell align="right">Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {labOrders.length > 0 ? (
                      labOrders.map((order) => (
                        <TableRow key={order.id} hover>
                          <TableCell>
                            <Box display="flex" alignItems="center">
                              <BiotechOutlined sx={{ color: '#667eea', mr: 1 }} />
                              <Box>
                                <Typography variant="body2" fontWeight="bold">
                                  {order.testName}
                                </Typography>
                                <Typography variant="caption" color="text.secondary">
                                  Code: {order.testCode}
                                </Typography>
                              </Box>
                            </Box>
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={order.priority}
                              size="small"
                              color={
                                order.priority === 'Stat'
                                  ? 'error'
                                  : order.priority === 'Urgent'
                                  ? 'warning'
                                  : 'default'
                              }
                            />
                          </TableCell>
                          <TableCell>
                            {new Date(order.orderDate).toLocaleDateString()}
                          </TableCell>
                          <TableCell>
                            <Box display="flex" alignItems="center">
                              {getStatusIcon(order.status)}
                              <Typography variant="body2" ml={1}>
                                {order.status}
                              </Typography>
                            </Box>
                          </TableCell>
                          <TableCell align="right">
                            {order.status !== 'Completed' && (
                              <>
                                <Tooltip title="Start Processing">
                                  <IconButton
                                    onClick={() =>
                                      handleStatusUpdate(order.id, 'InProgress')
                                    }
                                    color="primary"
                                    disabled={order.status === 'InProgress'}
                                  >
                                    <PlayArrow />
                                  </IconButton>
                                </Tooltip>
                                <Tooltip title="Add Result">
                                  <IconButton
                                    onClick={() => {
                                      setSelectedOrder(order);
                                      setOpenResultDialog(true);
                                    }}
                                    color="success"
                                  >
                                    <Description />
                                  </IconButton>
                                </Tooltip>
                              </>
                            )}
                          </TableCell>
                        </TableRow>
                      ))
                    ) : (
                      <TableRow>
                        <TableCell colSpan={5} align="center">
                          <Typography color="text.secondary">
                            No lab orders found
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
              <Science sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select a patient to view lab orders
              </Typography>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Create Lab Order Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>New Lab Order</DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Controller
                  name="testName"
                  control={control}
                  rules={{ required: 'Test name is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Test Name"
                      SelectProps={{ native: true }}
                      error={!!errors.testName}
                      helperText={errors.testName?.message}
                    >
                      <option value="">Select Test</option>
                      <option value="Complete Blood Count">Complete Blood Count (CBC)</option>
                      <option value="Basic Metabolic Panel">Basic Metabolic Panel</option>
                      <option value="Comprehensive Metabolic Panel">Comprehensive Metabolic Panel</option>
                      <option value="Lipid Panel">Lipid Panel</option>
                      <option value="Thyroid Function">Thyroid Function Tests</option>
                      <option value="Urinalysis">Urinalysis</option>
                      <option value="HbA1c">Hemoglobin A1c</option>
                      <option value="PT/INR">Prothrombin Time/INR</option>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name="testCode"
                  control={control}
                  rules={{ required: 'Test code is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="LOINC Code"
                      placeholder="e.g., 58410-2"
                      error={!!errors.testCode}
                      helperText={errors.testCode?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="priority"
                  control={control}
                  rules={{ required: 'Priority is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Priority"
                      SelectProps={{ native: true }}
                      error={!!errors.priority}
                      helperText={errors.priority?.message}
                    >
                      <option value="">Select Priority</option>
                      <option value="Routine">Routine</option>
                      <option value="Urgent">Urgent</option>
                      <option value="Stat">STAT</option>
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
                      label="Ordering Provider"
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
                  name="clinicalInfo"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={3}
                      label="Clinical Information"
                      placeholder="Reason for test, symptoms, etc."
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
              Create Order
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* Add Result Dialog */}
      <Dialog
        open={openResultDialog}
        onClose={() => setOpenResultDialog(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Add Lab Result</DialogTitle>
        <form onSubmit={resultForm.handleSubmit(handleAddResult)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Controller
                  name="value"
                  control={resultForm.control}
                  rules={{ required: 'Result value is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Result Value"
                      error={!!resultForm.formState.errors.value}
                      helperText={resultForm.formState.errors.value?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="unit"
                  control={resultForm.control}
                  render={({ field }) => (
                    <TextField {...field} fullWidth label="Unit" placeholder="e.g., mg/dL" />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="referenceRange"
                  control={resultForm.control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Reference Range"
                      placeholder="e.g., 70-100"
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name="flag"
                  control={resultForm.control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Flag"
                      SelectProps={{ native: true }}
                    >
                      <option value="">Select Flag</option>
                      <option value="Normal">Normal</option>
                      <option value="Low">Low</option>
                      <option value="High">High</option>
                      <option value="Critical">Critical</option>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name="notes"
                  control={resultForm.control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={3}
                      label="Notes"
                      placeholder="Additional comments or observations"
                    />
                  )}
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpenResultDialog(false)}>Cancel</Button>
            <Button
              type="submit"
              variant="contained"
              sx={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
              }}
            >
              Save Result
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default LabOrderManagement;