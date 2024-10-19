# Bookshop Website with ASP.NET Core [UNDER WORKING]

This project is a web application for a bookshop built using ASP.NET Core, designed to provide users with an intuitive and efficient way to browse, search, and purchase books online. The application leverages LocalDB for database management and is developed using Visual Studio.

## Key Functions
- **User Authentication**: Secure user registration and login system using Auth0.
- **Book Management**: Admin interface for adding, updating, and removing books from the inventory.
- **Shopping Cart**: Functionality to add and remove books from the shopping cart.
- **Search and Filter**: Users can search for books by title, author, or genre.
## Technologies Used
- ASP.NET Core
- Entity Framework Core
- Bootstrap 5
- Auth0
- LocalDB
## Getting Started

Follow these instructions to set up and run the project locally.

### Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download) (version 6.0 or higher)
- [Visual Studio](https://visualstudio.microsoft.com/) (version 2022 or higher) with ASP.NET and web development workload
- LocalDB installed (comes with Visual Studio)

### Installing
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/bookshop-website.git
2. Navigate to the project directory:
   ```bash
   cd bookshop-website
4. Restore the dependencies:
   ```bash
   dotnet restore
6. Update the LocalDB connection string in appsettings.json as needed:
   ```bash
   "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookshopDb;Trusted_Connection=True;MultipleActiveResultSets=true"}
8. Run the migrations to set up the database:
    ```bash
   dotnet ef database update
10. Start the application:
    ```bash
    dotnet run
## Usage
### User Feature
#### Sign in

#### Login

#### External authorization
You can use google authorization to login the website

#### Search a book
You can search book by title 

#### Category
You can find books with a specific genre or language

#### Add book to cart
You can press this button a book to your cart

### Admin Features
#### How to add a user as admin

#### Add a book

#### Update a book

#### Delete a book


## Sceenshots


## Limitations
- *Lack of features:* The application currently supports basic book management and does not include advanced features such as payment processing.
- *Limited database:* The database is limited to LocalDB; production deployment requires a more robust database solution.
- *Unresposive:* Some features may not be fully optimized for all screen sizes or browsers.
- *No wishlist:*
- *No comment section:*
- *No book rating:*
  
## Future Improvements
