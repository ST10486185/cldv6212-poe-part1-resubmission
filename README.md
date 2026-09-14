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




*Version:* V1.2

*Docker Hub Repository: https://hub.docker.com/r/st10478074daelen/coffeenchill-functions/tags
```
---
---
The working Docker image has been uploaded to Docker Hub:
<img width="1917" height="947" alt="image" src="https://github.com/user-attachments/assets/af21c308-eae1-4545-9c61-9124a3dd9dc3" />

<img width="1915" height="927" alt="image" src="https://github.com/user-attachments/assets/6bb51c8d-0393-4973-9206-9a0708650bb7" />

We forgot to show the Docker hub container running in the youtube video, these images provide evidence of it running.

## Team Contributions

### Ethan Gangadaran: ST10473828

- Models
- Postman Collection
- Postman testing
- ReadME

### Thashen Reddy: ST10474546

- Staff document service
- Dockerfile
- Docker testing
- Edit YouTube Video

### Daelen Govender:  ST10478074

- Project planning
- Menu HTTP endpoints
- Staff document HTTP endpoints
- Docker configuration and testing
- Docker testing

### Livash Naidoo: ST10486185

- Project planning
- Project setup
- Services
- Azure Files connection configuration
- ReadME

---

## Video Demonstration

Below is the link to the YouTube Demonstration Video:

https://youtu.be/kcNTskiAIDw
