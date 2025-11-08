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

Since this is a UI-only implementation without business logic, we will:
- Replace the current App.tsx content with the Login component as the initial view
- Add a placeholder navigation structure for the Register link (route will be defined but component implementation is deferred)
- Use React Router DOM for client-side routing (to be installed)

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

**Purpose:** Renders the login screen UI with form inputs and navigation

**Props:** None (stateless UI component for now)

**State:**
- `email: string` - Controlled input for email field
- `password: string` - Controlled input for password field

**Key Features:**
- Controlled form inputs using React useState
- Event handlers for input changes (no submission logic yet)
- Navigation to register route using React Router
- Responsive layout using Bootstrap grid system

**Component Structure:**
```typescript
interface LoginProps {}

const Login: React.FC<LoginProps> = () => {
  const [email, setEmail] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  
  // Event handlers for controlled inputs
  // Navigation handler for register link
  
  return (
    // JSX structure
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

### Form State Interface

```typescript
interface LoginFormState {
  email: string;
  password: string;
}
```

No backend data models are needed for this UI-only implementation.

## Error Handling

Since this is a UI-only implementation without business logic:
- No form validation is implemented yet
- No error messages are displayed
- Input fields accept any text without validation
- Button click handlers are placeholders (no-op functions)

Error handling will be added in future iterations when authentication logic is implemented.

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

### Future Enhancements (Out of Scope)

The following will be implemented in future iterations:
- Form validation (email format, password requirements)
- Authentication business logic
- API integration for login
- Error message display
- Loading states
- "Remember me" checkbox
- "Forgot password" link
- Success/failure feedback
- Session management
