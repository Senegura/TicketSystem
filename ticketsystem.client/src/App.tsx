import { Routes, Route } from 'react-router-dom';
import Login from './components/Login';
import './App.css';

function App() {
    return (
        <Routes>
            <Route path="/" element={<Login />} />
            <Route path="/register" element={
                <div className="container mt-5">
                    <div className="text-center">
                        <h1>Register</h1>
                        <p>Registration page coming soon...</p>
                    </div>
                </div>
            } />
        </Routes>
    );
}

export default App;