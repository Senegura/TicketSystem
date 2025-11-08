/**
 * Date formatting utilities for consistent date display across the application
 */

/**
 * Formats a date string to a short date format (e.g., "Jan 15, 2024")
 * Used in list views and tables where space is limited
 * @param dateString - ISO date string
 * @returns Formatted date string
 */
export const formatDateShort = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });
};

/**
 * Formats a date string to a full date and time format (e.g., "January 15, 2024, 02:30 PM")
 * Used in detail views where more information is appropriate
 * @param dateString - ISO date string
 * @returns Formatted date and time string
 */
export const formatDateLong = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};
