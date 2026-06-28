# ShopHub Startup Template

A clean ASP.NET Core MVC startup template designed for students to build E-Commerce projects using the Repository Pattern and Entity Framework Core.

## Features

- ASP.NET Core MVC
- Entity Framework Core
- Repository Pattern
- SQL Server Integration
- Identity Authentication
- Bootstrap UI
- AdminLTE Dashboard
- DataTables Integration
- Toastr Notifications
- SweetAlert2
- TinyMCE Support (Optional)
- File Upload Support
- Session Configuration

## Included Modules

### Category
- Create Category
- View Categories
- Edit Category
- Delete Category

### Product
- Create Product
- Upload Product Image
- View Products
- Edit Product
- Delete Product

## Project Structure

```
Controllers/
DataAccess/
Entities/
    Models/
    ViewModels/
Repositories/
Views/
wwwroot/
```

## Technologies

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- LINQ
- Bootstrap 5
- AdminLTE 3
- jQuery
- DataTables

## Database

Update the connection string inside:

```
appsettings.json
```

Then run:

```bash
Update-Database
```

or

```bash
dotnet ef database update
```

## Default Features

- Repository Pattern
- Dependency Injection
- CRUD Operations
- File Upload
- Entity Relationships
- ViewModels
- TempData Notifications

## Notes

This template is intended as a starting point for educational E-Commerce projects. Students are expected to extend it with additional features such as:

- Shopping Cart
- Orders
- Payments
- Reviews
- Wishlist
- Authentication Enhancements
- Dashboard Analytics

## License

Educational Use Only.
