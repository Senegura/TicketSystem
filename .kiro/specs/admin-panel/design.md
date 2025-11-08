# Design Document

## Overview

The Admin Panel feature provides an administrative interface for managing tickets with the ability to view, filter, and edit ticket status and resolution fields in bulk. This component extends the functionality of the ViewTickets component by adding editable fields for status and resolution, tracking modified tickets, and providing a save operation that updates all changed tickets via the backend API. Admin users are redirected to this panel after login, while regular users continue to use the read-only tickets list.

## Architecture

### Component Structure

```
AdminPanel (React Component)
├── AdminPanel.tsx (Main component logic)
└── AdminPanel.css (Component styling)
```

### Routing and Authentication

- Route: `/admin` (new route for admin panel)
- Post-login redirect logic:
  - Admin users (UserType = 2): Redirect to `/admin`
  - Regular users (UserType = 1): Redirect to `/tickets`
- Authentication: JWT token stored in `AuthToken` cookie
- Authorization: Component validates UserType claim from JWT token
- Unauthorized access: Redirect to login page
- Forbidden access (non-admin): Redirect to `/tickets` or display access denied message

### Data Flow

1. Component mounts after admin login/redirect
2. Component validates user is admin (UserType = 2)
3. Component initiates API call to `/api/tickets` (GetAllTickets endpoint)
4. Loading state is displayed while fetching data
5. On success: Tickets are stored in state and displayed in editable table
6. Admin applies filters (status dropdown, text search)
7. Filtered results are computed and displayed in real-time
8. Admin modifies ticket status or resolution fields
9. Modified tickets are tracked in component state
10. Admin clicks Save button
11. Component sends PUT requests to `/api/tickets/{id}` for each modified ticket
12. Success/error feedback is displayed
13. On success: Component refreshes ticket data and clears modified state
14. Admin can click ticket row (non-editable areas) to navigate to detail view

## Components and Interfaces

### AdminPanel Component

**Location:** `ticketsystem.client/src/components/AdminPanel.tsx`

**Props:** None

**State:**
```typescript
interface AdminPanelState {
  tickets: Ticket[];
  filteredTickets: Ticket[];
  modifiedTickets: Map<string, Ticket>; // Map of ticket ID to modified ticket object
  loading: boolean;
  saving: boolean;
  error: string | null;
  saveError: string | null;
  saveSuccess: string | null;
  statusFilter: TicketStatus | 'all';
  searchText: string;
}
```

**Key Functions:**
- `fetchTickets()`: Fetches all tickets from the API endpoint
- `handleStatusFilterChange(status: TicketStatus | 'all')`: Updates status filter and recomputes filtered tickets
- `handleSearchChange(text: string)`: Updates search text and recomputes filtered tickets
- `applyFilters()`: Applies both status and text filters to the tickets array
- `handleTicketStatusChange(ticketId: string, newStatus: TicketStatus)`: Updates ticket status and marks as modified
- `handleTicketResolutionChange(ticketId: string, newResolution: string)`: Updates ticket resolution and marks as modified
- `handleSave()`: Saves all modified tickets to the server
- `handleTicketClick(ticketId: string, event: React.MouseEvent)`: Navigates to ticket detail view (only if not clicking editable fields)
- `isTicketModified(ticketId: string)`: Checks if a ticket has been modified
- `getStatusLabel(status: TicketStatus)`: Returns display label for status enum
- `getStatusColor(status: TicketStatus)`: Returns CSS class for status badge
- `formatDate(date: string)`: Formats ISO date strings to readable format
- `validateAdminAccess()`: Validates user has admin privileges from JWT token

### API Integration

**Endpoints:**

1. **Get All Tickets**
   - Method: `GET`
   - URL: `https://localhost:7248/api/tickets`
   - Authentication: Cookie-based with JWT token in `AuthToken` cookie
   - Response: Array of Ticket objects

2. **Update Ticket**
   - Method: `PUT`
   - URL: `https://localhost:7248/api/tickets/{id}`
   - Authentication: Cookie-based with JWT token in `AuthToken` cookie
   - Request Body: Complete Ticket object
   - Response: Updated Ticket object

**Configuration:**
- API Base URL should be configurable (use existing `API_CONFIG` from `src/config/api.ts`)
- Default base URL: `https://localhost:7248`
- Full endpoints:
  - Get: `${API_CONFIG.baseUrl}/api/tickets`
  - Update: `${API_CONFIG.baseUrl}/api/tickets/${ticketId}`

