import { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { formatDateLong } from '../utils/dateUtils';
import { API_CONFIG } from '../config/api';
import '../styles/ViewTicket.css';

// Ticket Status Enum
const TicketStatus = {
  New: 0,
  InProgress: 1,
  Resolved: 2,
  Closed: 3
} as const;

type TicketStatus = typeof TicketStatus[keyof typeof TicketStatus];

// Ticket Interface
interface Ticket {
  id: string;
  name: string;
  email: string;
  description: string;
  summary: string;
  imageUrl: string;
  status: TicketStatus;
  resolution: string;
  createdAt: string;
  updatedAt: string;
}

// Error types for better error handling
type ErrorType = 'not-found' | 'server-error' | 'network-error' | 'invalid-id' | 'forbidden' | null;

const ViewTicket: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [ticket, setTicket] = useState<Ticket | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [errorType, setErrorType] = useState<ErrorType>(null);

  // Fetch ticket data from API
  const fetchTicket = async (ticketId: string) => {
    try {
      setLoading(true);
      setError(null);
      setErrorType(null);

      const response = await fetch(`${API_CONFIG.baseUrl}/api/tickets/${ticketId}`, {
        credentials: 'include' // Include cookies for authentication
      });

      // Handle authentication errors
      if (response.status === 401) {
        // Unauthorized - redirect to login
        navigate('/');
        return;
      }

      if (response.status === 403) {
        setError('You do not have permission to view this ticket.');
        setErrorType('forbidden');
        setLoading(false);
        return;
      }

      if (response.status === 404) {
        setError('The ticket you\'re looking for doesn\'t exist or has been removed.');
        setErrorType('not-found');
        setLoading(false);
        return;
      }

      if (response.status === 500) {
        setError('An error occurred while loading the ticket. Please try again later.');
        setErrorType('server-error');
        setLoading(false);
        return;
      }

      if (!response.ok) {
        setError('An error occurred while loading the ticket. Please try again later.');
        setErrorType('server-error');
        setLoading(false);
        return;
      }

      const data: Ticket = await response.json();
      setTicket(data);
      setLoading(false);
    } catch (err) {
      // Network error or other fetch errors
      setError('Unable to connect to the server. Please check your internet connection.');
      setErrorType('network-error');
      setLoading(false);
    }
  };

  // Retry function for recoverable errors
  const handleRetry = () => {
    if (id) {
      fetchTicket(id);
    }
  };

  useEffect(() => {
    if (id) {
      fetchTicket(id);
    } else {
      setError('The ticket ID format is invalid.');
      setErrorType('invalid-id');
      setLoading(false);
    }
  }, [id]);

  if (loading) {
    return (
      <div className="view-ticket-container">
        <div className="loading-state" role="status" aria-live="polite">
          <div className="loading-spinner" aria-hidden="true">
            <div className="spinner"></div>
          </div>
          <p className="loading-message" aria-label="Loading ticket details, please wait">Loading ticket...</p>
        </div>
      </div>
    );
  }

  if (error) {
    const isRecoverableError = errorType === 'server-error' || errorType === 'network-error';
    
    return (
      <div className="view-ticket-container">
        <div className="error-state" role="alert" aria-live="assertive">
          <div className="error-icon" aria-hidden="true">⚠️</div>
          <h2 className="error-title">
            {errorType === 'not-found' && 'Ticket Not Found'}
            {errorType === 'server-error' && 'Unable to Load Ticket'}
            {errorType === 'network-error' && 'Connection Error'}
            {errorType === 'invalid-id' && 'Invalid Ticket ID'}
            {errorType === 'forbidden' && 'Access Denied'}
          </h2>
          <p className="error-message">{error}</p>
          <nav className="error-actions" aria-label="Error recovery options">
            {isRecoverableError && (
              <button 
                onClick={handleRetry} 
                className="retry-button"
                aria-label="Retry loading ticket"
              >
                Retry
              </button>
            )}
            <Link to="/tickets" className="back-link" aria-label="Navigate back to tickets list">
              Back to Tickets
            </Link>
            <Link to="/" className="back-link" aria-label="Navigate to home page">
              Go to Home
            </Link>
          </nav>
        </div>
      </div>
    );
  }

  if (!ticket) {
    return (
      <div className="view-ticket-container">
        <div className="error-state" role="alert" aria-live="assertive">
          <div className="error-icon" aria-hidden="true">⚠️</div>
          <h2 className="error-title">Ticket Not Found</h2>
          <p className="error-message">The ticket you're looking for doesn't exist or has been removed.</p>
          <nav className="error-actions" aria-label="Navigation options">
            <Link to="/tickets" className="back-link" aria-label="Navigate back to tickets list">
              Back to Tickets
            </Link>
            <Link to="/" className="back-link" aria-label="Navigate to home page">
              Go to Home
            </Link>
          </nav>
        </div>
      </div>
    );
  }

  // Helper function to get status label
  const getStatusLabel = (status: TicketStatus): string => {
    const statusLabels = {
      [TicketStatus.New]: 'New',
      [TicketStatus.InProgress]: 'In Progress',
      [TicketStatus.Resolved]: 'Resolved',
      [TicketStatus.Closed]: 'Closed'
    };
    return statusLabels[status];
  };

  // Helper function to get status color CSS class
  const getStatusColor = (status: TicketStatus): string => {
    const statusColors = {
      [TicketStatus.New]: 'status-new',
      [TicketStatus.InProgress]: 'status-in-progress',
      [TicketStatus.Resolved]: 'status-resolved',
      [TicketStatus.Closed]: 'status-closed'
    };
    return statusColors[status];
  };



  return (
    <div className="view-ticket-container">
      <article className="view-ticket-card" aria-label="Ticket details">
        {/* Header Section */}
        <header className="ticket-header">
          <h1>Ticket Details</h1>
          <div className="ticket-id-status">
            <span className="ticket-id" aria-label={`Ticket ID ${ticket.id}`}>ID: {ticket.id}</span>
            <span 
              className={`status-badge ${getStatusColor(ticket.status)}`}
              role="status"
              aria-label={`Ticket status: ${getStatusLabel(ticket.status)}`}
            >
              {getStatusLabel(ticket.status)}
            </span>
          </div>
        </header>

        {/* Customer Details Section */}
        <section className="ticket-section customer-details" aria-labelledby="customer-heading">
          <h2 id="customer-heading">Customer Details</h2>
          <div className="section-content">
            <div className="detail-row">
              <span className="detail-label">Name:</span>
              <span className="detail-value">
                {ticket.name && ticket.name.trim() !== '' 
                  ? ticket.name 
                  : <span className="placeholder-text">Not provided</span>
                }
              </span>
            </div>
            <div className="detail-row">
              <span className="detail-label">Email:</span>
              <span className="detail-value">
                {ticket.email && ticket.email.trim() !== '' 
                  ? ticket.email 
                  : <span className="placeholder-text">Not provided</span>
                }
              </span>
            </div>
          </div>
        </section>

        {/* Issue Details Section */}
        <section className="ticket-section issue-details" aria-labelledby="issue-heading">
          <h2 id="issue-heading">Issue Details</h2>
          <div className="section-content">
            <div className="detail-row">
              <span className="detail-label">Summary:</span>
              <span className="detail-value">{ticket.summary}</span>
            </div>
            <div className="detail-row description-row">
              <span className="detail-label">Description:</span>
              <p className="detail-value description-text">
                {ticket.description && ticket.description.trim() !== '' 
                  ? ticket.description 
                  : <span className="placeholder-text">No description provided</span>
                }
              </p>
            </div>
          </div>
        </section>

        {/* Image Section (conditional - only show if imageUrl exists and is not empty) */}
        {ticket.imageUrl && ticket.imageUrl.trim() !== '' && (
          <section className="ticket-section image-section" aria-labelledby="image-heading">
            <h2 id="image-heading">Attached Image</h2>
            <div className="section-content">
              <img 
                src={ticket.imageUrl} 
                alt={`Attachment for ticket ${ticket.summary || ticket.id}: Visual reference provided by ${ticket.name || 'customer'}`}
                className="ticket-image"
              />
            </div>
          </section>
        )}

        {/* Resolution Section (conditional - only show if resolution exists and is not empty) */}
        {ticket.resolution && ticket.resolution.trim() !== '' && (
          <section className="ticket-section resolution-section" aria-labelledby="resolution-heading">
            <h2 id="resolution-heading">Resolution</h2>
            <div className="section-content">
              <p className="detail-value resolution-text">
                {ticket.resolution}
              </p>
            </div>
          </section>
        )}

        {/* Metadata Section */}
        <section className="ticket-section metadata-section" aria-labelledby="metadata-heading">
          <h2 id="metadata-heading">Ticket Information</h2>
          <div className="section-content">
            <div className="detail-row">
              <span className="detail-label">Created:</span>
              <span className="detail-value">{formatDateLong(ticket.createdAt)}</span>
            </div>
            <div className="detail-row">
              <span className="detail-label">Last Updated:</span>
              <span className="detail-value">{formatDateLong(ticket.updatedAt)}</span>
            </div>
          </div>
        </section>
      </article>
    </div>
  );
};

export default ViewTicket;
