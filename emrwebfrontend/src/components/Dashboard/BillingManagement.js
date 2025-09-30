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
} from '@mui/material';
import {
  Add,
  AttachMoney,
  Payment,
  Receipt,
  Print,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { billingAPI, patientAPI, insuranceAPI } from '../../services/api';

const BillingManagement = () => {
  const [billings, setBillings] = useState([]);
  const [patients, setPatients] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [paymentDialog, setPaymentDialog] = useState(false);
  const [selectedBilling, setSelectedBilling] = useState(null);

  const { control, handleSubmit, reset, formState: { errors } } = useForm();
  const paymentForm = useForm();

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

  const fetchBillings = async (patientId) => {
    try {
      const response = await billingAPI.getByPatient(patientId);
      setBillings(response.data);
    } catch (err) {
      setError('Failed to load billings');
    }
  };

  const onSubmit = async (data) => {
    try {
      await billingAPI.create({
        ...data,
        patientId: selectedPatient.id,
        invoiceDate: new Date().toISOString(),
        dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
        status: 'Pending',
        paidAmount: 0,
        balanceAmount: parseFloat(data.totalAmount),
      });
      fetchBillings(selectedPatient.id);
      setOpenDialog(false);
      reset();
    } catch (err) {
      setError('Failed to create billing');
    }
  };

  const handlePayment = async (data) => {
    try {
      await billingAPI.recordPayment(selectedBilling.id, {
        amount: parseFloat(data.amount),
        paymentMethod: data.paymentMethod,
        paymentDate: new Date().toISOString(),
      });
      fetchBillings(selectedPatient.id);
      setPaymentDialog(false);
      paymentForm.reset();
    } catch (err) {
      setError('Failed to record payment');
    }
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
              Billing & Insurance
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage invoices, payments, and insurance claims
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
            New Invoice
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
                  fetchBillings(patient.id);
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

        <Grid item xs={12} md={8}>
          {selectedPatient ? (
            <Paper elevation={3} sx={{ p: 3 }}>
              <Box display="flex" alignItems="center" mb={3}>
                <AttachMoney sx={{ fontSize: 40, color: '#667eea', mr: 2 }} />
                <Box>
                  <Typography variant="h6" fontWeight="bold">
                    Billing for {selectedPatient.firstName} {selectedPatient.lastName}
                  </Typography>
                </Box>
              </Box>

              <TableContainer>
                <Table>
                  <TableHead sx={{ bgcolor: '#f5f5f5' }}>
                    <TableRow>
                      <TableCell>Invoice #</TableCell>
                      <TableCell>Date</TableCell>
                      <TableCell>Total</TableCell>
                      <TableCell>Paid</TableCell>
                      <TableCell>Balance</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell align="right">Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {billings.length > 0 ? (
                      billings.map((billing) => (
                        <TableRow key={billing.id} hover>
                          <TableCell>
                            <Typography variant="body2" fontWeight="bold">
                              INV-{billing.id}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            {new Date(billing.invoiceDate).toLocaleDateString()}
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2" fontWeight="bold">
                              ${billing.totalAmount?.toFixed(2)}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            ${billing.paidAmount?.toFixed(2)}
                          </TableCell>
                          <TableCell>
                            <Typography
                              variant="body2"
                              fontWeight="bold"
                              color={billing.balanceAmount > 0 ? 'error' : 'success'}
                            >
                              ${billing.balanceAmount?.toFixed(2)}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={billing.status}
                              size="small"
                              color={
                                billing.status === 'Paid'
                                  ? 'success'
                                  : billing.status === 'Pending'
                                  ? 'warning'
                                  : 'default'
                              }
                            />
                          </TableCell>
                          <TableCell align="right">
                            {billing.balanceAmount > 0 && (
                              <Tooltip title="Record Payment">
                                <IconButton
                                  onClick={() => {
                                    setSelectedBilling(billing);
                                    setPaymentDialog(true);
                                  }}
                                  color="success"
                                >
                                  <Payment />
                                </IconButton>
                              </Tooltip>
                            )}
                            <Tooltip title="Print Invoice">
                              <IconButton color="secondary">
                                <Print />
                              </IconButton>
                            </Tooltip>
                          </TableCell>
                        </TableRow>
                      ))
                    ) : (
                      <TableRow>
                        <TableCell colSpan={7} align="center">
                          <Typography color="text.secondary">
                            No billing records found
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
              <Receipt sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select a patient to view billing
              </Typography>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Create Invoice Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>New Invoice</DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Controller
                  name="serviceDescription"
                  control={control}
                  rules={{ required: 'Description is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Service Description"
                      error={!!errors.serviceDescription}
                      helperText={errors.serviceDescription?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="totalAmount"
                  control={control}
                  rules={{ required: 'Amount is required', min: 0 }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Total Amount"
                      error={!!errors.totalAmount}
                      helperText={errors.totalAmount?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="billingCode"
                  control={control}
                  render={({ field }) => (
                    <TextField {...field} fullWidth label="CPT/ICD Code" />
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
                      label="Notes"
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
              Create Invoice
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* Record Payment Dialog */}
      <Dialog
        open={paymentDialog}
        onClose={() => setPaymentDialog(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Record Payment</DialogTitle>
        <form onSubmit={paymentForm.handleSubmit(handlePayment)}>
          <DialogContent>
            {selectedBilling && (
              <Alert severity="info" sx={{ mb: 2 }}>
                Balance Due: ${selectedBilling.balanceAmount?.toFixed(2)}
              </Alert>
            )}
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="amount"
                  control={paymentForm.control}
                  rules={{
                    required: 'Amount is required',
                    min: 0,
                    max: selectedBilling?.balanceAmount,
                  }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Payment Amount"
                      error={!!paymentForm.formState.errors.amount}
                      helperText={paymentForm.formState.errors.amount?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="paymentMethod"
                  control={paymentForm.control}
                  rules={{ required: 'Method is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Payment Method"
                      SelectProps={{ native: true }}
                      error={!!paymentForm.formState.errors.paymentMethod}
                      helperText={paymentForm.formState.errors.paymentMethod?.message}
                    >
                      <option value="">Select Method</option>
                      <option value="Cash">Cash</option>
                      <option value="Credit Card">Credit Card</option>
                      <option value="Debit Card">Debit Card</option>
                      <option value="Insurance">Insurance</option>
                      <option value="Check">Check</option>
                    </TextField>
                  )}
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setPaymentDialog(false)}>Cancel</Button>
            <Button
              type="submit"
              variant="contained"
              sx={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
              }}
            >
              Record Payment
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default BillingManagement;