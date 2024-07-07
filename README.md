# Movie Database Project

This is a simple C# web application for a movie database. The project includes user and role management, and a movie list. It was developed as a university project to demonstrate skills in ASP.NET Core and Entity Framework.

## Features

- **User Management**: Register, login, and manage user profiles.
- **Role Management**: Assign and manage roles (e.g., Admin, User) to control access to different parts of the application.
- **Movie List**: Add, view, edit, and delete movies from the database.

## Technologies Used

- **C#**
- **ASP.NET Core**
- **Entity Framework Core**
- **MSSQL** (Microsoft SQL Server for database management)
- **Bootstrap** (for UI)

## Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation

1. **Clone the repository**
    ```bash
    git clone https://github.com/Oxion7/MovieBase.git
    cd MovieBase
    ```

2. **Set up the database**
    - Ensure you have Microsoft SQL Server installed and running.
    - Update the connection string in `appsettings.json` with your SQL Server details.

3. **Apply migrations to set up the database schema**
    ```bash
    dotnet ef database update
    ```

4. **Run the application**
    ```bash
    dotnet run
    ```

5. **Open the application**
    - Navigate to `https://localhost:7331` in your browser.

## Usage

### User Management

- **Register**: Create a new user account.
- **Login**: Access the website with your credentials.

### Role Management

- **Admin Role**: Admins can manage user roles.
- **User Role**: Users can view and manage their own profiles and see the movie list.

### Movie List

- **Add Movie**: Managers can add new movies.
- **View Movies**: All users can view the list of movies.
- **Edit Movie**: Managers can edit movie details.
- **Delete Movie**: Managers can delete movies from the list.

## Project Structure

```plaintext
MovieBase/
│
├── Connected Services/
├── Dependencies/
├── Properties/
├── wwwroot/          # Static files
├── Controllers/      # Controllers for handling HTTP requests
├── Migrations/       # Database migrations
├── Models/           # Entity models for the database
├── ViewModels/       # View models for passing data between Controllers and Views
├── Views/            # Razor views for the UI
│
├── appsettings.json  # Configuration settings
├── EmailService.cs   # Service for handling email-related functionality
├── Program.cs        # Program entry point
├── RoleInitializer.cs # Initialization of roles (Admin, User, etc.)
└── Startup.cs        # Configuration of the application

