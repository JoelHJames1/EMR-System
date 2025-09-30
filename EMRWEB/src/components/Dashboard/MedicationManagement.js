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
  MenuItem,
  InputAdornment,
} from '@mui/material';
import {
  Add,
  Edit,
  Delete,
  Medication as MedicationIcon,
  Search,
  Warning,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { medicationAPI } from '../../services/api';

const MedicationManagement = () => {
  const [medications, setMedications] = useState([]);
  const [filteredMedications, setFilteredMedications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingMedication, setEditingMedication] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  const { control, handleSubmit, reset, setValue, formState: { errors } } = useForm();

  useEffect(() => {
    fetchMedications();
  }, []);

  useEffect(() => {
    if (searchTerm) {
      const filtered = medications.filter((med) =>
        med.genericName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        med.brandName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        med.ndc?.includes(searchTerm)
      );
      setFilteredMedications(filtered);
    } else {
      setFilteredMedications(medications);
    }
  }, [searchTerm, medications]);

  const fetchMedications = async () => {
    try {
      setLoading(true);
      const response = await medicationAPI.getAll();
      setMedications(response.data);
      setFilteredMedications(response.data);
    } catch (err) {
      setError('Failed to load medications');
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data) => {
    try {
      const medicationData = {
        ...data,
        isActive: true,
      };

      if (editingMedication) {
        await medicationAPI.update(editingMedication.id, medicationData);
        setSuccess('Medication updated successfully');
      } else {
        await medicationAPI.create(medicationData);
        setSuccess('Medication created successfully');
      }

      fetchMedications();
      handleCloseDialog();
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(`Failed to ${editingMedication ? 'update' : 'create'} medication`);
    }
  };

  const handleEdit = (medication) => {
    setEditingMedication(medication);
    setValue('genericName', medication.genericName);
    setValue('brandName', medication.brandName);
    setValue('ndc', medication.ndc);
    setValue('deaSchedule', medication.deaSchedule);
    setValue('form', medication.form);
    setValue('strength', medication.strength);
    setValue('unit', medication.unit);
    setValue('manufacturer', medication.manufacturer);
    setValue('description', medication.description);
    setOpenDialog(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this medication?')) {
      try {
        await medicationAPI.delete(id);
        setSuccess('Medication deleted successfully');
        fetchMedications();
        setTimeout(() => setSuccess(''), 3000);
      } catch (err) {
        setError('Failed to delete medication');
      }
    }
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingMedication(null);
    reset();
  };

  const getDEAScheduleColor = (schedule) => {
    if (!schedule || schedule === 'None') return 'default';
    const colors = {
      'I': 'error',
      'II': 'error',
      'III': 'warning',
      'IV': 'warning',
      'V': 'info',
    };
    return colors[schedule] || 'default';
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
              Medication Database
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage medication formulary with NDC codes and DEA schedules
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
            Add Medication
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

      <Paper elevation={3} sx={{ mb: 3, p: 2 }}>
        <TextField
          fullWidth
          placeholder="Search by name or NDC code..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <Search />
              </InputAdornment>
            ),
          }}
        />
      </Paper>

      <Paper elevation={3}>
        <TableContainer>
          <Table>
            <TableHead sx={{ bgcolor: '#f5f5f5' }}>
              <TableRow>
                <TableCell>Medication Name</TableCell>
                <TableCell>NDC Code</TableCell>
                <TableCell>Form & Strength</TableCell>
                <TableCell>DEA Schedule</TableCell>
                <TableCell>Manufacturer</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredMedications.length > 0 ? (
                filteredMedications.map((medication) => (
                  <TableRow key={medication.id} hover>
                    <TableCell>
                      <Typography variant="body1" fontWeight="bold">
                        {medication.genericName}
                      </Typography>
                      {medication.brandName && (
                        <Typography variant="caption" color="text.secondary">
                          Brand: {medication.brandName}
                        </Typography>
                      )}
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" fontFamily="monospace">
                        {medication.ndc || 'N/A'}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">
                        {medication.form} - {medication.strength} {medication.unit}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      {medication.deaSchedule && medication.deaSchedule !== 'None' ? (
                        <Chip
                          label={`Schedule ${medication.deaSchedule}`}
                          size="small"
                          color={getDEAScheduleColor(medication.deaSchedule)}
                          icon={<Warning />}
                        />
                      ) : (
                        <Chip label="Non-Controlled" size="small" variant="outlined" />
                      )}
                    </TableCell>
                    <TableCell>
                      <Typography variant="caption" color="text.secondary">
                        {medication.manufacturer || 'N/A'}
                      </Typography>
                    </TableCell>
                    <TableCell align="right">
                      <Tooltip title="Edit">
                        <IconButton
                          onClick={() => handleEdit(medication)}
                          color="primary"
                          size="small"
                        >
                          <Edit />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Delete">
                        <IconButton
                          onClick={() => handleDelete(medication.id)}
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
                  <TableCell colSpan={6} align="center">
                    <Typography color="text.secondary">
                      {searchTerm ? 'No medications found matching your search' : 'No medications in database'}
                    </Typography>
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {/* Add/Edit Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingMedication ? 'Edit Medication' : 'Add New Medication'}
        </DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="genericName"
                  control={control}
                  rules={{ required: 'Generic name is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Generic Name *"
                      placeholder="e.g., Lisinopril"
                      error={!!errors.genericName}
                      helperText={errors.genericName?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="brandName"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Brand Name"
                      placeholder="e.g., Prinivil, Zestril"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="ndc"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="NDC Code"
                      placeholder="e.g., 12345-678-90"
                      helperText="National Drug Code"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="deaSchedule"
                  control={control}
                  defaultValue="None"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="DEA Schedule"
                    >
                      <MenuItem value="None">Non-Controlled</MenuItem>
                      <MenuItem value="I">Schedule I (No accepted medical use)</MenuItem>
                      <MenuItem value="II">Schedule II (High abuse potential)</MenuItem>
                      <MenuItem value="III">Schedule III (Moderate/low dependence)</MenuItem>
                      <MenuItem value="IV">Schedule IV (Low abuse potential)</MenuItem>
                      <MenuItem value="V">Schedule V (Lowest abuse potential)</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <Controller
                  name="form"
                  control={control}
                  rules={{ required: 'Form is required' }}
                  defaultValue="Tablet"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Form *"
                      error={!!errors.form}
                      helperText={errors.form?.message}
                    >
                      <MenuItem value="Tablet">Tablet</MenuItem>
                      <MenuItem value="Capsule">Capsule</MenuItem>
                      <MenuItem value="Syrup">Syrup</MenuItem>
                      <MenuItem value="Injection">Injection</MenuItem>
                      <MenuItem value="Cream">Cream</MenuItem>
                      <MenuItem value="Ointment">Ointment</MenuItem>
                      <MenuItem value="Drops">Drops</MenuItem>
                      <MenuItem value="Inhaler">Inhaler</MenuItem>
                      <MenuItem value="Patch">Patch</MenuItem>
                      <MenuItem value="Suppository">Suppository</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={3}>
                <Controller
                  name="strength"
                  control={control}
                  rules={{ required: 'Strength is required' }}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Strength *"
                      placeholder="e.g., 10"
                      error={!!errors.strength}
                      helperText={errors.strength?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} sm={3}>
                <Controller
                  name="unit"
                  control={control}
                  rules={{ required: 'Unit is required' }}
                  defaultValue="mg"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      select
                      label="Unit *"
                      error={!!errors.unit}
                      helperText={errors.unit?.message}
                    >
                      <MenuItem value="mg">mg</MenuItem>
                      <MenuItem value="g">g</MenuItem>
                      <MenuItem value="mcg">mcg</MenuItem>
                      <MenuItem value="mL">mL</MenuItem>
                      <MenuItem value="L">L</MenuItem>
                      <MenuItem value="%">%</MenuItem>
                      <MenuItem value="IU">IU</MenuItem>
                      <MenuItem value="units">units</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid item xs={12}>
                <Controller
                  name="manufacturer"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Manufacturer"
                      placeholder="e.g., Pfizer, Merck, Novartis"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12}>
                <Controller
                  name="description"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      rows={3}
                      label="Description"
                      placeholder="Clinical information, indications, contraindications..."
                    />
                  )}
                />
              </Grid>

              {control._formValues.deaSchedule && control._formValues.deaSchedule !== 'None' && (
                <Grid item xs={12}>
                  <Alert severity="warning" icon={<Warning />}>
                    <Typography variant="caption">
                      <strong>Controlled Substance:</strong> This medication is classified as DEA Schedule{' '}
                      {control._formValues.deaSchedule}. Special prescribing requirements and tracking apply.
                    </Typography>
                  </Alert>
                </Grid>
              )}
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
              {editingMedication ? 'Update' : 'Add'} Medication
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default MedicationManagement;