# Full-Stack Task Management Architecture & Source Code

Welcome to the Task Management application! This is a complete, scalable full-stack application leveraging the power of modern **.NET 8+** and **Angular 17+**. 

This document serves to explain the architectural choices, project structure, and the logic guiding the application, making it simple for engineering teams to understand the project deeply upon review.

---

## 🏗️ Architecture & Technologies

### Backend (`TaskManager.API`)
The backend is an `ASP.NET Core Web API` serving as the data access and authentication layer.
* **C# & ASP.NET Core**: Chosen for its robust typing, performance, and built-in dependency injection.
* **Entity Framework Core (SQLite)**: Utilizing an embedded SQLite local database eliminates the need for reviewers to install and configure heavy database services like SQL Server. The database triggers upon running to serve as a fast sandbox.
* **Generic Repository Pattern**: Implemented to abstract direct database calls away from controllers. The `IRepository<T>` interface handles generic CRUD logic, ensuring DRY (Don't Repeat Yourself) code while specific operations (e.g., fetching tasks by a specific user) are extended in `ITaskRepository`.
* **JWT Authentication**: Secure endpoints using token-based authentication. The `AuthController` securely hashes passwords using `HMACSHA512` and issues secure JWT tokens.

### Frontend (`task-manager-ui`)
The frontend is an **Angular Workspace** focused on clean design and separation of concerns.
* **Angular (Standalone Components)**: Uses the modern approach of standalone components (no `NgModules`), removing boilerplate and increasing tree-shaking efficiency.
* **Bootstrap 5 UI**: Utilized for standard, clean, responsive styling, ensuring the app looks great both on desktop and mobile. 
* **HttpInterceptors**: Configured an `auth.interceptor.ts` to automatically attach the user's JWT token to the `Authorization` header of every outbound API request seamlessly.
* **Route Guards**: Evaluates routing via `auth.guard.ts` to ensure unauthenticated users cannot access the Dashboard routing space.

---

## 📂 Folder Structure

```text
├── TaskManager.API               # Backend Source Code
│   ├── Controllers               # Endpoints determining traffic (Auth, Tasks)
│   ├── Data                      # AppDbContext (EF Core context logic)
│   ├── DTOs                      # Data Transfer Objects (Hides true Model logic from API responses)
│   ├── Models                    # Database schemas (User.cs, TaskItem.cs)
│   └── Repositories              # Generic interface and implementation logic
└── task-manager-ui               # Frontend Source Code
    └── src/app
        ├── components            # Dashboard, Login, Register isolated views
        ├── guards                # Authentication Guards for routing
        ├── interceptors          # Attaches JWT Bearer tokens to requests
        ├── models                # TypeScript Types replicating Backend DTOs
        └── services              # API HTTP logic (task.service.ts, auth.service.ts)
```

---

## 🚀 API Endpoints Snapshot

| Method | Route | Description | Auth Required |
|---|---|---|---|
| POST | `/api/auth/register` | Registers a new user account | No |
| POST | `/api/auth/login` | Returns a JWT token upon login | No |
| GET | `/api/tasks` | Gets all tasks belonging to the user | Yes |
| POST | `/api/tasks` | Creates a new task | Yes |
| PUT | `/api/tasks/{id}` | Updates a task text or status | Yes |
| DELETE| `/api/tasks/{id}` | Deletes a task by ID | Yes |

---

## 🏃‍♂️ How to Run Locally

Because the project is standalone, you will need two terminals running concurrently to host the backend API and the frontend dashboard.

**Requirements:** .NET 8.0 SDK+, Node.js (v18+)

### 1. Start the API (Terminal 1)
```bash
cd TaskManager.API

# Run the .NET API
dotnet run
```
*Note: The API runs natively to `http://localhost:5214` to listen for Angular calls.*

### 2. Start the Angular UI (Terminal 2)
```bash
cd task-manager-ui

# Install dependencies if opening for the first time
npm install

# Start the Node development server
npm start
```
Go to `http://localhost:4200` to start using the app!

