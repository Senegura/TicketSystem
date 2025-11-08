# Design Document

## Overview

The Ticket View UI feature provides a dedicated screen for viewing complete ticket details by accessing a unique ticket ID (GUID). The component will fetch ticket data from the backend API endpoint `/api/tickets/{id:guid}` and display all relevant information in a clean, organized layout following the existing design patterns established in the NewTicket component.

## Architecture

### Component Structure

```
ViewTicket (React Component)
├── ViewTicket.tsx (Main component logic)
└── ViewTicket.css (Component styling)
```

### Routing

- Route: `/tickets/:id` where `:id` is the ticket GUID
- The route will be added to the existing React Router configuration in `App.tsx`

### Data Flow

1. Component mounts and extracts ticket ID from URL parameters
2. Component initiates API call to `/api/tickets/{id:guid}`
3. Loading state is displayed while fetching data
4. On success: Ticket data is rendered in organized sections
5. On error: Error message is displayed (404 for not found, generic error for other failures)

## Components and Interfaces

### ViewTicket Component

**Location:** `ticketsystem.client/src/components/ViewTicket.tsx`

**Props:** None (uses React Router's `useParams` hook to extract ticket ID from URL)

**State:**
```typescript
interface ViewTicketState {
  ticket: Ticket | null;
  loading: boolean;
  error: string | null;
}
```

**Key Functions:**
- `fetchTicket(id: string)`: Fetches ticket data from the API endpoint
- `getStatusColor(status: TicketStatus)`: Returns appropriate color class for status badge
- `formatDate(date: string)`: Formats ISO date strings to readable format

### API Integration

**Endpoint:** `GET /api/tickets/{id:guid}`

**Authentication:**
- The endpoint requires authentication via JWT token stored in HTTP-only cookie named "AuthToken"
- The browser automatically includes the cookie in requests (credentials: 'include' in fetch options)
- No manual cookie manipulation is required in JavaScript code

**Request Configuration:**
```typescript
fetch(`/api/tickets/${id}`, {
  method: 'GET',
  credentials: 'include', // Ensures cookies are sent with the request
  headers: {
    'Content-Type': 'application/json'
  }
})
```

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
- 401 Unauthorized: Display "Authentication required" message with link to login
- 403 Forbidden: Display "Access denied" message
- 404 Not Found: Display "Ticket not found" message
- 500 Server Error: Display generic error message
- Network Error: Display connection error message

## Data Models

### Ticket Interface (Frontend)

The frontend will use a TypeScript interface matching the backend Ticket model:

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

### Status Enum Mapping

```typescript
enum TicketStatus {
  New = 0,
  InProgress = 1,
  Resolved = 2,
  Closed = 3
}

// Display labels
const statusLabels = {
  [TicketStatus.New]: 'New',
  [TicketStatus.InProgress]: 'In Progress',
  [TicketStatus.Resolved]: 'Resolved',
  [TicketStatus.Closed]: 'Closed'
};
```

## UI Layout and Styling

### Layout Structure

The component will follow the same card-based layout pattern as NewTicket:

```
Container (full viewport, centered)
└── Card (white background, rounded corners, shadow)
    ├── Header Section
    │   ├── Ticket ID
    │   └── Status Badge
    ├── Customer Details Section
    │   ├── Name
    │   └── Email
    ├── Issue Details Section
    │   ├── Summary
    │   └── Description
    ├── Image Section (conditional)
    │   └── Uploaded Image
    ├── Resolution Section (conditional)
    │   └── Resolution Text
    └── Metadata Section
        ├── Created Date
        └── Updated Date
```

### Styling Approach

**File:** `ticketsystem.client/src/styles/ViewTicket.css`

**Design Principles:**
- Consistent with NewTicket component styling
- Background: `#f5f5f5`
- Card background: `#ffffff`
- Primary color: `#F5B5A8`
- Border radius: `16px` for card, `10px` for elements
- Font sizes: 56px heading, 20px labels, 18px content
- Responsive design with mobile breakpoints

**Status Badge Colors:**
- New: Blue (`#3498db`)
- In Progress: Orange (`#f39c12`)
- Resolved: Green (`#27ae60`)
- Closed: Gray (`#95a5a6`)

### Responsive Design

- Desktop (>1400px): Full padding and larger fonts
- Tablet (768px-1400px): Standard sizing
- Mobile (<576px): Reduced padding, minimum 16px font size to prevent iOS zoom

### Conditional Rendering

- **Image Section:** Only displayed when `imageUrl` is not empty
- **Resolution Section:** Only displayed when `resolution` is not empty
- **Error State:** Replaces entire card content when error occurs
- **Loading State:** Shows centered spinner while fetching data

## Error Handling

### Error States

1. **Authentication Required (401)**
   - Display: "Authentication required"
   - Message: "You must be logged in to view this ticket."
   - Action: Provide "Go to Login" button/link that navigates to `/login`

2. **Access Denied (403)**
   - Display: "Access denied"
   - Message: "You don't have permission to view this ticket."
   - Action: Provide link back to tickets list or home

3. **Ticket Not Found (404)**
   - Display: "Ticket not found"
   - Message: "The ticket you're looking for doesn't exist or has been removed."
   - Action: Provide link back to tickets list or home

4. **Server Error (500)**
   - Display: "Unable to load ticket"
   - Message: "An error occurred while loading the ticket. Please try again later."
   - Action: Provide retry button

5. **Network Error**
   - Display: "Connection error"
   - Message: "Unable to connect to the server. Please check your internet connection."
   - Action: Provide retry button

6. **Invalid Ticket ID Format**
   - Display: "Invalid ticket ID"
   - Message: "The ticket ID format is invalid."
   - Action: Provide link back to tickets list or home

### Loading State

- Display centered loading spinner with text "Loading ticket..."
- Minimum display time to prevent flashing on fast connections
- Accessible loading indicator with appropriate ARIA labels

## Implementation Notes

### Technology Stack
- React 19.1.1 with TypeScript
- React Router for routing and URL parameter extraction
- Fetch API for HTTP requests with credentials: 'include' for cookie-based authentication
- CSS for styling (no CSS-in-JS libraries)

### Authentication Implementation
- The backend uses JWT tokens stored in HTTP-only cookies (cookie name: "AuthToken")
- HTTP-only cookies cannot be accessed via JavaScript, providing security against XSS attacks
- The browser automatically sends cookies with requests when credentials: 'include' is set
- No manual cookie handling or token storage is required in the frontend code
- Authentication errors (401) should redirect users to the login page
- Authorization errors (403) should display an appropriate error message

### Accessibility Considerations
- Semantic HTML elements (section, article, header)
- Proper heading hierarchy (h1, h2, h3)
- Alt text for images
- ARIA labels for status badges and loading states
- Keyboard navigation support
- Sufficient color contrast for text and status badges

### Performance Considerations
- Single API call on component mount
- Conditional rendering to avoid unnecessary DOM elements
- Image lazy loading if supported
- Memoization of status color calculations if needed

### Future Enhancements (Out of Scope)
- Edit ticket functionality
- Add comments/notes to tickets
- Ticket history/audit log
- Print ticket view
- Share ticket link
