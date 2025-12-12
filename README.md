# FlowerLab 🍽️☁️

Повнофункціональна веб‑платформа для онлайн‑замовлення квітів, керування каталогом, оформлення замовлень, відгуків та адміністрування.

---

## ⭐ Огляд MVP
FlowerLab — це сучасна система онлайн‑замовлення квітів з мікросервісною архітектурою.

### 👤 Клієнтські можливості
- Реєстрація та авторизація
- Перегляд каталогу товарів
- Замовлення букетів онлайн
- Оплата через **LiqPay** 💳
- Історія замовлень
- Перегляд та додавання відгуків

### 🛠️ Адмін‑можливості
- Повне керування товарами (CRUD)
- Керування статусами замовлень
- Керування користувачами
- Оновлення складу через події RabbitMQ

### ⚙️ Технічні можливості
- Подієва архітектура (RabbitMQ)
- PostgreSQL (усі сервіси, крім ReviewService: MongoDB)
- Cloudinary для збереження зображень
- LiqPay для платежів
- React фронтенд

---

## 📂 Налаштування оточення & Setup (Local)

### Requirements
- Node.js 18+
- npm
- .NET 8 SDK
- PostgreSQL
- MongoDB (for ReviewService)
- RabbitMQ

---

## 📂 Environment Setup
Environment files **must be created manually** (no docker-compose).

Below are **example values**, not real credentials.

---

## 📁 `.env` for `backend/CatalogService/CatalogService.API/`
```
JWT_SECRET=ExampleJwtSecretKey123
JWT_ISSUER=Example.UsersService
JWT_AUDIENCE=Example.WebAPI
JWT_ACCESS_TOKEN_EXPIRATION_MINUTES=15
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7

CLOUDINARY_CLOUDNAME=example_cloud
CLOUDINARY_API_KEY=123456789012345
CLOUDINARY_API_SECRET=example_cloudinary_secret
```

---

## 📁 `.env` for `backend/OrderService/OrderService.API/`
```
JWT_SECRET=ExampleJwtSecretKey123
JWT_ISSUER=Example.UsersService
JWT_AUDIENCE=Example.WebAPI
JWT_ACCESS_TOKEN_EXPIRATION_MINUTES=15
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7

CLOUDINARY_CLOUDNAME=example_cloud
CLOUDINARY_API_KEY=123456789012345
CLOUDINARY_API_SECRET=example_cloudinary_secret

LIQPAY_PUBLIC_KEY=example_public_key
LIQPAY_PRIVATE_KEY=example_private_key
LIQPAY_SERVER_URL=https://example.ngrok-free.dev/api/orders/liqpay-callback
```

---

## 📁 `.env` for `backend/ReviewService/ReviewService.API/`
```
JWT_SECRET=ExampleJwtSecretKey123
JWT_ISSUER=Example.UsersService
JWT_AUDIENCE=Example.WebAPI
JWT_ACCESS_TOKEN_EXPIRATION_MINUTES=15
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7
```

---

## 📁 `.env` for `backend/UsersService/UsersService.API/`
```
JWT_SECRET=ExampleJwtSecretKey123
JWT_ISSUER=Example.UsersService
JWT_AUDIENCE=Example.WebAPI
JWT_ACCESS_TOKEN_EXPIRATION_MINUTES=15
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7

CLOUDINARY_CLOUDNAME=example_cloud
CLOUDINARY_API_KEY=123456789012345
CLOUDINARY_API_SECRET=example_cloudinary_secret
```

---

## 🚀 Backend Run
Inside each microservice:
```
dotnet restore
dotnet build
dotnet run
```

---

## 🖥️ Frontend Run
Frontend is built with **React (JSX)**, **Axios**, **Tailwind**, without Vite.
```
cd frontend
npm install
npm start
```
Access at: http://localhost:3000

---

## 📡 Architecture Overview
### Active Microservices
- **UsersService** — identity, roles, credentials (Identity + role seeding)
- **CatalogService** — menu & product management
- **OrderService** — order processing + LiqPay integration
- **ReviewService** — product & order reviews (MongoDB)
- **AggregatorService** — combines data from all microservices
- **Gateway** — entry point for all client requests

### Event-Driven Messaging (RabbitMQ)
Used for:
- Updating product stock
- Syncing user updates
- Broadcasting order status changes

---

## 🧪 Test Credentials
(Example only — not real)
- **User:** client@flowerlab.com / ClientPass123!
- **Admin:** admin@flowerlab.com / SecureAdminPass123!

---

## 🧑‍💻 Technology Stack
### Backend
- ASP.NET Core 8
- PostgreSQL
- MongoDB (ReviewService)
- RabbitMQ
- Cloudinary
- LiqPay

### Frontend
- React (JSX)
- Tailwind CSS
- Axios
- Zustand
- react-i18next

---

## 👥 Team
| Name | Role |
|------|------|
| Khrystafullova Oleksandra | Frontend |
| Melenko Denys | Frontend |
| Kaniovskyi Oleksandr | Backend + Tech Lead |
| Kyrpushko Yana | Designer + Business Analyst |
| Kozhokar Dmytro | Backend |
| Patsarniuk Khrystyna | QA / Tester |
| Pavliuk Yurii | Backend |
| Kate Chroney | PM |

---

## 📄 License
This project is developed as part of university coursework.

