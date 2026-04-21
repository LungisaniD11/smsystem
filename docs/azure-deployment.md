# Azure Deployment Guide

## 1. Provision Azure Resources

Create these services in Azure:

1. Resource Group
2. App Service Plan (Linux or Windows)
3. App Service for the MVC app
4. Cosmos DB account (SQL API)
5. Cosmos database: futuretech-students
6. Cosmos container: students with partition key /id
7. Storage account and blob container: student-images
8. Application Insights (linked to App Service)

## 2. Configure App Settings in App Service

Set configuration values under App Service -> Environment variables:

- Authentication__Google__ClientId
- Authentication__Google__ClientSecret
- Authentication__AdminAccess__AllowedEmails__0
- Authentication__AdminAccess__AllowedEmails__1
- ApplicationInsights__ConnectionString  *(copy from Application Insights resource → Connection String)*
- Azure__Cosmos__Endpoint
- Azure__Cosmos__Key
- Azure__Cosmos__DatabaseName
- Azure__Cosmos__ContainerName
- Azure__BlobStorage__ConnectionString
- Azure__BlobStorage__ContainerName
- Azure__BlobStorage__SasExpiryMinutes
- Azure__BlobStorage__MaxUploadSizeMb
- Azure__BlobStorage__ResizeWidth
- Azure__BlobStorage__ResizeHeight

## 3. Deployment Slots

1. In App Service, create a staging slot.
2. Deploy new builds to staging first.
3. Validate login, student CRUD, and image SAS access.
4. Swap staging slot with production for near-zero downtime updates.

## 4. Autoscaling

1. Open App Service Plan -> Scale out.
2. Enable custom autoscale.
3. Example rules:
   - Scale out +1 instance when CPU > 70% for 10 minutes.
   - Scale in -1 instance when CPU < 35% for 20 minutes.

## 5. Monitoring

1. Enable Application Insights in App Service.
2. Configure alerts for:
   - Failed requests
   - Server response time
   - Exceptions
3. Track custom logs for upload and CRUD failures.

## 6. Security Checklist

- Keep Google client secret and Azure keys only in App Service settings.
- Restrict admin access through Authentication__AdminAccess__AllowedEmails.
- Use HTTPS only.
- Rotate Cosmos and Storage secrets regularly.
- Keep SAS expiry short (15 minutes default).
