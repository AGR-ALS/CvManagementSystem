# Setup

## Clone Repository
```
git clone https://github.com/AGR-ALS/CvManagementSystem.git
cd CvManagementSystem/
```

## Enter enviromental variables

Create `.env` files and write variables into them according to `.env_examples` files.

For example:
```
EmailSettings__FromName='App'
EmailSettings__FromEmail='test@gmail.com'
EmailSettings__ToName='Recipient'
EmailSettings__ClientHost='smtp.gmail.com'
EmailSettings__ClientPort=465
EmailSettings__ClientLogin='test@gmail.com'
EmailSettings__ClientPassword='qwerty'
EmailSettings__UseSsl=true

RabbitMqSettings__Host=rabbitmq://rabbitmq
RabbitMqSettings__Username=test
RabbitMqSettings__Password=test
```
Each service contains its own `.env` file, make sure to go through all of them.

## Launch the App

```
docker compose up --build -d
```

### You can access the app at [localhost:3000](http://localhost:3000)

## Premade Admin

You can access premade admin user using credentials:
```
email: admin@admin.com
password: admin
```
*(these credentials work in the deployed version as well)*


# Key Features

## Core Architecture & Backend
- **Clean Architecture:** Built on **.NET 10** and **ASP.NET Core**, strictly following Clean Architecture principles (Domain, Application, Infrastructure, Api) with dependency injection, repository patterns, and separation of concerns.
- **Dynamic Attribute System:** Implemented a polymorphic attribute system supporting **8 data types** (String, Text/Markdown, Image, Numeric, Date, Period, Boolean, OneOfMany). Used **Table-Per-Type (TPT)** mapping strategy in EF Core for type-safe storage and querying of heterogeneous data in PostgreSQL 18.
- **Custom Access Rules Engine:** Developed a dynamic filtering engine that allows recruiters to define complex access rules for positions (e.g., "Experience > 3 years", "Education == Master", "Open to Relocation == Yes"). The system automatically validates candidate attributes against these rules using various `FilterOperator` types (Equal, NotEqual, GreaterThan, LessThan, Contains, Intersects, etc.).
- **Microservices & Event-Driven Architecture:** Separated email notification logic into a dedicated **MailService** microservice. Implemented asynchronous message exchange via **RabbitMQ** using **MassTransit**, ensuring decoupling and fault tolerance during peak loads.

## Security & Authentication

- **Multi-layer Auth System:** Implemented secure authentication using **JWT Access Tokens** + **Refresh Tokens** stored in HttpOnly/SameSite cookies. Integrated **OAuth 2.0** (Google, Facebook) for social login.
- **Policy-Based Authorization:** Built a custom RBAC system (Admin, Recruiter, Regular) using Policy-Based Authorization with custom attributes (`[AllowAdmin]`, `[AllowRecruiter]`, `[AllowRegular]`) and middleware/action filters for checking user block status.
- **Refresh Token Rotation:** Implemented automatic session refresh via `RefreshTokenAuthenticationMiddleware` that seamlessly renews expired access tokens without user intervention.

## Integrations

- **Odoo ERP Integration:** Integrated the CV system with Odoo ERP. Wrote a custom Python addon for Odoo and configured secure bi-directional data exchange via custom API tokens. Implemented aggregation of candidate application statistics on the backend (.NET) with transfer to ERP.
- **Salesforce CRM Integration:** Configured automatic data synchronization with Salesforce (creating Account and Contact via REST API with OAuth2 client_credentials flow).
- **Dropbox API Integration:** Implemented a support ticket system that automatically generates JSON reports and uploads them to Dropbox API with OAuth2 refresh_token flow which triggers **Power Automate** pipeline that sends to all admins email and push notification via **Power Automate** app with ticket information for further investigation.
- **S3-Compatible Object Storage:** Integrated **MinIO** for storing media files (avatars, images in resumes). Implemented **Presigned URL** generation, allowing the frontend to upload/download files directly from storage, bypassing the backend and reducing server load.

## UI/UX

- **Dynamic Forms & Validation:** Created universal components for rendering forms based on configuration (Dynamic Forms) to work with 8 different attribute types. Implemented on-the-fly validation, input masks, and autocomplete (e.g., when entering technologies).
- **Auto-Save Mechanism:** Implemented an Auto-Save feature for profile editing using custom React Hooks and Debounce, preventing data loss for users.
- **Real-time Polling:** Set up real-time polling for the vacancy discussion system (Discussions) without using WebSockets, with optimized re-renders.
- **Multi-language Support:** Configured internationalization (i18next) with Russian/English language switching.
- **Custom Error Handling:** Implemented custom API error handling (intercepting 401/403 with redirects and block status checks).

## Infrastructure

- **Fully Containerized:** Packaged the entire stack (Backend, Frontend, PostgreSQL 18, RabbitMQ, MinIO, Odoo, MailService) into a single `docker-compose.yml` for seamless local development and deployment.
- **Database Seeding:** Developed complex C# seeders using **Bogus** library that populate the database with realistic test data (users, positions, projects, attributes, discussions, access rules).
- **SPA Routing via Nginx:** Configured Nginx in the frontend Docker container to properly handle React Router SPA fallback routing.
- **OpenAPI Documentation:** Integrated Swagger/OpenAPI for automatic API documentation generation.

## Business Logic & Features

- **CV Management System:** Full-featured CV lifecycle management (create, edit, publish, like/unlike, version control). Candidates can create multiple CVs for different positions and attach their projects.
- **Position Management:** Recruiters can create, edit, clone, and delete positions with complex access rules, required technologies, and expertise levels (Junior/Middle/Senior).
- **Discussion System:** Built-in discussion/chat system for each position with real-time message updates.
- **Support Ticket System:** Users can create support tickets with priority levels, which are automatically formatted as JSON and uploaded to Dropbox.
- **User Management:** Admin panel for viewing, blocking/unblocking, and deleting users.