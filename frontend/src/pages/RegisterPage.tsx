import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Link, useNavigate } from 'react-router-dom';
import { useRegister } from '../hooks/useAuth';
import { RegisterRequest } from '../types';

const registerSchema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters'),
  email: z.string().email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
  confirmPassword: z.string(),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ["confirmPassword"],
});

type RegisterFormData = z.infer<typeof registerSchema>;

const RegisterPage: React.FC = () => {
  const registerMutation = useRegister();
  const navigate = useNavigate();
  
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
  });

  const onSubmit = async (data: RegisterFormData) => {
    const registerData: RegisterRequest = {
      name: data.name,
      email: data.email,
      password: data.password,
    };
    
    try {
      await registerMutation.mutateAsync(registerData);
      navigate('/login');
    } catch (error) {
      // Error is handled by the mutation's onError callback
    }
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
      marginBottom: '8px',
      boxSizing: 'border-box' as const
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
        <h2 style={styles.title}>Create your account</h2>
        <form onSubmit={handleSubmit(onSubmit)}>
          <div>
            <input
              {...register('name')}
              type="text"
              placeholder="Full Name"
              style={styles.input}
            />
            {errors.name && (
              <div style={styles.error}>{errors.name.message}</div>
            )}
          </div>
          
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
          
          <div>
            <input
              {...register('confirmPassword')}
              type="password"
              placeholder="Confirm Password"
              style={styles.input}
            />
            {errors.confirmPassword && (
              <div style={styles.error}>{errors.confirmPassword.message}</div>
            )}
          </div>

          <button
            type="submit"
            disabled={isSubmitting || registerMutation.isPending}
            style={styles.button}
          >
            {isSubmitting || registerMutation.isPending ? 'Creating Account...' : 'Create Account'}
          </button>

          <Link to="/login" style={styles.link}>
            Already have an account? Sign in
          </Link>
        </form>
      </div>
    </div>
  );
};

export default RegisterPage;