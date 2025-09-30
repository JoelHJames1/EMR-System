import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import {
  Box,
  Grid,
  Paper,
  Typography,
  Card,
  CardContent,
  Avatar,
  Chip,
  CircularProgress,
  Alert,
  LinearProgress,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Divider,
  Button,
  IconButton,
  Tooltip,
} from '@mui/material';
import {
  People,
  CalendarToday,
  LocalHospital,
  AttachMoney,
  Science,
  TrendingUp,
  TrendingDown,
  Medication,
  Warning,
  CheckCircle,
  Assignment,
  Phone,
  Email,
  Refresh,
  ArrowForward,
} from '@mui/icons-material';
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip as ChartTooltip,
  Legend,
  ResponsiveContainer,
  RadialBarChart,
  RadialBar,
} from 'recharts';
import { dashboardAPI } from '../../services/api';

const StatCard = ({ title, value, icon, color, trend, subtitle }) => (
  <motion.div
    initial={{ opacity: 0, y: 20 }}
    animate={{ opacity: 1, y: 0 }}
    whileHover={{ scale: 1.03, y: -5 }}
    transition={{ duration: 0.3 }}
  >
    <Card
      elevation={4}
      sx={{
        background: `linear-gradient(135deg, ${color}20 0%, ${color}05 100%)`,
        borderLeft: `5px solid ${color}`,
        height: '100%',
        position: 'relative',
        overflow: 'hidden',
      }}
    >
      <Box
        sx={{
          position: 'absolute',
          right: -20,
          bottom: -20,
          opacity: 0.1,
          transform: 'rotate(-15deg)',
        }}
      >
        {React.cloneElement(icon, { sx: { fontSize: 120 } })}
      </Box>
      <CardContent sx={{ position: 'relative', zIndex: 1 }}>
        <Box display="flex" justifyContent="space-between" alignItems="flex-start">
          <Box>
            <Typography variant="body2" color="text.secondary" gutterBottom fontWeight={500}>
              {title}
            </Typography>
            <Typography variant="h3" fontWeight="bold" color={color} gutterBottom>
              {value}
            </Typography>
            {subtitle && (
              <Typography variant="caption" color="text.secondary">
                {subtitle}
              </Typography>
            )}
            {trend !== undefined && (
              <Box display="flex" alignItems="center" mt={1}>
                {trend > 0 ? (
                  <TrendingUp fontSize="small" sx={{ color: '#4caf50' }} />
                ) : (
                  <TrendingDown fontSize="small" sx={{ color: '#f44336' }} />
                )}
                <Typography
                  variant="body2"
                  sx={{ color: trend > 0 ? '#4caf50' : '#f44336', ml: 0.5 }}
                  fontWeight={600}
                >
                  {Math.abs(trend)}% vs last month
                </Typography>
              </Box>
            )}
          </Box>
          <Avatar
            sx={{
              bgcolor: color,
              width: 60,
              height: 60,
              boxShadow: `0 4px 12px ${color}40`,
            }}
          >
            {icon}
          </Avatar>
        </Box>
      </CardContent>
    </Card>
  </motion.div>
);

