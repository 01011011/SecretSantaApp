# 🎅 Secret Santa App

A web application that automates Secret Santa gift exchange assignments. Add participants, define constraint groups (people who shouldn't be paired, such as spouses or family members), and let the algorithm handle the rest — guaranteeing a valid matching every time.

## What It Does

1. **Add Participants** — Enter names of people joining the gift exchange
2. **Create Constraint Groups** — Group people who should *not* give gifts to each other (e.g., couples, roommates)
3. **Match** — Click "Match" to run the algorithm and instantly see who gives a gift to whom

The matching algorithm ensures:
- Every person gives exactly one gift and receives exactly one gift
- No one is assigned to themselves
- Group constraints are respected (no gifting within the same group)
- A valid result is produced every time (if one exists)

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Runtime** | .NET 9 / ASP.NET Core |
| **Backend** | ASP.NET Core MVC + Web API |
| **Frontend** | Alpine.js 3, Bootstrap 5, Fetch API |
| **Data Storage** | In-memory (singleton repositories) |
| **DI** | Built-in ASP.NET Core dependency injection |
| **Algorithm** | Augmenting-path bipartite matching (guarantees optimal result) |

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later

### Run Locally

```bash
cd SecretSantaApp
dotnet run --urls "http://localhost:8888"
```

Open **http://localhost:8888** in your browser.

### Run with Docker

```bash
docker build -t secretsanta .
docker run -p 8888:8080 secretsanta
```

## API Endpoints

### Users

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/users` | List all users |
| `GET` | `/api/users/{id}` | Get user by ID |
| `POST` | `/api/users` | Create user (body: `"name"`) |
| `PUT` | `/api/users?id={id}` | Update user name (body: `"name"`) |
| `DELETE` | `/api/users?id={id}` | Delete user |
| `GET` | `/api/users/match` | Run matching algorithm |

### Groups

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/groups` | List all groups |
| `GET` | `/api/groups/{id}` | Get group by ID |
| `POST` | `/api/groups` | Create group (body: `{ Name, Users }`) |
| `PUT` | `/api/groups?id={id}` | Add users to group (body: `[users]`) |
| `DELETE` | `/api/groups?id={id}` | Remove user from group (body: `user`) |

## Project Structure

```
SecretSantaApp/
├── Program.cs                  # App entry point and DI configuration
├── Controllers/
│   ├── HomeController.cs       # Serves the main UI
│   ├── UsersController.cs      # User CRUD + matching API
│   └── GroupsController.cs     # Group CRUD API
├── Models/
│   ├── User.cs                 # Participant model
│   ├── Group.cs                # Constraint group model
│   └── UserGroupViewModel.cs   # Combined view model
├── Services/
│   ├── IUserRepository.cs      # User repository interface
│   ├── UserRepository.cs       # User storage + matching orchestration
│   ├── IGroupRepository.cs     # Group repository interface
│   ├── GroupRepository.cs      # Group storage
│   └── Graph.cs                # Bipartite matching algorithm
├── Infrastructure/
│   └── Helpers/Common.cs       # UserComparer + shuffle extensions
├── Views/
│   ├── Home/Index.cshtml       # Main SPA view (Knockout.js)
│   └── Shared/_Layout.cshtml   # Layout template
├── wwwroot/
│   ├── css/site.css            # Custom styles
│   └── js/secretsanta.js       # Alpine.js app component
└── Dockerfile                  # Multi-stage Docker build
```

## How the Matching Algorithm Works

The app models the problem as a **bipartite graph** where each participant is both a potential gifter (left side) and receiver (right side). Edges connect gifters to valid receivers (excluding themselves and same-group members).

It then finds a **maximum matching** using an augmenting-path algorithm:
1. For each gifter, attempt to assign a receiver via DFS
2. If a receiver is already taken, recursively try to reassign the existing gifter elsewhere
3. This backtracking guarantees finding a perfect matching whenever one exists

This is mathematically optimal — if any valid assignment exists, the algorithm will find one.

## Note

Data is stored in-memory and will be lost when the application restarts. This is by design for a lightweight, no-setup experience.
