import React, { useState } from 'react';
import { useQuery, useMutation } from 'react-query';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Button, TextField } from '@mui/material';
import axios from 'axios';

const fetchUsers = async () => {
    const res = await axios.get('https://localhost:7099/api/User');
    console.log(res.data); // show what the data looks like coming from web api
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

const UserGrid = () => {
    const { data: users, refetch, isLoading, isError } = useQuery('users', fetchUsers);
    const [selectedUser, setSelectedUser] = useState(null);

    const updateUserMutation = useMutation(updateUser, {
        onSuccess: () => {
            refetch();
        },
    });
    const deleteUserMutation = useMutation(deleteUser, {
        onSuccess: () => {
            refetch();
        },
    });

    const handleSelectUser = (user) => {
        setSelectedUser(user);
    };

    const handleUpdateUser = () => {
        updateUserMutation.mutate(selectedUser);
        setSelectedUser(null);
    };

    const handleDeleteUser = (id) => {
        deleteUserMutation.mutate(id);
    };

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        const [field, nestedField] = name.split('.');

        if (nestedField) {
            setSelectedUser({
                ...selectedUser,
                [field]: {
                    ...selectedUser[field],
                    [nestedField]: value,
                },
            });
        } else {
            setSelectedUser({
                ...selectedUser,
                [field]: value,
            });
        }
    };

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (isError) {
        return <div>Error loading data.</div>;
    }

    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>First Name</TableCell>
                        <TableCell>Last Name</TableCell>
                        <TableCell>Email</TableCell>
                        <TableCell>AddressLine</TableCell>
                        <TableCell>City</TableCell>
                        <TableCell>State</TableCell>
                        <TableCell>PostalCode</TableCell>
                        <TableCell>Actions</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {users.map((user) => (
                        <TableRow key={user.id}>
                            <TableCell>{user.firstName}</TableCell>
                            <TableCell>{user.lastName}</TableCell>
                            <TableCell>{user.email}</TableCell>
                            <TableCell>{user.address?.addressLine}</TableCell>
                            <TableCell>{user.address?.city}</TableCell>
                            <TableCell>{user.address?.state}</TableCell>
                            <TableCell>{user.address?.postalCode}</TableCell>
                            <TableCell>
                                <Button variant="contained" color="primary" onClick={() => handleSelectUser(user)}>
                                    Edit
                                </Button>
                                <Button variant="contained" color="secondary" onClick={() => handleDeleteUser(user.id)}>
                                    Delete
                                </Button>
                            </TableCell>
                        </TableRow>
                    ))}
                    {selectedUser && (
                        <TableRow>
                            <TableCell>
                                <TextField name="firstName" value={selectedUser.firstName} onChange={handleInputChange} />
                            </TableCell>
                            <TableCell>
                                <TextField name="lastName" value={selectedUser.lastName} onChange={handleInputChange} />
                            </TableCell>
                            <TableCell>
                                <TextField name="email" value={selectedUser.email} onChange={handleInputChange} />
                            </TableCell>
                            <TableCell>
                                <TextField name="address.addressLine" value={selectedUser.address.addressLine} onChange={handleInputChange} />
                            </TableCell>
                            <TableCell>
                                <TextField name="address.city" value={selectedUser.address.city} onChange={handleInputChange} />
                            </TableCell>
                            <TableCell>
                                <TextField name="address.state" value={selectedUser.address.state} onChange={handleInputChange} />
                            </TableCell>
                            <TableCell>
                                <TextField name="address.postalCode" value={selectedUser.address.postalCode} onChange={handleInputChange} />
                            </TableCell>
                            <TableCell>
                                <Button variant="contained" color="primary" onClick={handleUpdateUser}>
                                    Update
                                </Button>
                            </TableCell>
                        </TableRow>
                    )}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default UserGrid;
