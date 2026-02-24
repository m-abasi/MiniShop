# MiniShop — فروشگاه آنلاین

پروژه فروشگاه آنلاین با معماری Clean Architecture روی ASP.NET Core 10.

---

## اطلاعات ورود
| نقش   | ایمیل             | رمز عبور  |
|-------|-------------------|-----------|
| ادمین | admin@example.com | Admin123! |
| کاربر | ثبت‌نام از `/Account/Register` | دلخواه |

**قوانین رمز عبور:** حداقل ۶ کاراکتر، شامل یک حرف بزرگ، یک عدد، یک کاراکتر خاص  
مثال: `MyPass@1`، `Test123!`

---

## پیش‌نیازها

- .NET 10 SDK
- SQL Server (هر نسخه‌ای)
- Redis (نسخه ۶+)
- Visual Studio 2022 یا VS Code

---

## راه‌اندازی

### ۱. Connection Strings

فایل `MiniShop/appsettings.json` را باز کنید:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=.;Initial Catalog=MiniShop;Integrated Security=True;Trust Server Certificate=True;",
  "Redis": "localhost:6379"
}
```

### ۲. اجرای Redis

```bash
# با Docker:
docker run -d -p 6379:6379 redis

# بررسی اتصال:
redis-cli ping   # باید PONG برگردد
```

### ۳. Migration دیتابیس

**⚠ این مرحله الزامی است.**

```powershell
# در Package Manager Console (Visual Studio):
Update-Database -Project Infrastructure -StartupProject MiniShop
```

```bash
# یا در Terminal:
dotnet ef database update --project Infrastructure --startup-project MiniShop
```

### ۴. اجرای پروژه

```bash
dotnet run --project MiniShop
```

یا F5 در Visual Studio.

---

## ساختار پروژه

```
MiniShop/
├── Domain/           — Entityها (Product, Order, OrderItem, Category, ApplicationUser)
├── Application/      — CQRS Commands/Queries, Interfaces, Mapping Profiles, ViewModels
├── Infrastructure/   — EF Core DbContext, Migrations, Services (Redis, Cart)
└── MiniShop/         — ASP.NET Core MVC (Controllers, Views, Program.cs)
```

---

## امکانات

### کاربر عادی
- مرور و جستجوی محصولات
- فیلتر بر اساس دسته‌بندی
- افزودن به سبد خرید
- نهایی کردن خرید با فرم آدرس ارسال
- مشاهده سابقه سفارش‌ها

### ادمین (علاوه بر امکانات کاربر)
- داشبورد با آمار کلی (محصولات، سفارش‌ها، درآمد، موجودی کم)
- افزودن / ویرایش / حذف محصول
- مدیریت دسته‌بندی‌ها
- مشاهده و تغییر وضعیت سفارش‌ها

---

## تکنولوژی‌ها

| لایه | ابزار |
|------|-------|
| Web | ASP.NET Core 10 MVC |
| ORM | Entity Framework Core 10 + SQL Server |
| Auth | ASP.NET Core Identity (Cookie-based) |
| Messaging | MediatR (CQRS) |
| Mapping | AutoMapper + ProjectTo |
| Validation | FluentValidation |
| Cache | Redis (Cache-Aside Pattern) |
| Rate Limit | Redis Counter روی Login |
| Logging | Serilog (Console + File) |
| Cart | Session-based (SOLID refactored) |
| معماری | Clean Architecture |

---

## نکات مهم

- **موجودی:** هنگام ثبت سفارش، موجودی محصولات به‌صورت خودکار کسر و Cache بلافاصله Invalidate می‌شود
- **Cache:** لیست محصولات با TTL 5 دقیقه در Redis کش می‌شود؛ هر تغییر در محصول یا ثبت سفارش آن را باطل می‌کند
- **Rate Limit:** حداکثر ۵ تلاش ناموفق ورود در ۱۵ دقیقه به ازای هر IP
- **Logging:** لاگ‌ها در پوشه `Logs/` به صورت روزانه ذخیره می‌شوند
- **Session:** سبد خرید در Session ذخیره می‌شود (In-Memory)
