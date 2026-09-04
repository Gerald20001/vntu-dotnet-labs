# Практичні роботи з дисципліни «Розробка проєктів засобами платформи .NET»

**Вінницький національний технічний університет (ВНТУ)**  
**Факультет інформаційних технологій та комп'ютерної інженерії (ФІТКІ)**  
**Кафедра програмного забезпечення (ПЗ)**  
**Студент:** №17 за журналом групи  
**Розрахунковий варіант:** **Варіант №1**  
*(відповідно до регламенту курсу: для 4 варіантів у роботі, №17 $\rightarrow$ Варіант 1)*

---

## 📁 Структура рішень та репозиторію

Репозиторій організовано відповідно до вимог гайду [00-setup](https://mypo-vntu.github.io/vntu-dotnet-pract/00-setup/) та містить усі практичні роботи курсу:

```text
dotnet-labs/
├── DotNetLabs.sln                           # Головне рішення (Solution) з усіма проєктами
├── .gitignore                               # Ігнорування бінарних та тимчасових файлів .NET
├── README.md                                # Повний опис робіт та інструкції із запуску
│
├── 01-csharp-basics/                        # Практична робота №1 (Консольний застосунок)
│   ├── 01-csharp-basics.csproj
│   ├── Program.cs
│   └── REPORT_LAB01.md                      # Звіт ПР №1 + 12 контрольних питань
│
├── 02-oop-exceptions/                       # Практична робота №2 (ООП та винятки)
│   ├── 02-oop-exceptions.csproj
│   ├── Models/ (Shape, Circle, Rectangle)
│   ├── Exceptions/ (InvalidShapeDimensionException)
│   ├── Program.cs
│   └── REPORT_LAB02.md                      # Звіт ПР №2 + UML-діаграма + 12 контрольних питань
│
├── 03-generics-delegates-events/             # Практична робота №3 (Generics, Делегати, Події)
│   ├── 03-generics-delegates-events.csproj
│   ├── Collections/ (MyStack<T>)
│   ├── Events/ (OverflowEventArgs)
│   ├── Exceptions/ (EmptyStackException)
│   ├── Program.cs
│   └── REPORT_LAB03.md                      # Звіт ПР №3 + 5 контрольних питань
│
├── 04-linq-to-objects/                      # Практична робота №4 (LINQ to Objects)
│   ├── 04-linq-to-objects.csproj
│   ├── Program.cs                           # 8+ LINQ-запитів, Join, GroupBy, експорт CSV
│   └── REPORT_LAB04.md                      # Звіт ПР №4 + 5 контрольних питань
│
├── 05-aspnetcore-efcore-sqlite-crud/        # Практична робота №5 (ASP.NET Core Minimal API / CRUD)
│   ├── 05-aspnetcore-efcore-sqlite-crud.csproj
│   ├── Models/ (Author, Book)
│   ├── Data/ (LibraryDbContext, SQLite)
│   ├── Controllers/ (BooksController)
│   └── REPORT_LAB05.md                      # Звіт ПР №5 + 5 контрольних питань
│
├── 06-rest-api/                             # Практична робота №6 (RESTful API, DTO, RFC 7807)
│   ├── 06-rest-api.csproj
│   ├── DTOs/ (BookDtos, AuthorDtos, ReaderDtos, PagedResult)
│   ├── Middleware/ (ExceptionHandlingMiddleware - RFC 7807)
│   ├── Controllers/ (BooksController, AuthorsController, ReadersController)
│   └── REPORT_LAB06.md                      # Звіт ПР №6 + 5 контрольних питань
│
├── 07-auth-jwt/                             # Практична робота №7 (JWT автентифікація та RBAC)
│   ├── 07-auth-jwt.csproj
│   ├── Services/ (PasswordHasher PBKDF2, JwtTokenService)
│   ├── Controllers/ (AuthController, BooksController з ролями Admin/User)
│   └── REPORT_LAB07.md                      # Звіт ПР №7 + матриця ролей + 5 контрольних питань
│
└── 08-architecture-testing/                 # Практична робота №8 (Clean Architecture & xUnit/Moq)
    ├── 08-architecture-testing.csproj
    ├── Domain/ (Сутності, Інтерфейси репозиторіїв IUnitOfWork, Винятки)
    ├── Application/ (Сервіс BookService, DTOs)
    ├── Infrastructure/ (LibraryDbContext, Репозиторії, UnitOfWork)
    ├── Controllers/ (BooksController)
    ├── tests/LibraryApi.UnitTests/          # xUnit + Moq + WebApplicationFactory
    │   ├── LibraryApi.UnitTests.csproj
    │   ├── BookServiceTests.cs              # 8 модульних тестів (Unit Tests)
    │   └── BookApiIntegrationTests.cs       # 5 інтеграційних тестів (Integration Tests)
    └── REPORT_LAB08.md                      # Звіт ПР №8 + архітектурна схема + 6 контрольних питань
```

---

## 📌 Огляд реалізованих робіт

### 🟢 Модуль 1: Основи C# та ООП
- **ПР №1: «Базові конструкції C#. Консольний застосунок» (Варіант 1: Статистика результатів тестування)**
  - Масиви `int[] scores`, динамічна зміна через `Array.Resize`;
  - Середній бал, Min/Max з індексами, розподіл на категорії через `switch expression`;
  - Безпечне введення через `int.TryParse`, обробка граничних випадків (порожній масив).
- **ПР №2: «ООП та обробка виключень у консольному застосунку» (Варіант 1: Геометричні фігури)**
  - Абстрактний клас `Shape`, реалізація `IComparable<Shape>`;
  - Похідні класи `Circle`, `Rectangle` (та квадрат), перевизначення площі/периметра;
  - Поліморфна колекція `List<Shape>`, сортування, власний виняток `InvalidShapeDimensionException`, `try-catch-finally`.
- **ПР №3: «Узагальнення, делегати та події» (Варіант 1: Узагальнений стек `Stack<T>` з нуля)**
  - Власний узагальнений стек `MyStack<T>` з динамічним розширенням пам'яті;
  - Подія переповнення `Overflow` з користувацьким `OverflowEventArgs`;
  - Власні методи вищих порядків: `FindAll(Predicate<T>)`, `CountWhere(Func<T, bool>)`, `ForEach(Action<T>)`, `Select`.
- **ПР №4: «LINQ to Objects» (Варіант 1: Студенти та оцінки)**
  - Вибірка з 25 студентів 4 груп, пов'язаних з кураторами (`Join`);
  - 8 аналітичних запитів (фільтрація, пагінація `Skip/Take`, групування `GroupBy`, обчислення GPA);
  - Порівняння Query Syntax та Method Syntax, експорт зведеного звіту у CSV файл.

### 🔵 Модуль 2: Веб-розробка на ASP.NET Core (Мікропроєкт «Бібліотека», Варіант 1)
- **ПР №5: «Основи ASP.NET Core Web API та Entity Framework Core (SQLite CRUD)»**
  - Моделі `Book` та `Author` (зв'язок 1:N);
  - Налаштування `LibraryDbContext` з SQLite та початковими даними (Seed);
  - Асинхронні CRUD операції, OpenAPI та інтерактивна документація Scalar UI.
- **ПР №6: «Проєктування REST API: DTO, Валідація та Обробка помилок»**
  - Домен розширено: `Reader`, `BookLoan` (видача книг);
  - Розділення доменних моделей та DTO (`BookDto`, `CreateBookDto`), пагінація `PagedResult<T>`;
  - Фільтрація, пошук та сортування у запитах;
  - Централізований Middleware обробки помилок за стандартом **RFC 7807 ProblemDetails**.
- **ПР №7: «Автентифікація та авторизація через JWT токени»**
  - Безпечне хешування паролів сіллю через **PBKDF2 HMAC-SHA256**;
  - Генерація та верифікація токенів `JwtSecurityToken` з ролями (`Admin`, `User`);
  - Рольова безпека (RBAC): публічний доступ до каталогу, читання для зареєстрованих, модифікація виключно для адміністраторів (`[Authorize(Roles = "Admin")]`).
- **ПР №8: «Архітектурні патерни (Clean Architecture) та тестування (xUnit + Moq)»**
  - Розподіл на шари: **Domain**, **Application**, **Infrastructure**, **Presentation (API)**;
  - Патерни **Repository** (`IBookRepository`, `IAuthorRepository`) та **Unit of Work** (`IUnitOfWork`);
  - Сервісний шар `BookService` з валідацією бізнес-правил та тонкі REST-контролери;
  - **8 модульних тестів (Unit Tests)** на базі `xUnit` та `Moq`;
  - **5 інтеграційних тестів (Integration Tests)** з використанням `WebApplicationFactory`.

---

## 🚀 Як запускати проєкти

### Запуск через командний рядок (.NET CLI)

```bash
# Практичні роботи 1-4 (Консольні застосунки):
dotnet run --project 01-csharp-basics
dotnet run --project 02-oop-exceptions
dotnet run --project 03-generics-delegates-events
dotnet run --project 04-linq-to-objects

# Практичні роботи 5-8 (Веб-сервіси REST API):
dotnet run --project 05-aspnetcore-efcore-sqlite-crud
dotnet run --project 06-rest-api
dotnet run --project 07-auth-jwt
dotnet run --project 08-architecture-testing

# Запуск модульних та інтеграційних тестів (ПР №8):
dotnet test 08-architecture-testing/tests/LibraryApi.UnitTests
```

---

## 📄 Повні звіти до всіх робіт (Формати Word .docx та Markdown .md)

Усі звіти оформлені українською мовою з повними теоретичними обґрунтуваннями, архітектурними схемами, лістингами коду, таблицями та детальними відповідями на всі контрольні питання згідно з силабусом. Доступні як у форматі **Microsoft Word (.docx)** для здачі викладачу, так і у форматі **Markdown (.md)**:

1. **Практична робота №1:**
   - 📄 Word: [`01-csharp-basics/REPORT_LAB01.docx`](01-csharp-basics/REPORT_LAB01.docx)
   - 📝 Markdown: [`01-csharp-basics/REPORT_LAB01.md`](01-csharp-basics/REPORT_LAB01.md) (12 контрольних питань)
2. **Практична робота №2:**
   - 📄 Word: [`02-oop-exceptions/REPORT_LAB02.docx`](02-oop-exceptions/REPORT_LAB02.docx)
   - 📝 Markdown: [`02-oop-exceptions/REPORT_LAB02.md`](02-oop-exceptions/REPORT_LAB02.md) (12 контрольних питань + діаграма класів)
3. **Практична робота №3:**
   - 📄 Word: [`03-generics-delegates-events/REPORT_LAB03.docx`](03-generics-delegates-events/REPORT_LAB03.docx)
   - 📝 Markdown: [`03-generics-delegates-events/REPORT_LAB03.md`](03-generics-delegates-events/REPORT_LAB03.md) (5 контрольних питань)
4. **Практична робота №4:**
   - 📄 Word: [`04-linq-to-objects/REPORT_LAB04.docx`](04-linq-to-objects/REPORT_LAB04.docx)
   - 📝 Markdown: [`04-linq-to-objects/REPORT_LAB04.md`](04-linq-to-objects/REPORT_LAB04.md) (5 контрольних питань + CSV)
5. **Практична робота №5:**
   - 📄 Word: [`05-aspnetcore-efcore-sqlite-crud/REPORT_LAB05.docx`](05-aspnetcore-efcore-sqlite-crud/REPORT_LAB05.docx)
   - 📝 Markdown: [`05-aspnetcore-efcore-sqlite-crud/REPORT_LAB05.md`](05-aspnetcore-efcore-sqlite-crud/REPORT_LAB05.md) (5 контрольних питань)
6. **Практична робота №6:**
   - 📄 Word: [`06-rest-api/REPORT_LAB06.docx`](06-rest-api/REPORT_LAB06.docx)
   - 📝 Markdown: [`06-rest-api/REPORT_LAB06.md`](06-rest-api/REPORT_LAB06.md) (5 контрольних питань)
7. **Практична робота №7:**
   - 📄 Word: [`07-auth-jwt/REPORT_LAB07.docx`](07-auth-jwt/REPORT_LAB07.docx)
   - 📝 Markdown: [`07-auth-jwt/REPORT_LAB07.md`](07-auth-jwt/REPORT_LAB07.md) (5 контрольних питань)
8. **Практична робота №8:**
   - 📄 Word: [`08-architecture-testing/REPORT_LAB08.docx`](08-architecture-testing/REPORT_LAB08.docx)
   - 📝 Markdown: [`08-architecture-testing/REPORT_LAB08.md`](08-architecture-testing/REPORT_LAB08.md) (6 контрольних питань + Clean Architecture)
