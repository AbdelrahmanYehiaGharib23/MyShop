# 🛒 MyShop - E-Commerce Management System

A modern **E-Commerce Management System** built with **ASP.NET Core MVC (.NET 10)** following **N-Tier Architecture**, **Repository Pattern**, **Unit of Work**, and **Specification Pattern**.

The project includes a complete **Admin Dashboard** for managing the store and a modern **Customer Storefront** for browsing products, searching, sorting, managing the shopping cart, placing orders, making payments, and reviewing products.

---

## 🧭 System Workflow

MyShop is an online store backed by an admin dashboard. The sections below describe what happens behind the main customer and store actions, beyond the screenshots.

### Customer journey

1. **Discover products:** Customers browse the catalog, open product details, search by name, sort results, and move through server-paginated listings. They can also save products to Favorites.
2. **Build a cart:** Cart contents are kept in the customer session. Before checkout, the application refreshes product data and prices. If a product changed or became unavailable, the customer sees the updated cart and must review it before continuing.
3. **Place an order:** The customer enters delivery contact details and submits checkout. The application creates a pending order with a snapshot of the purchased items and prevents duplicate checkout submissions.
4. **Pay:** The customer chooses Stripe (USD) or Paymob (EGP). A successful browser redirect is not treated as proof of payment: the server verifies the provider callback/webhook, including the order and payment details, before marking the order paid. A pending or failed payment can be checked from the order/payment status pages.
5. **Follow delivery progress:** After payment, the customer can follow fulfillment stages in **My Orders**. The store enters the courier, tracking number, and estimated arrival date in the admin order page; those details then appear to the customer.

### Store and admin workflow

- Store staff use the admin dashboard to manage products, images, categories, customer accounts, roles, and orders.
- An order cannot enter fulfillment until its payment is confirmed. Fulfillment progresses one step at a time: **Awaiting preparation → Preparing → Shipped → Delivered**.
- Courier and tracking details are required before an order can be marked as shipped. These are details entered by the store; the project does not currently connect to a courier API or show a live GPS map.
- Product reviews and customer ratings are displayed with the product catalog. Order confirmation messages are sent through the configured email service.

### Application layers

- **MyShop.PL:** ASP.NET Core MVC web application, controllers, Razor views, view models, and static storefront/admin assets.
- **MyShop.BLL:** Application services, DTOs, mapping, and business workflows for products, categories, carts, favorites, orders, reviews, identity, email, and payments.
- **MyShop.DAL:** EF Core entities, database context, repositories, specifications, and unit-of-work implementation.

# 🚀 Features

## 🛍 Customer Store

* Modern Responsive Store UI
* Product Catalog
* Product Details
* Shopping Cart
* Customer Favorites
* Product Search
* Product Sorting
* Server-Side Pagination
* Dark & Light Theme
* Arabic and English interface support
* Session-Based Cart

---

## 📦 Product Management

* Create Product
* Update Product
* Delete Product
* Product Details
* Product Listing
* Product Image Upload
* Category Assignment

---

## 📂 Category Management

* Create Category
* Update Category
* Delete Category
* Category Listing
* Memory Cache (30 Minutes)
* Automatic Cache Invalidation after Create, Update, and Delete

---

## 🛒 Orders & Checkout

* Order Creation
* Checkout Process
* Order Summary
* Order Items Management
* Order Confirmation
* Customer Order Workflow
* Customer-facing fulfillment stages and store-provided delivery details

---

## 💳 Payment

* Stripe Payment Integration
* Paymob Payment Integration (EGP)
* Secure Checkout Payment Flow
* Server-verified payment status and webhook handling
* Integration with Order Checkout

---

## ⭐ Product Reviews

* Add Product Reviews
* Display Product Reviews
* Customer Ratings
* Review Management
* Product Rating Integration

---

## 👥 User Management

* Display Users
* Edit Users
* ASP.NET Core Identity Integration

---

## 🔐 Role Management

