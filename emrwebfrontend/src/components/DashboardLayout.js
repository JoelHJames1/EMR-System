import React, { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Box,
  Drawer,
  AppBar,
  Toolbar,
  List,
  Typography,
  Divider,
  IconButton,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Avatar,
  Menu,
  MenuItem,
  Chip,
  Badge,
} from '@mui/material';
import {
  Menu as MenuIcon,
  Dashboard as DashboardIcon,
  People,
  CalendarToday,
  LocalHospital,
  Medication,
  Science,
  AttachMoney,
  Settings,
  Logout,
  Notifications,
  AccountCircle,
  MedicalServices,
  Assignment,
} from '@mui/icons-material';
import UserContext from './UserContext';
import EnhancedDashboard from './Dashboard/EnhancedDashboard';
import PatientManagement from './Dashboard/PatientManagement';
import AppointmentManagement from './Dashboard/AppointmentManagement';
import PrescriptionManagement from './Dashboard/PrescriptionManagement';
import LabOrderManagement from './Dashboard/LabOrderManagement';
import EncounterManagement from './Dashboard/EncounterManagement';
import BillingManagement from './Dashboard/BillingManagement';
import VitalsManagement from './Dashboard/VitalsManagement';
import AllergyImmunizationManagement from './Dashboard/AllergyImmunizationManagement';

const drawerWidth = 280;

