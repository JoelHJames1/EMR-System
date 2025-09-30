import React, { useState, useContext } from 'react';
import { QueryClient, QueryClientProvider } from 'react-query';
import Drawer from '@mui/material/Drawer';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import UserGrid from './UserGrid';
import Box from '@mui/material/Box';
import axios from 'axios';
import UserContext from './UserContext';
import Avatar from '@mui/material/Avatar';

const queryClient = new QueryClient();

const fetchUsers = async () => {
    const res = await axios.get('https://localhost:7099/api/User/GetUsers/');
    return res.data;
};

const updateUser = async (user) => {
    const res = await axios.put(`https://localhost:7099/api/User/UpdateUser/${user.id}`, user);
    return res.data;
};

const deleteUser = async (id) => {
    const res = await axios.delete(`https://localhost:7099/api/User/DeleteUser/${id}`);
    return res.data;
};

const drawerWidth = 240;

const Dashboard = () => {
    const [selectedPage, setSelectedPage] = useState('Users');
    const pages = ['Users', 'Doctor', 'Nurse', 'Prescriptions', 'Settings'];
    const user = useContext(UserContext);

    return (
        <QueryClientProvider client={queryClient}>
            <Box sx={{ display: 'flex' }}>
                <Drawer
                    sx={{
                        width: drawerWidth,
                        flexShrink: 0,
                        '& .MuiDrawer-paper': {
                            width: drawerWidth,
                            boxSizing: 'border-box',
                        },
                    }}
                    variant="permanent"
                >
                    <Box sx={{ display: 'flex', alignItems: 'center', p: 2 }}>
                        <Avatar alt={user.UserName} src={user.PictureUrl} />
                        <ListItemText primary={`Welcome, ${user.firstName} ${user.lastName}!`} />
                    </Box>
                    <List>
                        {pages.map((page) => (
                            <ListItem button key={page} onClick={() => setSelectedPage(page)}>
                                <ListItemText primary={page} />
                            </ListItem>
                        ))}
                    </List>
                </Drawer>
                <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
                    {selectedPage === 'Users' && <UserGrid />}
                    {/* Add more pages as needed */}
                </Box>
            </Box>
        </QueryClientProvider>
    );
};

export default Dashboard;
