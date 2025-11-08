# Design Document

## Overview

The View Tickets UI feature provides a tickets list screen that displays all tickets in a table format with filtering capabilities. This screen serves as the main dashboard after user login, allowing users and admins to view all tickets, filter by status and text search, and navigate to individual ticket details. The component fetches data from the backend `GetAllTicketsAsync` service method via a REST API endpoint and provides a read-only view of ticket information.

## Architecture

### Component Structure

```
ViewTickets (React Component)
├── ViewTickets.tsx (Main component logic)
└── ViewTickets.css (Component styling)
```

### Routing

- Route: `/tickets` (replaces the placeholder "Coming Soon" route in App.tsx)
- Post-login redirect: Users will be redirected to `/tickets` after successful authentication
- Navigation: Clicking a ticket row navigates to `/tickets/:id` (existing ViewTicket component)

### Data Flow

1. Component mounts after user login/redirect
2. Component initiates API call to `/api/tickets` (GetAllTickets endpoint)
3. Loading state is displayed while fetching data
4. On success: Tickets are stored in state and displayed in table
5. User applies filters (status dropdown, text search)
6. Filtered results are computed and displayed in real-time
7. User clicks ticket row → Navigate to `/tickets/:id` with ticket GUID
8. On error: Error message is displayed with retry option

## Components and Interfaces

### ViewTickets Component

**Location:** `ticketsystem.client/src/components/ViewTickets.tsx`

**Props:** None

**State:**
```typescript
interface ViewTicketsState {
  tickets: Ticket[];
  filteredTickets: Ticket[];
  loading: boolean;
  error: string | null;
  statusFilter: TicketStatus | 'all';
  searchText: string;
}
```

**Key Functions:**
- `fetchTickets()`: Fetches all tickets from the API endpoint
- `handleStatusFilterChange(status: TicketStatus | 'all')`: Updates status filter and recomputes filtered tickets
- `handleSearchChange(text: string)`: Updates search text and recomputes filtered tickets
- `applyFilters()`: Applies both status and text filters to the tickets array
- `handleTicketClick(ticketId: string)`: Navigates to the ticket detail view
- `getStatusLabel(status: TicketStatus)`: Returns display label for status enum
- `getStatusColor(status: TicketStatus)`: Returns CSS class for status badge
- `formatDate(date: string)`: Formats ISO date strings to readable format

### API Integration

**Endpoint:** `GET https://localhost:7248/api/tickets`

**Configuration:**
- API Base URL should be configurable (stored in a config file or environment variable)
- Default base URL: `https://localhost:7248`
- Full endpoint constructed as: `${API_BASE_URL}/api/tickets`

**Authentication:**
- Uses cookie-based authentication with JWT token stored in `AuthToken` cookie
- All API requests must include `credentials: 'include'` in fetch options to send cookies
- Backend validates the JWT token from the cookie and checks user authorization

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
- 403 Forbidden: Display access denied message
- 500 Server Error: Display error message with retry button
- Network Error: Display connection error message with retry button
- Empty response: Display "No tickets found" message

## Data Models

### Ticket Interface (Frontend)

The frontend will use the same TypeScript interface as ViewTicket component:

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

## UI Layout and Styling

### Layout Structure

```
Container (full viewport, centered)
└── Card (white background, rounded corners, shadow)
    ├── Header Section
    │   └── Page Title ("Tickets")
    ├── Filters Section
    │   ├── Status Dropdown
    │   └── Search Input
    ├── Table Section
    │   ├── Table Header
    │   │   ├── ID Column
    │   │   ├── Name Column
    │   │   ├── Status Column
    │   │   ├── Summary Column
    │   │   └── Created Date Column
    │   └── Table Body
    │       └── Ticket Rows (clickable)
    └── Empty State (when no tickets match filters)
```

### Styling Approach

**File:** `ticketsystem.client/src/styles/ViewTickets.css`

