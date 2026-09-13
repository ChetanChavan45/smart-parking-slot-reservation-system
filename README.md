````markdown
# 🚗 Smart Parking & Slot Reservation System

A web-based parking management application built with **C#, ASP.NET Core MVC, .NET 8, Entity Framework Core, and SQL Server**.

The application supports parking reservations, parking-space management, user authentication, payments, reviews, and role-based access for users, operators, and administrators.

The project follows a layered architecture:

**Controller → Service → Repository**

This structure improves separation of concerns, code maintainability, debugging, testing, and future development.

> **Note:** This repository contains the application source code and technical documentation. Sensitive database credentials, email credentials, deployment settings, and private configuration are not included.

> **Project Background:** This repository is an adapted and further-developed version of the Smart Parking System originally created by **Sara Bajrić** as part of the Object-Oriented Analysis and Design course at the Faculty of Electrical Engineering, University of Sarajevo.  
> Original repository: https://github.com/sarabajric/SmartParkingSystem

---

# ✨ Features

## 👤 User

- Register and securely log in
- Browse available parking spaces
- Search and filter parking locations
- Create parking reservations
- Modify existing reservations
- Cancel reservations
- Extend reservation duration
- Complete parking payments
- View reservation history
- Submit parking reviews
- Manage personal profile information

## 👨‍💼 Operator

- Monitor active parking reservations
- Manage parking availability
- Update parking-space status
- Support parking operations
- Track reservation-related information

## 👨‍💻 Administrator

- Manage application users
- Manage parking zones
- Manage parking spaces
- Monitor reservations
- Manage user reviews
- Monitor system activity
- Support administrative operations

---

# 🛠️ Technologies

## Backend

- **C#**
- **ASP.NET Core MVC**
- **.NET 8**
- **Entity Framework Core**
- **ASP.NET Identity**
- **Microsoft SQL Server**

## Frontend

- Razor Views
- Bootstrap 5
- HTML5
- CSS3
- JavaScript

## Software Engineering Concepts

- Object-Oriented Programming
- MVC Architecture
- Dependency Injection
- Repository Pattern
- Service Layer Pattern
- Relational Database Management
- Authentication and Authorization
- Role-Based Access Control
- Exception Handling
- Data Validation
- Application Debugging
- Separation of Concerns

---

# 🏗️ Application Architecture

The application is organized into multiple layers to keep responsibilities separated and make the code easier to maintain.