const DashboardLayout = () => {
  const [mobileOpen, setMobileOpen] = useState(false);
  const [selectedPage, setSelectedPage] = useState('dashboard');
  const [anchorEl, setAnchorEl] = useState(null);
  const navigate = useNavigate();
  const user = JSON.parse(localStorage.getItem('user') || '{}');

  const menuItems = [
    { id: 'dashboard', label: 'Dashboard', icon: <DashboardIcon />, color: '#667eea' },
    { id: 'patients', label: 'Patients', icon: <People />, color: '#764ba2' },
    { id: 'appointments', label: 'Appointments', icon: <CalendarToday />, color: '#f093fb' },
    { id: 'encounters', label: 'Encounters', icon: <LocalHospital />, color: '#4facfe' },
    { id: 'prescriptions', label: 'Prescriptions', icon: <Medication />, color: '#43e97b' },
    { id: 'lab-orders', label: 'Lab Orders', icon: <Science />, color: '#fa709a' },
    { id: 'vitals', label: 'Vitals', icon: <MedicalServices />, color: '#30cfd0' },
    { id: 'allergies', label: 'Allergies & Vaccines', icon: <Assignment />, color: '#f44336' },
    { id: 'billing', label: 'Billing', icon: <AttachMoney />, color: '#ffc107' },
  ];

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleMenuClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    navigate('/login');
  };

  const renderPageContent = () => {
    switch (selectedPage) {
      case 'dashboard':
        return <EnhancedDashboard />;
      case 'patients':
        return <PatientManagement />;
      case 'appointments':
        return <AppointmentManagement />;
      case 'encounters':
        return <EncounterManagement />;
      case 'prescriptions':
        return <PrescriptionManagement />;
      case 'lab-orders':
        return <LabOrderManagement />;
      case 'vitals':
        return <VitalsManagement />;
      case 'allergies':
        return <AllergyImmunizationManagement />;
      case 'billing':
        return <BillingManagement />;
      default:
        return <EnhancedDashboard />;
    }
  };

  const drawer = (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      {/* Logo Section */}
      <Box
        sx={{
          p: 3,
          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
          color: 'white',
        }}
      >
        <Box display="flex" alignItems="center" mb={2}>
          <LocalHospital sx={{ fontSize: 40, mr: 2 }} />
          <Typography variant="h5" fontWeight="bold">
            EMR System
          </Typography>
        </Box>
        <Typography variant="caption">
          Healthcare Management Platform
        </Typography>
      </Box>

      {/* User Profile Section */}
      <Box sx={{ p: 2, bgcolor: '#f5f5f5' }}>
        <Box display="flex" alignItems="center">
          <Avatar
            sx={{
              bgcolor: '#667eea',
              width: 48,
              height: 48,
              mr: 2,
            }}
          >
            {user.firstName?.[0]}{user.lastName?.[0]}
          </Avatar>
          <Box flexGrow={1}>
            <Typography variant="body1" fontWeight="bold">
              {user.firstName} {user.lastName}
            </Typography>
            <Chip
              label={user.roles?.[0] || 'User'}
              size="small"
              color="primary"
              sx={{ mt: 0.5 }}
            />
          </Box>
        </Box>
      </Box>

      <Divider />

      {/* Navigation Menu */}
      <List sx={{ flexGrow: 1, px: 1, py: 2 }}>
        {menuItems.map((item) => (
          <motion.div
            key={item.id}
            whileHover={{ x: 5 }}
            transition={{ type: 'spring', stiffness: 300 }}
          >
            <ListItem disablePadding sx={{ mb: 0.5 }}>
              <ListItemButton
                selected={selectedPage === item.id}
                onClick={() => setSelectedPage(item.id)}
                sx={{
                  borderRadius: 2,
                  '&.Mui-selected': {
                    background: `linear-gradient(135deg, ${item.color}15 0%, ${item.color}05 100%)`,
                    borderLeft: `4px solid ${item.color}`,
                    '&:hover': {
                      background: `linear-gradient(135deg, ${item.color}25 0%, ${item.color}15 100%)`,
                    },
                  },
                  '&:hover': {
                    background: `${item.color}10`,
                  },
                }}
              >
                <ListItemIcon sx={{ color: item.color }}>
                  {item.icon}
                </ListItemIcon>
                <ListItemText
                  primary={item.label}
                  primaryTypographyProps={{
                    fontWeight: selectedPage === item.id ? 'bold' : 'normal',
                  }}
                />
              </ListItemButton>
            </ListItem>
          </motion.div>
        ))}
      </List>

      <Divider />

      {/* Bottom Menu */}
      <List sx={{ px: 1, py: 2 }}>
        <ListItem disablePadding>
          <ListItemButton onClick={handleLogout} sx={{ borderRadius: 2 }}>
            <ListItemIcon>
              <Logout />
            </ListItemIcon>
            <ListItemText primary="Logout" />
          </ListItemButton>
        </ListItem>
      </List>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      {/* App Bar */}
      <AppBar
        position="fixed"
        sx={{
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          ml: { sm: `${drawerWidth}px` },
          background: 'white',
          color: '#333',
          boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{ mr: 2, display: { sm: 'none' } }}
          >
            <MenuIcon />
          </IconButton>

          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1, fontWeight: 'bold' }}>
            {menuItems.find((item) => item.id === selectedPage)?.label || 'Dashboard'}
          </Typography>

          <Box display="flex" alignItems="center" gap={2}>
            <IconButton color="inherit">
              <Badge badgeContent={4} color="error">
                <Notifications />
              </Badge>
            </IconButton>

            <IconButton onClick={handleMenuClick} color="inherit">
              <Avatar sx={{ width: 32, height: 32, bgcolor: '#667eea' }}>
                {user.firstName?.[0]}
              </Avatar>
            </IconButton>

            <Menu
              anchorEl={anchorEl}
              open={Boolean(anchorEl)}
              onClose={handleMenuClose}
              anchorOrigin={{
                vertical: 'bottom',
                horizontal: 'right',
              }}
              transformOrigin={{
                vertical: 'top',
                horizontal: 'right',
              }}
            >
              <MenuItem onClick={handleMenuClose}>
                <ListItemIcon>
                  <AccountCircle fontSize="small" />
                </ListItemIcon>
                Profile
              </MenuItem>
              <MenuItem onClick={handleMenuClose}>
                <ListItemIcon>
                  <Settings fontSize="small" />
                </ListItemIcon>
                Settings
              </MenuItem>
              <Divider />
              <MenuItem onClick={handleLogout}>
                <ListItemIcon>
                  <Logout fontSize="small" />
                </ListItemIcon>
                Logout
              </MenuItem>
            </Menu>
          </Box>
        </Toolbar>
      </AppBar>

      {/* Sidebar */}
      <Box
        component="nav"
        sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
      >
        {/* Mobile drawer */}
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{
            keepMounted: true,
          }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>

        {/* Desktop drawer */}
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>

      {/* Main Content */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          minHeight: '100vh',
          bgcolor: '#f8f9fa',
        }}
      >
        <Toolbar />
        <AnimatePresence mode="wait">
          <motion.div
            key={selectedPage}
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            exit={{ opacity: 0, x: -20 }}
            transition={{ duration: 0.3 }}
          >
            {renderPageContent()}
          </motion.div>
        </AnimatePresence>
      </Box>
    </Box>
  );
};

export default DashboardLayout;