const EnhancedDashboard = () => {
  const [stats, setStats] = useState(null);
  const [appointmentStats, setAppointmentStats] = useState(null);
  const [labStats, setLabStats] = useState(null);
  const [billingStats, setBillingStats] = useState(null);
  const [providerWorkload, setProviderWorkload] = useState(null);
  const [activity, setActivity] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const [statsRes, appointmentRes, labRes, billingRes, providerRes, activityRes] =
        await Promise.all([
          dashboardAPI.getStatistics(),
          dashboardAPI.getAppointmentStats(30),
          dashboardAPI.getLabStats(),
          dashboardAPI.getBillingSummary(30),
          dashboardAPI.getProviderWorkload(30),
          dashboardAPI.getActivity(48),
        ]);

      setStats(statsRes.data);
      setAppointmentStats(appointmentRes.data);
      setLabStats(labRes.data);
      setBillingStats(billingRes.data);
      setProviderWorkload(providerRes.data);
      setActivity(activityRes.data);
    } catch (err) {
      setError('Failed to load dashboard data');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleRefresh = async () => {
    setRefreshing(true);
    await fetchDashboardData();
    setRefreshing(false);
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress size={60} />
      </Box>
    );
  }

  if (error) {
    return <Alert severity="error">{error}</Alert>;
  }

  const COLORS = ['#667eea', '#764ba2', '#f093fb', '#4facfe', '#43e97b', '#fa709a'];

  return (
    <Box>
      {/* Header */}
      <motion.div initial={{ opacity: 0, y: -20 }} animate={{ opacity: 1, y: 0 }}>
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
          <Box>
            <Typography variant="h3" fontWeight="bold" gutterBottom>
              Dashboard Overview
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Real-time insights into your healthcare operations
            </Typography>
          </Box>
          <Tooltip title="Refresh Data">
            <IconButton
              onClick={handleRefresh}
              disabled={refreshing}
              sx={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                color: 'white',
                '&:hover': {
                  background: 'linear-gradient(135deg, #764ba2 0%, #667eea 100%)',
                },
              }}
            >
              <Refresh />
            </IconButton>
          </Tooltip>
        </Box>
      </motion.div>

      {/* Main Statistics Cards */}
      <Grid container spacing={3} mb={4}>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Total Patients"
            value={stats?.totalPatients?.toLocaleString() || 0}
            subtitle="Active in system"
            icon={<People />}
            color="#667eea"
            trend={5.2}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Today's Appointments"
            value={stats?.todayAppointments || 0}
            subtitle={`${stats?.totalEncounters || 0} encounters this month`}
            icon={<CalendarToday />}
            color="#764ba2"
            trend={-2.1}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Active Prescriptions"
            value={stats?.activePrescriptions || 0}
            subtitle="Currently prescribed"
            icon={<Medication />}
            color="#f093fb"
            trend={8.4}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Pending Labs"
            value={stats?.pendingLabOrders || 0}
            subtitle="Awaiting results"
            icon={<Science />}
            color="#4facfe"
            trend={3.8}
          />
        </Grid>
      </Grid>

      {/* Financial Overview */}
      <Grid container spacing={3} mb={4}>
        <Grid item xs={12} md={8}>
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.2 }}
          >
            <Paper elevation={4} sx={{ p: 3, height: '450px' }}>
              <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                <Typography variant="h6" fontWeight="bold">
                  Appointment & Revenue Trends
                </Typography>
                <Chip label="Last 30 Days" color="primary" size="small" />
              </Box>
              <ResponsiveContainer width="100%" height="90%">
                <AreaChart data={appointmentStats?.byDate || []}>
                  <defs>
                    <linearGradient id="colorAppointments" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#667eea" stopOpacity={0.8} />
                      <stop offset="95%" stopColor="#667eea" stopOpacity={0} />
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                  <XAxis
                    dataKey="date"
                    tickFormatter={(date) =>
                      new Date(date).toLocaleDateString('en-US', {
                        month: 'short',
                        day: 'numeric',
                      })
                    }
                  />
                  <YAxis />
                  <ChartTooltip
                    labelFormatter={(date) => new Date(date).toLocaleDateString()}
                  />
                  <Legend />
                  <Area
                    type="monotone"
                    dataKey="count"
                    stroke="#667eea"
                    strokeWidth={3}
                    fillOpacity={1}
                    fill="url(#colorAppointments)"
                    name="Appointments"
                  />
                </AreaChart>
              </ResponsiveContainer>
            </Paper>
          </motion.div>
        </Grid>

        <Grid item xs={12} md={4}>
          <motion.div
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.3 }}
          >
            <Paper elevation={4} sx={{ p: 3, height: '450px' }}>
              <Typography variant="h6" fontWeight="bold" gutterBottom>
                Financial Summary
              </Typography>
              <Box sx={{ mt: 3 }}>
                <Box mb={3}>
                  <Box display="flex" justifyContent="space-between" mb={1}>
                    <Typography variant="body2" color="text.secondary">
                      Total Billed
                    </Typography>
                    <Typography variant="h6" fontWeight="bold" color="#667eea">
                      ${billingStats?.totalBilled?.toLocaleString() || 0}
                    </Typography>
                  </Box>
                  <LinearProgress
                    variant="determinate"
                    value={100}
                    sx={{
                      height: 8,
                      borderRadius: 4,
                      bgcolor: '#e0e0e0',
                      '& .MuiLinearProgress-bar': {
                        background: 'linear-gradient(90deg, #667eea 0%, #764ba2 100%)',
                      },
                    }}
                  />
                </Box>

                <Box mb={3}>
                  <Box display="flex" justifyContent="space-between" mb={1}>
                    <Typography variant="body2" color="text.secondary">
                      Total Collected
                    </Typography>
                    <Typography variant="h6" fontWeight="bold" color="#4facfe">
                      ${billingStats?.totalPaid?.toLocaleString() || 0}
                    </Typography>
                  </Box>
                  <LinearProgress
                    variant="determinate"
                    value={
                      (billingStats?.totalPaid / billingStats?.totalBilled) * 100 || 0
                    }
                    sx={{
                      height: 8,
                      borderRadius: 4,
                      bgcolor: '#e0e0e0',
                      '& .MuiLinearProgress-bar': {
                        background: 'linear-gradient(90deg, #4facfe 0%, #00f2fe 100%)',
                      },
                    }}
                  />
                </Box>

                <Box mb={3}>
                  <Box display="flex" justifyContent="space-between" mb={1}>
                    <Typography variant="body2" color="text.secondary">
                      Outstanding
                    </Typography>
                    <Typography variant="h6" fontWeight="bold" color="#f093fb">
                      ${billingStats?.totalOutstanding?.toLocaleString() || 0}
                    </Typography>
                  </Box>
                  <LinearProgress
                    variant="determinate"
                    value={
                      (billingStats?.totalOutstanding / billingStats?.totalBilled) * 100 ||
                      0
                    }
                    sx={{
                      height: 8,
                      borderRadius: 4,
                      bgcolor: '#e0e0e0',
                      '& .MuiLinearProgress-bar': {
                        background: 'linear-gradient(90deg, #f093fb 0%, #f5576c 100%)',
                      },
                    }}
                  />
                </Box>

                <Divider sx={{ my: 2 }} />

                <Box
                  sx={{
                    p: 2,
                    background: 'linear-gradient(135deg, #43e97b15 0%, #38f9d715 100%)',
                    borderRadius: 2,
                    borderLeft: '4px solid #43e97b',
                  }}
                >
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    Collection Rate
                  </Typography>
                  <Typography variant="h4" fontWeight="bold" color="#43e97b">
                    {billingStats?.collectionRate?.toFixed(1) || 0}%
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Target: 95%
                  </Typography>
                </Box>
              </Box>
            </Paper>
          </motion.div>
        </Grid>
      </Grid>

      {/* Lab Status and Provider Workload */}
      <Grid container spacing={3} mb={4}>
        <Grid item xs={12} md={6}>
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.4 }}
          >
            <Paper elevation={4} sx={{ p: 3, height: '400px' }}>
              <Typography variant="h6" fontWeight="bold" gutterBottom>
                Laboratory Status Overview
              </Typography>
              <ResponsiveContainer width="100%" height="85%">
                <PieChart>
                  <Pie
                    data={[
                      { name: 'Pending', value: labStats?.pending || 0 },
                      { name: 'In Progress', value: labStats?.inProgress || 0 },
                      { name: 'Completed', value: labStats?.completed || 0 },
                    ]}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    label={({ name, percent }) =>
                      `${name}: ${(percent * 100).toFixed(0)}%`
                    }
                    outerRadius={100}
                    fill="#8884d8"
                    dataKey="value"
                  >
                    {[0, 1, 2].map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <ChartTooltip />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            </Paper>
          </motion.div>
        </Grid>

        <Grid item xs={12} md={6}>
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.5 }}
          >
            <Paper elevation={4} sx={{ p: 3, height: '400px', overflow: 'auto' }}>
              <Typography variant="h6" fontWeight="bold" gutterBottom>
                Top Providers by Workload
              </Typography>
              <List>
                {providerWorkload?.workload?.slice(0, 5).map((provider, index) => (
                  <React.Fragment key={index}>
                    <ListItem>
                      <ListItemAvatar>
                        <Avatar sx={{ bgcolor: COLORS[index % COLORS.length] }}>
                          {provider.providerName?.charAt(0)}
                        </Avatar>
                      </ListItemAvatar>
                      <ListItemText
                        primary={provider.providerName}
                        secondary={provider.specialization}
                      />
                      <Box textAlign="right">
                        <Typography variant="body2" fontWeight="bold">
                          {provider.encountersCount} Encounters
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          {provider.appointmentsCount} Appointments
                        </Typography>
                      </Box>
                    </ListItem>
                    {index < 4 && <Divider variant="inset" component="li" />}
                  </React.Fragment>
                ))}
              </List>
            </Paper>
          </motion.div>
        </Grid>
      </Grid>

      {/* Recent Activity */}
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.6 }}
          >
            <Paper elevation={4} sx={{ p: 3 }}>
              <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
                <Typography variant="h6" fontWeight="bold">
                  Recent Activity Feed
                </Typography>
                <Button
                  endIcon={<ArrowForward />}
                  sx={{ color: '#667eea', fontWeight: 600 }}
                >
                  View All
                </Button>
              </Box>
              <Grid container spacing={2}>
                {activity.recentEncounters?.slice(0, 6).map((encounter, index) => (
                  <Grid item xs={12} md={6} key={index}>
                    <motion.div whileHover={{ scale: 1.02 }} transition={{ duration: 0.2 }}>
                      <Box
                        display="flex"
                        alignItems="center"
                        p={2}
                        sx={{
                          background: 'linear-gradient(135deg, #667eea10 0%, #764ba210 100%)',
                          borderRadius: 2,
                          borderLeft: '4px solid #667eea',
                          cursor: 'pointer',
                          '&:hover': {
                            background:
                              'linear-gradient(135deg, #667eea20 0%, #764ba220 100%)',
                          },
                        }}
                      >
                        <Avatar sx={{ bgcolor: '#667eea', mr: 2 }}>
                          <LocalHospital />
                        </Avatar>
                        <Box flexGrow={1}>
                          <Typography variant="body1" fontWeight="bold">
                            {encounter.patientName}
                          </Typography>
                          <Typography variant="caption" color="text.secondary">
                            with {encounter.providerName || 'N/A'}
                          </Typography>
                          <Typography variant="caption" display="block" color="text.secondary">
                            {new Date(encounter.date).toLocaleString()}
                          </Typography>
                        </Box>
                        <Chip
                          label={encounter.status}
                          size="small"
                          color={encounter.status === 'Completed' ? 'success' : 'primary'}
                        />
                      </Box>
                    </motion.div>
                  </Grid>
                ))}
              </Grid>
            </Paper>
          </motion.div>
        </Grid>
      </Grid>
    </Box>
  );
};

export default EnhancedDashboard;