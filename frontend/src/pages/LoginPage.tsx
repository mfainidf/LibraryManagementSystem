import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Link } from 'react-router-dom';
import { useLogin } from '../hooks/useAuth';
import { LoginRequest } from '../types';

const loginSchema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(1, 'Password is required'),
});

type LoginFormData = z.infer<typeof loginSchema>;

const LoginPage: React.FC = () => {
  const loginMutation = useLogin();
  
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    const loginData: LoginRequest = {
      email: data.email,
      password: data.password,
    };
    
    await loginMutation.mutateAsync(loginData);
  };

  const styles = {
    container: {
      minHeight: '100vh',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      backgroundColor: '#f9fafb',
      padding: '48px 16px'
    },
    formContainer: {
      maxWidth: '400px',
      width: '100%',
      padding: '32px',
      backgroundColor: 'white',
      borderRadius: '8px',
      boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)'
    },
    title: {
      fontSize: '24px',
      fontWeight: 'bold',
      textAlign: 'center' as const,
      marginBottom: '24px',
      color: '#1f2937'
    },
    input: {
      width: '100%',
      padding: '12px',
      border: '1px solid #d1d5db',
      borderRadius: '4px',
      fontSize: '14px',
      marginBottom: '8px'
    },
    button: {
      width: '100%',
      padding: '12px',
      backgroundColor: '#2563eb',
      color: 'white',
      border: 'none',
      borderRadius: '4px',
      fontSize: '14px',
      fontWeight: '500',
      cursor: 'pointer',
      marginTop: '16px'
    },
    error: {
      color: '#dc2626',
      fontSize: '12px',
      marginBottom: '8px'
    },
    link: {
      display: 'block',
      textAlign: 'center' as const,
      marginTop: '16px',
      color: '#2563eb',
      textDecoration: 'none',
      fontSize: '14px'
    }
  };

  return (
    <div style={styles.container}>
      <div style={styles.formContainer}>
        <h2 style={styles.title}>
          Library Management System
        </h2>
        <form onSubmit={handleSubmit(onSubmit)}>
          <div>
            <input
              {...register('email')}
              type="email"
              placeholder="Email address"
              style={styles.input}
            />
            {errors.email && (
              <div style={styles.error}>{errors.email.message}</div>
            )}
          </div>
          <div>
            <input
              {...register('password')}
              type="password"
              placeholder="Password"
              style={styles.input}
            />
            {errors.password && (
              <div style={styles.error}>{errors.password.message}</div>
            )}
          </div>

          <button
            type="submit"
            disabled={isSubmitting || loginMutation.isPending}
            style={styles.button}
          >
            {isSubmitting || loginMutation.isPending ? 'Signing in...' : 'Sign in'}
          </button>

          <Link to="/register" style={styles.link}>
            Don't have an account? Sign up
          </Link>
        </form>
      </div>
    </div>
  );
};

export default LoginPage;