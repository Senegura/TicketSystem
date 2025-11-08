# Design Document

## Overview

This feature adds a publicly accessible ticket submission interface to the TicketSystem React application. The implementation will create a new `NewTicket` component that mirrors the design and structure of the existing `Login` component, including matching CSS styling. The feature will integrate with the existing routing system and add navigation from the login screen to the new ticket submission screen.

## Architecture

### Component Structure

```
ticketsystem.client/src/
├── components/
│   ├── Login.tsx (existing - will be modified)
│   └── NewTicket.tsx (new)
├── styles/
│   ├── Login.css (existing)
│   └── NewTicket.css (new - mirrors Login.css)
├── config.ts (existing - will be modified)
└── App.tsx (existing - will be modified)
```

### Routing

The application uses React Router for navigation. A new route will be added:
- `/new-ticket` - Public route for ticket submission

The existing `/` route (Login screen) will be modified to include a link to `/new-ticket`.

### API Integration

The component will communicate with the backend API endpoint:
- **Endpoint**: `https://localhost:7248/api/tickets`
- **Method**: POST
- **Content-Type**: application/json
- **Request Body**: CreateTicketRequest model

## Components and Interfaces

### NewTicket Component

**Location**: `ticketsystem.client/src/components/NewTicket.tsx`

**Responsibilities**:
- Render a form with fields for full name, email, and issue description
- Validate form inputs before submission
- Handle form submission via POST request to the tickets API
- Display error messages when submission fails
- Display success messages and clear form when submission succeeds
- Provide navigation back to login screen

**State Management**:
```typescript
const [fullName, setFullName] = useState<string>('');
const [email, setEmail] = useState<string>('');
const [issueDescription, setIssueDescription] = useState<string>('');
const [error, setError] = useState<string>('');
const [success, setSuccess] = useState<string>('');
const [isLoading, setIsLoading] = useState<boolean>(false);
```

**Key Functions**:
- `handleFullNameChange`: Updates full name state and clears messages
- `handleEmailChange`: Updates email state and clears messages
- `handleIssueDescriptionChange`: Updates issue description state and clears messages
- `handleSubmit`: Validates inputs, sends POST request, handles response
- `handleBackToLogin`: Navigates back to login screen

### CSS Styling

**Location**: `ticketsystem.client/src/styles/NewTicket.css`

The styling will replicate the Login.css structure with the following classes:
- `.new-ticket-container` - Full-page container with centered content
- `.new-ticket-wrapper` - Max-width wrapper
- `.new-ticket-card` - White card with shadow and rounded corners
- `.new-ticket-heading` - Large heading matching login style
- `.new-ticket-button` - Primary action button with #F5B5A8 color
- `.back-to-login-container` - Container for back link
- `.back-to-login-link` - Styled link matching register link pattern

### Configuration Updates

**Location**: `ticketsystem.client/src/config.ts`

Add tickets endpoint to API_ENDPOINTS:
```typescript
export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: `${API_BASE_URL}/api/auth/login`,
    REGISTER: `${API_BASE_URL}/api/auth/register`,
  },
  TICKETS: {
    CREATE: `${API_BASE_URL}/api/tickets`,
  },
} as const;
```

### Routing Updates

**Location**: `ticketsystem.client/src/App.tsx`

Add new route:
```typescript
<Route path="/new-ticket" element={<NewTicket />} />
```

### Login Component Updates

**Location**: `ticketsystem.client/src/components/Login.tsx`

Add a new link section after the register link:
```typescript
<div className="new-ticket-link-container text-center mt-3">
  <span>Need to submit a ticket? </span>
  <button
    type="button"
    className="new-ticket-link"
    onClick={handleNewTicketClick}
  >
    Create Ticket
  </button>
</div>
```

Add corresponding CSS classes to Login.css to style the new link consistently with the register link.

## Data Models

### CreateTicketRequest (TypeScript Interface)

```typescript
interface CreateTicketRequest {
  fullName: string;
  email: string;
  issueDescription: string;
}
```

This interface matches the backend C# model:
- `FullName` (C#) → `fullName` (TypeScript)
- `Email` (C#) → `email` (TypeScript)
- `IssueDescription` (C#) → `issueDescription` (TypeScript)

### API Response Handling

The component will handle the following response scenarios:

**Success (200 OK)**:
- Display success message: "Ticket submitted successfully!"
- Clear all form fields
- Keep success message visible for user confirmation

**Client Error (400 Bad Request)**:
- Display error message based on response body or generic validation error
- Maintain form data so user can correct issues

**Server Error (500 Internal Server Error)**:
- Display generic error message: "An error occurred while submitting your ticket. Please try again."
- Maintain form data

**Network Error**:
- Display generic error message: "Unable to connect to the server. Please check your connection and try again."
- Maintain form data

## Error Handling

### Client-Side Validation

Before submitting the form, validate:
1. Full name is not empty or whitespace-only
2. Email is not empty or whitespace-only
3. Issue description is not empty or whitespace-only

Display error message: "All fields are required" if validation fails.

### API Error Handling

Wrap the fetch call in try-catch block:
- **Try block**: Handle HTTP response status codes
- **Catch block**: Handle network errors and unexpected exceptions
- **Finally block**: Reset loading state

### Error Message Display

Error messages will be displayed using Bootstrap alert styling:
```html
<div className="alert alert-danger" role="alert">
  {error}
</div>
```

### Success Message Display

Success messages will be displayed using Bootstrap alert styling:
```html
<div className="alert alert-success" role="alert">
  {success}
</div>
```

## Implementation Notes

### Design Consistency

The NewTicket component will maintain visual consistency with the Login component by:
- Using identical container/wrapper/card structure
- Applying the same color scheme (#F5B5A8 for primary actions)
- Matching font sizes, padding, and spacing
- Using the same responsive breakpoints
- Following the same form control styling

### Form Field Considerations

The issue description field will use a `<textarea>` element instead of `<input>` to allow multi-line text entry. The textarea will be styled to match the form-control styling of other inputs.

### State Management

Error and success messages are mutually exclusive:
- When displaying an error, success state is cleared
- When displaying success, error state is cleared
- When user types in any field, both error and success states are cleared

### Navigation Pattern

The component will use React Router's `useNavigate` hook for programmatic navigation, consistent with the Login component's approach.

### Accessibility

The form will include:
- Proper `<label>` elements with `htmlFor` attributes
- ARIA role attributes on alert messages
- Semantic HTML structure
- Minimum touch target sizes (44x44px)

### Loading State

During API submission:
- Disable the submit button
- Change button text to "Submitting..."
- Prevent multiple simultaneous submissions
