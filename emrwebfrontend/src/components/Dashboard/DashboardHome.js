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
} from '@mui/material';
import {
  People,
  CalendarToday,
  LocalHospital,
  AttachMoney,
  Science,
  TrendingUp,
  TrendingDown,
} from '@mui/icons-material';
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts';
import { dashboardAPI } from '../../services/api';

const StatCard = ({ title, value, icon, color, trend }) => (
  <motion.div
    initial={{ opacity: 0, y: 20 }}
    animate={{ opacity: 1, y: 0 }}
    whileHover={{ scale: 1.05 }}
    transition={{ duration: 0.3 }}
  >
    <Card
      elevation={3}
      sx={{
        background: `linear-gradient(135deg, ${color}15 0%, ${color}05 100%)`,
        borderLeft: `4px solid ${color}`,
        height: '100%',
      }}
    >
      <CardContent>
        <Box display="flex" justifyContent="space-between" alignItems="center">
          <Box>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              {title}
            </Typography>
            <Typography variant="h4" fontWeight="bold">
              {value}
            </Typography>
            {trend && (
              <Box display="flex" alignItems="center" mt={1}>
                {trend > 0 ? (
                  <TrendingUp fontSize="small" color="success" />
                ) : (
                  <TrendingDown fontSize="small" color="error" />
                )}
                <Typography
                  variant="caption"
                  color={trend > 0 ? 'success.main' : 'error.main'}
                  ml={0.5}
                >
                  {Math.abs(trend)}%
                </Typography>
              </Box>
            )}
          </Box>
          <Avatar
            sx={{
              bgcolor: color,
              width: 56,
              height: 56,
            }}
          >
            {icon}
          </Avatar>
        </Box>
      </CardContent>
    </Card>
  </motion.div>
);

