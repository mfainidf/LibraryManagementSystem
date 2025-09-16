export interface User {
  id: number;
  name: string;
  email: string;
  role: UserRole;
  isEnabled: boolean;
  createdAt: string;
}

export enum UserRole {
  User = 'User',
  Administrator = 'Administrator',
  Supervisor = 'Supervisor'
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
}

export interface AppError {
  message: string;
  code?: string;
  details?: string[];
}