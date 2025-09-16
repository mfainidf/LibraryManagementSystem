import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../services/auth';
import { useProfile, useLogout } from '../hooks/useAuth';
import { UserRole } from '../types';

const ProfilePage: React.FC = () => {
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
      color: '#1f2937',
      textDecoration: 'none'
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
      maxWidth: '800px',
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
    field: {
      marginBottom: '16px'
    },
    label: {
      display: 'block',
      fontSize: '14px',
      fontWeight: '500',
      color: '#374151',
      marginBottom: '4px'
    },
    value: {
      fontSize: '14px',
      color: '#1f2937'
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
          <Link to="/dashboard" style={styles.title}>
            Library Management System
          </Link>
          <div style={styles.navActions}>
            <Link to="/dashboard" style={styles.link}>Dashboard</Link>
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
        <h1 style={{fontSize: '24px', fontWeight: 'bold', marginBottom: '24px'}}>Profile</h1>

        <div style={styles.card}>
          <h2 style={styles.cardTitle}>Personal Information</h2>
          <div style={styles.grid}>
            <div style={styles.field}>
              <label style={styles.label}>Full Name</label>
              <p style={styles.value}>{user?.name}</p>
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Email Address</label>
              <p style={styles.value}>{user?.email}</p>
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Role</label>
              <p style={styles.value}>
                {user?.role === UserRole.Administrator ? 'Administrator' : 
                 user?.role === UserRole.Supervisor ? 'Supervisor' : 'User'}
              </p>
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Status</label>
              <p style={{...styles.value, color: user?.isEnabled ? '#10b981' : '#ef4444'}}>
                {user?.isEnabled ? 'Active' : 'Inactive'}
              </p>
            </div>
            <div style={styles.field}>
              <label style={styles.label}>Member Since</label>
              <p style={styles.value}>
                {user?.createdAt ? new Date(user.createdAt).toLocaleDateString() : 'N/A'}
              </p>
            </div>
          </div>
        </div>

        <div style={styles.card}>
          <h2 style={styles.cardTitle}>Security</h2>
          <div style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center'}}>
            <div>
              <h3 style={{fontSize: '16px', fontWeight: '500', marginBottom: '4px'}}>Password</h3>
              <p style={{fontSize: '14px', color: '#6b7280'}}>
                Change your account password
              </p>
            </div>
            <button style={{...styles.button, backgroundColor: '#2563eb'}}>
              Change Password
            </button>
          </div>
        </div>
      </main>
    </div>
  );
};

export default ProfilePage;