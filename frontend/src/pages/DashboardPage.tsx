import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../services/auth';
import { useProfile, useLogout } from '../hooks/useAuth';
import { UserRole } from '../types';

const DashboardPage: React.FC = () => {
  const { state } = useAuth();
  const { data: profile, isLoading, error } = useProfile();
  const logoutMutation = useLogout();

  const handleLogout = () => {
    logoutMutation.mutate();
  };

  const styles = {
    container: {
      minHeight: '100vh',
      backgroundColor: '#f9fafb'
    },
    nav: {
      backgroundColor: 'white',
      borderBottom: '1px solid #e5e7eb',
      padding: '16px'
    },
    navContent: {
      maxWidth: '1200px',
      margin: '0 auto',
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center'
    },
    title: {
      fontSize: '20px',
      fontWeight: '600',
      color: '#1f2937'
    },
    navActions: {
      display: 'flex',
      gap: '16px',
      alignItems: 'center'
    },
    link: {
      color: '#374151',
      textDecoration: 'none',
      padding: '8px 12px'
    },
    button: {
      backgroundColor: '#dc2626',
      color: 'white',
      border: 'none',
      padding: '8px 12px',
      borderRadius: '4px',
      cursor: 'pointer',
      fontSize: '14px'
    },
    main: {
      maxWidth: '1200px',
      margin: '0 auto',
      padding: '24px'
    },
    card: {
      backgroundColor: 'white',
      borderRadius: '8px',
      boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
      padding: '24px',
      marginBottom: '24px'
    },
    cardTitle: {
      fontSize: '18px',
      fontWeight: '500',
      marginBottom: '16px'
    },
    grid: {
      display: 'grid',
      gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
      gap: '16px'
    },
    statCard: {
      padding: '16px',
      borderRadius: '8px',
      textAlign: 'center' as const
    },
    loading: {
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      minHeight: '100vh'
    }
  };

  if (isLoading) {
    return (
      <div style={styles.loading}>
        <div>Loading...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div style={styles.loading}>
        <div style={{color: '#dc2626'}}>Error loading profile</div>
      </div>
    );
  }

  const user = profile || state.user;

  return (
    <div style={styles.container}>
      <nav style={styles.nav}>
        <div style={styles.navContent}>
          <h1 style={styles.title}>Library Management System</h1>
          <div style={styles.navActions}>
            <Link to="/profile" style={styles.link}>Profile</Link>
            <button
              onClick={handleLogout}
              disabled={logoutMutation.isPending}
              style={styles.button}
            >
              {logoutMutation.isPending ? 'Logging out...' : 'Logout'}
            </button>
          </div>
        </div>
      </nav>

      <main style={styles.main}>
        <div style={styles.card}>
          <h2 style={styles.cardTitle}>Welcome back, {user?.name}!</h2>
          <div style={styles.grid}>
            <div style={{...styles.statCard, backgroundColor: '#dbeafe'}}>
              <h3 style={{color: '#1d4ed8', fontSize: '14px', fontWeight: '500'}}>Role</h3>
              <p style={{color: '#1e40af', fontSize: '18px', fontWeight: '600'}}>
                {user?.role === UserRole.Administrator ? 'Administrator' : 
                 user?.role === UserRole.Supervisor ? 'Supervisor' : 'User'}
              </p>
            </div>
            <div style={{...styles.statCard, backgroundColor: '#dcfce7'}}>
              <h3 style={{color: '#15803d', fontSize: '14px', fontWeight: '500'}}>Status</h3>
              <p style={{color: '#166534', fontSize: '18px', fontWeight: '600'}}>
                {user?.isEnabled ? 'Active' : 'Inactive'}
              </p>
            </div>
            <div style={{...styles.statCard, backgroundColor: '#fdf4ff'}}>
              <h3 style={{color: '#9333ea', fontSize: '14px', fontWeight: '500'}}>Member Since</h3>
              <p style={{color: '#7c3aed', fontSize: '18px', fontWeight: '600'}}>
                {user?.createdAt ? new Date(user.createdAt).toLocaleDateString() : 'N/A'}
              </p>
            </div>
          </div>
        </div>

        <div style={styles.grid}>
          <div style={styles.card}>
            <h3 style={styles.cardTitle}>Catalog Management</h3>
            <p style={{color: '#6b7280', marginBottom: '16px'}}>
              Browse and manage the library catalog
            </p>
            <button style={{...styles.button, backgroundColor: '#2563eb'}}>
              Coming Soon
            </button>
          </div>

          <div style={styles.card}>
            <h3 style={styles.cardTitle}>Loan Management</h3>
            <p style={{color: '#6b7280', marginBottom: '16px'}}>
              Manage book loans and returns
            </p>
            <button style={{...styles.button, backgroundColor: '#2563eb'}}>
              Coming Soon
            </button>
          </div>

          <div style={styles.card}>
            <h3 style={styles.cardTitle}>Search & Filters</h3>
            <p style={{color: '#6b7280', marginBottom: '16px'}}>
              Advanced search and filtering options
            </p>
            <button style={{...styles.button, backgroundColor: '#2563eb'}}>
              Coming Soon
            </button>
          </div>
        </div>
      </main>
    </div>
  );
};

export default DashboardPage;