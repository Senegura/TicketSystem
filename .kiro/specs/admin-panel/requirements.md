# Requirements Document

## Introduction

This feature provides an admin panel for managing tickets with the ability to view all tickets in a table format, update ticket status and resolution text, and save changes in bulk. Admin users are redirected to this panel after login instead of the regular tickets list screen. The panel builds upon the view-tickets-ui functionality but adds editing capabilities for ticket status and resolution fields, with a save button to commit all changes to the server.

## Glossary

- **Admin Panel**: A UI component that displays all tickets in an editable table format for administrative users
- **Ticket System**: The application that manages support tickets
- **GetAllTickets Endpoint**: The backend API endpoint `https://localhost:7248/api/tickets` that retrieves all tickets from the server
- **UpdateTicket Endpoint**: The backend API endpoint `https://localhost:7248/api/tickets/{id}` that updates a specific ticket on the server
- **API Base URL**: The configurable base URL for backend API calls (default: `https://localhost:7248`)
- **Status Filter**: A dropdown control that filters tickets by their current status
- **Text Search**: A search input that filters tickets by name or description content
- **Admin User**: A user with administrative privileges (UserType = 2)
- **Regular User**: A non-administrative user (UserType = 1)
- **Ticket Status**: The current state of a ticket (New, In Progress, Resolved, Closed)
- **Resolution Text**: The administrative notes or solution description for a ticket
- **Bulk Update**: The ability to modify multiple tickets and save all changes in a single operation
- **Dirty State**: A ticket that has been modified but not yet saved to the server
- **Save Operation**: The process of sending all modified tickets to the server via PUT requests

## Requirements

### Requirement 1

**User Story:** As an admin user, I want to be redirected to the admin panel after login, so that I can immediately access ticket management functionality.

#### Acceptance Criteria

1. WHEN an admin user successfully logs in, THE Ticket System SHALL redirect the admin user to the admin panel screen
2. WHEN a regular user successfully logs in, THE Ticket System SHALL redirect the regular user to the read-only tickets list screen
3. THE Ticket System SHALL determine the redirect destination based on the UserType claim in the JWT token
4. THE Ticket System SHALL prevent access to the admin panel for unauthenticated users
5. THE Ticket System SHALL prevent access to the admin panel for regular users (UserType = 1)

### Requirement 2

**User Story:** As an admin user, I want to see all tickets displayed in a table format with the same filtering capabilities as the regular tickets list, so that I can quickly find and manage specific tickets.

#### Acceptance Criteria

1. THE Admin Panel SHALL retrieve all tickets by calling the GetAllTickets endpoint at `https://localhost:7248/api/tickets`
2. THE Admin Panel SHALL use a configurable API Base URL for constructing the endpoint URL
3. THE Admin Panel SHALL display tickets in a table with columns for ticket ID, name, status, summary, resolution, and created date
4. THE Admin Panel SHALL display a loading indicator while fetching ticket data from the server
5. WHEN the GetAllTickets endpoint returns an error, THE Admin Panel SHALL display an error message to the user
6. THE Admin Panel SHALL provide a status filter dropdown with options for all available ticket statuses
7. THE Admin Panel SHALL provide a text search input field for filtering by name or description
8. WHEN filters are applied, THE Admin Panel SHALL update the displayed tickets in real-time

### Requirement 3

**User Story:** As an admin user, I want to update the status of tickets directly in the table, so that I can efficiently manage ticket workflow.

#### Acceptance Criteria

1. THE Admin Panel SHALL display the ticket status as an editable dropdown control in each table row
2. THE Admin Panel SHALL populate the status dropdown with all available ticket status options (New, In Progress, Resolved, Closed)
3. WHEN an admin user changes a ticket status dropdown, THE Admin Panel SHALL mark that ticket as modified
4. THE Admin Panel SHALL preserve the selected status value in the UI until the save operation completes
5. THE Admin Panel SHALL allow multiple ticket statuses to be changed before saving
6. THE Admin Panel SHALL display the current status value from the server when the component loads

### Requirement 4

**User Story:** As an admin user, I want to update the resolution text for tickets directly in the table, so that I can document solutions and administrative notes.

#### Acceptance Criteria

1. THE Admin Panel SHALL display the ticket resolution as an editable text input field in each table row
2. WHEN an admin user modifies the resolution text, THE Admin Panel SHALL mark that ticket as modified
3. THE Admin Panel SHALL preserve the entered resolution text in the UI until the save operation completes
4. THE Admin Panel SHALL allow multiple ticket resolutions to be changed before saving
5. THE Admin Panel SHALL display the current resolution value from the server when the component loads
6. THE Admin Panel SHALL allow empty resolution text for tickets that have not been resolved