```text
User Request
     |
     v
Controller
     |
     v
Service Layer
     |
     v
Repository Layer
     |
     v
Entity Framework Core
     |
     v
SQL Server Database
````

### Controller Layer

Controllers handle incoming HTTP requests, user interaction, application navigation, and communication with the service layer.

### Service Layer

The service layer contains application and business logic and coordinates operations between controllers and repositories.

### Repository Layer

Repositories handle data-access operations and database-related logic.

### Data Layer

Entity Framework Core and the application database context are used to communicate with the SQL Server database.

---

# 📂 Project Structure

```text
SmartParkingSystem
├── Areas
├── Controllers
├── Data
│   └── Migrations
├── Enums
├── Models
├── Repositories
├── Services
│   └── Interfaces
├── Settings
├── ViewModels
├── Views
├── wwwroot
├── docs
├── Program.cs
├── PametniParkingSistem.csproj
├── appsettings.json
└── Dockerfile
```

---

# 🚀 Main Functionalities

* Authentication and Authorization
* User Registration and Login
* Role-Based Access Control
* Parking Reservation Management
* Parking Space Management
* Parking Zone Management
* Reservation Modification and Cancellation
* Payment Processing
* Review Management
* Email Notifications
* User Profile Management
* Dashboard and Application Monitoring
* SQL Server Database Operations
* Entity Framework Core Migrations

---

# 💻 C# and Object-Oriented Programming

The application uses C# and Object-Oriented Programming principles to organize application logic into reusable and maintainable components.

Important concepts used include:

* Classes and Objects
* Interfaces
* Encapsulation
* Abstraction
* Dependency Injection
* Reusable Service Classes
* Repository Interfaces
* Model Classes
* ViewModels
* Enumerations
* Structured Exception Handling

Interfaces are used between service and repository layers to reduce dependency between components and improve maintainability.

---

# 🗄️ Database

The application uses **Microsoft SQL Server** as the relational database.

Database communication is handled using **Entity Framework Core**.

The project includes database entities and migrations for application data such as:

* Users
* Parking Zones
* Parking Spaces
* Reservations
* Payments
* Reviews
* Support Requests
* Email Notifications

The application demonstrates important RDBMS concepts including:

* Tables
* Relationships
* Primary Keys
* Foreign Keys
* Data Validation
* CRUD Operations
* Database Migrations
* Relational Data Mapping

---

# 🔐 Authentication and Authorization

Authentication is implemented using **ASP.NET Identity**.

The application supports role-based authorization for different types of users:

* User
* Operator
* Administrator

Each role has access only to the operations allowed for that role.

This helps protect application functionality and provides controlled access to system resources.

---

# 🧪 Development, Testing, and Debugging

The project provides practical experience with common software-development activities including:

* Developing application components
* Debugging application behavior
* Investigating software defects
* Reviewing application flow
* Validating user input
* Testing CRUD functionality
* Checking database operations
* Analyzing exceptions
* Performing root-cause analysis
* Maintaining application code
* Reviewing and improving existing modules

These activities help improve application reliability and maintainability.

---

# 📚 Technical Documentation

Project documentation is available inside the `docs` directory.

* 📐 [Class Diagram](docs/ClassDiagram.pdf)
* 🧩 [Component Diagram](docs/ComponentDiagram.pdf)
* 🏗️ [Deployment Diagram](docs/DeploymentDiagram.pdf)
* 🗄️ [Entity Relationship Diagram](docs/ERD.pdf)

The documentation helps explain the system structure, components, database relationships, and application design.

---

# ⚙️ Getting Started

## Prerequisites

Install the following tools before running the application:

* Visual Studio 2022 or newer
* .NET 8 SDK
* Microsoft SQL Server or LocalDB
* Git

---

# 📥 Clone the Repository

```bash
git clone https://github.com/ChetanChavan45/smart-parking-slot-reservation-system.git
```

Move into the project directory:

```bash
cd smart-parking-slot-reservation-system
```

---

# ▶️ Run the Application

1. Open the project in **Visual Studio 2022**.
2. Restore the required **NuGet packages**.
3. Configure your local SQL Server connection.
4. Review the configuration inside `appsettings.json`.
5. Apply Entity Framework Core migrations.
6. Build the solution.
7. Run the application.

You can also restore dependencies using:

```bash
dotnet restore
```

Build the application:

```bash
dotnet build
```

Run the application:

```bash
dotnet run
```

---

# 🔒 Security and Configuration

Sensitive information should never be committed to a public GitHub repository.

Examples include:

* SQL Server passwords
* Database credentials
* Email passwords
* API keys
* Authentication secrets
* Production connection strings
* Deployment credentials

Local development configuration or environment variables should be used for sensitive values.

---

# 🎯 Learning and Development Focus

This repository is being used to strengthen practical software-development skills in:

* C#
* .NET
* ASP.NET Core MVC
* Object-Oriented Programming
* Microsoft SQL Server
* Relational Database Concepts
* Entity Framework Core
* Software Development
* Testing
* Debugging
* Defect Investigation
* Root-Cause Analysis
* Code Organization
* Technical Documentation
* Git Version Control

---

# 📖 Attribution

This repository is based on the original Smart Parking System project developed by **Sara Bajrić**.

Original repository:

[https://github.com/sarabajric/SmartParkingSystem](https://github.com/sarabajric/SmartParkingSystem)

The original project was developed as part of an Object-Oriented Analysis and Design course at the Faculty of Electrical Engineering, University of Sarajevo.

This repository is maintained as an adapted learning and development version for further work with **C#, .NET, Object-Oriented Programming, SQL Server, debugging, application maintenance, and software-development practices**.

---

# 👨‍💻 Maintained By

**Chetan Chavan**

GitHub: [https://github.com/ChetanChavan45](https://github.com/ChetanChavan45)

```
```
