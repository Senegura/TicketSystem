import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { API_ENDPOINTS } from '../config';
import '../styles/NewTicket.css';

const NewTicket: React.FC = () => {
  const [fullName, setFullName] = useState<string>('');
  const [email, setEmail] = useState<string>('');
  const [issueDescription, setIssueDescription] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [success, setSuccess] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const navigate = useNavigate();

  const handleFullNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFullName(e.target.value);
    setError(''); // Clear error when user types
    setSuccess(''); // Clear success when user types
  };

  const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEmail(e.target.value);
    setError(''); // Clear error when user types
    setSuccess(''); // Clear success when user types
  };

  const handleIssueDescriptionChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    setIssueDescription(e.target.value);
    setError(''); // Clear error when user types
    setSuccess(''); // Clear success when user types
  };

  const handleBackToLogin = () => {
    navigate('/');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validate all required fields
    if (!fullName.trim() || !email.trim() || !issueDescription.trim()) {
      setError('All fields are required');
      setSuccess('');
      return;
    }

    setIsLoading(true);
    setError('');
    setSuccess('');

    try {
      // Create FormData object
      const formData = new FormData();
      formData.append('fullName', fullName);
      formData.append('email', email);
      formData.append('issueDescription', issueDescription);

      const response = await fetch(API_ENDPOINTS.TICKETS.CREATE, {
        method: 'POST',
        body: formData,
      });

      if (response.ok) {
        // Handle 200 OK response
        setSuccess('Ticket submitted successfully!');
        setError('');
        // Clear form fields
        setFullName('');
        setEmail('');
        setIssueDescription('');
      } else if (response.status === 400) {
        // Handle 400 Bad Request
        try {
          const errorData = await response.json();
          setError(errorData.message || 'Invalid input. Please check your fields and try again.');
        } catch {
          setError('Invalid input. Please check your fields and try again.');
        }
        setSuccess('');
      } else if (response.status === 500) {
        // Handle 500 Internal Server Error
        setError('An error occurred while submitting your ticket. Please try again.');
        setSuccess('');
      } else {
        // Handle other error responses
        setError('An error occurred while submitting your ticket. Please try again.');
        setSuccess('');
      }
    } catch (err) {
      // Handle network errors
      setError('Unable to connect to the server. Please check your connection and try again.');
      setSuccess('');
      console.error('Ticket submission error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="new-ticket-container">
      <div className="new-ticket-wrapper">
        <div className="new-ticket-card">
          <h1 className="new-ticket-heading text-center">Submit a Ticket</h1>
          {error && (
            <div className="alert alert-danger" role="alert">
              {error}
            </div>
          )}
          {success && (
            <div className="alert alert-success" role="alert">
              {success}
            </div>
          )}
          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label htmlFor="fullName" className="form-label">
                Full Name
              </label>
              <input
                type="text"
                className="form-control"
                id="fullName"
                name="fullName"
                value={fullName}
                onChange={handleFullNameChange}
              />
            </div>
            <div className="mb-3">
              <label htmlFor="email" className="form-label">
                Email
              </label>
              <input
                type="email"
                className="form-control"
                id="email"
                name="email"
                value={email}
                onChange={handleEmailChange}
              />
            </div>
            <div className="mb-3">
              <label htmlFor="issueDescription" className="form-label">
                Issue Description
              </label>
              <textarea
                className="form-control"
                id="issueDescription"
                name="issueDescription"
                value={issueDescription}
                onChange={handleIssueDescriptionChange}
                rows={5}
              />
            </div>
            <button 
              type="submit" 
              className="btn new-ticket-button w-100"
              disabled={isLoading}
            >
              {isLoading ? 'Submitting...' : 'Submit Ticket'}
            </button>
          </form>
          <div className="back-to-login-container text-center mt-3">
            <span>Already have an account? </span>
            <button
              type="button"
              className="back-to-login-link"
              onClick={handleBackToLogin}
            >
              Back to Login
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default NewTicket;
