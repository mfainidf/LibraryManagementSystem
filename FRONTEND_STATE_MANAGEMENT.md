# Frontend State Management Documentation

## Overview

This document describes the comprehensive frontend state management implementation for the Library Management System. The solution provides a modern, scalable approach to managing both client and server state in a React/TypeScript application.

## Architecture

### State Management Layers

1. **Server State Management** - React Query (TanStack Query)
2. **Client State Management** - React Context API + useReducer
3. **Form State Management** - React Hook Form + Zod validation
4. **Error State Management** - Centralized error handling + Toast notifications

## Core Components

### 1. API Layer (`src/services/api.ts`)

The API layer provides a centralized service for communicating with the backend:

- **Axios Instance**: Configured with interceptors for authentication and error handling
- **Request Interceptors**: Automatically attach JWT tokens to requests
- **Response Interceptors**: Handle 401 errors and redirect to login
- **Error Handling**: Centralized error processing with type safety

```typescript
// Automatic token attachment
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
```

### 2. Authentication Context (`src/services/auth.tsx`)

Manages client-side authentication state:

- **State Management**: User information and authentication status
- **Persistence**: Automatic localStorage synchronization
- **Session Restoration**: Restores user session on app initialization

```typescript
const authReducer = (state: AuthState, action: AuthAction): AuthState => {
  switch (action.type) {
    case 'LOGIN_SUCCESS':
      return {
        user: action.payload.user,
        token: action.payload.token,
        isAuthenticated: true,
      };
    // ... other cases
  }
};
```

### 3. React Query Hooks (`src/hooks/useAuth.ts`)

Custom hooks for server state management:

- **Data Fetching**: Automatic caching and background updates
- **Mutations**: Optimistic updates with rollback on failure
- **Error Handling**: Integrated with toast notifications
- **Cache Management**: Intelligent cache invalidation strategies

### 4. Query Configuration

React Query is configured with optimized defaults:

```typescript
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      gcTime: 10 * 60 * 1000, // 10 minutes
      retry: (failureCount, error) => {
        // Smart retry logic based on error type
        if (error?.response?.status >= 400 && error?.response?.status < 500) {
          return false; // Don't retry client errors
        }
        return failureCount < 3;
      },
      refetchOnWindowFocus: false,
    },
  },
});
```

## Key Features

### 1. Data Fetching Layer

- **Automatic Caching**: Responses are cached based on query keys
- **Background Updates**: Data is refreshed in the background
- **Stale While Revalidate**: Shows cached data while fetching fresh data
- **Deduplication**: Multiple requests for the same data are deduplicated

### 2. Cache Management

- **Smart Invalidation**: Cache is invalidated based on user actions
- **Optimistic Updates**: UI updates immediately before server confirmation
- **Rollback Strategy**: Failed optimistic updates are automatically rolled back

```typescript
export const useLogin = () => {
  const { login } = useAuth();
  
  return useMutation({
    mutationFn: authApi.login,
    onSuccess: (data) => {
      login(data.user, data.token); // Optimistic update
      toast.success('Login successful!');
    },
    onError: (error: AppError) => {
      toast.error(error.message || 'Login failed');
    },
  });
};
```

### 3. Optimistic Updates

Example implementation for user profile updates:

```typescript
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
```

### 4. Error Handling

Multi-layer error handling strategy:

1. **API Level**: HTTP errors are caught and transformed
2. **Hook Level**: React Query handles retry logic and error states
3. **UI Level**: Toast notifications provide user feedback
4. **Global Level**: 401 errors trigger automatic logout

```typescript
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
```

## State Flow Examples

### 1. User Login Flow

1. User submits login form
2. `useLogin` mutation is triggered
3. API request is sent with form data
4. On success:
   - User data is stored in AuthContext
   - JWT token is stored in localStorage
   - User is redirected to dashboard
   - Success toast is shown
5. On error:
   - Error toast is displayed
   - Form remains accessible for retry

### 2. Profile Data Flow

1. User navigates to profile page
2. `useProfile` query is triggered
3. If cached data exists, it's shown immediately
4. Background request fetches fresh data
5. UI updates if data has changed
6. Cache is valid for 5 minutes

### 3. Logout Flow

1. User clicks logout button
2. `useLogout` mutation is triggered
3. Local state is cleared immediately (optimistic)
4. All cached queries are invalidated
5. User is redirected to login page

## Performance Optimizations

### 1. Query Key Management

Consistent query key structure for efficient caching:

```typescript
export const queryKeys = {
  profile: ['profile'] as const,
  users: ['users'] as const,
} as const;
```

### 2. Selective Fetching

Queries are only enabled when necessary:

```typescript
export const useProfile = () => {
  const { state } = useAuth();
  
  return useQuery({
    queryKey: queryKeys.profile,
    queryFn: authApi.getProfile,
    enabled: state.isAuthenticated, // Only fetch when authenticated
  });
};
```

### 3. Memory Management

- Automatic garbage collection of unused queries
- Configurable cache time limits
- Selective cache invalidation

## Testing Strategy

### 1. Mock Service Worker (MSW)

API calls are mocked for testing:

```typescript
// Mock successful login
rest.post('/api/auth/login', (req, res, ctx) => {
  return res(
    ctx.json({
      success: true,
      data: {
        token: 'mock-jwt-token',
        user: mockUser
      }
    })
  );
});
```

### 2. Query Testing

React Query provides testing utilities:

```typescript
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

test('useProfile returns user data', async () => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } }
  });
  
  const { result } = renderHook(() => useProfile(), {
    wrapper: ({ children }) => (
      <QueryClientProvider client={queryClient}>
        {children}
      </QueryClientProvider>
    ),
  });
  
  await waitFor(() => expect(result.current.isSuccess).toBe(true));
});
```

## Security Considerations

### 1. Token Management

- JWT tokens are stored in localStorage (consider httpOnly cookies for production)
- Automatic token refresh before expiration
- Secure token transmission via HTTPS

### 2. XSS Protection

- All user inputs are validated and sanitized
- TypeScript ensures type safety
- Content Security Policy headers (configured on backend)

### 3. CSRF Protection

- SameSite cookie attributes
- CSRF token validation for state-changing operations

## Best Practices

### 1. Query Organization

- Group related queries by feature
- Use consistent naming conventions
- Implement proper error boundaries

### 2. State Normalization

- Keep server state separate from client state
- Use React Query for server state
- Use Context/Redux for client state

### 3. Performance

- Implement pagination for large datasets
- Use selective queries to fetch only needed data
- Implement proper loading states

## Future Enhancements

### 1. Offline Support

- Service Worker integration
- Background sync for offline actions
- Conflict resolution strategies

### 2. Real-time Updates

- WebSocket integration with React Query
- Server-Sent Events for live data
- Optimistic updates for real-time features

### 3. Advanced Caching

- Persistent cache with IndexedDB
- Cache compression for large datasets
- Intelligent prefetching strategies

## Conclusion

This state management implementation provides a robust foundation for the Library Management System frontend. It balances performance, developer experience, and maintainability while following modern React patterns and best practices.

The system is designed to scale with the application's growth and can be extended with additional features as needed.