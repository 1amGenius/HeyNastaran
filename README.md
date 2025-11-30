# 🤖 HeyNastaran! Telegram Bot

A personal Telegram bot for **Nastaran**, my best friend in the world <3. Providing utility features like weather updates, quotes, motivational messages, music suggestions, and idea/inspiration management. Built with **.NET 8** and **MongoDB**.

---

## 📋 Table of Contents

1. [Project Overview](#-project-overview)  
2. [Features](#-features)  
3. [Tech Stack](#-tech-stack)  
4. [Architecture](#-architecture)  
5. [Setup & Installation](#%EF%B8%8F-setup--installation)  
6. [Configuration](#-configuration)  
7. [Folder Structure](#-folder-structure)  
8. [Future Roadmap](#-future-roadmap)  
9. [Contributing](#-contributing--i-dont-know-why-but-thanks-anyways-)  
10. [License](#-license)  

---

## 🌟 Project Overview

This bot is designed for one user (Nastaran) and provides utility functionality such as:  

- 🌤 Weather forecasts for the next 7 days, including sunrise and sunset times.  
- ✨ Daily motivational or upbeat quotes.  
- 🎵 Daily music recommendations.  
- 📢 Notifications when favorite artists release new tracks (hope I can implement it for free).  
- 📝 Storage and retrieval of ideas (text) and inspirations (images).  

The backend is designed with **clean architecture principles**, **dependency injection**, and **MongoDB** for data persistence.

---

## ✅ Features

### Implemented
- [x] Telegram webhook handling via `TelegramController`.  
- [x] Orchestrator service `TelegramBotService` calling domain services.  
- [x] User management (`UserService` + `UserRepository`).  
- [x] Quotes (`QuoteService` + `QuoteRepository`).  
- [x] Ideas (`IdeaService` + `IdeaRepository`).  
- [x] Inspirations (`InspirationService` + `InspirationRepository`).  
- [x] MongoDB integration with `IMongoClient` singleton.  
- [x] Swagger/OpenAPI documentation in development.  

### Planned / Future
- [x] 🌤 Fetch weather data via a public API.  
- [ ] ✨ Send daily motivational notes.  
- [ ] 🎵 Send daily music (Spotify or soundcload integration or public Telegram channels).  
- [ ] 📢 Track favorite artists’ releases.  
- [x] ⚡ Inline commands for Telegram (e.g., `/note`, `/idea`).  
- [ ] ⏰ Scheduled tasks (daily notes, music, notifications).  

---

## 🛠 Tech Stack

**Backend**:  
- [.NET 8](https://dotnet.microsoft.com/)  
- [ASP.NET Core Web API](https://docs.microsoft.com/aspnet/core/web-api/)  

**Database**:  
- [MongoDB Atlas (cloud)](https://www.mongodb.com/cloud/atlas)  
- [MongoDB.Driver](https://www.nuget.org/packages/MongoDB.Driver/)  

**Telegram Integration**:  
- [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)  

**Dev / Deployment Tools**:  
- [Docker](https://docker.com/)

**Other Libraries / Planned**:  
- `System.Text.Json` for JSON serialization (used in responses and webhook parsing).  
- `Hangfire` or similar (planned) for background jobs/scheduled tasks.  
- `SpotifyAPI-NET` or similar (planned) for fetching music releases.  

---

## 🏗 Architecture

The architecture follows **clean, layered design**:

```
Controller (TelegramController)
		|
		▼
TelegramBotService (orchestrator)
		|
		▼
Domain Services
(UserService, QuoteService, IdeaService, InspirationService)
		|
		▼
Repositories
(UserRepository, QuoteRepository, IdeaRepository, InspirationRepository)
		|
		▼
MongoDB (collections)
(Users, Quotes, Ideas, Inspirations)
```

**Dependency Injection** is used throughout:

- Controllers → TelegramBotService → Services → Repositories → MongoDB  
- Services and repositories are scoped; MongoClient is a singleton.  

---

## ⚙️ Setup & Installation

### Standard .NET Setup

1. Clone the repository:
	```bash
	git clone https://github.com/yourusername/nastaran-telegram-bot.git
	cd nastaran-telegram-bot
	```
2. Restore NuGet packages:
	```bash
	dotnet restore
	```
3. Add your MongoDB connection string (see [Configuration](#configuration))
4. Build and run the project:
	```bash
	dotnet run
	```
5. Expose your API for Telegram webhook testing (optional, e.g., via ngrok):
	```bash
	ngrok http 5000
	```

### Using Docker (optional)

1. Build the Docker image:
  ```bash
  docker build -t nastaran-telegram-bot .
  ```
2. Run the container:
  ```bash
  docker run -d -p 8080:8080 -p 8081:8081 --name nastaran-bot \
  -e ConnectionStrings__MongoDb="mongodb+srv://<username>:<password>@cluster0.odc1y5f.mongodb.net/nastaranBotDb" \
  -e TelegramBotToken="<your_token_here>" \
  nastaran-telegram-bot
  ```

- The bot will run inside the container and be accessible at http://localhost:8080 (or http://localhost:8081).
- Environment variables are used for secrets like MongoDB connection string and Telegram bot token.
- Adjust port mapping (-p) if you want to use different host ports.
  
---

## 🔐 Configuration

- appsettings.json (committed) → default/dummy values.
- appsettings.Development.json (ignored via .gitignore) → real MongoDB connection string:
	```json
	{
	  "ConnectionStrings": {
		"MongoDb": "mongodb+srv://<username>:<password>@cluster0.odc1y5f.mongodb.net/nastaranBotDb?retryWrites=true&w=majority"
	  }
	}
	```
- You can also use environment variables for secrets:
	```
	ConnectionStrings__MongoDb="mongodb+srv://<username>:<password>@cluster0.odc1y5f.mongodb.net/nastaranBotDb"
	```
- Telegram Bot token can also be stored as an environment variable.

  ---

## 📂 Folder Structure

```
/Controllers
|   TelegramController.cs

/Contracts
|	/User
|	|	LocationDto.cs
|	|	PrefrencesDto.cs
|	|	UserCreateDto.cs
|	|	UserUpdateDto.cs
|
|	/Weather
|	|	ReverseGeocodingResult.cs
|	|	WeatherData.cs

/Models
|   User.cs
|   Quote.cs
|   Idea.cs
|   Inspiration.cs
|	Weather.cs

/Repositories
|   /User
|   |    IUserRepository.cs
|   |    UserRepository.cs
|	
|   /Quote
|   |    IQuoteRepository.cs
|   |    QuoteRepository.cs
|
|   /Idea
|   |    IIdeaRepository.cs
|   |    IdeaRepository.cs
|
|   /Inspiration
|   |    IInspirationRepository.cs
|   |    InspirationRepository.cs

/Services
|   /User
|   |   IUserService.cs
|   |   UserService.cs
|
|   /Quote
|   |   IQuoteService.cs
|   |   QuoteService.cs
|
|   /Idea
|   |   IIdeaService.cs
|   |   IdeaService.cs
|
|   /Inspiration
|   |   IInspirationService.cs
|   |   InspirationService.cs
|
|   /TelegramBot
|	|	/Handlers
|	|	|	/Commands
|	|	|	|
|	|	|	|	/Idea
|	|	|	|	|	IdeaCommandHandler.cs
|	|	|	|
|	|	|	|	/Inspiration
|	|	|	|	|	InspirationCommandHandler.cs
|	|	|	|
|	|	|	|	/Note
|	|	|	|	|	NoteCommandHandler.cs
|	|	|	|
|	|	|	|	/Quote
|	|	|	|
|	|	|	|	/Song
|	|	|	|
|	|	|	|	/Weather
|	|	|	|	|	WeatherCommandHandler.cs
|	|	|	|
|	|	|	|	StartCommandHandler.cs
|	|	|
|	|	|	/Updates
|	|	|	|
|	|	|	|	/Idea
|	|	|	|
|	|	|	|	/Inspiration
|	|	|	|
|	|	|	|	/Note
|	|	|	|
|	|	|	|	/Quote
|	|	|	|
|	|	|	|	/Song
|	|	|	|
|	|	|	|	/Weather
|	|	|	|	|	WeatherCallbackHandler.cs
|	|	|	|	|	WeatherCityHandler.cs
|	|	|	|	|	WeatherLocationHandler.cs
|	|	|	|	|	WeatherSearchCityHandler.cs
|	|	|	|	|	WeatherSearchStartHandler.cs
|	|	|	|
|	|	|
|	|	|	/Interfaces
|	|	|	|	
|	|	|	|	ICommandHandler.cs
|	|	|	|	IUpdateHandler.cs
|	|	|
|	|	|	/Routing
|	|	|	|
|	|	|	|	CommandRouter.cs
|	|	|	|	UpdateRouter.cs
|	|	
|   |   TelegramBotService.cs

/Utils
|	/Helpers
|	|	/Music
|	|	|	/Clients
|	|	|	|	MusicApiClient.cs
|	|	|	/Interfaces
|	|	|	|	IMusicApiClient.cs
|	|	
|	|	/Scheduler
|	|	|	/Clients
|	|	|	|	Scheduler.cs
|	|	|	/Interfaces
|	|	|	|	IScheduler.cs
|	|
|	|	/Weather
|	|	|	/Clients
|	|	|	|	WeatherApiClient.cs
|	|	|	|	WeatherHttpClient.cs
|	|	|	/Interfaces
|	|	|	|	IWeatherApiClient.cs
|	|	|	|	IWeatherHttpClient.cs
|
|	/Formaters
|	|	FormatWeather.cs
|
|	/Mappers
|	|	PrefrencesMapper.cs
|	|	UserMapper.cs
|	|	WeatherMapper.cs
|
|	/Urls
|	|	WeatherApiUrls.cs
|	
|	BotButtons.cs
|	CodeUtility.cs

Program.cs
appsettings.json
appsettings.Development.json (ignored)
Dockerfile
LICENSE.txt
```

---

## 🚀 Future Roadmap

1. Integrate Weather API (daily + 7-day forecast).
2. Daily motivational messages via Telegram.
3. Music recommendations using Spotify, soundcload or Telegram channels.
4. Scheduled tasks (daily updates).
5. Extend TelegramBotService with inline commands.
6. Logging and monitoring enhancements.
7. Unit tests for services using mocked repositories.

---

## 🤝 Contributing ( I don't know why... but thanks anyways )

1. Fork the repository
2. Create a branch for your feature (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -am 'Add feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the [MIT License](LICENSE.txt) — see the [LICENSE](LICENSE.txt) file for details.

The MIT License is a permissive open-source license that allows you to:

- Use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the project.
- Include this project in your own projects, even commercially.

⚠️ **Disclaimer:** The software is provided "as-is", without warranty of any kind. Use it at your own risk.

