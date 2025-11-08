# Requirements Document

## Introduction

This feature implements a responsive login screen UI for the TicketSystem React application. The login screen will be displayed when the application starts and will provide users with a clean, mobile-friendly interface to enter their credentials. The design follows the provided mockup with email and password fields, a login button, and a link to navigate to a registration screen.

## Glossary

- **Login Screen**: The initial user interface component displayed when the React application starts, allowing users to authenticate
- **React Application**: The frontend SPA (Single Page Application) built with React 19.1.1 and TypeScript
- **Bootstrap**: The CSS framework (version to be determined) used for responsive design and mobile compatibility
- **Register Link**: A clickable navigation element that directs users to the registration screen
- **Mobile Device**: Smartphones and tablets with screen widths typically ranging from 320px to 768px
- **Responsive Design**: UI layout that adapts to different screen sizes and orientations
- **Authentication Token**: A JWT (JSON Web Token) returned by the server upon successful login in both the response body and as an HTTP-only cookie named "AuthToken"
- **AuthToken Cookie**: An HTTP-only cookie automatically set by the server upon successful authentication, used to authenticate subsequent API requests
- **UserType**: An enumeration indicating the user's role in the system (Customer, User, or Admin)
- **Login Endpoint**: The server API endpoint at "/api/auth/login" that validates user credentials and returns authentication information
- **Error Message**: A user-visible text notification displayed on the Login Screen when authentication fails or an error occurs

## Requirements

### Requirement 1

**User Story:** As a user opening the TicketSystem application, I want to see a login screen immediately, so that I can authenticate before accessing the system

#### Acceptance Criteria

1. WHEN the React application starts, THE Login Screen SHALL be displayed as the initial view
2. THE Login Screen SHALL render a centered card layout with a white background
3. THE Login Screen SHALL display the title "Login" at the top of the form
4. THE Login Screen SHALL remain visible until the user successfully authenticates or navigates away

### Requirement 2

**User Story:** As a user, I want to enter my email address and password, so that I can provide my credentials for authentication

#### Acceptance Criteria

1. THE Login Screen SHALL display an "Email address" label above the email input field
2. THE Login Screen SHALL provide a text input field for email address entry
3. THE Login Screen SHALL display a "Password" label above the password input field
4. THE Login Screen SHALL provide a password input field that masks entered characters
5. THE Login Screen SHALL render input fields with rounded corners and light borders matching the design mockup

### Requirement 3

**User Story:** As a user, I want to submit my login credentials using a clearly visible button, so that I can attempt to authenticate

#### Acceptance Criteria

1. THE Login Screen SHALL display a "Login" button below the password field
2. THE Login Screen SHALL render the login button with a coral/salmon pink background color (#F5B5A8 or similar)
3. THE Login Screen SHALL render the login button with white text
4. THE Login Screen SHALL render the login button spanning the full width of the form
5. THE Login Screen SHALL render the login button with rounded corners

### Requirement 4

**User Story:** As a new user without an account, I want to navigate to a registration screen, so that I can create a new account

#### Acceptance Criteria

1. THE Login Screen SHALL display the text "Need an account?" below the login button
2. THE Login Screen SHALL display a "Register" link adjacent to the "Need an account?" text
3. THE Login Screen SHALL render the "Register" link in a coral/salmon pink color matching the login button
4. WHEN the user clicks the Register link, THE Login Screen SHALL navigate to the registration route
5. THE Login Screen SHALL render the registration link text without an underline by default

### Requirement 5

**User Story:** As a mobile user, I want the login screen to be fully functional on my smartphone, so that I can access the system from any device

#### Acceptance Criteria

1. THE Login Screen SHALL use Bootstrap responsive utilities for layout
2. WHEN displayed on mobile devices with screen width less than 768px, THE Login Screen SHALL render the form at full width with appropriate padding
3. WHEN displayed on tablet and desktop devices, THE Login Screen SHALL render the form in a centered container with maximum width constraints
4. THE Login Screen SHALL maintain readable font sizes on mobile devices (minimum 16px for inputs to prevent zoom on iOS)
5. THE Login Screen SHALL ensure touch targets (buttons and links) are at least 44x44 pixels for mobile usability
6. THE Login Screen SHALL render correctly in both portrait and landscape orientations on mobile devices

### Requirement 6

**User Story:** As a user, I want my credentials to be authenticated against the server, so that I can securely access the system

#### Acceptance Criteria

1. WHEN the user clicks the Login button, THE Login Screen SHALL send an HTTP POST request to "/api/auth/login" with username and password
2. THE Login Screen SHALL include the username and password in the request body as JSON
3. WHEN the server returns a 200 OK response, THE Login Screen SHALL extract the token and user information from the response
4. WHEN the server returns a 401 Unauthorized response, THE Login Screen SHALL display an error message "Invalid username or password"
5. WHEN the server returns a 400 Bad Request response, THE Login Screen SHALL display an error message "Username and password are required"
6. WHEN the server returns any other error response, THE Login Screen SHALL display an error message "An error occurred during login. Please try again."

### Requirement 7

**User Story:** As an authenticated user, I want to be redirected to the appropriate screen based on my user type, so that I can access the features relevant to my role

#### Acceptance Criteria

1. WHEN authentication succeeds and the UserType is Customer, THE Login Screen SHALL navigate to the create support tickets screen
2. WHEN authentication succeeds and the UserType is User, THE Login Screen SHALL navigate to the view tickets screen
3. WHEN authentication succeeds and the UserType is Admin, THE Login Screen SHALL navigate to the view tickets screen
4. THE Login Screen SHALL include credentials in API requests to allow the server to set the AuthToken HTTP-only cookie
5. THE Login Screen SHALL clear any displayed error messages upon successful authentication
