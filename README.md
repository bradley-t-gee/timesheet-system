# Timesheet System

A simple in-memory timesheet entry system built with ASP.NET Core MVC (.NET 8). This application allows users to log time against projects, with full CRUD functionality, validation, and unit tests.

---

## Features

- Add, edit, and delete timesheet entries
- View all entries for a given user and week
- Summary of total hours per project and per week
- Prevent duplicate entries for the same user/project/date
- Form validation (including custom date validation)
- Razor-based front-end (MVC pattern)
- In-memory data storage (no database)
- Fully unit tested (using xUnit)

---

## Tech Stack

- **.NET 8**
- **ASP.NET Core MVC**
- **Razor Views**
- **xUnit** (unit testing)
- **C# 11** (`required` keyword, nullable reference types)
- **In-memory storage** (singleton service)

---

## Project Structure

TimesheetSolution/
├── TimesheetSystem/ # Main ASP.NET Core MVC app
│ ├── Controllers/ # TimesheetController (main UI logic)
│ ├── Models/ # TimesheetEntry, ViewModels
│ ├── Services/ # ITimesheetService & TimesheetService
│ ├── ValidationAttributes/ # Custom validation (e.g., NoFutureDate)
│ └── Views/ # Razor pages
├── TimesheetSystem.Tests/ # xUnit test project
└── README.md

---

## Running the Application

# Build the project

`dotnet build`

# Run the app

`dotnet run --project TimesheetSystem`

# Run tests

From the TimesheetSystem.Tests folder, run:

`dotnet test`

---
