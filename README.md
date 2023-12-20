# AspNetSpaceShip
=======
# Spaceship Manager Browser Game

## Description
Spaceship Manager is a browser-based strategy game where players manage spaceships and space stations, embarking on various missions in a dynamic space environment.

## Getting Started

### Prerequisites
- Docker and Docker Compose (for Dockerized setup)
- .NET SDK (for non-Dockerized setup)
- Node.js (for non-Dockerized setup)
- PostgreSQL (for non-Dockerized setup)
- IDE such as Visual Studio Code or IntelliJ Rider

### Running with Docker (Recommended)

1. **Clone the Repository**
- git clone https://github.com/yourusername/spaceship-game.git
- cd spaceship-game

2. **Using Docker Compose**
- Ensure `docker-compose.yml` is present in the root directory.
- Run the application using the command:
  ```
  docker-compose up
  ```

3. **Access the Application**
- Frontend: Open `http://localhost:3000` in your browser.
- Backend API: Access `http://localhost:5056/swagger` for Swagger UI.

### Running without Docker

1. **Database Setup**
- Install PostgreSQL and create a database named `spaceship`.
- Update the backend's `appsettings.json`:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=spaceship;Username=postgres;Password=your_password"
  }
  ```

2. **Backend Setup**
- In the backend directory, run:
  ```
  dotnet restore
  dotnet build
  dotnet run
  ```
- Access Swagger UI at `http://localhost:5056/swagger`.

3. **Frontend Setup**
- In the frontend directory, run:
  ```
  npm install
  npm start
  ```
- Open `http://localhost:3000` in your browser.

### IDE Setup

#### Visual Studio Code
- Install the C# extension.
- Use launch configurations to run or debug the application.

#### IntelliJ Rider
- Open the solution or project file.
- Utilize Rider's built-in tools to run or debug.

## Contributing
Contributions are welcome! Please follow the standard GitHub flow for submitting issues or pull requests.

## License
This project is licensed under [Your License Choice].

## Additional Notes
- Ensure you change the password in the `docker-compose.yml` and `appsettings.json` files to maintain security.
- Replace any placeholder URLs with your project's actual URLs.
