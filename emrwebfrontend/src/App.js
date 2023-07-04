import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import LoginForm from './components/LoginForm';
import RegisterForm from './components/RegisterForm';
import Dashboard from './components/Dashboard';
import UserContext from './components/UserContext';  // Update the import statement

const App = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    // Get user from localStorage
    let user = localStorage.getItem('user');

    // Parse only if 'user' is a valid JSON string
    user = (user && user !== "undefined") ? JSON.parse(user) : {};

    return (
        <UserContext.Provider value={user}>
            <Router>
                <Routes>
                    <Route path="/" element={<Navigate to="/login" replace />} />
                    <Route path="/login" element={<LoginForm />} />
                    <Route path="/register" element={<RegisterForm />} />
                    <Route path="/dashboard" element={isAuthenticated ? <Dashboard /> : <Navigate to="/login" replace />} />
                </Routes>
            </Router>
        </UserContext.Provider>
    );
};

export default App;
