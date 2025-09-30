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
  Avatar,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  OutlinedInput,
  Checkbox,
  ListItemText,
} from '@mui/material';
import {
  Add,
  Edit,
  Delete,
  People,
  AdminPanelSettings,
  Security,
  Block,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { authAPI } from '../../services/api';

// Healthcare roles
const ROLES = [
  'Administrator',
  'Doctor',
  'Nurse',
  'Receptionist',
  'Lab Technician',
  'Billing Staff',
];

const UserManagement = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingUser, setEditingUser] = useState(null);
  const [selectedRoles, setSelectedRoles] = useState([]);

  const { control, handleSubmit, reset, setValue, formState: { errors } } = useForm();

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const response = await authAPI.getAllUsers();
      setUsers(response.data);
    } catch (err) {
      setError('Failed to load users');
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data) => {
    try {
      const userData = {
        ...data,
        roles: selectedRoles,
      };

      if (editingUser) {
        await authAPI.updateUser(editingUser.id, userData);
        setSuccess('User updated successfully');
      } else {
        await authAPI.register(userData);
        setSuccess('User created successfully');
      }

      fetchUsers();
      handleCloseDialog();
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(`Failed to ${editingUser ? 'update' : 'create'} user: ${err.response?.data?.message || err.message}`);
    }
  };

  const handleEdit = (user) => {
    setEditingUser(user);
    setValue('firstName', user.firstName);
    setValue('lastName', user.lastName);
    setValue('email', user.email);
    setValue('phoneNumber', user.phoneNumber);
    setSelectedRoles(user.roles || []);
    setOpenDialog(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to deactivate this user?')) {
      try {
        await authAPI.deleteUser(id);
        setSuccess('User deactivated successfully');
        fetchUsers();
        setTimeout(() => setSuccess(''), 3000);
      } catch (err) {
        setError('Failed to deactivate user');
      }
    }
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingUser(null);
    setSelectedRoles([]);
    reset();
  };

  const getRoleColor = (role) => {
    const colors = {
      'Administrator': '#e74c3c',
      'Doctor': '#3498db',
      'Nurse': '#27ae60',
      'Receptionist': '#f39c12',
      'Lab Technician': '#9b59b6',
      'Billing Staff': '#16a085',
    };
    return colors[role] || '#667eea';
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
              User Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage system users and role assignments (Admin Only)
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
            Add User
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

      <Paper elevation={3}>
        <TableContainer>
          <Table>
            <TableHead sx={{ bgcolor: '#f5f5f5' }}>
              <TableRow>
                <TableCell>User</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Phone</TableCell>
                <TableCell>Roles</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {users.length > 0 ? (
                users.map((user) => (
                  <TableRow key={user.id} hover>
                    <TableCell>
                      <Box display="flex" alignItems="center">
                        <Avatar
                          sx={{
                            bgcolor: user.roles?.includes('Administrator')
                              ? '#e74c3c'
                              : '#667eea',
                            mr: 2,
                          }}
                        >
                          {user.firstName?.[0]}{user.lastName?.[0]}
                        </Avatar>
                        <Box>
                          <Typography variant="body1" fontWeight="bold">
                            {user.firstName} {user.lastName}
                          </Typography>
                          <Typography variant="caption" color="text.secondary">
                            ID: {user.id?.substring(0, 8)}
                          </Typography>
                        </Box>
                      </Box>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{user.email}</Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">
                        {user.phoneNumber || 'N/A'}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Box display="flex" gap={0.5} flexWrap="wrap">
                        {user.roles?.map((role) => (
                          <Chip
                            key={role}
                            label={role}
                            size="small"
                            sx={{
                              bgcolor: `${getRoleColor(role)}20`,
                              color: getRoleColor(role),
                              fontWeight: 'bold',
                            }}
                          />
                        ))}
                      </Box>
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={user.isActive ? 'Active' : 'Inactive'}
                        size="small"
                        color={user.isActive ? 'success' : 'default'}
                      />
                    </TableCell>
                    <TableCell align="right">
                      <Tooltip title="Edit">
                        <IconButton
                          onClick={() => handleEdit(user)}
                          color="primary"
                          size="small"
                        >
                          <Edit />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Deactivate">
                        <IconButton
                          onClick={() => handleDelete(user.id)}
                          color="error"
                          size="small"
                          disabled={user.roles?.includes('Administrator')}
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
                    <Typography color="text.secondary">No users found</Typography>
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {/* Add/Edit Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          <Box display="flex" alignItems="center">
            {editingUser ? <Edit sx={{ mr: 1 }} /> : <Add sx={{ mr: 1 }} />}
            {editingUser ? 'Edit User' : 'Create New User'}
          </Box>
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

              <Grid item xs={12}>
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
                      disabled={editingUser}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12}>
                <Controller
                  name="phoneNumber"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Phone Number"
                      placeholder="+1 (555) 123-4567"
                    />
                  )}
                />
              </Grid>

              {!editingUser && (
                <>
                  <Grid item xs={12}>
                    <Controller
                      name="password"
                      control={control}
                      rules={{
                        required: 'Password is required',
                        minLength: {
                          value: 8,
                          message: 'Password must be at least 8 characters',
                        },
                      }}
                      render={({ field }) => (
                        <TextField
                          {...field}
                          fullWidth
                          type="password"
                          label="Password"
                          error={!!errors.password}
                          helperText={errors.password?.message}
                        />
                      )}
                    />
                  </Grid>

                  <Grid item xs={12}>
                    <Controller
                      name="confirmPassword"
                      control={control}
                      rules={{
                        required: 'Confirm password is required',
                        validate: (value) =>
                          value === control._formValues.password || 'Passwords do not match',
                      }}
                      render={({ field }) => (
                        <TextField
                          {...field}
                          fullWidth
                          type="password"
                          label="Confirm Password"
                          error={!!errors.confirmPassword}
                          helperText={errors.confirmPassword?.message}
                        />
                      )}
                    />
                  </Grid>
                </>
              )}

              <Grid item xs={12}>
                <FormControl fullWidth>
                  <InputLabel>Roles *</InputLabel>
                  <Select
                    multiple
                    value={selectedRoles}
                    onChange={(e) => setSelectedRoles(e.target.value)}
                    input={<OutlinedInput label="Roles *" />}
                    renderValue={(selected) => (
                      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                        {selected.map((value) => (
                          <Chip
                            key={value}
                            label={value}
                            size="small"
                            sx={{
                              bgcolor: `${getRoleColor(value)}20`,
                              color: getRoleColor(value),
                            }}
                          />
                        ))}
                      </Box>
                    )}
                  >
                    {ROLES.map((role) => (
                      <MenuItem key={role} value={role}>
                        <Checkbox checked={selectedRoles.indexOf(role) > -1} />
                        <ListItemText primary={role} />
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12}>
                <Alert severity="info" icon={<Security />}>
                  <Typography variant="caption">
                    <strong>Role Permissions:</strong>
                    <br />
                    • <strong>Administrator:</strong> Full system access
                    <br />
                    • <strong>Doctor:</strong> Clinical records, prescriptions, orders
                    <br />
                    • <strong>Nurse:</strong> Patient care, vitals, medication admin
                    <br />
                    • <strong>Receptionist:</strong> Appointments, patient registration
                    <br />
                    • <strong>Lab Technician:</strong> Lab orders and results
                    <br />
                    • <strong>Billing Staff:</strong> Billing, insurance, payments
                  </Typography>
                </Alert>
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog}>Cancel</Button>
            <Button
              type="submit"
              variant="contained"
              disabled={selectedRoles.length === 0}
              sx={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
              }}
            >
              {editingUser ? 'Update' : 'Create'} User
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default UserManagement;