**Authentication:**
- Uses cookie-based authentication with JWT token stored in `AuthToken` cookie
- All API requests must include `credentials: 'include'` in fetch options to send cookies
- Backend validates the JWT token from the cookie and checks user authorization
- Admin panel validates UserType claim is 2 (Admin) before allowing access

**Response Type:**
```typescript
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

enum TicketStatus {
  New = 0,
  InProgress = 1,
  Resolved = 2,
  Closed = 3
}
```

**Error Handling:**
- 401 Unauthorized: Redirect to login page (authentication failed or expired)
- 403 Forbidden: Redirect to `/tickets` or display access denied message (not an admin)
- 404 Not Found: Display error for specific ticket update failure
- 500 Server Error: Display error message with retry button
- Network Error: Display connection error message with retry button

## Data Models

### Ticket Interface (Frontend)

The frontend will use the same TypeScript interface as ViewTickets component:

```typescript
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
```

### Modified Tickets Tracking

```typescript
// Map structure for tracking modified tickets
const modifiedTickets = new Map<string, Ticket>();

// When a ticket is modified:
modifiedTickets.set(ticketId, updatedTicket);

// When checking if a ticket is modified:
const isModified = modifiedTickets.has(ticketId);

// When getting modified ticket data:
const modifiedTicket = modifiedTickets.get(ticketId);

// When clearing modified state after save:
modifiedTickets.clear();
```

### Status Enum and Labels

```typescript
enum TicketStatus {
  New = 0,
  InProgress = 1,
  Resolved = 2,
  Closed = 3
}

const statusLabels = {
  [TicketStatus.New]: 'New',
  [TicketStatus.InProgress]: 'In Progress',
  [TicketStatus.Resolved]: 'Resolved',
  [TicketStatus.Closed]: 'Closed'
};

const statusFilterOptions = [
  { value: 'all', label: 'All Statuses' },
  { value: TicketStatus.New, label: 'New' },
  { value: TicketStatus.InProgress, label: 'In Progress' },
  { value: TicketStatus.Resolved, label: 'Resolved' },
  { value: TicketStatus.Closed, label: 'Closed' }
];
```

### Filtering Logic

**Status Filter:**
- If `statusFilter === 'all'`: Include all tickets
- Otherwise: Include only tickets where `ticket.status === statusFilter`

**Text Search:**
- Case-insensitive matching
- Search in `ticket.name` field
- Search in `ticket.description` field
- Match if either field contains the search text

**Combined Filters:**
- Apply status filter first
- Then apply text search filter to the status-filtered results
- Real-time filtering as user types (no debouncing for simplicity)
- Filters apply to both original and modified ticket data

## UI Layout and Styling

### Layout Structure

```
Container (full viewport, centered)
└── Card (white background, rounded corners, shadow)
    ├── Header Section
    │   └── Page Title ("Admin Panel")
    ├── Filters and Actions Section
    │   ├── Status Dropdown
    │   ├── Search Input
    │   └── Save Button (right-aligned, enabled when changes exist)
    ├── Table Section
    │   ├── Table Header
    │   │   ├── ID Column
    │   │   ├── Name Column
    │   │   ├── Status Column (editable)
    │   │   ├── Summary Column
    │   │   ├── Resolution Column (editable)
    │   │   └── Created Date Column
    │   └── Table Body
    │       └── Ticket Rows (clickable, with modified indicator)
    └── Empty State (when no tickets match filters)
```

### Styling Approach

**File:** `ticketsystem.client/src/styles/AdminPanel.css`

**Design Principles:**
- Based on ViewTickets component styling for consistency
- Background: `#f5f5f5`
- Card background: `#ffffff`
- Primary color: `#F5B5A8`
- Border radius: `16px` for card, `10px` for inputs
- Font sizes: 56px heading, 20px labels, 18px content
- Table styling: Clean borders, alternating row colors for readability
- Modified row indicator: Light yellow background (`#fffbea`) or left border accent

**Filters and Actions Section:**
- Horizontal layout with filters on left, Save button on right
- Save button styling:
  - Enabled: Primary color (`#F5B5A8`), white text, bold
  - Disabled: Gray background (`#e0e0e0`), gray text (`#999999`)
  - Hover (enabled): Darker shade (`#f3a091`)
  - Loading state: Spinner icon, disabled interaction
- Consistent input styling with rounded corners and subtle borders

