# TinyLedger Web API

TinyLedger is a simple financial transaction API built with ASP.NET Core. It allows basic operations like creating accounts, performing deposits and withdrawals, and retrieving transaction history — ideal for learning or prototyping financial systems.

## 🚀 Getting Started

To run the project:

1. Clone or download the solution.
2. Open the solution in Visual Studio or your preferred IDE.
3. Run the project. No additional setup is required.

## 📬 Making Requests

You can interact with the API in two ways:

- Using the included [`TinyLedger.WebApi.http`](TinyLedger.WebApi.http) file for direct HTTP requests within supported editors (like Visual Studio or VS Code with REST Client extension).
- Using the built-in Swagger UI at:  
  `https://localhost:7060/swagger`

## 🧪 In-Memory Database

The project uses an **in-memory database** via Entity Framework Core. This means:

- All data is stored in memory and lost when the application stops.
- A **default account** is seeded at startup for convenience.
- You can also create new accounts at runtime.

## 📝 API Usage Instructions

### ➕ Create an Account
- **Endpoint:** `POST /Accounts`
- **Body:**
  ```json
  {
    "user": "username"
  }
  ```

### 💰 View Account Balance
- **Endpoint:** `GET /Accounts/{accountId}`

### 💳 Deposit Funds
- **Endpoint:** `POST /Accounts/{accountId}/transactions`
- **Body:**
  ```json
  {
    "amount": 100.50,
    "description": "Initial deposit"
  }
  ```

### 💸 Withdraw Funds
- **Endpoint:** `PATCH /Accounts/{accountId}/transactions`
- **Body:**
  ```json
  {
    "amount": 50.25,
    "description": "Groceries"
  }
  ```

### 📜 View Transaction History
- **Endpoint:** `GET /Accounts/{accountId}/transactions`

## 🛠 Technologies Used

- ASP.NET Core
- Entity Framework Core (In-Memory provider)
- Swagger / Swashbuckle
- C# 12 / .NET 8


## 🔁 Transaction Responses

All transaction endpoints (`POST` and `PATCH` to `/Accounts/{accountId}/transactions`) return a **unique identifier** for each transaction that is created. This identifier can be used to track or audit individual operations.

# 📌 Author's Note

Due to time constraints, I opted not to include more advanced architectural tools in this project, such as FluentValidation, AutoMapper and MediatR. However, if you're interested in seeing a more "real-world" implementation of my work, I invite you to explore one of my personal projects:

🔗 **Ragstore Backend**  
[https://github.com/aleffmoura/Ragstore/tree/master/Backend](https://github.com/aleffmoura/Ragstore/tree/master/Backend)  
> While the README is not thoroughly documented, the codebase itself is well-structured and showcases my development approach. As it is a personal project, I prioritized functionality over documentation. The project incorporates several advanced concepts, including multi-tenancy.

Additionally, I have published a NuGet package that I use professionally to facilitate working with `Result` and `Option` types in C#, applying some functional programming concepts:

🔗 **Functional Concepts for C#**  
[https://github.com/aleffmoura/FunctionalConcepts](https://github.com/aleffmoura/FunctionalConcepts)