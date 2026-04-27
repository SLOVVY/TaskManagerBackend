TaskManager API

Бэкенд-сервис для управления задачами с комментариями и аналитикой. 
Проект построен на ASP.NET Core 9.0 с использованием Entity Framework Core и PostgreSQL.

Функциональность:
- CRUD операции с задачами
- Комментарии к задачам
- Аналитика по задачам:
- - Количество задач по статусам
- - Просроченные задачи
- - Среднее время выполнения
- - Топ исполнителей по просрочкам
- Фильтрация по статусу и исполнителю
- Docker контейнеризация

Технологии:
- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core 9.0
- PostgreSQL 15
- Docker & Docker Compose
- Swagger/OpenAPI

Быстрый старт:

Требования:
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) (для локальной разработки)
- [Git](https://git-scm.com/)

Запуск с Docker (рекомендуется):

1. Клонируйте репозиторий
bash:
git clone https://github.com/SLOWY/TaskManagerBackend.git
cd TaskManagerBackend

2. Запустите приложение
bash:
docker-compose up -d --build

3. Откройте Swagger UI
URL:
http://localhost:5000/swagger
