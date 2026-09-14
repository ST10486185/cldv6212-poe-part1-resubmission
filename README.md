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
```
---
