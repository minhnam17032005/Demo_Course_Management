# Demo Course Management System (.NET / C#)

A practical backend project built with **ASP.NET Core Web API + Entity Framework Core + SQL Server**, developed to strengthen real-world backend engineering skills and transition professionally into the **C# / .NET ecosystem**.

This project is not just basic CRUD. It focuses on how production systems are commonly designed: clean layering, business rules, authentication, authorization, status workflows, data integrity, and maintainable architecture.

## Why I Built This Project

After exploring backend development across different technologies, I decided to focus deeply on **C# and ASP.NET Core** because of its strong enterprise adoption, clean architecture support, performance, and long-term career opportunities.

To make that transition seriously, I built this project to practice how real backend systems are structured and delivered in companies using .NET.

## Core Features

### Authentication & Security

* User registration / login
* JWT Access Token
* Refresh Token ready structure
* Password hashing with BCrypt
* Profile management
* Change password
* Logout flow

### Authorization (RBAC)

* Role-based access control
* Roles: **Admin / Staff / Customer**
* Permissions management
* Role-Permission assignment
* Protected endpoints using authorization rules

### Business Modules

* Users management
* Products management
* Categories management
* Orders management
* Order Items relationship

### Real Business Logic

* Order lifecycle: Pending / Completed / Cancelled
* Prevent deleting products used in pending orders
* Prevent disabling users with active pending orders
* Automatic stock update when placing / cancelling orders
* Soft delete + restore flow
* Active / inactive state handling

### Engineering Practices

* DTO pattern
* Repository pattern
* Service layer architecture
* Global exception middleware
* Validation handling
* Clean API response format
* SQL relational design with foreign keys

## Tech Stack

* **C#**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**
* **JWT Authentication**
* **BCrypt**
* **Swagger / OpenAPI**
* **Git / GitHub**

## What This Project Demonstrates

This project reflects my transition into **C# backend development** with a focus on practical engineering rather than tutorial-only learning.

It demonstrates ability to:

* Build scalable REST APIs
* Design relational databases
* Implement authorization systems
* Handle real business rules
* Write maintainable layered code
* Work with enterprise .NET backend patterns

## Current Direction

I am continuing to deepen my expertise in the C# / .NET ecosystem with next-step topics such as:

* Clean Architecture
* Redis caching
* Background jobs
* Docker deployment
* Unit / Integration testing
* Advanced authorization
* Microservices concepts

## Goal

To become a strong backend engineer specializing in **C# / ASP.NET Core**, capable of building reliable business systems used in production environments.
