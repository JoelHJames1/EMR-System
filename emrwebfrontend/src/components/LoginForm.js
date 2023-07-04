import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import logo from "../logo/logo.png";
import { Toast } from 'react-bootstrap';

const LoginForm = () => {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [showToast, setShowToast] = useState(false);

    const handleSubmit = async (event) => {
        event.preventDefault();

        try {
            const user = {
                Email: email,
                password,
            };

            console.log("Login user:", user);

            const response = await fetch("https://localhost:7099/api/User/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(user),
            });

            if (response.ok) {
                const data = await response.json();
                console.log(data); // Add this line
                localStorage.setItem("token", JSON.stringify(data.token));
                localStorage.setItem("user", JSON.stringify(data.user));
                navigate("/dashboard");
            } else if (response.status === 401) {
                setShowToast(true);
            } else {
                const errorData = await response.json();
                console.error('Server responded with error:', errorData);
                console.error('Validation errors:', errorData.errors);
                throw new Error("Failed to login");
            }
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <div className="container d-flex align-items-center justify-content-center vh-100">
            <div className="card p-4">
                <div className="text-center mb-4">
                    <img
                        src={logo}
                        alt="Logo"
                        className="logo"
                        style={{ width: "500px", height: "400px" }}
                    />
                </div>
                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label htmlFor="email" className="form-label">
                            Email
                        </label>
                        <input
                            type="email"
                            className="form-control"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>
                    <div className="mb-3">
                        <label htmlFor="password" className="form-label">
                            Password
                        </label>
                        <input
                            type="password"
                            className="form-control"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary">
                        Login
                    </button>
                    <p className="mt-3 text-center">
                        Don't have an account?{" "}
                        <Link to="/register">Register a new account</Link>
                    </p>
                </form>
            </div>
            <Toast onClose={() => setShowToast(false)} show={showToast} delay={3000} autohide>
                <Toast.Header>
                    <strong className="mr-auto">Login Error</strong>
                </Toast.Header>
                <Toast.Body>Wrong username or password.</Toast.Body>
            </Toast>
        </div>
    );
};

export default LoginForm;
