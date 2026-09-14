# CoffeeNChill Canteen Management System

## CLDV6212: Cloud Development B POE Part 1

---

## Project Description

CoffeeNChill is a Management System developed for a University Canteen in order to modernize operations.

Part 1 focuses on creating a simple cloud-based backend using Azure Functions, Tables, Files, and Docker Hub.

The system provides digital menu management and staff document storage.

Menu items are stored in an Azure table called 'MenuItems'. The category is used as the PartitionKey and the SKU is used as the RowKey.

The system also allows for staff documents to be uploaded, listed, and downloaded. They are stored in an Azure File Share called 'staff-docs'.

---

## Project Structure

```text
CoffeeNChill_Part1

    Functions
        MenuFunctions.cs
        DocumentFunctions.cs

    Models
        MenuItem.cs
        MenuItemDtos.cs
        DocumentMetadata.cs

    Services
        IMenuTableService.cs
        MenuTableService.cs
        IStaffDocumentService.cs
        StaffDocumentService.cs

    docs
        CoffeeNChill-Part1.postman_collection.json

    Dockerfile
    Program.cs
    host.json
    local.settings.json
    CoffeeNChill_Part1.csproj

Azure Table Storage

The project uses an Azure Table called 'MenuItems'

Each menu item stores a:

- PartitionKey
- RowKey
- Name
- Description
- Price
- IsAvailable

---

## Azure Files

The project uses an Azure File Share called 'staff-docs'

The file share is used to store staff documents.

The Azure Storage Connection String is stored locally in local.settings.json

---

## Running Azurite

Azurite is used to provide local Azure Storage services during development

---

## Local Settings

The application uses local.settings.json for local configuration.

The Azure connection String must be added locally inside this file.

---

## API Endpoints

### Menu Endpoints

- *Create Menu Item:* POST /api/menu
- *Get All Menu Items:* GET /api/menu
- *Get Menu Items By Category:* GET /api/menu/category/{category}
- *Update Menu Item:* PUT /api/menu/{category}/{id}
- *Delete Menu Item:* DELETE /api/menu/{category}/{id}

### Document Endpoints

- *Upload Document:* POST /api/documents/upload
  - Use multipart/form-data
  - Form-data field is file
  - Currently allows PDF, DOCX, and TXT

- *Get Documents:* GET /api/documents

- *Download Document:* GET /api/documents/download/{fileName}

---

## Postman Testing

The Postman Collection can be found in /docs/CoffeeNChill-Part1.postman_collection.json

The file consists of all API endpoints tested using postman.

---

## Docker

A Dockerfile is included in the project, it is used to create a container image for the Azure Functions Application

---

## Docker Hub

The working Docker image has been uploaded to Docker Hub:

*Version:* V1.2

*Docker Hub Repository: https://hub.docker.com/r/st10478074daelen/coffeenchill-functions/tags
```
---
