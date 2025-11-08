# Implementation Plan

- [x] 1. Update API configuration





  - Add TICKETS endpoint configuration to config.ts with CREATE endpoint pointing to `${API_BASE_URL}/api/tickets`
  - _Requirements: 1.4_
- [x] 2. Create NewTicket component styling




- [ ] 2. Create NewTicket component styling

  - Create NewTicket.css file in styles directory
  - Implement all CSS classes mirroring Login.css structure: container, wrapper, card, heading, button, and link styles
  - Apply #F5B5A8 color scheme for primary button
  - Include responsive breakpoints for mobile and large screens
  - Style textarea element to match form-control styling
  - _Requirements: 1.3_

- [x] 3. Implement NewTicket component



- [x] 3.1 Create component file and basic structure


  - Create NewTicket.tsx in components directory
  - Import required dependencies (useState, useNavigate, API_ENDPOINTS)
  - Import NewTicket.css
  - Define CreateTicketRequest TypeScript interface
  - Set up component function with state variables for fullName, email, issueDescription, error, success, and isLoading
  - _Requirements: 1.1, 1.2_

- [x] 3.2 Implement form input handlers


  - Write handleFullNameChange function to update state and clear messages
  - Write handleEmailChange function to update state and clear messages
  - Write handleIssueDescriptionChange function to update state and clear messages
  - Write handleBackToLogin function using useNavigate to navigate to root path
  - _Requirements: 1.2, 3.3_

- [x] 3.3 Implement form submission logic


  - Write handleSubmit function with form validation for all required fields
  - Implement POST request to tickets API endpoint with proper headers and body
  - Handle success response (200 OK) by displaying success message and clearing form fields
  - Handle error responses (400, 500) by displaying appropriate error messages
  - Handle network errors in catch block
  - Manage loading state in finally block
  - _Requirements: 1.4, 3.1, 3.2, 3.3, 3.4, 4.1, 4.2, 4.3, 4.4_

- [x] 3.4 Build component JSX structure


  - Create container/wrapper/card structure matching Login component
  - Add heading "Submit a Ticket"
  - Render error alert when error state is set
  - Render success alert when success state is set
  - Create form with three fields: Full Name (input), Email (input), Issue Description (textarea)
  - Add submit button with loading state handling
  - Add back to login link section
  - _Requirements: 1.2, 1.3, 3.1, 3.4, 4.1, 4.3_

- [x] 4. Add routing for NewTicket component



  - Import NewTicket component in App.tsx
  - Add route for /new-ticket path with NewTicket element
  - _Requirements: 1.1_

- [x] 5. Update Login component with navigation link





  - Add handleNewTicketClick function in Login.tsx using useNavigate to navigate to /new-ticket
  - Add new-ticket-link-container div after register-link-container with "Need to submit a ticket?" text and "Create Ticket" button
  - Add CSS classes to Login.css for new-ticket-link-container and new-ticket-link matching register link styling
  - _Requirements: 2.1, 2.2, 2.3_
