import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { formatDateShort } from '../utils/dateUtils';
import { API_CONFIG } from '../config/api';
import '../styles/AdminPanel.css';

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

const AdminPanel: React.FC = () => {
  // Component state
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [filteredTickets, setFilteredTickets] = useState<Ticket[]>([]);
  const [modifiedTickets, setModifiedTickets] = useState<Map<string, Ticket>>(new Map());
  const [loading, setLoading] = useState<boolean>(true);
  const [saving, setSaving] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveSuccess, setSaveSuccess] = useState<string | null>(null);
  const [statusFilter, setStatusFilter] = useState<TicketStatus | 'all'>('all');
  const [searchText, setSearchText] = useState<string>('');

  const navigate = useNavigate();

  // Handle ticket status change
  const handleTicketStatusChange = (ticketId: string, newStatus: TicketStatus) => {
    // Find the original ticket or get the current modified version
    const currentTicket = getCurrentTicket(ticketId);
    if (!currentTicket) return;

    // Create updated ticket with new status
    const updatedTicket: Ticket = {
      ...currentTicket,
      status: newStatus
    };

    // Add to modified tickets map
    setModifiedTickets(prev => {
      const newMap = new Map(prev);
      newMap.set(ticketId, updatedTicket);
      return newMap;
    });
  };

  // Handle ticket resolution change
  const handleTicketResolutionChange = (ticketId: string, newResolution: string) => {
    // Find the original ticket or get the current modified version
    const currentTicket = getCurrentTicket(ticketId);
    if (!currentTicket) return;

    // Create updated ticket with new resolution
    const updatedTicket: Ticket = {
      ...currentTicket,
      resolution: newResolution
    };

    // Add to modified tickets map
    setModifiedTickets(prev => {
      const newMap = new Map(prev);
      newMap.set(ticketId, updatedTicket);
      return newMap;
    });
  };

  // Check if a ticket is modified
  const isTicketModified = (ticketId: string): boolean => {
    return modifiedTickets.has(ticketId);
  };

  // Get the current ticket data (modified or original)
  const getCurrentTicket = (ticketId: string): Ticket => {
    return modifiedTickets.get(ticketId) || tickets.find(t => t.id === ticketId)!;
  };

  // Handle save operation
  const handleSave = async () => {
    setSaving(true);
    setSaveError(null);
    setSaveSuccess(null);

    const updatePromises: Promise<{ ticketId: string; success: boolean; error?: string; statusCode?: number }>[] = [];

    // Create update promises for each modified ticket
    modifiedTickets.forEach((ticket, ticketId) => {
      const promise = fetch(`${API_CONFIG.baseUrl}/api/tickets/${ticketId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify(ticket)
      })
        .then(async response => {
          if (response.ok) {
            return { ticketId, success: true };
          } else {
            // Handle specific HTTP status codes
            if (response.status === 401) {
              return { ticketId, success: false, error: 'Unauthorized', statusCode: 401 };
            } else if (response.status === 403) {
              return { ticketId, success: false, error: 'Forbidden', statusCode: 403 };
            } else {
              return { ticketId, success: false, error: `HTTP ${response.status}`, statusCode: response.status };
            }
          }
        })
        .catch(error => {
          return { ticketId, success: false, error: error.message };
        });

      updatePromises.push(promise);
    });

    // Wait for all updates to complete
    const results = await Promise.all(updatePromises);

    // Check for authentication/authorization errors
    const authError = results.find(r => r.statusCode === 401);
    const forbiddenError = results.find(r => r.statusCode === 403);

    if (authError) {
      // Handle 401 - redirect to login
      setSaving(false);
      navigate('/login');
      return;
    }

    if (forbiddenError) {
      // Handle 403 - access denied
      setSaveError('Access denied. You do not have permission to update tickets.');
      setSaving(false);
      return;
    }

    // Process results
    const successCount = results.filter(r => r.success).length;
    const failures = results.filter(r => !r.success);

    if (failures.length === 0) {
      // All updates succeeded
      setSaveSuccess(`Successfully updated ${successCount} ticket(s)`);
      setModifiedTickets(new Map()); // Clear modified state
      await fetchTickets(); // Refresh ticket data
    } else if (successCount > 0) {
      // Partial save failure - some succeeded, some failed
      const failedIds = failures.map(f => f.ticketId.substring(0, 8)).join(', ');
      setSaveError(`Warning: ${successCount} ticket(s) updated successfully, but ${failures.length} ticket(s) failed: ${failedIds}. Click Save to retry failed tickets.`);

      // Remove successfully updated tickets from modified state
      const newModifiedTickets = new Map(modifiedTickets);
      results.filter(r => r.success).forEach(r => {
        newModifiedTickets.delete(r.ticketId);
      });
      setModifiedTickets(newModifiedTickets);
      
      // Refresh ticket data to show successfully updated tickets
      await fetchTickets();
    } else {
      // All updates failed
      const failedIds = failures.map(f => f.ticketId.substring(0, 8)).join(', ');
      setSaveError(`Failed to update ${failures.length} ticket(s): ${failedIds}. Please try again.`);
    }

    setSaving(false);
  };

  // Apply filters to tickets
  const applyFilters = () => {
    let filtered = [...tickets];

    // Apply status filter
    if (statusFilter !== 'all') {
      filtered = filtered.filter(ticket => ticket.status === statusFilter);
    }

    // Apply text search filter
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
  };

  // Handle search text change
  const handleSearchChange = (text: string) => {
    setSearchText(text);
  };

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

  // Handle ticket row click to navigate to detail view
  const handleTicketClick = (ticketId: string, event: React.MouseEvent<HTMLTableRowElement> | React.KeyboardEvent<HTMLTableRowElement>) => {
    const target = event.target as HTMLElement;

    // Check if click is on editable field or its children
    if (
      target.tagName === 'SELECT' ||
      target.tagName === 'INPUT' ||
      target.tagName === 'OPTION' ||
      target.closest('.editable-field')
    ) {
      // Don't navigate when clicking editable fields
      return;
    }

    // Navigate to ticket detail view
    navigate(`/tickets/${ticketId}`);
  };

  // Handle keyboard navigation for ticket rows
  const handleTicketKeyDown = (ticketId: string, event: React.KeyboardEvent<HTMLTableRowElement>) => {
    // Navigate on Enter key
    if (event.key === 'Enter') {
      handleTicketClick(ticketId, event);
    }
  };

  // Fetch tickets on component mount (same as ViewTickets)
  useEffect(() => {
    fetchTickets();
  }, []);

  // Apply filters whenever tickets, statusFilter, or searchText changes
  useEffect(() => {
    applyFilters();
  }, [tickets, statusFilter, searchText]);

  // Auto-dismiss success message after 5 seconds
  useEffect(() => {
    if (saveSuccess) {
      const timer = setTimeout(() => {
        setSaveSuccess(null);
      }, 5000);
      
      return () => clearTimeout(timer);
    }
  }, [saveSuccess]);

  // Warn before leaving page with unsaved changes (browser navigation)
  useEffect(() => {
    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      if (modifiedTickets.size > 0) {
        e.preventDefault();
        // Modern browsers ignore custom messages and show their own
        e.returnValue = '';
      }
    };

    window.addEventListener('beforeunload', handleBeforeUnload);

    return () => {
      window.removeEventListener('beforeunload', handleBeforeUnload);
    };
  }, [modifiedTickets]);

  return (
    <div className="admin-panel-container">
      <div className="admin-panel-card">
        <header className="admin-header">
          <h1>Admin Panel</h1>
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
            {/* Save Success Message */}
            {saveSuccess && (
              <div className="save-success-banner" role="status" aria-live="polite">
                <span className="success-icon" aria-hidden="true">✓</span>
                <span className="banner-message">{saveSuccess}</span>
                <button 
                  className="banner-close-button" 
                  onClick={() => setSaveSuccess(null)}
                  aria-label="Close success message"
                >
                  ×
                </button>
              </div>
            )}

            {/* Save Error Message */}
            {saveError && (
              <div className="save-error-banner" role="alert" aria-live="assertive">
                <span className="error-icon" aria-hidden="true">⚠️</span>
                <span className="banner-message">{saveError}</span>
                <button 
                  className="banner-close-button" 
                  onClick={() => setSaveError(null)}
                  aria-label="Close error message"
                >
                  ×
                </button>
              </div>
            )}

            {/* Filters and Actions Section */}
            <section className="filters-actions-section" aria-label="Filter tickets and actions">
              <div className="filters-group">
                <div className="filter-group">
                  <label htmlFor="status-filter" className="filter-label">
                    Status:
                  </label>
                  <select
                    id="status-filter"
                    className="status-filter"
                    value={statusFilter}
                    onChange={(e) => {
                      const value = e.target.value;
                      handleStatusFilterChange(value === 'all' ? 'all' : parseInt(value, 10) as TicketStatus);
                    }}
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
              </div>

              <div className="actions-group">
                <button
                  className={`save-button ${modifiedTickets.size === 0 || saving ? 'disabled' : ''}`}
                  disabled={modifiedTickets.size === 0 || saving}
                  onClick={handleSave}
                  aria-label={saving ? 'Saving changes' : 'Save changes'}
                >
                  {saving ? (
                    <>
                      <span className="button-spinner" aria-hidden="true"></span>
                      Saving...
                    </>
                  ) : (
                    'Save'
                  )}
                </button>
              </div>
            </section>

            {/* Table Section */}
            {filteredTickets.length > 0 ? (
              <section className="table-section">
                <table className="tickets-table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Name</th>
                      <th>Status</th>
                      <th>Summary</th>
                      <th>Resolution</th>
                      <th>Created Date</th>
                    </tr>
                  </thead>
                  <tbody>
                    {filteredTickets.map((ticket) => {
                      const currentTicket = getCurrentTicket(ticket.id);
                      const isModified = isTicketModified(ticket.id);
                      
                      return (
                        <tr 
                          key={ticket.id}
                          className={`ticket-row ${isModified ? 'modified' : ''}`}
                          onClick={(e) => handleTicketClick(ticket.id, e)}
                          onKeyDown={(e) => handleTicketKeyDown(ticket.id, e)}
                          role="button"
                          tabIndex={0}
                          aria-label={`Ticket ${ticket.name}, status ${getStatusLabel(currentTicket.status)}, created ${formatDateShort(ticket.createdAt)}. Press Enter to view details.`}
                        >
                          <td className="ticket-id" data-label="ID">{ticket.id.substring(0, 8)}...</td>
                          <td className="ticket-name" data-label="Name">{ticket.name}</td>
                          <td className="ticket-status" data-label="Status">
                            <select
                              className={`status-dropdown editable-field ${getStatusColor(currentTicket.status)}`}
                              value={currentTicket.status}
                              onChange={(e) => handleTicketStatusChange(ticket.id, parseInt(e.target.value, 10) as TicketStatus)}
                              onClick={(e) => e.stopPropagation()}
                              disabled={saving}
                              aria-label={`Change status for ticket ${ticket.name}`}
                            >
                              <option value={TicketStatus.New}>New</option>
                              <option value={TicketStatus.InProgress}>In Progress</option>
                              <option value={TicketStatus.Resolved}>Resolved</option>
                              <option value={TicketStatus.Closed}>Closed</option>
                            </select>
                          </td>
                          <td className="ticket-summary" data-label="Summary">{ticket.summary}</td>
                          <td className="ticket-resolution" data-label="Resolution">
                            <input
                              type="text"
                              className="resolution-input editable-field"
                              value={currentTicket.resolution || ''}
                              onChange={(e) => handleTicketResolutionChange(ticket.id, e.target.value)}
                              onClick={(e) => e.stopPropagation()}
                              disabled={saving}
                              placeholder="Enter resolution..."
                              aria-label={`Resolution for ticket ${ticket.name}`}
                            />
                          </td>
                          <td className="ticket-date" data-label="Created">{formatDateShort(ticket.createdAt)}</td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              </section>
            ) : (
              <div className="empty-filter-state">
                <p>No tickets match your filters</p>
                <p className="empty-filter-hint">Try adjusting your status or search filters</p>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};

export default AdminPanel;