* Create Roles
* Display Roles
* Assign Roles to Users
* Edit User Roles

---

## 🔑 Authentication & Authorization

* ASP.NET Core Identity
* Login
* Register
* Forgot Password
* OTP Verification
* Password Reset
* Role-Based Authorization

---

## 📧 Email Integration

* Order Confirmation Emails
* Email Templates
* Order Notification Workflow

---

## ⚡ Performance

* Server-Side Pagination
* Specification Pattern
* Dynamic Searching
* Dynamic Sorting
* Memory Cache
* Optimized Database Queries using Skip/Take

---

## 🏗 Architecture

* N-Tier Architecture
* Repository Pattern
* Unit Of Work
* Specification Pattern
* Service Layer
* DTO Pattern
* AutoMapper
* Dependency Injection

---

## 💾 Database

* SQL Server
* Entity Framework Core
* Code First
* EF Core Migrations
* SQL Server Database Backup Included

---

# 🛠 Technologies

* ASP.NET Core MVC (.NET 10)
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* AutoMapper
* Bootstrap 5
* AdminLTE
* LINQ
* Memory Cache
* Session
* Stripe
* Paymob
* Repository Pattern
* Unit Of Work
* Specification Pattern

---

# 📂 Project Structure

```text
MyShop
├── Database
│   └── MyShop.bak
├── Images                 # README screenshots
├── MyShop.PL              # ASP.NET Core MVC app, controllers, views, and web assets
├── MyShop.BLL             # Application services, DTOs, and business workflows
├── MyShop.DAL             # EF Core entities, persistence, repositories, and specifications
├── tests                  # Unit and HTTP integration tests
└── MyShop..sln
```

The solution separates the web interface, application workflows, and database access into three projects. The web project calls business services; those services use the data-access abstractions rather than putting database rules in Razor views.

---
# 🚀 Getting Started

## Prerequisites

* .NET 10 SDK
* SQL Server / SQL Server Express
* Visual Studio 2022

---

## Clone Repository

```bash
git clone https://github.com/AbdelrahmanYehiaGharib23/MyShop.git

cd MyShop
```

---

## Restore NuGet Packages

```bash
dotnet restore
```

---

## Configure Database

### Option 1 — Recommended

Restore the SQL Server backup located in:

```text
Database/MyShop.bak
```

After restoring the database, update the connection string inside:

```text
MyShop.PL/appsettings.json
```

---

### Option 2

Create the database using Entity Framework Core Migrations:

```bash
dotnet ef database update
```

---

## Configure Stripe

Add your Stripe configuration to:

```text
MyShop.PL/appsettings.json
```

Example:

```json
{
  "Stripe": {
    "PublishableKey": "your_publishable_key",
    "SecretKey": "your_secret_key"
  }
}
```

> ⚠️ Do not commit your real Stripe Secret Key to GitHub. Use **User Secrets** or environment variables for production environments.

## Configure Paymob

Set the Paymob values through **User Secrets** for local development or environment variables on the hosting provider. Do not commit live keys or HMAC secrets.

Required settings:

```text
Paymob:SecretKey
Paymob:PublicKey
Paymob:IntegrationId
Paymob:HmacSecret
Paymob:CallbackBaseUrl
```

`Paymob:CallbackBaseUrl` must be a public HTTPS base URL so Paymob can reach the payment notification endpoint. Payment is confirmed from the verified webhook; the browser redirect alone does not mark an order as paid.

---

## Run the Project

```bash
dotnet run
```

or simply run the solution using **Visual Studio**.

---

## Run Tests

Run the unit and HTTP integration test suites from the repository root:

```bash
dotnet test MyShop..sln
```

To generate a Cobertura coverage report:

```bash
dotnet test MyShop..sln --coverlet --coverlet-output-format cobertura
```

See [`tests/README.md`](tests/README.md) for the test layout and contribution guidance. GitHub Actions runs the same test suite on pushes and pull requests.

---

