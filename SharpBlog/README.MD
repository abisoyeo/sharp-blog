# SharpBlog API

SharpBlog is a simple and extensible ASP.NET Core Web API for managing blog posts, authors, tags, and categories. It uses SQL Server for data persistence and demonstrates clean architecture practices like the repository pattern, DTO usage, and modular design.

## ✨ Features

- CRUD operations for blog posts  
- Author-based post management  
- Category and tag support (string-based)  
- Query filtering by author, tag, and category  
- DTOs for safe data transfer  
- Entity Framework Core + SQL Server  
- Swagger UI for API testing  

## 📁 Project Structure

```
SharpBlog/
├── Controllers/
│   └── BlogController.cs
├── Data/
│   ├── SharpBlogDbContext.cs
│   └── Repository/
│       ├── IBlogRepo.cs
│       └── BlogRepo.cs
├── DTOs/
│   ├── BlogPostDTO.cs
│   └── BlogPostResponseDTO.cs
├── Models/
│   ├── BlogPost.cs
│   └── Author.cs
├── Program.cs
└── SharpBlog.csproj
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/)
- (Optional) Visual Studio 2022+ or VS Code

### Setup Instructions

1. **Clone the repository**

```bash
git clone https://github.com/your-username/SharpBlogApp.git
cd SharpBlogApp
```

2. **Update the DB connection string** in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SharpBlogDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. **Apply Migrations & Seed Data**

```bash
dotnet ef database update
```

4. **Run the App**

```bash
dotnet run
```

> Access Swagger at: `https://localhost:<port>/swagger`

## 📦 API Endpoints

### Blog Posts

| Method | Route           | Description               |
|--------|------------------|---------------------------|
| GET    | `/api/blogs`     | Get all posts (with optional filters) |
| GET    | `/api/blogs/{id}`| Get single post by ID     |
| POST   | `/api/blogs`     | Create new post           |
| PUT    | `/api/blogs/{id}`| Update post by ID         |
| DELETE | `/api/blogs/{id}`| Delete post by ID         |

### Filtering Example

```http
GET /api/blogs?authorName=Jane&tag=dotnet&category=Backend
```

## 🧠 Technologies Used

- ASP.NET Core 8 Web API  
- Entity Framework Core  
- SQL Server  
- Swagger / Swashbuckle   

## 📤 Deployment Notes

You can deploy this API to:

- Azure App Services 
- [Render](https://render.com/)
- [Railway](https://railway.app/)
- Docker-based hosting (w/ SQL Server container)

## ✅ To-Do

- Add pagination & sorting
- Add image upload support
- Create a client app (React/Blazor)

## 📄 License

This project is licensed under the MIT License.