const DashboardHome = () => {
  const [stats, setStats] = useState(null);
  const [appointmentStats, setAppointmentStats] = useState(null);
  const [labStats, setLabStats] = useState(null);
  const [billingStats, setBillingStats] = useState(null);
  const [activity, setActivity] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const [statsRes, appointmentRes, labRes, billingRes, activityRes] = await Promise.all([
        dashboardAPI.getStatistics(),
        dashboardAPI.getAppointmentStats(7),
        dashboardAPI.getLabStats(),
        dashboardAPI.getBillingSummary(30),
        dashboardAPI.getActivity(24),
      ]);

      setStats(statsRes.data);
      setAppointmentStats(appointmentRes.data);
      setLabStats(labRes.data);
      setBillingStats(billingRes.data);
      setActivity(activityRes.data);
    } catch (err) {
      setError('Failed to load dashboard data');
      console.error(err);
    } finally {
      setLoading(false);
    }
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

  const COLORS = ['#667eea', '#764ba2', '#f093fb', '#4facfe'];

  return (
    <Box>
      <motion.div
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
      >
        <Typography variant="h4" fontWeight="bold" gutterBottom>
          Dashboard Overview
        </Typography>
        <Typography variant="body1" color="text.secondary" mb={4}>
          Welcome back! Here's what's happening today.
        </Typography>
      </motion.div>

      {/* Statistics Cards */}
      <Grid container spacing={3} mb={4}>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Total Patients"
            value={stats?.totalPatients || 0}
            icon={<People />}
            color="#667eea"
            trend={5.2}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Today's Appointments"
            value={stats?.todayAppointments || 0}
            icon={<CalendarToday />}
            color="#764ba2"
            trend={-2.1}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Pending Lab Orders"
            value={stats?.pendingLabOrders || 0}
            icon={<Science />}
            color="#f093fb"
            trend={3.8}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Outstanding Balance"
            value={`$${stats?.outstandingBalance?.toLocaleString() || 0}`}
            icon={<AttachMoney />}
            color="#4facfe"
            trend={-1.5}
          />
        </Grid>
      </Grid>

      <Grid container spacing={3}>
        {/* Appointment Trends */}
        <Grid item xs={12} md={8}>
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.2 }}
          >
            <Paper elevation={3} sx={{ p: 3, height: '400px' }}>
              <Typography variant="h6" fontWeight="bold" gutterBottom>
                Appointment Trends (Last 7 Days)
              </Typography>
              <ResponsiveContainer width="100%" height="90%">
                <LineChart data={appointmentStats?.byDate || []}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis
                    dataKey="date"
                    tickFormatter={(date) => new Date(date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}
                  />
                  <YAxis />
                  <Tooltip
                    labelFormatter={(date) => new Date(date).toLocaleDateString()}
                  />
                  <Legend />
                  <Line
                    type="monotone"
                    dataKey="count"
                    stroke="#667eea"
                    strokeWidth={3}
                    dot={{ r: 5 }}
                    activeDot={{ r: 8 }}
                  />
                </LineChart>
              </ResponsiveContainer>
            </Paper>
          </motion.div>
        </Grid>

        {/* Appointment Status Distribution */}
        <Grid item xs={12} md={4}>
          <motion.div
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.3 }}
          >
            <Paper elevation={3} sx={{ p: 3, height: '400px' }}>
              <Typography variant="h6" fontWeight="bold" gutterBottom>
                Appointment Status
              </Typography>
              <ResponsiveContainer width="100%" height="85%">
                <PieChart>
                  <Pie
                    data={appointmentStats?.byStatus || []}
                    dataKey="count"
                    nameKey="status"
                    cx="50%"
                    cy="50%"
                    outerRadius={80}
                    label
                  >
                    {(appointmentStats?.byStatus || []).map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            </Paper>
          </motion.div>
        </Grid>

        {/* Lab Statistics */}
        <Grid item xs={12} md={6}>
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.4 }}
          >
            <Paper elevation={3} sx={{ p: 3 }}>
              <Typography variant="h6" fontWeight="bold" gutterBottom>
                Laboratory Status
              </Typography>
              <Grid container spacing={2} mt={1}>
                <Grid item xs={6}>
                  <Box textAlign="center" p={2}>
                    <Typography variant="h3" color="#667eea" fontWeight="bold">
                      {labStats?.pending || 0}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Pending
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={6}>
                  <Box textAlign="center" p={2}>
                    <Typography variant="h3" color="#764ba2" fontWeight="bold">
                      {labStats?.inProgress || 0}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      In Progress
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12}>
                  <Box textAlign="center" p={2}>
                    <Typography variant="h3" color="#4facfe" fontWeight="bold">
                      {labStats?.completed || 0}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Completed
                    </Typography>
                  </Box>
                </Grid>
              </Grid>
            </Paper>
          </motion.div>
        </Grid>

        {/* Billing Summary */}
        <Grid item xs={12} md={6}>
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.5 }}
          >
            <Paper elevation={3} sx={{ p: 3 }}>
              <Typography variant="h6" fontWeight="bold" gutterBottom>
                Billing Summary (Last 30 Days)
              </Typography>
              <Box mt={3}>
                <Box display="flex" justifyContent="space-between" mb={2}>
                  <Typography variant="body2" color="text.secondary">
                    Total Billed
                  </Typography>
                  <Typography variant="h6" fontWeight="bold" color="#667eea">
                    ${billingStats?.totalBilled?.toLocaleString() || 0}
                  </Typography>
                </Box>
                <Box display="flex" justifyContent="space-between" mb={2}>
                  <Typography variant="body2" color="text.secondary">
                    Total Paid
                  </Typography>
                  <Typography variant="h6" fontWeight="bold" color="#4facfe">
                    ${billingStats?.totalPaid?.toLocaleString() || 0}
                  </Typography>
                </Box>
                <Box display="flex" justifyContent="space-between" mb={2}>
                  <Typography variant="body2" color="text.secondary">
                    Outstanding
                  </Typography>
                  <Typography variant="h6" fontWeight="bold" color="#f093fb">
                    ${billingStats?.totalOutstanding?.toLocaleString() || 0}
                  </Typography>
                </Box>
                <Box display="flex" justifyContent="space-between" mt={3} pt={2} borderTop="1px solid #e0e0e0">
                  <Typography variant="body2" color="text.secondary">
                    Collection Rate
                  </Typography>
                  <Chip
                    label={`${billingStats?.collectionRate?.toFixed(1) || 0}%`}
                    color="success"
                    size="small"
                  />
                </Box>
              </Box>
            </Paper>
          </motion.div>
        </Grid>

        {/* Recent Activity */}
        <Grid item xs={12}>
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.6 }}
          >
            <Paper elevation={3} sx={{ p: 3 }}>
              <Typography variant="h6" fontWeight="bold" gutterBottom>
                Recent Activity (Last 24 Hours)
              </Typography>
              <Grid container spacing={2} mt={1}>
                {activity.recentEncounters?.slice(0, 5).map((encounter, index) => (
                  <Grid item xs={12} key={index}>
                    <Box
                      display="flex"
                      alignItems="center"
                      p={2}
                      sx={{
                        background: '#f5f5f5',
                        borderRadius: 2,
                        '&:hover': { background: '#e0e0e0' },
                      }}
                    >
                      <LocalHospital sx={{ color: '#667eea', mr: 2 }} />
                      <Box flexGrow={1}>
                        <Typography variant="body1" fontWeight="bold">
                          {encounter.patientName}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          Encounter with {encounter.providerName || 'N/A'} -{' '}
                          {new Date(encounter.date).toLocaleString()}
                        </Typography>
                      </Box>
                      <Chip label={encounter.status} size="small" color="primary" />
                    </Box>
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

export default DashboardHome;