**Design Principles:**
- Consistent with ViewTicket and NewTicket component styling
- Background: `#f5f5f5`
- Card background: `#ffffff`
- Primary color: `#F5B5A8`
- Border radius: `16px` for card, `10px` for inputs
- Font sizes: 56px heading, 20px labels, 18px content
- Table styling: Clean borders, alternating row colors for readability
- Hover effects on table rows to indicate clickability

**Filter Section:**
- Horizontal layout with status dropdown on left, search input on right
- Consistent input styling with rounded corners and subtle borders
- Clear visual separation from table section

**Table Styling:**
- Header row: Bold text, light background (`#f8f9fa`)
- Data rows: Alternating background colors for better readability
- Hover state: Light highlight (`#f0f0f0`) with pointer cursor
- Column widths: ID (15%), Name (20%), Status (15%), Summary (35%), Created (15%)
- Text alignment: Left for text columns, center for status badges

**Status Badge Colors:**
- New: Blue (`#3498db`)
- In Progress: Orange (`#f39c12`)
- Resolved: Green (`#27ae60`)
- Closed: Gray (`#95a5a6`)

### Responsive Design

- Desktop (>1400px): Full table with all columns
- Tablet (768px-1400px): Adjust column widths, maintain all columns
- Mobile (<768px): Consider stacking filters vertically, reduce padding
- Mobile (<576px): Card-based layout instead of table (each ticket as a card)

### Conditional Rendering

- **Loading State:** Display centered spinner with "Loading tickets..." message
- **Error State:** Display error message with retry button
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
   - Display: "Access Denied"
   - Message: "You do not have permission to view tickets."
   - Action: No retry, display message only

3. **Server Error (500)**
   - Display: "Unable to load tickets"
   - Message: "An error occurred while loading tickets. Please try again later."
   - Action: Provide retry button

4. **Network Error**
   - Display: "Connection error"
   - Message: "Unable to connect to the server. Please check your internet connection."
   - Action: Provide retry button

5. **Empty Response**
   - Display: "No tickets found"
   - Message: "There are no tickets in the system yet."
   - Action: No retry needed, just informational

### Loading State

- Display centered loading spinner with text "Loading tickets..."
- Accessible loading indicator with appropriate ARIA labels
- Disable filters during loading

## Implementation Notes

### Technology Stack
- React 19.1.1 with TypeScript
- React Router for navigation
- Fetch API for HTTP requests
- CSS for styling (no CSS-in-JS libraries)

### Configuration Management
- Create a configuration file (e.g., `src/config/api.ts`) to store the API base URL
- Use environment variables or a config object to manage the base URL
- Example configuration:
```typescript
export const API_CONFIG = {
  baseUrl: 'https://localhost:7248'
};
```
- Import and use this configuration in the component for API calls

### Authentication Integration
- Authentication is handled by Login component which sets the `AuthToken` cookie
- Post-login redirect will navigate to `/tickets` route
- All API requests must include `credentials: 'include'` to send the authentication cookie
- Backend validates the JWT token from the cookie on each request
- If authentication fails (401), user should be redirected to login page

### Navigation Integration
- Use React Router's `useNavigate` hook for programmatic navigation
- Navigate to `/tickets/:id` when ticket row is clicked
- Pass ticket GUID as URL parameter

### Performance Considerations
- Single API call on component mount
- Client-side filtering (no API calls for filter changes)
- Memoization of filtered results if performance issues arise
- Consider pagination if ticket count grows large (future enhancement)

### Accessibility Considerations
- Semantic HTML elements (table, thead, tbody, tr, th, td)
- Proper heading hierarchy (h1 for page title)
- ARIA labels for filters and status badges
- Keyboard navigation support for table rows (Enter key to navigate)
- Screen reader announcements for filter changes
- Sufficient color contrast for text and status badges
- Focus indicators for interactive elements

### Future Enhancements (Out of Scope)
- Pagination for large ticket lists
- Sorting by column headers
- Advanced filters (date range, customer email)
- Bulk actions (select multiple tickets)
- Export to CSV
- Refresh button to manually reload tickets
- Real-time updates via WebSocket
