🚗 AutoDealerPro
AutoDealerPro is a distributed automotive management system built with .NET 8. It combines a modern MVC web application with Artificial Intelligence (ML.NET) for price prediction and gRPC microservices for real-time financial calculations.

📋 Project Overview
This project demonstrates a Microservices-Ready Architecture where distinct components handle specific business logic:

Inventory Management: A robust CRUD system for managing thousands of vehicles.

AI Price Consultant: An integrated Machine Learning model that analyzes vehicle specifications to predict fair market value.

Real-Time Conversion: A high-performance gRPC service that handles currency conversion (USD ↔ EUR/RON).

🛠️ Tech Stack & Architecture
The solution consists of three distinct projects:

1. 🖥️ AutoDealer.Web (The Client)
Framework: ASP.NET Core MVC (.NET 8).

Database: Microsoft SQL Server (LocalDB) via Entity Framework Core.

Frontend: Razor Views, Bootstrap 5, HTML5/CSS3.

Role: Handles user interaction, database persistence, and orchestrates calls to backend services.

2. 🧠 AutoDealer.API (The Brain)
Type: RESTful Web API.

Technology: ML.NET (Machine Learning for .NET).

Algorithm: LightGBM Regression (FastTree).

Role: Acts as a "Black Box" prediction engine. Receives vehicle data (Year, Mileage, HP, Brand) and returns a price estimate.

3. ⚡ AutoDealer.GRPC (The Calculator)
Type: gRPC Service.

Protocol: HTTP/2, Protobuf.

Role: Provides ultra-low latency currency conversion. It is consumed by the Web App to display prices in USD, EUR, and RON simultaneously.

✨ Key Features
Intelligent Data Seeding: Automatically imports and normalizes over 50,000 records from CSV files upon startup.

Smart Price Evaluation:

Compares the seller's price against the AI-predicted market value.

Displays badges like "Good Deal" or "Overpriced".

Advanced Search: Filter vehicles by Brand, Model, or text search.

Dual-Protocol Communication:

Uses HTTP/REST for AI predictions.

Uses gRPC for financial data.

Responsive UI: Modern dashboard design with detailed vehicle views.

🚀 Getting Started
Prerequisites
Visual Studio 2022 (with ASP.NET and Web Development workload).

.NET 8.0 SDK.

SQL Server Express or LocalDB.

Installation & Running
Clone the repository:

Bash

git clone https://github.com/YourUsername/AutoDealerPro.git
Open the Solution: Open AutoDealerPro.sln in Visual Studio.

Configure Startup Projects (Crucial): Since this is a distributed system, all services must run simultaneously.

Right-click on Solution 'AutoDealerPro' -> Properties.

Select Multiple startup projects.

Set Action to Start for all three projects:

AutoDealer.API

AutoDealer.GRPC

AutoDealer.Web

Run the Application: Press F5. The browser will open the Web App. The API and gRPC services will run in the background (console windows).
