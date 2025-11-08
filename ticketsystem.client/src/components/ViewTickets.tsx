import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { formatDateShort } from '../utils/dateUtils';
import { API_CONFIG } from '../config/api';
import '../styles/ViewTickets.css';

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

const ViewTickets: React.FC = () => {
  // Component state
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [filteredTickets, setFilteredTickets] = useState<Ticket[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [statusFilter, setStatusFilter] = useState<TicketStatus | 'all'>('all');
  const [searchText, setSearchText] = useState<string>('');
  const [filterAnnouncement, setFilterAnnouncement] = useState<string>('');

  const navigate = useNavigate();

  // Fetch tickets from API
  const fetchTickets = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await fetch(`${API_CONFIG.baseUrl}/api/tickets`, {
        credentials: 'include' // Include cookies for authentication
      });

      // Handle authentication errors
      if (response.status === 401) {
        // Unauthorized - redirect to login
        navigate('/login');
        return;
      }

      if (response.status === 403) {
        setError('You do not have permission to view tickets.');
        setLoading(false);
        return;
      }

      if (response.status === 500) {
        setError('An error occurred while loading tickets. Please try again later.');
        setLoading(false);
        return;
      }

      if (!response.ok) {
        setError('An error occurred while loading tickets. Please try again later.');
        setLoading(false);
        return;
      }

      const data: Ticket[] = await response.json();
      setTickets(data);
      setFilteredTickets(data);
      setLoading(false);
    } catch (err) {
      // Network error or other fetch errors
      setError('Unable to connect to the server. Please check your internet connection.');
      setLoading(false);
    }
  };

  // Fetch tickets on component mount
  useEffect(() => {
    fetchTickets();
  }, []);

  // Apply filters to tickets
  const applyFilters = () => {
    let filtered = tickets;

    // Apply status filter
    if (statusFilter !== 'all') {
      filtered = filtered.filter(ticket => ticket.status === statusFilter);
    }

    // Apply text search filter (case-insensitive)
    if (searchText.trim() !== '') {
      const searchLower = searchText.toLowerCase();
      filtered = filtered.filter(ticket => 
        ticket.name.toLowerCase().includes(searchLower) ||
        ticket.description.toLowerCase().includes(searchLower)
      );
    }

    setFilteredTickets(filtered);
  };

  // Handle status filter change
  const handleStatusFilterChange = (status: TicketStatus | 'all') => {
    setStatusFilter(status);
    
    // Announce filter change to screen readers
    const statusLabel = status === 'all' ? 'All Statuses' : getStatusLabel(status as TicketStatus);
    setFilterAnnouncement(`Status filter changed to ${statusLabel}`);
  };

  // Handle search text change
  const handleSearchChange = (text: string) => {
    setSearchText(text);
    
    // Announce search change to screen readers
    if (text.trim() === '') {
      setFilterAnnouncement('Search filter cleared');
    } else {
      setFilterAnnouncement(`Searching for ${text}`);
    }
  };

  // Handle ticket row click - navigate to ticket detail view
  const handleTicketClick = (ticketId: string) => {
    navigate(`/tickets/${ticketId}`);
  };

  // Get status label from enum value
  const getStatusLabel = (status: TicketStatus): string => {
    switch (status) {
      case TicketStatus.New:
        return 'New';
      case TicketStatus.InProgress:
        return 'In Progress';
      case TicketStatus.Resolved:
        return 'Resolved';
      case TicketStatus.Closed:
        return 'Closed';
      default:
        return 'Unknown';
    }
  };

  // Get status color class from enum value
  const getStatusColor = (status: TicketStatus): string => {
    switch (status) {
      case TicketStatus.New:
        return 'status-new';
      case TicketStatus.InProgress:
        return 'status-in-progress';
      case TicketStatus.Resolved:
        return 'status-resolved';
      case TicketStatus.Closed:
        return 'status-closed';
      default:
        return '';
    }
  };



  // Apply filters whenever tickets, statusFilter, or searchText changes
  useEffect(() => {
    applyFilters();
  }, [tickets, statusFilter, searchText]);

  // Clear filter announcement after it's been read
  useEffect(() => {
    if (filterAnnouncement) {
      const timer = setTimeout(() => {
        setFilterAnnouncement('');
      }, 1000);
      return () => clearTimeout(timer);
    }
  }, [filterAnnouncement]);

  return (
    <div className="view-tickets-container">
      <div className="view-tickets-card">
        {/* Screen reader announcements for filter changes */}
        <div 
          role="status" 
          aria-live="polite" 
          aria-atomic="true"
          className="sr-only"
        >
          {filterAnnouncement}
        </div>

        <header className="tickets-header">
          <h1>Tickets</h1>
        </header>

        {loading && (
          <div className="loading-state" role="status" aria-live="polite" aria-label="Loading tickets">
            <div className="loading-spinner" aria-hidden="true">
              <div className="spinner"></div>
            </div>
            <p className="loading-message">Loading tickets...</p>
          </div>
        )}

        {error && (
          <div className="error-state" role="alert" aria-live="assertive">
            <div className="error-icon" aria-hidden="true">⚠️</div>
            <h2 className="error-title">Unable to Load Tickets</h2>
            <p className="error-message">{error}</p>
            <button onClick={fetchTickets} className="retry-button">
              Retry
            </button>
          </div>
        )}

        {!loading && !error && tickets.length === 0 && (
          <div className="empty-state">
            <p>No tickets found</p>
          </div>
        )}

        {!loading && !error && tickets.length > 0 && (
          <>
            {/* Filters section */}
            <section className="filters-section" aria-label="Filter tickets">
              <div className="filter-group">
                <label htmlFor="status-filter" className="filter-label">
                  Status:
                </label>
                <select
                  id="status-filter"
                  className="status-filter"
                  value={statusFilter}
                  onChange={(e) => handleStatusFilterChange(
                    e.target.value === 'all' ? 'all' : Number(e.target.value) as TicketStatus
                  )}
                  aria-label="Filter by status"
                >
                  <option value="all">All Statuses</option>
                  <option value={TicketStatus.New}>New</option>
                  <option value={TicketStatus.InProgress}>In Progress</option>
                  <option value={TicketStatus.Resolved}>Resolved</option>
                  <option value={TicketStatus.Closed}>Closed</option>
                </select>
              </div>

              <div className="filter-group">
                <label htmlFor="search-filter" className="filter-label">
                  Search:
                </label>
                <input
                  id="search-filter"
                  type="text"
                  className="search-filter"
                  placeholder="Search by name or description..."
                  value={searchText}
                  onChange={(e) => handleSearchChange(e.target.value)}
                  aria-label="Search tickets by name or description"
                />
              </div>
            </section>

            {/* Table section */}
            <section className="table-section">
              {filteredTickets.length === 0 ? (
                <div className="empty-filter-state">
                  <p>No tickets match your filters</p>
                  <p className="empty-filter-hint">Try adjusting your filters to see more results</p>
                </div>
              ) : (
                <table className="tickets-table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Name</th>
                      <th>Status</th>
                      <th>Summary</th>
                      <th>Created Date</th>
                    </tr>
                  </thead>
                  <tbody>
                    {filteredTickets.map((ticket) => (
                      <tr 
                        key={ticket.id}
                        onClick={() => handleTicketClick(ticket.id)}
                        className="ticket-row"
                        role="button"
                        tabIndex={0}
                        onKeyDown={(e) => {
                          if (e.key === 'Enter' || e.key === ' ') {
                            e.preventDefault();
                            handleTicketClick(ticket.id);
                          }
                        }}
                        aria-label={`View ticket ${ticket.name}, status ${getStatusLabel(ticket.status)}, created ${formatDateShort(ticket.createdAt)}`}
                      >
                        <td className="ticket-id" data-label="ID">{ticket.id.substring(0, 8)}...</td>
                        <td className="ticket-name" data-label="Name">{ticket.name}</td>
                        <td className="ticket-status" data-label="Status">
                          <span 
                            className={`status-badge ${getStatusColor(ticket.status)}`}
                            aria-label={`Status: ${getStatusLabel(ticket.status)}`}
                          >
                            {getStatusLabel(ticket.status)}
                          </span>
                        </td>
                        <td className="ticket-summary" data-label="Summary">{ticket.summary}</td>
                        <td className="ticket-date" data-label="Created">{formatDateShort(ticket.createdAt)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </section>
          </>
        )}
      </div>
    </div>
  );
};

export default ViewTickets;
