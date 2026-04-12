# FutureTech Academy Secure Student Management System

ASP.NET Core MVC application implementing secure admin-only student management with Google OAuth 2.0, Azure Cosmos DB, and Azure Blob Storage with SAS-protected profile image access.

## Implemented Features

- OAuth 2.0 login using Google
- Admin allow-list role-based authorization
- Student CRUD:
  - Add student with validated JPEG/PNG profile image
  - Edit student details and optionally replace image
  - Search by name, surname, or ID
  - Paginated student list
  - Soft delete (mark inactive)
  - Permanent delete
- Azure Cosmos DB document storage for student records
- Azure Blob Storage for profile images
- Time-limited SAS links for image viewing
- Image resizing before upload (ImageSharp)
- xUnit tests for student service and admin access logic

## Student Document Shape in Cosmos DB

```json
{
  "id": "student-001",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "mobileNumber": "+1234567890",
  "enrolmentStatus": "Active",
  "profileImageUrl": "https://<storageaccount>.blob.core.windows.net/student-images/student-001.jpg"
}
```

## Configuration

Update these files with your real secrets and endpoints:

- FutureTech.StudentManagement.Web/appsettings.json
- FutureTech.StudentManagement.Web/appsettings.Development.json

Required sections:

- Authentication.Google.ClientId
- Authentication.Google.ClientSecret
- Authentication.AdminAccess.AllowedEmails
- Authentication.AdminAccess.AllowedDomains (optional but recommended)
- Authentication.AdminAccess.AllowedEmailsCsv (optional for Azure App Settings)
- Authentication.AdminAccess.AllowedDomainsCsv (optional for Azure App Settings)
- Azure.Cosmos.Endpoint
- Azure.Cosmos.Key
- Azure.Cosmos.DatabaseName
- Azure.Cosmos.ContainerName
- Azure.BlobStorage.ConnectionString
- Azure.BlobStorage.ContainerName

For Azure App Service, arrays can be awkward to manage in Application Settings. You can set CSV values instead:

- Authentication__AdminAccess__AllowedEmailsCsv = admin1@futuretech.ac.za,admin2@futuretech.ac.za
- Authentication__AdminAccess__AllowedDomainsCsv = futuretech.ac.za

## Local Run

1. Restore and build:
   - dotnet restore
   - dotnet build
2. Run web app:
   - dotnet run --project FutureTech.StudentManagement.Web
3. Open the HTTPS URL shown in terminal.

## Testing

Run all tests:

- dotnet test FutureTech.StudentManagement.Tests

## Azure Deployment

1. Create Azure resources:
   - App Service + App Service Plan
   - Cosmos DB account, database, container
   - Storage account with student-images container
   - Application Insights
2. Configure App Service application settings from appsettings keys.
3. Set up deployment slots (staging and production).
4. Enable autoscale on the App Service plan.
5. Validate login, CRUD, image upload, and SAS retrieval in staging before swapping.
