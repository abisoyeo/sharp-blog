# SharpBlog API

A comprehensive ASP.NET Core Web API for managing blog posts with authentication, authorization, and advanced querying capabilities. Built with clean architecture principles and modern .NET practices.

## Features

* Authentication & Authorization: JWT-based authentication with role-based access control
* User Management: Registration, login, and profile updates
* Blog Post Management: Full CRUD operations with author-specific endpoints
* Advanced Querying: Search, filtering, sorting, and pagination
* Tag System: Dynamic tag creation and management
* Category Support: Organize posts by categories
* Role-Based Access: Admin, Author, and Reader roles
* Data Transfer Objects: Safe data transfer with DTOs
* Repository Pattern: Clean separation of concerns
* Swagger Documentation: Interactive API documentation

## Architecture

### Tech Stack

* Framework: ASP.NET Core 8
* Database: SQL Server with Entity Framework Core
* Authentication: JWT Bearer tokens
* Documentation: Swagger/OpenAPI
* Pattern: Repository pattern with dependency injection

### Project Structure

```

SharpBlog/
├── Controllers/
│   ├── BlogPostsController.cs
│   └── AccountController.cs
├── Data/
│   ├── BlogDbContext.cs
│   └── Repository/
│       ├── IBlogRepo.cs
│       ├── BlogRepo.cs
│       └── IUserRepo.cs
├── Models/
│   ├── BlogPost.cs
│   ├── User.cs
│   ├── Tag.cs
│   └── DTOs/
│       ├── BlogPostDTO.cs
│       ├── BlogPostResponseDTO.cs
│       ├── RegisterUserDTO.cs
│       ├── LoginDTO.cs
│       └── UpdateUserDTO.cs
├── Services/
│   └── IBlogAuthenticationService.cs
└── Program.cs

````

## Getting Started

### Prerequisites

* .NET 8 SDK
* SQL Server (LocalDB, Express, or full version)
* (Optional) Visual Studio 2022+ or VS Code

### Installation

```bash
git clone [https://github.com/your-username/SharpBlog.git](https://github.com/your-username/SharpBlog.git)
cd SharpBlog
````

### Configure Database Connection

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SharpBlogDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Authentication": {
    "SecretKey": "your-super-secret-key-here-min-256-bits",
    "Issuer": "SharpBlog",
    "Audience": "SharpBlogUsers"
  }
}
```

### Apply Database Migrations

```bash
dotnet ef database update
```

### Run the Application

```bash
dotnet run
```

### Access Swagger UI

Navigate to: `https://localhost:<port>/swagger`

## API Endpoints

### Authentication

| Method | Endpoint             | Description           | Auth Required |
| :----- | :------------------- | :-------------------- | :------------ |
| `POST` | `/api/account/register` | Register new user     | No            |
| `POST` | `/api/account/login`    | Login user            | No            |
| `PATCH`| `/api/account/update`   | Update user profile   | Yes           |

### Blog Posts

| Method | Endpoint             | Description              | Roles                  |
| :----- | :------------------- | :----------------------- | :--------------------- |
| `GET`  | `/api/blogposts`     | Get all posts with filtering | Admin, Author, Reader  |
| `GET`  | `/api/blogposts/{id}` | Get single post          | Admin, Author, Reader  |
| `GET`  | `/api/blogposts/me`  | Get current user's posts | Author                 |
| `GET`  | `/api/blogposts/me/{id}` | Get specific user post | Author                 |
| `POST` | `/api/blogposts/me`  | Create new post          | Admin, Author          |
| `PUT`  | `/api/blogposts/me/{id}` | Update user's post       | Admin, Author          |
| `DELETE`| `/api/blogposts/me/{id}` | Delete user's post       | Admin, Author          |

### Query Parameters for `GET /api/blogposts`

  * `author`: Filter by author name
  * `tag`: Filter by tag name
  * `category`: Filter by category
  * `search`: Search in title, content, or category
  * `sortBy`: Sort by 'title', 'createdat', or 'author'
  * `isDescending`: Sort direction (true/false)
  * `pageNumber`: Page number (default: 1)
  * `pageSize`: Items per page (default: 10)

### Example Requests

**Register User:**

```bash
curl -X POST "https://localhost:5001/api/account/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "password": "SecurePassword123!",
    "role": "Author"
  }'
```

**Login:**

```bash
curl -X POST "https://localhost:5001/api/account/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePassword123!"
  }'
```

**Create Blog Post:**

```bash
curl -X POST "https://localhost:5001/api/blogposts/me" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Getting Started with ASP.NET Core",
    "content": "This is a comprehensive guide...",
    "category": "Programming",
    "tags": ["dotnet", "aspnetcore", "tutorial"]
  }'
```

**Search Posts:**

```bash
curl "https://localhost:5001/api/blogposts?search=dotnet&sortBy=createdat&isDescending=true&pageSize=5"
```

## Authentication & Authorization

### User Roles

  * **Admin**: Full access to all operations
  * **Author**: Can create, read, update, and delete their own posts
  * **Reader**: Can only read posts

### JWT Token

Include the JWT token in the `Authorization` header:

`Authorization: Bearer <your-jwt-token>`

## Response Format

### Successful Response

```json
{
  "id": 1,
  "title": "Sample Blog Post",
  "content": "This is the content...",
  "authorName": "John Doe",
  "tags": ["dotnet", "tutorial"],
  "category": "Programming",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T12:00:00Z"
}
```

### Paginated Response

```json
{
  "items": [...],
  "totalItems": 25,
  "pageNumber": 1,
  "pageSize": 10
}
```

### Error Response

```json
{
  "error": "Blog post with ID 999 not found."
}
```

## Development

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

```


## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SharpBlogDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Authentication": {
    "SecretKey": "your-secret-key-must-be-at-least-256-bits-long",
    "Issuer": "SharpBlog",
    "Audience": "SharpBlogUsers"
  },
  "AllowedHosts": "*"
}
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.