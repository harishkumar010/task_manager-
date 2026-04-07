# Full-Stack Task Management Application

A complete Task Management web application built using **ASP.NET Core Web API** for the backend and modern **Angular 17+ ** for the frontend.

## Features
- **JWT Authentication** (Register & Login mechanisms).
- **Generic Repository Pattern** ensuring a clean, scalable backend architecture.
- **Entity Framework Core (SQLite)**, offering zero-configuration plug-and-play database support.
- **RESTful API** managing Create, Read, Update, and Delete endpoints.
- **Modern Angular Frontend** leveraging standalone components, Bootstrap 5 UI, built-in search filters, and route guards.

---

## How to Run the Project Locally

The project is split into two halves: the `.NET Backend` and the `Angular Frontend`. You will need exactly two terminal windows to run them concurrently. 

### Prerequisites
- [.NET 8.0 SDK or newer](https://dotnet.microsoft.com/download)
- [Node.js (v18 or newer)](https://nodejs.org/)
- Angular CLI (`npm install -g @angular/cli`)

### 1. Start the Backend API (Terminal 1)
Because the project uses SQLite, there is no need to configure complex SQL Server connections. It will work out of the box.

```bash
# Navigate into the Backend directory
cd TaskManager.API

# The Entity Framework migrations are already applied, simply run:
dotnet run
```
*Note: The API will start on a port defined in `launchSettings.json` (e.g., `http://localhost:5214`).*

### 2. Start the Frontend (Terminal 2)

```bash
# Navigate into the Frontend directory
cd task-manager-ui

# Install dependencies (only required the first time)
npm install

# Start the development server
npm start
```

### 3. Open the App!
Open your web browser and navigate to `http://localhost:4200`. You can freely register a new user and start organizing your tasks!
