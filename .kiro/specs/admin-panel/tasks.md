# Implementation Plan

- [x] 1. Create AdminPanel component structure and basic layout



  - Create `ticketsystem.client/src/components/AdminPanel.tsx` with component skeleton
  - Create `ticketsystem.client/src/styles/AdminPanel.css` with base styling copied from ViewTickets.css
  - Set up component state for tickets, filteredTickets, modifiedTickets (Map), loading, saving, error states, and filters
  - Implement basic JSX structure with header, filters section, and table placeholder
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6_

- [x] 2. Implement authentication and authorization logic





  - Add admin access validation on component mount to check UserType claim from JWT token
  - Implement redirect to `/tickets` or access denied message for non-admin users (UserType !== 2)
  - Implement redirect to `/login` for unauthenticated users
  - _Requirements: 1.3, 1.4, 1.5_

- [x] 3. Implement ticket fetching and display functionality





  - Implement `fetchTickets()` function to call GET `/api/tickets` endpoint with credentials
  - Add loading state display with spinner and "Loading tickets..." message
  - Add error handling for 401, 403, 500, and network errors
  - Display tickets in table format with columns: ID, Name, Status, Summary, Resolution, Created Date
  - Implement empty state display for no tickets
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [x] 4. Implement filtering functionality





  - Add status filter dropdown with all ticket status options and "All Statuses" option
  - Add text search input field for filtering by name or description
  - Implement `applyFilters()` function for real-time filtering
  - Implement `handleStatusFilterChange()` and `handleSearchChange()` functions
  - Add empty filter state display when no tickets match filters
  - _Requirements: 2.6, 2.7, 2.8_

- [x] 5. Implement editable status dropdown in table rows





  - Replace static status badge with editable dropdown control in each table row
  - Populate dropdown with all ticket status options (New, In Progress, Resolved, Closed)
  - Implement `handleTicketStatusChange()` function to update ticket status and mark as modified
  - Add modified ticket to modifiedTickets Map when status changes
  - Display current status value from server or modified state
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6_
-

- [x] 6. Implement editable resolution text input in table rows




  - Add text input field for resolution in each table row
  - Implement `handleTicketResolutionChange()` function to update resolution text and mark as modified
  - Add modified ticket to modifiedTickets Map when resolution changes
  - Display current resolution value from server or modified state
  - Allow empty resolution text
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6_
- [x] 7. Implement visual indicators for modified tickets




- [ ] 7. Implement visual indicators for modified tickets

  - Add CSS class to modified ticket rows for visual distinction (light yellow background or left border)
  - Implement `isTicketModified()` function to check if ticket is in modifiedTickets Map
  - Apply visual indicator class conditionally based on modified state
  - Maintain visual indicators across filter changes
  - _Requirements: 7.1, 7.2, 7.3, 7.4_
-

- [x] 8. Implement Save button and bulk update functionality




  - Add Save button to filters/actions section (right-aligned)
  - Implement button enabled/disabled state based on modifiedTickets.size
  - Implement `handleSave()` function to send PUT requests for all modified tickets
  - Send PUT request to `/api/tickets/{id}` for each modified ticket with complete ticket object
  - Display loading state on Save button while saving (spinner, "Saving..." text)
  - Disable Save button and editable fields during save operation
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 5.9_
-

- [x] 9. Implement save operation feedback and error handling




  - Display success message banner when all updates succeed with count of updated tickets
  - Display error message banner when updates fail with list of failed ticket IDs
  - Clear modified state for successfully updated tickets
  - Refresh ticket data from server after successful save
  - Handle 401 errors with redirect to login
  - Handle 403 errors with access denied message
  - Handle partial save failures with warning message and retry option
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7_
-

- [x] 10. Implement navigation to ticket detail view




  - Implement `handleTicketClick()` function to navigate to `/tickets/:id`
  - Prevent navigation when clicking on editable fields (status dropdown, resolution input)
  - Add click event handler to table rows with proper event target checking
  - Add keyboard navigation support (Enter key) for ticket rows
  - Add hover effect for clickable areas (non-editable)
  - _Requirements: 9.1, 9.2, 9.3, 9.4_

- [x] 11. Implement unsaved changes warning





  - Add `beforeunload` event listener to warn when leaving page with unsaved changes
  - Display browser confirmation dialog with message about losing unsaved changes
  - Implement React Router navigation blocker for in-app navigation with unsaved changes
  - Display custom confirmation dialog when navigating within app
  - Remove warning when no unsaved changes exist
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5_

- [x] 12. Update Login component to redirect admins to admin panel




  - Modify Login component's post-login redirect logic
  - Check UserType from JWT token response or decoded token
  - Redirect to `/admin` if UserType = 2 (Admin)
  - Redirect to `/tickets` if UserType = 1 (User)
  - _Requirements: 1.1, 1.2_
-

- [x] 13. Add admin panel route to App.tsx




  - Import AdminPanel component in App.tsx
  - Add route for `/admin` path with AdminPanel component
  - Ensure route is protected (authentication handled by component)
  - _Requirements: 1.1_
-

- [x] 14. Style AdminPanel component for consistency and responsiveness




  - Apply consistent styling with ViewTickets component (colors, fonts, spacing)
  - Style Save button with enabled/disabled/loading states
  - Style editable fields (status dropdown, resolution input) with focus states
  - Style modified row indicator (background color or border)
  - Style success and error message banners
  - Implement responsive design for tablet and mobile screens
  - Ensure accessibility with proper focus indicators and ARIA labels
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6_
