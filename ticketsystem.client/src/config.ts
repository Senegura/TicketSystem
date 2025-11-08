// API Configuration
// Use relative URLs to go through Vite proxy in development
// In production, these will resolve to the same origin as the frontend
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';

export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: `${API_BASE_URL}/api/auth/login`,
    REGISTER: `${API_BASE_URL}/api/auth/register`,
  },
  TICKETS: {
    CREATE: `${API_BASE_URL}/api/tickets`,
  },
} as const;
