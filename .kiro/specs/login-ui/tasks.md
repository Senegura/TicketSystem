# Implementation Plan

- [x] 1. Install and configure React Router DOM





  - Install react-router-dom and its TypeScript types
  - Update main.tsx to wrap App with BrowserRouter
  - _Requirements: 1.1, 4.4_

- [x] 2. Create Login component with form structure





  - Create src/components/Login.tsx file
  - Implement component with email and password state using useState
  - Add controlled input fields for email and password
  - Add login button (no functionality yet, just UI)
  - Add "Need an account? Register" link with navigation to /register route
  - _Requirements: 1.2, 1.3, 2.1, 2.2, 2.3, 2.4, 2.5, 3.1, 4.1, 4.2, 4.3, 4.4_

- [x] 3. Implement Bootstrap responsive layout





  - Add Bootstrap container, row, and col classes for responsive grid
  - Configure column widths for mobile (col-12), tablet (col-md-8, col-lg-6), and desktop (col-xl-4)
  - Center the form card on the page using flexbox utilities
  - _Requirements: 5.1, 5.2, 5.3_

- [x] 4. Create custom styles for Login component





  - Create src/styles/Login.css file
  - Style the login card with white background, shadow, and rounded corners
  - Style the "Login" heading with proper size and weight
  - Style input fields with rounded corners and light borders matching the design
  - Style the login button with coral/salmon background (#F5B5A8), white text, full width, and rounded corners
  - Style the register link with coral/salmon color and remove default underline
  - Add hover effects for button and link
  - Ensure minimum 16px font size for inputs on mobile
  - Ensure touch targets are at least 44x44px
  - _Requirements: 2.5, 3.2, 3.3, 3.4, 3.5, 4.3, 4.5, 5.4, 5.5_

- [x] 5. Update App.tsx to use routing





  - Import BrowserRouter, Routes, and Route from react-router-dom
  - Define route for "/" that renders Login component
  - Define placeholder route for "/register" with temporary content
  - Remove existing weather forecast demo code
  - _Requirements: 1.1, 4.4_


- [x] 6. Verify responsive behavior




  - Test layout on mobile viewport (< 576px)
  - Test layout on tablet viewport (576px - 768px)
  - Test layout on desktop viewport (> 768px)
  - Verify form renders correctly in portrait and landscape orientations
  - _Requirements: 5.2, 5.3, 5.6_

- [x] 7. Add API integration types and interfaces





  - Create TypeScript interfaces for LoginRequest and LoginResponse in Login.tsx
  - Define UserType type union ('Customer' | 'User' | 'Admin')
  - _Requirements: 6.1, 6.2, 6.3, 7.1, 7.2, 7.3_
-

- [x] 8. Implement error state and loading state




  - Add error state to Login component using useState
  - Add isLoading state to Login component using useState
  - Add error message display above the form using Bootstrap alert component
  - Update login button to show loading state and disable during API call
  - Clear error when user types in input fields
  - _Requirements: 6.4, 6.5, 6.6, 7.5_
-

- [x] 9. Implement login API call handler




  - Create handleSubmit function for form submission
  - Prevent default form submission behavior
  - Validate username and password are not empty
  - Make POST request to /api/auth/login with credentials: 'include'
  - Handle 200 OK response and extract token, userId, and userType
  - Handle 400, 401, and other error responses with appropriate error messages
  - Set loading state before and after API call
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 7.4_
- [x] 10. Implement navigation based on UserType




- [ ] 10. Implement navigation based on UserType

  - Import useNavigate hook from react-router-dom
  - Create navigation logic that checks UserType from response
  - Navigate to /create-ticket for Customer users
  - Navigate to /tickets for User and Admin users
  - Clear error message on successful authentication
  - _Requirements: 7.1, 7.2, 7.3, 7.5_
- [x] 11. Add placeholder routes for future screens




- [ ] 11. Add placeholder routes for future screens

  - Add /create-ticket route in App.tsx with placeholder component
  - Add /tickets route in App.tsx with placeholder component
  - Display "Coming Soon" messages for placeholder routes
  - _Requirements: 7.1, 7.2, 7.3_
-

- [x] 12. Update form to use username instead of email




  - Change email state variable to username
  - Update label text from "Email address" to "Username"
  - Update input field id and name attributes
  - Ensure consistency with server API expectations
  - _Requirements: 6.1, 6.2_
