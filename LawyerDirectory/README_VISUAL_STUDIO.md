# Lawyer Directory Web Application

A complete full-stack web application for searching and browsing lawyers by practice area, experience, location, and more. Built with ASP.NET Core, Entity Framework Core, and vanilla JavaScript.

## 🎯 Features

- **Smart Search**: Filter lawyers by area of practice, years of experience, state, and city
- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- **Real-time Validation**: Input validation with helpful error messages
- **Beautiful UI**: Modern, professional interface with gradient backgrounds
- **Database Seeded**: Comes with 20 sample lawyers across multiple states
- **RESTful API**: Clean API endpoints for all operations

## 📋 Prerequisites

Before running this application, ensure you have:

1. **Visual Studio 2022** (or later) - [Download here](https://visualstudio.microsoft.com/downloads/)
   - Workload: "ASP.NET and web development"
   - Component: ".NET 8.0 Runtime"

2. **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)

3. **Git** (for version control) - [Download here](https://git-scm.com/downloads)

## 🚀 Getting Started with Visual Studio

### Option 1: Open Directly in Visual Studio

1. **Open Visual Studio 2022**

2. **Open the Project**:
   - Click **File → Open → Project/Solution**
   - Navigate to the `LawyerDirectory` folder
   - Select `LawyerDirectory.csproj`
   - Click **Open**

3. **Restore NuGet Packages**:
   - Visual Studio will automatically restore packages
   - Or right-click on the project → **Restore NuGet Packages**

4. **Build the Solution**:
   - Press `Ctrl + Shift + B`
   - Or click **Build → Build Solution**

5. **Run the Application**:
   - Press `F5` (Debug mode) or `Ctrl + F5` (Without debugging)
   - Or click the green **Play** button (usually shows "LawyerDirectory")

6. **Access the Application**:
   - Your default browser will open automatically
   - Navigate to: `https://localhost:7xxx` or `http://localhost:5xxx`
   - The exact port number will be shown in the console

### Option 2: Using Command Line (Alternative)

If you prefer using the command line:

```bash
# Navigate to the LawyerDirectory folder
cd LawyerDirectory

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

Then open your browser and go to: `https://localhost:5001` (or the port shown in the terminal)

## 📁 Project Structure

```
LawyerDirectory/
├── Controllers/
│   └── LawyersController.cs       # API endpoints for lawyer operations
├── Data/
│   ├── LawyerContext.cs           # Entity Framework database context
│   └── SeedData.cs                # Sample data seeder
├── Models/
│   └── Lawyer.cs                  # Lawyer data model
├── wwwroot/                       # Static files (frontend)
│   ├── css/
│   │   └── styles.css             # Responsive CSS styling
│   ├── js/
│   │   └── app.js                 # Frontend JavaScript logic
│   └── index.html                 # Main HTML page
├── appsettings.json               # Configuration settings
├── Program.cs                     # Application entry point
└── LawyerDirectory.csproj         # Project file
```

## 🗄️ Database Schema

The application uses SQLite with the following schema:

| Column             | Type         | Description                          |
|--------------------|--------------|--------------------------------------|
| Id                 | int          | Primary key                          |
| Name               | string(200)  | Lawyer's full name                   |
| ProfilePictureUrl  | string(500)  | URL to profile picture               |
| AreaOfPractice     | string(500)  | Practice areas (comma-separated)     |
| Description        | string(2000) | Professional description             |
| YearsOfExperience  | int          | Years practicing law                 |
| State              | string(100)  | State where lawyer practices         |
| City               | string(100)  | City where lawyer practices          |
| Website            | string(500)  | Firm or personal website             |
| AmlawRanking       | string(10)   | AmLaw firm ranking (or "NR")         |

## 🔌 API Endpoints

All API endpoints are under `/api/lawyers`:

| Endpoint                          | Method | Description                        |
|-----------------------------------|--------|------------------------------------|
| `/api/lawyers/search`             | GET    | Search lawyers with filters        |
| `/api/lawyers/states`             | GET    | Get all available states           |
| `/api/lawyers/cities?state={}`    | GET    | Get cities for a specific state    |
| `/api/lawyers/practices`          | GET    | Get all practice areas             |
| `/api/lawyers`                    | GET    | Get all lawyers                    |

### Search Query Parameters

- `areaOfPractice` (string): Filter by practice area (partial match)
- `minYears` (int): Minimum years of experience
- `maxYears` (int): Maximum years of experience
- `state` (string): Filter by state
- `city` (string): Filter by city

**Example**:
```
GET /api/lawyers/search?areaOfPractice=Corporate&minYears=10&state=California
```

## 🎨 Frontend Features

1. **Dynamic Dropdowns**:
   - State dropdown populated from database
   - City dropdown updates based on selected state

2. **Input Validation**:
   - Minimum years cannot be negative
   - Maximum years must be ≥ minimum years
   - Real-time error messages

3. **Responsive Table**:
   - Desktop: Traditional table layout
   - Mobile: Cards with stacked information

4. **Loading States**:
   - Animated spinner during API calls
   - Smooth transitions

## 🔧 Troubleshooting

### Issue: "The type or namespace name 'Microsoft' could not be found"

**Solution**: Restore NuGet packages
- Right-click project → **Restore NuGet Packages**
- Or run: `dotnet restore`

### Issue: "Unable to bind to https://localhost:5001"

**Solution**: Port might be in use
1. Open `Properties/launchSettings.json`
2. Change the port numbers
3. Or stop other applications using those ports

### Issue: Database doesn't have data

**Solution**: Delete the database and restart
1. Delete `lawyers.db` file (if it exists)
2. Restart the application
3. The seed data will be automatically inserted

### Issue: Build errors about .NET 8.0

**Solution**: Install .NET 8.0 SDK
- Download from: https://dotnet.microsoft.com/download/dotnet/8.0
- Restart Visual Studio after installation

## 📝 Git Instructions

### Initial Setup (First Time)

```bash
# Navigate to the LawyerDirectory folder
cd LawyerDirectory

# Initialize git repository
git init

# Add all files
git add .

# Create initial commit
git commit -m "Initial commit: Lawyer Directory Application"

# Create and switch to a new branch
git checkout -b development
```

### Daily Git Workflow

```bash
# Check status of your changes
git status

# Add specific files
git add Controllers/LawyersController.cs
# Or add all changes
git add .

# Commit with a descriptive message
git commit -m "Add new search filter for AmLaw ranking"

# Create a new branch for a feature
git checkout -b feature/add-email-field

# Switch between branches
git checkout main
git checkout development

# View commit history
git log --oneline

# Push to remote repository (if you have one)
git remote add origin https://github.com/yourusername/lawyer-directory.git
git push -u origin main
```

### Connecting to GitHub

```bash
# Create a new repository on GitHub first, then:

# Add remote repository
git remote add origin https://github.com/yourusername/lawyer-directory.git

# Push your code
git push -u origin main

# For subsequent pushes
git push
```

## 🛠️ Customization

### Adding More Lawyers

Edit `Data/SeedData.cs` and add more `Lawyer` objects to the list:

```csharp
new Lawyer
{
    Name = "Your Lawyer Name",
    ProfilePictureUrl = "https://i.pravatar.cc/150?img=21",
    AreaOfPractice = "Your Practice Area",
    Description = "Your description",
    YearsOfExperience = 10,
    State = "Your State",
    City = "Your City",
    Website = "https://yourwebsite.com",
    AmlawRanking = "NR"
}
```

Then delete `lawyers.db` and restart the application.

### Changing the Theme Colors

Edit `wwwroot/css/styles.css` and update the gradient colors:

```css
/* Change from purple/blue to your colors */
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
```

### Using a Different Database

To use SQL Server instead of SQLite:

1. Update `LawyerDirectory.csproj`:
   ```xml
   <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
   ```

2. Update `Program.cs`:
   ```csharp
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
   ```

3. Update `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LawyerDirectory;Trusted_Connection=True;"
   }
   ```

## 📱 Testing on Mobile

1. **Find your local IP address**:
   - Windows: Run `ipconfig` in Command Prompt
   - Look for "IPv4 Address" (e.g., 192.168.1.100)

2. **Update launch settings**:
   - Edit `Properties/launchSettings.json`
   - Change `"applicationUrl"` to include your IP:
     ```json
     "applicationUrl": "https://0.0.0.0:7001;http://0.0.0.0:5001"
     ```

3. **Access from mobile**:
   - Connect mobile to same WiFi network
   - Open browser and go to: `http://YOUR_IP:5001`

## 📄 License

This project is provided as-is for educational and commercial use.

## 🤝 Support

For issues or questions:
1. Check the Troubleshooting section above
2. Review the code comments in the source files
3. Consult ASP.NET Core documentation: https://docs.microsoft.com/aspnet/core

## 🎓 Learning Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [JavaScript MDN Web Docs](https://developer.mozilla.org/en-US/docs/Web/JavaScript)
- [Git Documentation](https://git-scm.com/doc)

---

**Enjoy using the Lawyer Directory Application!** 🎉
