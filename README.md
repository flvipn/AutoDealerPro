# AutoDealerPro

AutoDealerPro is a distributed automotive management system built with **.NET 10**. The solution integrates a modern MVC web application with **Artificial Intelligence (ML.NET)** for price prediction and **gRPC microservices** for real-time financial calculations.

## Project Overview

This project demonstrates a microservices-ready architecture where distinct components handle specific business logic:

1.  **Inventory Management:** A robust system for managing vehicle records, featuring search, filtering, and pagination.
2.  **AI Price Analysis:** An integrated Machine Learning model that compares the seller's price against a predicted market value to determine if a deal is fair.
3.  **Real-Time Conversion:** A high-performance gRPC service that handles currency conversion (USD to EUR/RON) instantly.

## Architecture & Technologies

The solution is split into three distinct projects:

* **AutoDealer.Web (Client):**
    * Built with **ASP.NET Core MVC**.
    * Uses **SQL Server** and Entity Framework Core for data persistence.
    * Handles the user interface and orchestrates communication with backend services.

* **AutoDealer.API (AI Engine):**
    * A RESTful Web API utilizing **ML.NET**.
    * Uses a trained LightGBM regression model to predict vehicle prices based on year, mileage, horsepower, and brand.

* **AutoDealer.GRPC (Microservice):**
    * A dedicated gRPC service using HTTP/2 and Protobuf.
    * Provides low-latency currency conversion services for the web application.

## Key Features

* **Smart Data Seeding:** Automatically imports and processes over 50,000 records from CSV files upon startup.
* **Price Prediction:** Uses AI to flag vehicles as "Good Deal" or "Overpriced" based on market trends.
* **Dual-Protocol Communication:** Implements both HTTP/REST (for AI) and gRPC (for financial data) within the same view.
* **Advanced UI:** A responsive dashboard that aggregates data from the database, the AI API, and the gRPC service into a single unified view.

## Getting Started

### Prerequisites
* Visual Studio 2022
* .NET 8.0 SDK
* SQL Server (LocalDB or Express)

### How to Run
Since this is a distributed system, all three projects must run simultaneously to ensure full functionality.

1.  Clone the repository.
2.  Open `AutoDealerPro.sln` in Visual Studio.
3.  Right-click on the Solution -> **Properties**.
4.  Select **Multiple startup projects**.
5.  Set the Action to **Start** for:
    * `AutoDealer.API`
    * `AutoDealer.GRPC`
    * `AutoDealer.Web`
6.  Press **F5** to run.

## Contact

**Flaviu Iepan**
Connect on [LinkedIn](https://www.linkedin.com/in/flaviu-iepan)
