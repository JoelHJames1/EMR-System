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
  Avatar,
  Tooltip,
  MenuItem,
  Divider,
} from '@mui/material';
import {
  Add,
  Edit,
  Delete,
  MedicalInformation,
  Phone,
  Email,
  Badge,
  CheckCircle,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { providerAPI } from '../../services/api';

const ProviderManagement = () => {
  const [providers, setProviders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingProvider, setEditingProvider] = useState(null);
  const [viewingProvider, setViewingProvider] = useState(null);

  const { control, handleSubmit, reset, setValue, formState: { errors } } = useForm();

  useEffect(() => {
    fetchProviders();
  }, []);

  const fetchProviders = async () => {
    try {
      setLoading(true);
      const response = await providerAPI.getAll();
      setProviders(response.data);
    } catch (err) {
      setError('Failed to load providers');
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data) => {
    try {
      const providerData = {
        ...data,
        isActive: true,
      };

      if (editingProvider) {
        await providerAPI.update(editingProvider.id, providerData);
        setSuccess('Provider updated successfully');
      } else {
        await providerAPI.create(providerData);
        setSuccess('Provider created successfully');
      }

      fetchProviders();
      handleCloseDialog();
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(`Failed to ${editingProvider ? 'update' : 'create'} provider`);
    }
  };

  const handleEdit = (provider) => {
    setEditingProvider(provider);
    setValue('firstName', provider.firstName);
    setValue('lastName', provider.lastName);
    setValue('specialization', provider.specialization);
    setValue('npiNumber', provider.npiNumber);
    setValue('deaNumber', provider.deaNumber);
    setValue('licenseNumber', provider.licenseNumber);
    setValue('licenseState', provider.licenseState);
    setValue('phone', provider.phone);
    setValue('email', provider.email);
    setValue('credentials', provider.credentials);
    setValue('department', provider.department);
    setValue('officeLocation', provider.officeLocation);
    setOpenDialog(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to deactivate this provider?')) {
      try {
        await providerAPI.delete(id);
        setSuccess('Provider deactivated successfully');
        fetchProviders();
        setTimeout(() => setSuccess(''), 3000);
      } catch (err) {
        setError('Failed to deactivate provider');
      }
    }
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingProvider(null);
    reset();
  };

  const getSpecializationColor = (specialization) => {
    const colors = {
      'Cardiology': '#e74c3c',
      'Neurology': '#9b59b6',
      'Pediatrics': '#3498db',
      'Orthopedics': '#16a085',
      'General Practice': '#27ae60',
      'Internal Medicine': '#f39c12',
      'Surgery': '#c0392b',
      'Psychiatry': '#8e44ad',
    };
    return colors[specialization] || '#667eea';
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
              Provider Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage healthcare providers, doctors, and specialists
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
            Add Provider
          </Button>
        </Box>
      </motion.div>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      {success && (
        <Alert severity="success" sx={{ mb: 3 }} onClose={() => setSuccess('')}>
          {success}
        </Alert>
      )}

      <Grid container spacing={3}>
        {providers.map((provider) => (
          <Grid item xs={12} sm={6} md={4} key={provider.id}>
            <motion.div
              whileHover={{ scale: 1.02 }}
              transition={{ type: 'spring', stiffness: 300 }}
            >
              <Card
                elevation={3}
                sx={{
                  cursor: 'pointer',
                  borderTop: `4px solid ${getSpecializationColor(provider.specialization)}`,
                  '&:hover': { boxShadow: 6 },
                }}
                onClick={() => setViewingProvider(provider)}
              >
                <CardContent>
                  <Box display="flex" alignItems="center" mb={2}>
                    <Avatar
                      sx={{
                        width: 60,
                        height: 60,
                        bgcolor: getSpecializationColor(provider.specialization),
                        mr: 2,
                      }}
                    >
                      {provider.firstName?.[0]}{provider.lastName?.[0]}
                    </Avatar>
                    <Box flexGrow={1}>
                      <Typography variant="h6" fontWeight="bold">
                        Dr. {provider.firstName} {provider.lastName}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {provider.credentials || 'MD'}
                      </Typography>
                    </Box>
                    {provider.isActive && (
                      <CheckCircle sx={{ color: '#4caf50' }} />
                    )}
                  </Box>

                  <Chip
                    label={provider.specialization}
                    size="small"
                    sx={{
                      bgcolor: `${getSpecializationColor(provider.specialization)}20`,
                      color: getSpecializationColor(provider.specialization),
                      fontWeight: 'bold',
                      mb: 2,
                    }}
                  />

                  <Divider sx={{ my: 2 }} />

                  <Box display="flex" alignItems="center" mb={1}>
                    <Badge sx={{ fontSize: 16, color: 'text.secondary', mr: 1 }} />
                    <Typography variant="caption" color="text.secondary">
                      NPI: {provider.npiNumber || 'Not provided'}
                    </Typography>
                  </Box>

                  {provider.phone && (
                    <Box display="flex" alignItems="center" mb={1}>
                      <Phone sx={{ fontSize: 16, color: 'text.secondary', mr: 1 }} />
                      <Typography variant="caption" color="text.secondary">
                        {provider.phone}
                      </Typography>
                    </Box>
                  )}

                  {provider.email && (
                    <Box display="flex" alignItems="center" mb={1}>
                      <Email sx={{ fontSize: 16, color: 'text.secondary', mr: 1 }} />
                      <Typography variant="caption" color="text.secondary" noWrap>
                        {provider.email}
                      </Typography>
                    </Box>
                  )}

                  {provider.department && (
                    <Box display="flex" alignItems="center">
                      <MedicalInformation sx={{ fontSize: 16, color: 'text.secondary', mr: 1 }} />
                      <Typography variant="caption" color="text.secondary">
                        {provider.department}
                      </Typography>
                    </Box>
                  )}

                  <Box display="flex" gap={1} justifyContent="flex-end" mt={2}>
                    <Tooltip title="Edit">
                      <IconButton
                        onClick={(e) => {
                          e.stopPropagation();
                          handleEdit(provider);
                        }}
                        color="primary"
                        size="small"
                      >
                        <Edit />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Deactivate">
                      <IconButton
                        onClick={(e) => {
                          e.stopPropagation();
                          handleDelete(provider.id);
                        }}
                        color="error"
                        size="small"
                      >
                        <Delete />
                      </IconButton>
                    </Tooltip>
                  </Box>
                </CardContent>
              </Card>
            </motion.div>
          </Grid>
        ))}
      </Grid>

      {providers.length === 0 && (
        <Paper elevation={3} sx={{ p: 5, textAlign: 'center', mt: 3 }}>
          <MedicalInformation sx={{ fontSize: 100, color: '#e0e0e0', mb: 2 }} />
          <Typography variant="h6" color="text.secondary">
            No providers found
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Add your first healthcare provider
          </Typography>
        </Paper>
      )}

      {/* Add/Edit Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingProvider ? 'Edit Provider' : 'Add New Provider'}
        </DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="firstName"
                  control={control}
                  rules={{ required: 'First name is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="First Name"
                      error={!!errors.firstName}
                      helperText={errors.firstName?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="lastName"
                  control={control}
                  rules={{ required: 'Last name is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Last Name"
                      error={!!errors.lastName}
                      helperText={errors.lastName?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="specialization"
                  control={control}
                  rules={{ required: 'Specialization is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Specialization"
                      error={!!errors.specialization}
                      helperText={errors.specialization?.message}
                    >
                      <MenuItem value="General Practice">General Practice</MenuItem>
                      <MenuItem value="Internal Medicine">Internal Medicine</MenuItem>
                      <MenuItem value="Cardiology">Cardiology</MenuItem>
                      <MenuItem value="Neurology">Neurology</MenuItem>
                      <MenuItem value="Pediatrics">Pediatrics</MenuItem>
                      <MenuItem value="Orthopedics">Orthopedics</MenuItem>
                      <MenuItem value="Surgery">Surgery</MenuItem>
                      <MenuItem value="Psychiatry">Psychiatry</MenuItem>
                      <MenuItem value="Dermatology">Dermatology</MenuItem>
                      <MenuItem value="Ophthalmology">Ophthalmology</MenuItem>
                      <MenuItem value="Emergency Medicine">Emergency Medicine</MenuItem>
                      <MenuItem value="Anesthesiology">Anesthesiology</MenuItem>
                      <MenuItem value="Radiology">Radiology</MenuItem>
                      <MenuItem value="Pathology">Pathology</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="credentials"
                  control={control}
                  defaultValue="MD"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Credentials"
                      placeholder="e.g., MD, DO, NP, PA"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="npiNumber"
                  control={control}
                  rules={{ required: 'NPI number is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="NPI Number"
                      placeholder="10-digit NPI"
                      error={!!errors.npiNumber}
                      helperText={errors.npiNumber?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="deaNumber"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="DEA Number"
                      placeholder="For controlled substance prescribing"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="licenseNumber"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="License Number"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="licenseState"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="License State"
                      placeholder="e.g., CA, NY, TX"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="phone"
                  control={control}
                  rules={{ required: 'Phone is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Phone Number"
                      placeholder="+1 (555) 123-4567"
                      error={!!errors.phone}
                      helperText={errors.phone?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="email"
                  control={control}
                  rules={{
                    required: 'Email is required',
                    pattern: {
                      value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                      message: 'Invalid email address',
                    },
                  }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="email"
                      label="Email"
                      error={!!errors.email}
                      helperText={errors.email?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="department"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Department"
                      placeholder="e.g., Emergency, Surgery"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="officeLocation"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Office Location"
                      placeholder="e.g., Building A, Room 201"
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
              {editingProvider ? 'Update' : 'Add'} Provider
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* View Dialog */}
      <Dialog
        open={Boolean(viewingProvider)}
        onClose={() => setViewingProvider(null)}
        maxWidth="sm"
        fullWidth
      >
        {viewingProvider && (
          <>
            <DialogTitle>
              <Box display="flex" alignItems="center">
                <Avatar
                  sx={{
                    width: 50,
                    height: 50,
                    bgcolor: getSpecializationColor(viewingProvider.specialization),
                    mr: 2,
                  }}
                >
                  {viewingProvider.firstName?.[0]}{viewingProvider.lastName?.[0]}
                </Avatar>
                <Box>
                  <Typography variant="h6">
                    Dr. {viewingProvider.firstName} {viewingProvider.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {viewingProvider.credentials || 'MD'}
                  </Typography>
                </Box>
              </Box>
            </DialogTitle>
            <DialogContent>
              <Grid container spacing={2}>
                <Grid item xs={12}>
                  <Chip
                    label={viewingProvider.specialization}
                    sx={{
                      bgcolor: `${getSpecializationColor(viewingProvider.specialization)}20`,
                      color: getSpecializationColor(viewingProvider.specialization),
                      fontWeight: 'bold',
                    }}
                  />
                </Grid>
                <Grid item xs={6}>
                  <Typography variant="caption" color="text.secondary">
                    NPI Number
                  </Typography>
                  <Typography variant="body2" fontWeight="bold">
                    {viewingProvider.npiNumber || 'N/A'}
                  </Typography>
                </Grid>
                <Grid item xs={6}>
                  <Typography variant="caption" color="text.secondary">
                    DEA Number
                  </Typography>
                  <Typography variant="body2" fontWeight="bold">
                    {viewingProvider.deaNumber || 'N/A'}
                  </Typography>
                </Grid>
                <Grid item xs={6}>
                  <Typography variant="caption" color="text.secondary">
                    License Number
                  </Typography>
                  <Typography variant="body2" fontWeight="bold">
                    {viewingProvider.licenseNumber || 'N/A'}
                  </Typography>
                </Grid>
                <Grid item xs={6}>
                  <Typography variant="caption" color="text.secondary">
                    License State
                  </Typography>
                  <Typography variant="body2" fontWeight="bold">
                    {viewingProvider.licenseState || 'N/A'}
                  </Typography>
                </Grid>
                <Grid item xs={12}>
                  <Divider />
                </Grid>
                <Grid item xs={6}>
                  <Typography variant="caption" color="text.secondary">
                    Phone
                  </Typography>
                  <Typography variant="body2" fontWeight="bold">
                    {viewingProvider.phone}
                  </Typography>
                </Grid>
                <Grid item xs={6}>
                  <Typography variant="caption" color="text.secondary">
                    Email
                  </Typography>
                  <Typography variant="body2" fontWeight="bold">
                    {viewingProvider.email}
                  </Typography>
                </Grid>
                {viewingProvider.department && (
                  <Grid item xs={6}>
                    <Typography variant="caption" color="text.secondary">
                      Department
                    </Typography>
                    <Typography variant="body2" fontWeight="bold">
                      {viewingProvider.department}
                    </Typography>
                  </Grid>
                )}
                {viewingProvider.officeLocation && (
                  <Grid item xs={6}>
                    <Typography variant="caption" color="text.secondary">
                      Office Location
                    </Typography>
                    <Typography variant="body2" fontWeight="bold">
                      {viewingProvider.officeLocation}
                    </Typography>
                  </Grid>
                )}
              </Grid>
            </DialogContent>
            <DialogActions>
              <Button onClick={() => setViewingProvider(null)}>Close</Button>
              <Button
                onClick={() => {
                  handleEdit(viewingProvider);
                  setViewingProvider(null);
                }}
                variant="contained"
                startIcon={<Edit />}
              >
                Edit
              </Button>
            </DialogActions>
          </>
        )}
      </Dialog>
    </Box>
  );
};

export default ProviderManagement;