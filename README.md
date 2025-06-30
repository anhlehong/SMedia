# Social Network Development (Backend)

This repository contains the backend for **Social Network Development**, a dynamic web application designed for seamless social interactions. It powers secure user authentication, profile management, real-time messaging, group creation, and post sharing. Built with .NET 8.0 and Clean Architecture, the backend integrates Azure SQL Database for robust data management, Socket.IO for instant communication, and Redis for performance optimization, delivering a scalable and secure platform.

## Table of Contents
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Related Repositories](#related-repositories)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Database Migrations](#database-migrations)
- [Running the Application](#running-the-application)
- [Contributing](#contributing)
- [License](#license)

## Features
- **Secure Authentication**: Implements JWT-based login and registration with BCrypt.Net password hashing and HttpOnly cookies for enhanced security.
- **Profile Management**: Enables users to update display names and bios, stored efficiently in Azure SQL Database.
- **Real-Time Messaging**: Powers instant chat with Socket.IO, supporting offline message storage for uninterrupted communication.
- **Group Interactions**: Facilitates group creation, joining, and management, with support for group posts.
- **Performance Optimization**: Uses Redis caching for fast data retrieval and scalability.
- **Email Verification**: Supports secure account verification via email using NETCore.MailKit.

## Tech Stack
- .NET 8.0 (ASP.NET Core Web API)
- C#
- Entity Framework Core
- BCrypt.Net
- JWT
- Socket.IO
- Serilog
- Mapster
- Microsoft.Extensions.Caching.Redis
- NETCore.MailKit
- Microsoft SQL Server
- Azure SQL Database

## Related Repositories
- **Frontend**: [https://github.com/anhlehong/FE-SMedia](https://github.com/anhlehong/FE-SMedia) - The Next.js + React 18 frontend for Social Network Development. Refer to its README for setup instructions.

## Prerequisites
Before setting up the backend, ensure you have:
- **.NET SDK**: Version 8.0
- **SQL Server**: Local instance or Azure SQL Database
- **Redis**: Local or cloud-based instance for caching
- **Git**: For cloning the repository
- (Optional) Visual Studio or VS Code for development

## Setup Instructions
1. **Clone the Backend Repository**:
   ```bash
   git clone https://github.com/anhlehong/SMedia.git
   cd SMedia
   ```

2. **Install Dependencies**:
   Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. **Configure Environment Variables**:
   Create a `.env` file in the root of the backend project (`SMedia`) to configure environment variables. Follow these steps to generate secure values for sensitive keys:
   - Create the `.env` file:
     ```bash
     touch .env
     ```
   - Open `.env` in a text editor (e.g., VS Code) and add the following configuration:
     ```env
     CONNECTION_STRING=your_database_connection_string
     JWT_KEY=your_secure_jwt_key
     JWT_ISSUER=MyApp
     JWT_AUDIENCE=MyClientApp
     EMAIL_SMTP_SERVER=smtp.gmail.com
     EMAIL_PORT=587
     EMAIL_SENDER_EMAIL=your_email@gmail.com
     EMAIL_PASSWORD=your_email_app_password
     EMAIL_SENDER_NAME=Social Media
     FQDN_FRONTEND=http://localhost:3000
     ```
   - **Generate Secure Values**:
     - **CONNECTION_STRING**: Obtain from your Azure SQL Database or local SQL Server. For Azure, use the format:
       ```env
       Server=tcp:<your-server>.database.windows.net,1433;Initial Catalog=SMedia;Persist Security Info=False;User ID=<your-user>;Password=<your-password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
       ```
       For local SQL Server, use:
       ```env
       Server=localhost;Database=SocialNetworkDb;Trusted_Connection=True;TrustServerCertificate=True;
       ```
       Replace `<your-server>`, `<your-user>`, and `<your-password>` with your database credentials.
     - **JWT_KEY**: Generate a secure key using a tool like OpenSSL:
       ```bash
       openssl rand -base64 32
       ```
       Copy the output (e.g., `irDzddN1IyvH72j8eib6n8fZ7qSf8O2F3fWIKVczuExGOJtQZ4gF9qQZdAfISzYL`) into `JWT_KEY`.
     - **EMAIL_SENDER_EMAIL** and **EMAIL_PASSWORD**: Use a Gmail account with an [App Password](https://support.google.com/accounts/answer/185833) for security. Replace `your_email@gmail.com` with your email and `your_email_app_password` with the generated App Password.
     - **JWT_ISSUER** and **JWT_AUDIENCE**: Keep as `MyApp` and `MyClientApp` or adjust to match your application’s configuration.
     - **FQDN_FRONTEND**: Set to the frontend URL (e.g., `http://localhost:3000` for local development or the deployed frontend URL).
   - **Note**: Do not commit the `.env` file to Git; ensure it’s listed in `.gitignore`.

## Database Migrations
To manage database schema changes using Entity Framework Core:
1. Navigate to the `Infrastructure` folder:
   ```bash
   cd Infrastructure
   ```
2. Ensure the `CONNECTION_STRING` in the `.env` file is correctly configured for your SQL Server or Azure SQL Database.
3. Install the Entity Framework Core CLI tool if not already installed:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
4. Add a new migration for schema changes, outputting to the `Data/Migrations` folder:
   ```bash
   dotnet ef migrations add InitialCreate -o Data/Migrations -s ../SMedia
   ```
   Replace `InitialCreate` with a descriptive name (e.g., `AddUserProfileFields`) for subsequent migrations.
5. Apply the migration to update the database:
   ```bash
   dotnet ef database update -s ../SMedia
   ```
6. To rollback a migration (if needed):
   ```bash
   dotnet ef migrations remove -s ../SMedia
   ```

**Troubleshooting**:
- Ensure SQL Server or Azure SQL Database is accessible (check firewall settings for Azure).
- Verify the project structure: `Infrastructure` contains EF Core configurations, and `SMedia` is the startup project.
- If migrations fail, check the connection string and ensure `dotnet-ef` is installed.

## Running the Application
1. Navigate to the `SMedia` project directory:
   ```bash
   cd ../SMedia
   ```
2. Run the backend:
   ```bash
   dotnet run
   ```
   The backend will start on `http://localhost:5000` (or the port specified in the project configuration).

   **Note**: Ensure the Azure SQL Database (or local SQL Server) and Redis are running and accessible before starting the application.

## Contributing
Contributions are welcome! Follow these steps:
1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/your-feature`).
3. Commit changes (`git commit -m 'Add your feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Open a pull request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
