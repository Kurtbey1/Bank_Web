This is a robust, secure, and scalable Banking Backend System built with ASP.NET Core Web API. The project focuses on "Business Integrity," ensuring that financial data is handled with the highest standards of security and reliability.

🚀 Key Features
Financial Engine: Full support for Deposits, Withdrawals, and Transfers with strict balance validation.

Security First:

JWT Authentication: Secure access using JSON Web Tokens.

Role-Based Access Control (RBAC): Distinct permissions for Admins and Employees.

Password Hashing: Industry-standard hashing for customer and card credentials.

Business Logic Integrity:

Soft Delete: Employees are never "hard deleted" from the database to maintain an audit trail.

Data Validation: Strict input validation for phone numbers (Jordanian format 07xxxxxxx), salaries, and transaction amounts.

Performance: Optimized SQL queries with Strategic Indexing on high-traffic columns (AccountID, IsDeleted).

🏗️ Technical Architecture
Framework: .NET 8 / ASP.NET Core Web API.

Database: Microsoft SQL Server (MSSQL).

ORM: Entity Framework Core (EF Core) with a Service-Repository Pattern approach.

Documentation: Swagger UI (OpenAPI) for interactive API testing.

📊 Database Schema (The Blueprint)
The system is built on a relational schema designed for high data integrity:

Branches & Employees: Managing the human capital and physical locations.

Customers & Accounts: A One-to-Many relationship supporting multiple account types.

Cards: A One-to-One secure relationship with bank accounts.

Transactions & Loans: Tracking every penny with UTC timestamps and interest rate calculations.

🛠️ Lessons Learned & "Senior" Decisions
During the development of this project, several critical engineering decisions were made:

Decimal over Int: Switched all financial fields from INT to DECIMAL(18,2) to support precision in monetary calculations.

Global Error Handling: Implemented defensive programming to catch null references and invalid inputs before they reach the database.

Naming Conventions: Resolved EF Core mapping issues (like the CustomersCUID conflict) by using Fluent API and Data Annotations.

Audit Trail over Deletion: Prioritized Soft Delete logic to ensure no banking record is ever truly lost, which is vital for regulatory compliance.

🚀 How to Run
Clone the Repository.

Database Setup:

Run the provided SQL Script in your SQL Server Management Studio (SSMS).

Update the ConnectionStrings in appsettings.json.

Build & Run:

Bash

dotnet build
dotnet run
Test: Open https://localhost:xxxx/swagger to explore the endpoints.

👨‍💻 Developed By
[Your Name] Backend Developer / Associate Software Engineer