# 📌 Current Progress

* ✅ Product CRUD
* ✅ Category CRUD
* ✅ User Management
* ✅ Role Management
* ✅ Authentication
* ✅ Authorization
* ✅ Shopping Cart
* ✅ Customer Favorites
* ✅ Product Search
* ✅ Product Sorting
* ✅ Server-Side Pagination
* ✅ Category Memory Cache
* ✅ Product Image Upload
* ✅ ASP.NET Core Identity
* ✅ Orders
* ✅ Checkout
* ✅ Stripe Payment Integration
* ✅ Paymob Payment Integration
* ✅ Customer order and delivery progress
* ✅ Order Confirmation Email
* ✅ Product Reviews & Ratings
* ✅ SQL Server Database Backup

---

# 📸 Screenshots

The screenshots below show the current customer storefront and admin workflows. They are linked to the image files in the [`Images`](Images) folder.

## 🛍 Customer Store

The storefront supports both light and dark themes and is designed for desktop and mobile. Customers can browse and search the catalog, then continue from a product into cart, checkout, and order tracking.

| Home — Light | Home — Dark |
| --- | --- |
| <img src="Images/CustomerHomeLight.png" alt="Customer storefront in light mode" width="460"> | <img src="Images/CustomerHomeDark.png" alt="Customer storefront in dark mode" width="460"> |

| Home — Dark variant | Product search |
| --- | --- |
| <img src="Images/CustomerHomeDark2.png" alt="Customer storefront dark mode variant" width="460"> | <img src="Images/SearchProduct.png" alt="Customer product search" width="460"> |

Search helps customers narrow the catalog before opening an item. The cart keeps the selected products for the current session, and checkout refreshes their prices before creating an order.

| Shopping cart | Checkout |
| --- | --- |
| <img src="Images/ShoppingCart.png" alt="Customer shopping cart" width="460"> | <img src="Images/Checkout.png" alt="Customer checkout and payment selection" width="460"> |

Choose a payment provider at checkout: Stripe processes card payment in USD, while Paymob processes card payment in EGP. The application waits for a verified server callback before treating the order as paid.

| Pay securely with Stripe | Pay securely with Paymob |
| --- | --- |
| <img src="Images/Pay%20securely%20with%20Stripe.png" alt="Stripe payment experience" width="460"> | <img src="Images/Pay%20securely%20with%20Paymob.png" alt="Paymob payment experience" width="460"> |

After payment, customers can see the order status and delivery journey from their account. Delivery details appear when store staff enter them in the admin order workflow.

| Payment confirmed | My orders |
| --- | --- |
| <img src="Images/Payment%20confirmed.png" alt="Payment confirmation page" width="460"> | <img src="Images/My%20Orders.png" alt="Customer order history and delivery progress" width="460"> |

| Delivery information | Product reviews |
| --- | --- |
| <img src="Images/Delivery%20Information.png" alt="Customer delivery information" width="460"> | <img src="Images/Product%20Reviews.png" alt="Product reviews and ratings" width="460"> |

| Login | Register |
| --- | --- |
| <img src="Images/Login.png" alt="Customer login page" width="460"> | <img src="Images/Register.png" alt="Customer registration page" width="460"> |

| Arabic translation |
| --- |
| <img src="Images/Translate%20into%20Arabic.png" alt="Storefront translated into Arabic" width="460"> |

## 🛠 Admin Dashboard

Authorized staff use the dashboard to maintain the catalog and categories, manage users and roles, and process orders. Paid orders move through fulfillment in sequence; courier and tracking information is shared with the customer after dispatch.

| Admin home — Dark | Admin home — Light |
| --- | --- |
| <img src="Images/AdminHomeDark.png" alt="Admin dashboard in dark mode" width="460"> | <img src="Images/AdminHomeLight.png" alt="Admin dashboard in light mode" width="460"> |

