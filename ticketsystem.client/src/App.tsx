import { Routes, Route } from 'react-router-dom';
import Login from './components/Login';
import NewTicket from './components/NewTicket';
import ViewTicket from './components/ViewTicket';
import ViewTickets from './components/ViewTickets';
import AdminPanel from './components/AdminPanel';
import './App.css';

function App() {
    return (
        <Routes>
            <Route path="/" element={<Login />} />
            <Route path="/login" element={<Login />} />
            <Route path="/new-ticket" element={<NewTicket />} />
            <Route path="/register" element={
                <div className="container mt-5">
                    <div className="text-center">
                        <h1>Register</h1>
                        <p>Registration page coming soon...</p>
                    </div>
                </div>
            } />
            <Route path="/create-ticket" element={
                <div className="container mt-5">
                    <div className="text-center">
                        <h1>Create Ticket</h1>
                        <p>Coming Soon</p>
                    </div>
                </div>
            } />
            <Route path="/admin" element={<AdminPanel />} />
            <Route path="/tickets" element={<ViewTickets />} />
            <Route path="/tickets/:id" element={<ViewTicket />} />
        </Routes>
    );
}

export default App;