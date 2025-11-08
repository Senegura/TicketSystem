# Design Document

## Overview

This design document outlines the implementation of a responsive login screen UI for the TicketSystem React application. The login screen will be the initial view displayed when the application starts, featuring a clean, centered form with email and password inputs, a login button, and a link to the registration screen. The implementation will use Bootstrap 5.3.8 (already installed) for responsive design and mobile compatibility.

## Architecture

### Component Structure

```
App.tsx (Root)
└── Login.tsx (Login Screen Component)
    ├── Email Input Field
    ├── Password Input Field
    ├── Login Button
    └── Register Link
```

### Routing Strategy

The application will use React Router DOM for navigation:
- Login component at "/" route (initial view)
- Register route at "/register" (placeholder for future implementation)
- Create tickets route at "/create-ticket" (placeholder for Customer users)
- View tickets route at "/tickets" (placeholder for User and Admin users)
- Navigation occurs after successful authentication based on UserType

### File Structure

```
ticketsystem.client/src/
├── components/
│   └── Login.tsx          # Login screen component
├── styles/
│   └── Login.css          # Login-specific styles
├── App.tsx                # Updated to show Login as initial view
├── App.css                # Existing app styles
├── main.tsx               # Entry point (minimal changes)
└── index.css              # Global styles
```

## Components and Interfaces

### Login Component

**File:** `src/components/Login.tsx`

**Purpose:** Renders the login screen UI with form inputs, handles authentication, and navigates based on user type

**Props:** None

**State:**
- `username: string` - Controlled input for username/email field
- `password: string` - Controlled input for password field
- `error: string` - Error message to display when authentication fails
- `isLoading: boolean` - Loading state during API call

**Key Features:**
- Controlled form inputs using React useState
- Form submission handler that calls the login API endpoint
- Error message display for authentication failures
- Navigation based on UserType using React Router
- Responsive layout using Bootstrap grid system

**Component Structure:**
```typescript
interface LoginProps {}

interface LoginResponse {
  token: string;
  userId: number;
  userType: 'Customer' | 'User' | 'Admin';
}

const Login: React.FC<LoginProps> = () => {
  const [username, setUsername] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const navigate = useNavigate();
  
  const handleSubmit = async (e: React.FormEvent) => {
    // Prevent default form submission
    // Clear previous errors
    // Validate inputs
    // Call API endpoint
    // Handle response and navigate
  };
  
  return (
    // JSX structure with error display
  );
};
```

### Styling Approach

**Bootstrap Classes Used:**
- `container` - Main wrapper for centering
- `row` and `col` - Grid system for responsive layout
- `form-control` - Input field styling
- `btn` - Button styling
- `text-center` - Text alignment
- Responsive utilities: `col-12`, `col-md-6`, `col-lg-4`

