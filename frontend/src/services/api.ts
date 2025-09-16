import axios, { AxiosResponse } from 'axios';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  ChangePasswordRequest,
  User,
  ApiResponse,
  AppError
} from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

// Create axios instance
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Clear auth data on unauthorized
      localStorage.removeItem('authToken');
      localStorage.removeItem('authUser');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Helper function to handle API responses
const handleApiResponse = <T>(response: AxiosResponse<ApiResponse<T>>): T => {
  if (response.data.success && response.data.data !== undefined) {
    return response.data.data;
  }
  
  const error: AppError = {
    message: response.data.message || 'An error occurred',
    details: response.data.errors
  };
  throw error;
};

// Helper function to handle API errors
const handleApiError = (error: any): never => {
  if (error.response?.data) {
    const apiError: AppError = {
      message: error.response.data.message || 'An error occurred',
      details: error.response.data.errors
    };
    throw apiError;
  }
  
  const networkError: AppError = {
    message: error.message || 'Network error occurred'
  };
  throw networkError;
};

// Auth API
export const authApi = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    try {
      const response = await apiClient.post<ApiResponse<LoginResponse>>('/auth/login', credentials);
      return handleApiResponse(response);
    } catch (error) {
      return handleApiError(error);
    }
  },

  register: async (userData: RegisterRequest): Promise<User> => {
    try {
      const response = await apiClient.post<ApiResponse<User>>('/auth/register', userData);
      return handleApiResponse(response);
    } catch (error) {
      return handleApiError(error);
    }
  },

  changePassword: async (passwordData: ChangePasswordRequest): Promise<boolean> => {
    try {
      const response = await apiClient.post<ApiResponse<boolean>>('/auth/change-password', passwordData);
      return handleApiResponse(response);
    } catch (error) {
      return handleApiError(error);
    }
  },

  getProfile: async (): Promise<User> => {
    try {
      const response = await apiClient.get<ApiResponse<User>>('/auth/profile');
      return handleApiResponse(response);
    } catch (error) {
      return handleApiError(error);
    }
  },
};

export { apiClient };