**Table Styling:**
- Header row: Bold text, light background (`#f8f9fa`)
- Data rows: Alternating background colors for better readability
- Modified rows: Light yellow background (`#fffbea`) or left border accent (`#f39c12`)
- Hover state: Light highlight (`#f0f0f0`) with pointer cursor (only on non-editable areas)
- Column widths: ID (12%), Name (18%), Status (15%), Summary (25%), Resolution (20%), Created (10%)
- Text alignment: Left for text columns, center for status dropdowns

**Editable Fields:**
- Status dropdown: Inline dropdown with same styling as filter dropdown
- Resolution input: Text input field with rounded corners, full width of column
- Focus states: Blue border (`#2980b9`) with subtle shadow
- Modified indicator: Visual cue on row (background color or border)

**Status Dropdown Colors:**
- New: Blue (`#3498db`)
- In Progress: Orange (`#f39c12`)
- Resolved: Green (`#27ae60`)
- Closed: Gray (`#95a5a6`)

**Save Button States:**
- Default (disabled): Gray background, gray text, no hover effect
- Enabled: Primary color background, white text, hover effect
- Loading: Spinner icon, disabled, "Saving..." text
- Success: Brief green background flash, then return to default state

### Responsive Design

- Desktop (>1400px): Full table with all columns
- Tablet (768px-1400px): Adjust column widths, maintain all columns
- Mobile (<768px): Consider stacking filters vertically, reduce padding
- Mobile (<576px): Card-based layout instead of table (each ticket as a card with editable fields)

### Conditional Rendering

- **Loading State:** Display centered spinner with "Loading tickets..." message
- **Saving State:** Display spinner on Save button with "Saving..." text
- **Error State:** Display error message with retry button
- **Save Error State:** Display error banner above table with specific error details
- **Save Success State:** Display success banner above table with count of updated tickets
- **Empty State (No Tickets):** Display "No tickets found" message
- **Empty State (No Matches):** Display "No tickets match your filters" with suggestion to adjust filters
- **Table:** Only displayed when tickets are loaded and not empty

## Error Handling

### Error States

1. **Unauthorized (401)**
   - Display: Redirect to login page
   - Message: Authentication token is missing or expired
   - Action: Automatic redirect to `/login`

2. **Forbidden (403)**
   - Display: Redirect to `/tickets` or show "Access Denied"
   - Message: "You do not have permission to access the admin panel."
   - Action: Automatic redirect to `/tickets` for regular users

3. **Server Error (500) - Fetch Tickets**
   - Display: "Unable to load tickets"
   - Message: "An error occurred while loading tickets. Please try again later."
   - Action: Provide retry button

4. **Server Error (500) - Update Tickets**
   - Display: Error banner above table
   - Message: "Failed to update some tickets. Please try again."
   - Details: List of ticket IDs that failed to update
   - Action: Provide retry button, preserve modified state

5. **Network Error**
   - Display: "Connection error"
   - Message: "Unable to connect to the server. Please check your internet connection."
   - Action: Provide retry button

6. **Partial Save Failure**
   - Display: Warning banner above table
   - Message: "Some tickets were updated successfully, but X tickets failed."
   - Details: List of ticket IDs that failed
   - Action: Provide retry button for failed tickets only

7. **Empty Response**
   - Display: "No tickets found"
   - Message: "There are no tickets in the system yet."
   - Action: No retry needed, just informational

### Loading States

- **Initial Load:** Display centered loading spinner with text "Loading tickets..."
- **Saving:** Display spinner on Save button with "Saving..." text, disable all editable fields
- **Accessible loading indicators:** Appropriate ARIA labels for screen readers

## Implementation Notes

### Technology Stack
- React 19.1.1 with TypeScript
- React Router for navigation
- Fetch API for HTTP requests
- CSS for styling (no CSS-in-JS libraries)

### Configuration Management
- Use existing `API_CONFIG` from `src/config/api.ts` for API base URL
- No additional configuration needed

### Authentication and Authorization Integration
- Authentication is handled by Login component which sets the `AuthToken` cookie
- Post-login redirect logic needs to be updated in Login component:
  - Check UserType claim from JWT token response
  - If UserType = 2 (Admin): Navigate to `/admin`
  - If UserType = 1 (User): Navigate to `/tickets`
- AdminPanel component validates UserType on mount:
  - Extract JWT token from `AuthToken` cookie
  - Decode token and check UserType claim
  - If not admin: Redirect to `/tickets` or show access denied
- All API requests must include `credentials: 'include'` to send the authentication cookie
- Backend validates the JWT token from the cookie on each request

