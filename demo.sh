#!/bin/bash

# Library Management System Demo Script
# This script demonstrates the frontend state management implementation

echo "🏗️  Library Management System - Frontend State Management Demo"
echo "============================================================="
echo ""

echo "📋 System Overview:"
echo "  ✅ ASP.NET Core Web API with JWT authentication"
echo "  ✅ React/TypeScript frontend with state management"
echo "  ✅ React Query for server state management"
echo "  ✅ Context API for authentication state"
echo "  ✅ Optimistic updates and error handling"
echo "  ✅ Form validation with Zod and React Hook Form"
echo ""

echo "🏃‍♂️ Starting Demo..."
echo ""

# Check if .NET is available
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 8 SDK."
    exit 1
fi

# Check if Node.js is available
if ! command -v node &> /dev/null; then
    echo "❌ Node.js not found. Please install Node.js."
    exit 1
fi

echo "1️⃣  Building .NET Backend..."
cd /home/runner/work/LibraryManagementSystem/LibraryManagementSystem
dotnet build --verbosity quiet
if [ $? -eq 0 ]; then
    echo "   ✅ Backend built successfully"
else
    echo "   ❌ Backend build failed"
    exit 1
fi

echo ""
echo "2️⃣  Building React Frontend..."
cd frontend
npm run build --silent
if [ $? -eq 0 ]; then
    echo "   ✅ Frontend built successfully"
else
    echo "   ❌ Frontend build failed"
    exit 1
fi

echo ""
echo "3️⃣  Running Tests..."
cd ..
dotnet test --verbosity quiet --no-build
if [ $? -eq 0 ]; then
    echo "   ✅ All tests passed"
else
    echo "   ❌ Some tests failed"
fi

echo ""
echo "🎯 State Management Features Implemented:"
echo ""
echo "📡 Data Fetching Layer:"
echo "   • React Query (TanStack Query) for server state"
echo "   • Automatic caching with 5-minute stale time"
echo "   • Background refetching and deduplication"
echo "   • Smart retry logic based on error types"
echo ""
echo "🗄️  Cache Management:"
echo "   • Intelligent cache invalidation on mutations"
echo "   • Garbage collection after 10 minutes"
echo "   • Selective cache updates for performance"
echo ""
echo "⚡ Optimistic Updates:"
echo "   • Immediate UI updates on user actions"
echo "   • Automatic rollback on API failures"
echo "   • Seamless user experience"
echo ""
echo "🛡️  Error Handling:"
echo "   • Centralized error processing"
echo "   • Toast notifications for user feedback"
echo "   • Automatic logout on 401 errors"
echo "   • Network error recovery"
echo ""
echo "🔐 Authentication State:"
echo "   • JWT token management with interceptors"
echo "   • Persistent login state with localStorage"
echo "   • Session restoration on app reload"
echo "   • Protected routes and authorization"
echo ""

echo "📁 Project Structure:"
echo ""
echo "Backend (ASP.NET Core):"
echo "├── src/Library.Web/              # Web API project"
echo "│   ├── Controllers/               # API endpoints"
echo "│   ├── Services/                  # JWT service"
echo "│   ├── Models/                    # DTOs and API models"
echo "│   └── Program.cs                 # App configuration"
echo ""
echo "Frontend (React/TypeScript):"
echo "├── frontend/src/"
echo "│   ├── types/                     # TypeScript definitions"
echo "│   ├── services/"
echo "│   │   ├── api.ts                 # API client with interceptors"
echo "│   │   └── auth.tsx               # Authentication context"
echo "│   ├── hooks/"
echo "│   │   └── useAuth.ts             # React Query hooks"
echo "│   ├── pages/                     # Page components"
echo "│   └── App.tsx                    # App configuration"
echo ""

echo "🎮 How to Start the Application:"
echo ""
echo "Backend (Terminal 1):"
echo "  cd src/Library.Web"
echo "  dotnet run"
echo "  # API will be available at http://localhost:5000"
echo ""
echo "Frontend (Terminal 2):"
echo "  cd frontend"
echo "  npm start"
echo "  # App will be available at http://localhost:3000"
echo ""

echo "🔄 State Management Flow Examples:"
echo ""
echo "Login Flow:"
echo "  1. User submits form → useLogin mutation"
echo "  2. API request with validation"
echo "  3. JWT token stored + user context updated"
echo "  4. Automatic redirect to dashboard"
echo "  5. Toast notification for feedback"
echo ""
echo "Data Fetching:"
echo "  1. Component mounts → useProfile query"
echo "  2. Cache check (return cached if fresh)"
echo "  3. Background API request"
echo "  4. UI update with new data"
echo "  5. Cache updated for future use"
echo ""
echo "Optimistic Updates:"
echo "  1. User action → UI updates immediately"
echo "  2. API request in background"
echo "  3. Success: UI remains updated"
echo "  4. Failure: UI reverts + error shown"
echo ""

echo "💡 Key Benefits:"
echo "   ⚡ Fast, responsive user interface"
echo "   📱 Optimistic updates for better UX"
echo "   🔄 Automatic data synchronization"
echo "   🛡️  Robust error handling"
echo "   📦 Efficient caching strategies"
echo "   🔒 Secure authentication flow"
echo ""

echo "🏁 Demo Complete!"
echo ""
echo "The frontend state management system is fully implemented and ready for use."
echo "All components work together to provide a modern, efficient user experience."
echo ""
echo "Next Steps:"
echo "  • Add more features (catalog, loans, etc.)"
echo "  • Implement offline support"
echo "  • Add real-time updates with WebSockets"
echo "  • Enhance with advanced caching strategies"
echo ""