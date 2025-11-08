import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { API_ENDPOINTS } from '../config';
import '../styles/Login.css';

// API Integration Types
type UserType = 'Customer' | 'User' | 'Admin';

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  token: string;
  userId: number;
  userType: UserType;
}

const Login: React.FC = () => {
  const [username, setUsername] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const navigate = useNavigate();

  const handleUsernameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setUsername(e.target.value);
    setError(''); // Clear error when user types
  };

  const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPassword(e.target.value);
    setError(''); // Clear error when user types
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validate username and password are not empty
    if (!username.trim() || !password.trim()) {
      setError('Username and password are required');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      const response = await fetch(API_ENDPOINTS.AUTH.LOGIN, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include', // Important: allows cookies to be sent/received
        body: JSON.stringify({ 
          username, 
          password 
        } as LoginRequest),
      });

      if (response.ok) {
        // Handle 200 OK response
        const data: LoginResponse = await response.json();
        // Extract token, userId, and userType from response
        console.log('Login successful:', { token: data.token, userId: data.userId, userType: data.userType });
        
        // Clear error message on successful authentication
        setError('');
        
        // Navigate based on UserType
        switch (data.userType) {
          case 'Customer':
            navigate('/create-ticket');
            break;
          case 'User':
          case 'Admin':
            navigate('/tickets');
            break;
          default:
            setError('Unknown user type');
        }
      } else if (response.status === 400) {
        // Handle 400 Bad Request
        setError('Username and password are required');
      } else if (response.status === 401) {
        // Handle 401 Unauthorized
        setError('Invalid username or password');
      } else {
        // Handle other error responses
        setError('An error occurred during login. Please try again.');
      }
    } catch (err) {
      // Handle network errors
      setError('An error occurred during login. Please try again.');
      console.error('Login error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleRegisterClick = () => {
    navigate('/register');
  };

  return (
    <div className="login-container">
      <div className="login-wrapper">
        <div className="login-card">
              <h1 className="login-heading text-center">Login</h1>
              {error && (
                <div className="alert alert-danger" role="alert">
                  {error}
                </div>
              )}
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="username" className="form-label">
                    Username
                  </label>
                  <input
                    type="text"
                    className="form-control"
                    id="username"
                    name="username"
                    value={username}
                    onChange={handleUsernameChange}
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
                    onChange={handlePasswordChange}
                  />
                </div>
                <button 
                  type="submit" 
                  className="btn login-button w-100"
                  disabled={isLoading}
                >
                  {isLoading ? 'Loading...' : 'Login'}
                </button>
              </form>
              <div className="register-link-container text-center mt-3">
                <span>Need an account? </span>
                <button
                  type="button"
                  className="register-link"
                  onClick={handleRegisterClick}
                >
                  Register
                </button>
              </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