### Requirement 5

**User Story:** As an admin user, I want to save all modified tickets with a single button click, so that I can efficiently commit multiple changes at once.

#### Acceptance Criteria

1. THE Admin Panel SHALL provide a "Save" button that is visible to admin users
2. WHEN no tickets have been modified, THE Admin Panel SHALL disable the Save button
3. WHEN one or more tickets have been modified, THE Admin Panel SHALL enable the Save button
4. WHEN the Save button is clicked, THE Admin Panel SHALL send a PUT request to the UpdateTicket endpoint for each modified ticket
5. THE Admin Panel SHALL use the endpoint pattern `https://localhost:7248/api/tickets/{id}` for each update request
6. THE Admin Panel SHALL include the complete ticket object in the request body for each PUT request
7. THE Admin Panel SHALL send update requests sequentially or in parallel based on implementation choice
8. THE Admin Panel SHALL display a loading indicator while save operations are in progress
9. THE Admin Panel SHALL disable the Save button while save operations are in progress

### Requirement 6

**User Story:** As an admin user, I want to see clear feedback about the save operation results, so that I know which tickets were successfully updated and which failed.

#### Acceptance Criteria

1. WHEN all ticket updates succeed, THE Admin Panel SHALL display a success message indicating the number of tickets updated
2. WHEN one or more ticket updates fail, THE Admin Panel SHALL display an error message listing the failed ticket IDs
3. WHEN ticket updates complete successfully, THE Admin Panel SHALL clear the modified state for those tickets
4. WHEN ticket updates complete successfully, THE Admin Panel SHALL refresh the ticket data from the server
5. WHEN ticket updates fail due to authentication errors (401), THE Admin Panel SHALL redirect the user to the login page
6. WHEN ticket updates fail due to authorization errors (403), THE Admin Panel SHALL display an access denied message
7. THE Admin Panel SHALL display specific error messages for network failures and server errors

### Requirement 7

**User Story:** As an admin user, I want modified tickets to be visually distinguished from unmodified tickets, so that I can see which changes are pending save.

#### Acceptance Criteria

1. WHEN a ticket has been modified, THE Admin Panel SHALL apply a visual indicator to that ticket row
2. THE Admin Panel SHALL use a distinct background color or border style for modified ticket rows
3. WHEN a ticket is saved successfully, THE Admin Panel SHALL remove the visual indicator from that ticket row
4. THE Admin Panel SHALL maintain visual indicators across filter changes (modified tickets remain highlighted when filters are applied)

### Requirement 8

**User Story:** As an admin user, I want the admin panel to have a clean and organized layout similar to the regular tickets list, so that the interface is familiar and easy to use.

#### Acceptance Criteria

1. THE Admin Panel SHALL organize filters in a clearly visible section above the table
2. THE Admin Panel SHALL display the Save button in a prominent location (e.g., top-right of the filters section)
3. THE Admin Panel SHALL use consistent spacing and typography throughout the interface
4. THE Admin Panel SHALL display the table with proper column alignment and row spacing
5. THE Admin Panel SHALL be responsive and display properly on different screen sizes
6. THE Admin Panel SHALL use styling consistent with other components in the Ticket System

### Requirement 9

**User Story:** As an admin user, I want to click on a ticket row to view full ticket details, so that I can access complete information about a specific ticket while managing the list.

#### Acceptance Criteria

1. WHEN an admin user clicks on a non-editable area of a ticket row, THE Ticket System SHALL navigate to the ticket detail screen
2. THE Ticket System SHALL pass the ticket ID to the ticket detail screen for data retrieval
3. THE Admin Panel SHALL NOT navigate to the detail screen when clicking on editable fields (status dropdown, resolution input)
4. THE Admin Panel SHALL provide visual feedback when hovering over clickable areas of ticket rows
5. WHEN an admin user navigates to a ticket detail and returns to the admin panel, THE Admin Panel SHALL preserve any unsaved changes

### Requirement 10

**User Story:** As an admin user, I want to be warned before navigating away from the admin panel with unsaved changes, so that I don't accidentally lose my work.

#### Acceptance Criteria

1. WHEN an admin user attempts to navigate away from the admin panel with unsaved changes, THE Admin Panel SHALL display a confirmation dialog
2. THE confirmation dialog SHALL clearly indicate that unsaved changes will be lost
3. WHEN the admin user confirms navigation, THE Admin Panel SHALL allow the navigation to proceed
4. WHEN the admin user cancels navigation, THE Admin Panel SHALL remain on the current screen with changes preserved
5. THE Admin Panel SHALL NOT display a confirmation dialog when there are no unsaved changes
