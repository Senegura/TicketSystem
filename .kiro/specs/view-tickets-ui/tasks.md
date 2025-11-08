# Implementation Plan

- [x] 1. Create backend API endpoint for GetAllTickets





  - Create TicketsController in `TicketSystem.Server/Controllers/TicketsController.cs`
  - Implement GET `/api/tickets` endpoint that calls `ITicketService.GetAllTicketsAsync()`
  - Add proper error handling and return appropriate HTTP status codes
  - Configure dependency injection for ITicketService in Program.cs if not already configured
  - _Requirements: 2.1, 2.4_

- [x] 2. Create ViewTickets component structure and routing





  - Create `ticketsystem.client/src/components/ViewTickets.tsx` with basic component structure
  - Update route `/tickets` in `App.tsx` to use ViewTickets component instead of placeholder
  - Set up component state for tickets, filteredTickets, loading, error, statusFilter, and searchText
  - Implement useEffect hook to trigger data fetch on component mount
  - _Requirements: 2.1, 2.3_

- [x] 3. Create API configuration file


  - Create `ticketsystem.client/src/config/api.ts` with API_CONFIG object
  - Define baseUrl property with value `https://localhost:7248`
  - Export API_CONFIG for use in components
  - _Requirements: 2.1, 2.2_

- [x] 4. Implement API integration and data fetching



  - Import API_CONFIG from config file
  - Create `fetchTickets` function to call `${API_CONFIG.baseUrl}/api/tickets` endpoint
  - Include `credentials: 'include'` in fetch options to send authentication cookies
  - Implement error handling for authentication errors (401, 403), server errors, and network errors
  - Redirect to login page on 401 Unauthorized
  - Implement loading state management during API calls
  - Handle empty response scenario
  - Add retry functionality for recoverable errors
  - _Requirements: 2.1, 2.3, 2.4, 2.5_

- [x] 5. Implement filtering logic
  - Create `applyFilters` function that combines status and text search filters
  - Implement `handleStatusFilterChange` function to update status filter state
  - Implement `handleSearchChange` function to update search text state
  - Implement case-insensitive text matching for name and description fields
  - Update filteredTickets state whenever filters change
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 4.1, 4.2, 4.3, 4.4, 4.5, 5.1, 5.2, 5.3_

- [x] 6. Create filters UI section

  - Implement status dropdown with "All Statuses" option and all TicketStatus enum values
  - Implement text search input field with placeholder text
  - Wire up onChange handlers to filter functions
  - Apply consistent styling with other components
  - _Requirements: 3.1, 3.4, 4.1, 4.5, 8.1_

- [x] 7. Create table layout and structure





  - Implement table with columns: ID, Name, Status, Summary, Created Date
  - Create table header row with column labels
  - Implement table body with ticket rows
  - Add proper semantic HTML (table, thead, tbody, tr, th, td)
  - _Requirements: 2.2, 8.1, 8.3_

- [x] 8. Implement ticket row rendering and navigation





  - Map filteredTickets array to table rows
  - Display ticket data in appropriate columns (ID, name, status badge, summary, formatted date)
  - Implement `handleTicketClick` function using useNavigate to navigate to `/tickets/:id`
  - Add onClick handler to table rows
  - Add hover effect styling to indicate rows are clickable
  - _Requirements: 2.2, 6.1, 6.2, 7.1, 7.2, 7.3_

- [x] 9. Implement status badge display in table



  - Create status label mapping from enum values to display text
  - Implement `getStatusLabel` function
  - Implement `getStatusColor` function to return appropriate CSS class
  - Render status badges in status column with color coding
  - _Requirements: 2.2_

- [x] 10. Implement date formatting










  - Create `formatDate` function to convert ISO date strings to readable format
  - Apply date formatting to createdAt field in table
  - Ensure consistent date format across the application
  - _Requirements: 2.2_

- [x] 11. Create error and loading states UI




  - Implement loading spinner with "Loading tickets..." message
  - Create error message component for server errors
  - Create error message component for network errors
  - Add retry button functionality for recoverable errors
  - Implement empty state for no tickets found
  - Implement empty state for no matching filters
  - _Requirements: 2.3, 2.4, 5.3_

- [x] 12. Implement ViewTickets component styling




  - Create `ticketsystem.client/src/styles/ViewTickets.css` file
  - Apply card-based layout matching ViewTicket and NewTicket design patterns
  - Style container with `#f5f5f5` background and centered layout
  - Style card with white background, 16px border radius, and shadow
  - Style filters section with horizontal layout
  - Style table with clean borders, header styling, and alternating row colors
  - Style status badges with appropriate colors (New: blue, InProgress: orange, Resolved: green, Closed: gray)
  - Add hover effects for clickable table rows
  - _Requirements: 7.3, 8.1, 8.2, 8.3_

- [x] 13. Add responsive design and mobile support



  - Implement desktop styles for screens >1400px
  - Implement tablet styles for screens 768px-1400px
  - Implement mobile styles for screens <768px (vertical filter layout)
  - Implement mobile card-based layout for screens <576px (replace table with cards)
  - Ensure touch targets meet minimum 44x44px requirement
  - Test table responsiveness and readability on different screen sizes
  - _Requirements: 8.4_

- [x] 14. Update Login component to redirect to tickets list




  - Modify Login component to navigate to `/tickets` after successful authentication
  - Ensure both regular users and admins are redirected to `/tickets`
  - _Requirements: 1.1, 1.2_

- [x] 15. Add accessibility features




  - Use semantic HTML elements (table, thead, tbody)
  - Implement proper heading hierarchy (h1 for page title)
  - Add ARIA labels for filters, status badges, and loading indicators
  - Add keyboard navigation support for table rows (Enter key to navigate)
  - Ensure sufficient color contrast for all text and status badges
  - Add focus indicators for interactive elements
  - Add screen reader announcements for filter changes
  - _Requirements: 8.1, 8.2, 8.3_
