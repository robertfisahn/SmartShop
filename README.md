# ASP.NET Core + Angular

## Project Overview

This project consists of two applications:
1. **ASP.NET Core (REST API)** – SmartShopAPI
2. **Angular** – SmartShopUI

Both applications can be run in two ways:
- **Using Docker**
- **Manually (locally) via the console** – requires the appropriate tools and dependencies installed.

---

## System Requirements

<details>
<summary><strong>📋 View Requirements</strong></summary>

### Docker:
- Docker installed on your machine

### Manual Setup:
- .NET SDK (version 8.0) – download from [here](https://dotnet.microsoft.com/download)
- Node.js (version 20.17.0) – download from [here](https://nodejs.org/)
- Angular CLI (version 18.2.5) – install it globally:
    ```bash
    npm install -g @angular/cli@18.2.5
    ```
- **SQL Server Express** – download and install from [here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (choose the Express version)

> ⚠️ **IMPORTANT:**  
> By default, the application is configured to use SQL Server Express. If you are using a different version of SQL Server or a custom setup, you will need to update the connection string in the `.env` file.

- **RabbitMQ** – download and install from [here](https://www.rabbitmq.com/download.html).  
  Make sure it is running locally at `localhost:5672` with the default credentials: `guest / guest`.  
  The management UI is available at [http://localhost:15672](http://localhost:15672).


- **SendGrid** – [Create a free SendGrid account](https://sendgrid.com/) and generate an API key.

</details>

---

## ⚠️ Environment Configuration (`.env` file) ⚠️

<details>
<summary><strong>⚙️ Environment Setup</strong></summary>

To run the project properly – whether **locally** or using **Docker** – you need to create a `.env` file in the **root** directory of the repository (`SmartShop/`).

This file contains environment-specific configuration such as database connection strings, JWT secrets, SMTP keys, and RabbitMQ settings.  
Both the **ASP.NET Core API** and **Docker containers** rely on values from this file.

> ⚠️ **Note:**  
> The `.env` file is intentionally excluded from version control (`.gitignore`).  
> You must **create it manually** before running the project.

### Example `.env` file (template):

```env
# Database connection string
ConnectionStrings__SmartShopDbConnection=Server=localhost\SQLEXPRESS;Database=SmartShopDb;Trusted_Connection=True;TrustServerCertificate=True;

# JWT authentication
Authentication__JwtKey=your_secret_jwt_key_here
Authentication__JwtIssuer=http://localhost:5000
Authentication__JwtExpireDays=2

# SendGrid email configuration
SendGrid__ApiKey=your_sendgrid_api_key
SendGrid__FromEmail=your_email@example.com
SendGrid__FromName=SmartShopAPI

# RabbitMQ configuration
RabbitMQ__UserName=guest
RabbitMQ__Password=guest
RabbitMQ__Host=localhost
RabbitMQ__Port=5672

# Docker compatibility aliases (used by docker-compose)
RABBITMQ_DEFAULT_USER=guest
RABBITMQ_DEFAULT_PASS=guest
```

</details>

---

## Running the Application
<details>
<summary><strong>🐳 1. Using Docker (Prebuilt Images)</strong></summary>

Uses latest prebuilt images from Docker Hub via continuous delivery. **Fastest option!**

1. Clone the repository:
    ```bash
   git clone https://github.com/robertfisahn/SmartShop
   cd SmartShop
    
2. Run with prebuilt images:
    ```bash
   docker-compose -f docker-compose.deploy.yml up
    
3. The application should be accessible at:
   `http://localhost:4288`

</details>
<details>
<summary><strong>🐳 2. Using Docker (Build from Source)</strong></summary>

1. Clone the repository:
    ```bash
   git clone https://github.com/robertfisahn/SmartShop
   cd SmartShop
    
2. Run the application using Docker Compose:
    ```bash
   docker-compose up --build
    
3. The application should be accessible at:
   `http://localhost:4288`

</details>

<details>
<summary><strong>🔧 3. Running Manually (Development Mode)</strong></summary>

#### Backend (ASP.NET Core)

1. Clone the repository:
    ```bash
    git clone https://github.com/robertfisahn/SmartShop
    cd SmartShop/SmartShopAPI/SmartShopAPI

2. Install dependencies and run the application:
    ```bash
   dotnet restore
   dotnet run

3. The backend should be accessible at: `http://localhost:5108/swagger`

#### Frontend (Angular)

1. Navigate to the Angular folder:  
    ```bash
   cd SmartShop/SmartShopUI/SmartShopUI

2. Install dependencies:
    ```bash
   npm install
    
3. Run the Angular application:
    ```bash
   ng serve
    
4. The frontend will be accessible at: `http://localhost:4200`

</details>

---

## User Authentication

You can log into the application using the following demo accounts:

- **Admin Account**:
   - Email: `admin@admin.com`
   - Password: `admin123`
   
- **User Account**:
   - Email: `user@user.com`
   - Password: `user1234`