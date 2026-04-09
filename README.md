Here’s a simple, clean README you can use for now:

# 🌍 Blocked Countries API

A simple **.NET Core Web API** that manages blocked countries and validates IP addresses using a third-party geolocation service.

---

## 🚀 Features

- Block / unblock countries
- Get all blocked countries (with pagination & search)
- Lookup country information by IP address
- Check if the current user IP is blocked
- Log blocked access attempts
- Temporarily block countries (with automatic expiration)

---

## 🛠️ Tech Stack

- .NET 8 Web API
- In-memory storage (ConcurrentDictionary / Lists)
- HttpClient for external API calls
- Swagger for API documentation

---

## 🔑 Setup

1. Clone the repository

```bash
git clone <your-repo-url>
cd <project-folder>
```

2. Configure API settings (optional)

```json
// appsettings.json
"GeoApi": {
  "BaseUrl": "https://ipapi.co/",
  "ApiKey": ""
}
```

> Note: API key is optional for ipapi.co (limited usage without it)

3. Run the project

```bash
dotnet run
```

4. Open Swagger

`https://localhost:{port}/swagger`

---

## 📌 Main Endpoints

| Method | Endpoint                             | Description                        |
|-------|--------------------------------------|------------------------------------|
| POST  | /api/countries/block                 | Block a country                    |
| DELETE| /api/countries/block/{code}          | Unblock a country                  |
| GET   | /api/countries/blocked               | Get blocked countries              |
| GET   | /api/ip/lookup                       | Get country by IP                  |
| GET   | /api/ip/check-block                  | Check if current IP is blocked     |
| GET   | /api/logs/blocked-attempts           | Get logs                           |
| POST  | /api/countries/temporal-block        | Temporarily block a country        |

---

## 🧠 Notes

- Data is stored in-memory (no database)
- Designed for demonstration and assignment purposes
- Thread-safe collections are used

---

## 📄 License

This project is for assessment purposes.