**Custom CSS (Login.css):**
- Login card background and shadow
- Coral/salmon button color (#F5B5A8)
- Custom input field styling (rounded corners, borders)
- Register link styling (color, hover effects)
- Mobile-specific adjustments

## Data Models

### Login Request

```typescript
interface LoginRequest {
  username: string;
  password: string;
}
```

### Login Response

```typescript
interface LoginResponse {
  token: string;
  userId: number;
  userType: 'Customer' | 'User' | 'Admin';
}
```

### UserType Enum

The UserType values from the server response:
- `'Customer'` - Regular customer users who create support tickets
- `'User'` - Support staff who view and manage tickets
- `'Admin'` - Administrators who view and manage tickets with elevated permissions

## API Integration

### Login Endpoint

**URL:** `/api/auth/login`  
**Method:** POST  
**Content-Type:** application/json

**Request Body:**
```json
{
  "username": "user@example.com",
  "password": "password123"
}
```

**Success Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 123,
  "userType": "Customer"
}
```

**Error Responses:**
- **400 Bad Request:** Missing username or password
- **401 Unauthorized:** Invalid credentials
- **500 Internal Server Error:** Server error

**Cookie Behavior:**
The server automatically sets an HTTP-only cookie named "AuthToken" upon successful authentication. The browser will automatically include this cookie in subsequent requests to the same domain. No client-side cookie management is required.

### Fetch Configuration

API calls must include credentials to allow the browser to send and receive cookies:

```typescript
fetch('/api/auth/login', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  credentials: 'include', // Important: allows cookies to be sent/received
  body: JSON.stringify({ username, password })
})
```

## Error Handling

### Client-Side Validation

Before making the API call:
- Check if username and password fields are not empty
- Display error message if either field is empty

### API Error Handling

The component will handle different HTTP status codes:

**400 Bad Request:**
- Display: "Username and password are required"

**401 Unauthorized:**
- Display: "Invalid username or password"

**Network Errors or Other Status Codes:**
- Display: "An error occurred during login. Please try again."

### Error Display

- Error messages will be displayed above the form in a Bootstrap alert component
- Error styling: Red text or Bootstrap danger alert
- Errors will be cleared when:
  - User starts typing in either input field
  - Successful authentication occurs
  - User clicks the login button again (new attempt)

### Loading State

- Disable the login button while the API request is in progress
- Show loading indicator (text change or spinner) on the button
- Prevent multiple simultaneous login attempts

## Implementation Notes

### Bootstrap Integration

Bootstrap 5.3.8 is already installed and imported in `main.tsx`:
```typescript
import 'bootstrap/dist/css/bootstrap.min.css'
```

No additional installation is required.

### React Router Setup

React Router DOM needs to be installed for navigation:
```bash
npm install react-router-dom
npm install --save-dev @types/react-router-dom
```

Basic routing structure:
```typescript
<BrowserRouter>
  <Routes>
    <Route path="/" element={<Login />} />
    <Route path="/register" element={<div>Register (Coming Soon)</div>} />
  </Routes>
</BrowserRouter>
```

### Responsive Breakpoints

Following Bootstrap 5 breakpoints:
- **Mobile (< 576px):** Full-width form with padding
- **Tablet (576px - 768px):** Centered form, 80% width
- **Desktop (> 768px):** Centered form, max 400px width

### Color Palette

Based on the provided design mockup:
- **Primary Button:** #F5B5A8 (coral/salmon pink)
- **Button Text:** #FFFFFF (white)
- **Register Link:** #F5B5A8 (matching button color)
- **Background:** #F5F5F5 (light gray)
- **Card Background:** #FFFFFF (white)
- **Input Borders:** #CCCCCC (light gray)
- **Text:** #333333 (dark gray)

### Typography

- **Heading (Login):** Bold, 48px, centered
- **Labels:** Regular, 18px, left-aligned
- **Input Text:** Regular, 16px (minimum for mobile to prevent zoom)
- **Button Text:** Bold, 18px, white
- **Register Text:** Regular, 16px

### Accessibility Considerations

- Proper label associations using `htmlFor` attribute
- Semantic HTML elements (`<form>`, `<label>`, `<input>`, `<button>`)
- Sufficient color contrast for text and backgrounds
- Touch targets minimum 44x44px for mobile
- Keyboard navigation support (native form elements)

### Navigation Logic

After successful authentication, the component will navigate based on UserType:

```typescript
const handleLoginSuccess = (response: LoginResponse) => {
  setError('');
  
  switch (response.userType) {
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
};
```

### Placeholder Routes

The following routes will be defined in App.tsx but will show placeholder content:
- `/create-ticket` - "Create Ticket (Coming Soon)" message
- `/tickets` - "View Tickets (Coming Soon)" message

These routes will be implemented in future specs.

### Future Enhancements (Out of Scope)

The following will be implemented in future iterations:
- Advanced form validation (email format, password requirements)
- "Remember me" checkbox
- "Forgot password" link
- Session timeout handling
- Token refresh logic
- Logout functionality
