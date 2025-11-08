// API Configuration
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7248';

export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: `${API_BASE_URL}/api/auth/login`,
    REGISTER: `${API_BASE_URL}/api/auth/register`,
  },
} as const;