### Navigation Integration
- Use React Router's `useNavigate` hook for programmatic navigation
- Navigate to `/tickets/:id` when ticket row is clicked (non-editable areas only)
- Pass ticket GUID as URL parameter
- Prevent navigation when clicking on editable fields (status dropdown, resolution input)
- Implement unsaved changes warning:
  - Use `beforeunload` event to warn when leaving page with unsaved changes
  - Use React Router's navigation blocker to warn when navigating within app

### State Management
- Use React useState for component state
- Track modified tickets in a Map for efficient lookup and updates
- Merge modified ticket data with original ticket data for display
- Clear modified state after successful save operation
- Preserve modified state across filter changes

### Save Operation Logic

```typescript
const handleSave = async () => {
  setSaving(true);
  setSaveError(null);
  setSaveSuccess(null);
  
  const updatePromises: Promise<{ticketId: string, success: boolean, error?: string}>[] = [];
  
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
    .then(response => {
      if (response.ok) {
        return { ticketId, success: true };
      } else {
        return { ticketId, success: false, error: `HTTP ${response.status}` };
      }
    })
    .catch(error => {
      return { ticketId, success: false, error: error.message };
    });
    
    updatePromises.push(promise);
  });
  
  // Wait for all updates to complete
  const results = await Promise.all(updatePromises);
  
  // Process results
  const successCount = results.filter(r => r.success).length;
  const failures = results.filter(r => !r.success);
  
  if (failures.length === 0) {
    // All updates succeeded
    setSaveSuccess(`Successfully updated ${successCount} ticket(s)`);
    setModifiedTickets(new Map()); // Clear modified state
    await fetchTickets(); // Refresh ticket data
  } else {
    // Some or all updates failed
    const failedIds = failures.map(f => f.ticketId.substring(0, 8)).join(', ');
    setSaveError(`Failed to update ${failures.length} ticket(s): ${failedIds}`);
    
    // Remove successfully updated tickets from modified state
    const newModifiedTickets = new Map(modifiedTickets);
    results.filter(r => r.success).forEach(r => {
      newModifiedTickets.delete(r.ticketId);
    });
    setModifiedTickets(newModifiedTickets);
  }
  
  setSaving(false);
};
```

### Performance Considerations
- Single API call on component mount
- Client-side filtering (no API calls for filter changes)
- Efficient modified ticket tracking using Map data structure
- Parallel update requests for better performance (Promise.all)
- Consider sequential updates if parallel causes issues
- Memoization of filtered results if performance issues arise

### Accessibility Considerations
- Semantic HTML elements (table, thead, tbody, tr, th, td)
- Proper heading hierarchy (h1 for page title)
- ARIA labels for filters, editable fields, and Save button
- Keyboard navigation support for table rows and editable fields
- Screen reader announcements for filter changes and save operations
- Sufficient color contrast for text, status badges, and modified indicators
- Focus indicators for interactive elements
- Disabled state properly communicated to screen readers
- Form labels associated with editable fields

### Unsaved Changes Warning

```typescript
// Warn before leaving page with unsaved changes
useEffect(() => {
  const handleBeforeUnload = (e: BeforeUnloadEvent) => {
    if (modifiedTickets.size > 0) {
      e.preventDefault();
      e.returnValue = '';
    }
  };
  
  window.addEventListener('beforeunload', handleBeforeUnload);
  
  return () => {
    window.removeEventListener('beforeunload', handleBeforeUnload);
  };
}, [modifiedTickets]);

// Warn before navigating within app with unsaved changes
// Use React Router's useBlocker or similar mechanism
```

### Click Handling for Editable Fields

```typescript
// Prevent row click navigation when clicking editable fields
const handleTicketClick = (ticketId: string, event: React.MouseEvent) => {
  const target = event.target as HTMLElement;
  
  // Check if click is on editable field or its children
  if (
    target.tagName === 'SELECT' ||
    target.tagName === 'INPUT' ||
    target.tagName === 'OPTION' ||
    target.closest('.editable-field')
  ) {
    // Don't navigate
    return;
  }
  
  // Navigate to ticket detail
  navigate(`/tickets/${ticketId}`);
};
```

### Future Enhancements (Out of Scope)
- Undo/redo functionality for edits
- Bulk status update (select multiple tickets, apply same status)
- Inline validation for resolution text (e.g., required for Resolved status)
- Auto-save draft changes to localStorage
- Real-time collaboration (show when other admins are editing)
- Audit log of changes made by admins
- Export filtered tickets to CSV
- Advanced filters (date range, customer email)
- Sorting by column headers
- Pagination for large ticket lists
