import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { authApi } from '../services/api';
import { useAuth } from '../services/auth';
import { AppError } from '../types';
import toast from 'react-hot-toast';

// Query keys
export const queryKeys = {
  profile: ['profile'] as const,
  users: ['users'] as const,
} as const;

// Auth hooks
export const useLogin = () => {
  const { login } = useAuth();
  
  return useMutation({
    mutationFn: authApi.login,
    onSuccess: (data) => {
      login(data.user, data.token);
      toast.success('Login successful!');
    },
    onError: (error: AppError) => {
      toast.error(error.message || 'Login failed');
    },
  });
};

export const useRegister = () => {
  return useMutation({
    mutationFn: authApi.register,
    onSuccess: () => {
      toast.success('Registration successful! Please log in.');
    },
    onError: (error: AppError) => {
      toast.error(error.message || 'Registration failed');
    },
  });
};

export const useLogout = () => {
  const { logout } = useAuth();
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async () => {
      // No API call needed for logout, just clear local state
      return Promise.resolve();
    },
    onSuccess: () => {
      logout();
      queryClient.clear(); // Clear all cached data
      toast.success('Logged out successfully');
    },
  });
};

export const useChangePassword = () => {
  return useMutation({
    mutationFn: authApi.changePassword,
    onSuccess: () => {
      toast.success('Password changed successfully!');
    },
    onError: (error: AppError) => {
      toast.error(error.message || 'Failed to change password');
    },
  });
};

export const useProfile = () => {
  const { state } = useAuth();
  
  return useQuery({
    queryKey: queryKeys.profile,
    queryFn: authApi.getProfile,
    enabled: state.isAuthenticated, // Only fetch when authenticated
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: (failureCount, error: any) => {
      // Don't retry on 401 errors
      if (error?.response?.status === 401) {
        return false;
      }
      return failureCount < 3;
    },
  });
};

// Optimistic update hook example for future use
export const useOptimisticUpdate = <T,>(
  queryKey: readonly unknown[],
  updateFn: (oldData: T | undefined, newData: Partial<T>) => T | undefined
) => {
  const queryClient = useQueryClient();
  
  return (newData: Partial<T>) => {
    // Optimistically update the cache
    queryClient.setQueryData(queryKey, (oldData: T | undefined) => 
      updateFn(oldData, newData)
    );
  };
};

// Error handling hook
export const useApiError = () => {
  return (error: AppError) => {
    console.error('API Error:', error);
    
    if (error.details && error.details.length > 0) {
      error.details.forEach(detail => toast.error(detail));
    } else {
      toast.error(error.message || 'An unexpected error occurred');
    }
  };
};