| Add a product | Edit products |
| --- | --- |
| <img src="Images/Adding%20a%20product%20from%20the%20Admin%20panel.png" alt="Adding a product from the admin panel" width="460"> | <img src="Images/Edit%20Products.png" alt="Admin product editing page" width="460"> |

| Categories | Users |
| --- | --- |
| <img src="Images/Display%20categories.png" alt="Admin category management" width="460"> | <img src="Images/Display%20Users.png" alt="Admin user management" width="460"> |

| Create a role | Manage roles |
| --- | --- |
| <img src="Images/Create%20Role.png" alt="Creating an admin role" width="460"> | <img src="Images/Manage%20Role.png" alt="Managing user roles" width="460"> |

| Order management | Fulfillment and delivery |
| --- | --- |
| <img src="Images/Order%20Management.png" alt="Admin order management" width="460"> | <img src="Images/Fulfillment%20and%20delivery.png" alt="Admin fulfillment and delivery workflow" width="460"> |

## Storefront release setup

* Checkout now refreshes cart prices from the product records and asks the customer to review the total when prices change. It snapshots current product data into the order and uses a unique checkout request key to prevent duplicate orders. Apply the `AddCheckoutRequestId` Entity Framework migration before deploying this version.
* Payment status pages read the order state from the server. Paymob’s browser return is informational; the order becomes paid only after the HMAC-validated webhook confirms the amount, currency, and integration. Configure `Paymob:SecretKey`, `Paymob:PublicKey`, `Paymob:IntegrationId`, `Paymob:HmacSecret`, and `Paymob:CallbackBaseUrl` with the provider’s live values and a public HTTPS URL.
* A declined Stripe payment attempt leaves the order pending so the customer can retry. Paymob returns to a status page that refreshes the server state and offers a way back to payment while the order is pending.
* Product records do not currently contain stock quantity, reservation, or shipping rules. The storefront does not claim a stock level or add an unconfigured shipping charge. Add the store’s actual inventory and delivery policies before showing availability or final delivery charges.
* Store events are pushed to `window.dataLayer` using standard event names (`view_item_list`, `view_item`, `search`, `add_to_cart`, `begin_checkout`, `select_payment_method`, and `purchase`). To connect Google Tag Manager, set `Analytics:GoogleTagManagerId` to the store’s `GTM-...` container ID. No external analytics script loads while that setting is empty; choose and implement the required consent behavior before enabling the container.
* The public storefront includes offline feedback, focus-visible states, reduced-motion support, image dimensions, and mobile layouts. Verify the deployed checkout with the live payment accounts and test the storefront on current iOS Safari and Android Chrome before launch.

# 📖 Future Improvements

* Dashboard Analytics
* Inventory management and stock reservation
* Coupons & Discounts
* REST API
* JWT Authentication
* Docker Support

---

# ⚠️ Notes

* Built using **ASP.NET Core MVC (.NET 10)**.
* Follows **N-Tier Architecture** with clear separation of concerns.
* Uses **Repository Pattern**, **Unit Of Work**, and **Specification Pattern**.
* Implements **Server-Side Pagination**, **Searching**, and **Sorting**.
* Uses **Memory Cache** to improve category retrieval performance.
* Shopping Cart is implemented using **ASP.NET Core Session**.
* Authentication and Authorization are powered by **ASP.NET Core Identity**.
* Orders and Checkout are integrated into the customer purchasing workflow.
* Payments are supported through **Stripe** and **Paymob**.
* Customers can view order fulfillment progress and delivery details entered by the store.
* Product Reviews and Ratings are supported for products.
* Order confirmation emails are sent through the email integration.
* A SQL Server backup (`Database/MyShop.bak`) is included for quick project setup.

---

# 👨‍💻 Author

**Abdelrahman Yehia Gharib Emam**

Junior Backend .NET Developer

* GitHub: https://github.com/AbdelrahmanYehiaGharib23
* LinkedIn: https://www.linkedin.com/in/abdelrahman-yehia-gharib-emam-ba88092a9
