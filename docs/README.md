# Library Management System Documentation

## Overview
This is a library management system built with .NET that allows users to manage books, borrowers, and lending operations.

## Project Structure
- `Library.Console`: Contains the console application UI and user interaction logic
- `Library.Core`: Contains the core business logic, interfaces, and domain models
- `Library.Infrastructure`: Contains the data access layer, repositories, and services implementation

## Features
### Authentication
- User registration with email and password
- User login
- Password change functionality
- Secure password hashing using BCrypt

## Technical Details
### Database
- SQLite database
- Entity Framework Core for data access
- Code-first migrations

### Security
- Password hashing with BCrypt
- Input validation and sanitization
- Null safety checks

## Getting Started
1. Clone the repository
2. Ensure .NET 9.0 SDK is installed
3. Run `dotnet restore` to restore dependencies
4. Run `dotnet ef database update` to create the database
5. Run `dotnet run` in the Library.Console directory to start the application
