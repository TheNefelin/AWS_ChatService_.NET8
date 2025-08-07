# AWS_ChatService_.NET8

Microservicio de chat construido en .NET 8 siguiendo principios de Clean Architecture y SOLID, preparado para despliegue en AWS Alchemy Lab.

### Correr API con Dokerfile
- Abrir CMD o PowerShell en la raiz de la solucion
- [App URL](http://localhost:5000/swagger/index.html)
```bash
docker build -t artema-chat-app .
docker run -d -p 5000:80 --name chat-app artema-chat-app
```

### Dependencias

| Proyecto                             | Dependencias                                                                                    |
| ------------------------------------ | ----------------------------------------------------------------------------------------------- |
| **AWS\_ChatService\_Infrastructure** | Dapper<br>Npgsql<br>Microsoft.Extensions.Configuration.Abstractions<br>AWS\_ChatService\_Domain |
| **AWS\_ChatService\_Application**    | Microsoft.Extensions.Logging.Abstractions<br>AWS\_ChatService\_Domain                           |
| **AWS\_ChatService\_API**            | AWS\_ChatService\_Infrastructure<br>AWS\_ChatService\_Application                               |
| **xUnitTest**                        | Moq<br>AWS\_ChatService\_Application                                                            |

---

## 🧱 Estructura del proyecto

```
AWS_ChatService_.NET8/
│
├── AWS_ChatService_API/                → Web API
│   ├── Controllers/
│   │   ├── ChatRoomsController.cs      → Controlador REST para salas de chat (aún no implementado)
│   │   ├── MessagesController.cs       → Controlador REST para mensajes
│   │   └── UsersController.cs          → Controlador REST para usuarios (aún no implementado)
│   ├── Hubs/
│   │   └── ChatHub.cs                  → SignalR Hub para chat en tiempo real
│   ├── appsettings.json                → Configuración de la aplicación (cadena de conexión, etc.)
│   └── Program.cs                      → Configuración de la API y servicios
│
├── AWS_ChatService_Application/        → Class Library
│   ├── Common/
|   │   └── ResponseApi
│   ├── DTOs/
│   │   ├── ChatRoomDto.cs              → DTO para salas de chat (aún no implementado)
│   │   ├── CreateChatRoomDto.cs        → DTO para crear salas de chat
│   │   ├── CreateUserDto.cs            → DTO para crear usuarios
│   │   ├── MessageDto.cs               → DTO para mensajes
│   │   └── UserDto.cs                  → DTO para usuarios (aún no implementado)
│   ├── Interfaces/
│   │   ├── IChatRoomService.cs         → Interfaz de servicio para salas de chat (aún no implementado)
│   │   ├── IMessageService.cs          → Interfaz de servicio para mensajes
│   │   └── IUserService.cs             → Interfaz de servicio para usuarios (aún no implementado)
|   ├── Mappers/
│   │   └── UserMapper.cs
│   └── Services/
│       ├── ChatRoomService.cs          → Lógica de negocio para sala de chat
│       ├── MessageService.cs           → Lógica de negocio para mensajes
│       └── UserService.cs              → Lógica de negocio para usuarios
│
├── AWS_ChatService_Domain/             → Class Library
│   ├── Entities/
│   │   ├── ChatRoom.cs                 → Entidad de sala de chat
│   │   ├── Message.cs                  → Entidad de mensaje
│   │   └── User.cs                     → Entidad de usuario
│   └── Interfaces/
│       ├── IChatRoomRepository.cs      → Repositorio de salas de chat
│       ├── IMessageRepository.cs       → Repositorio de mensajes
│       └── IUserRepository.cs          → Repositorio de usuarios
│
├── AWS_ChatService_Infrastructure/     → Class Library
│   ├── Configuration/
│   │   └── DapperConnectionFactory.cs  → Fábrica de conexiones Dapper
│   └── Repositories/
│       ├── ChatRoomRepository.cs       → Implementación de IChatRoomRepository
│       ├── MessageRepository.cs        → Implementación de IMessageRepository
│       └── UserRepository.cs           → Implementación de IUserRepository
│
├── xUnitTest/
│   └── SinTestAun/
│
├── Dockerfile
├── PostgreSQL.sql
└── README.md                           → Documentación técnica
```

---

## 📦 Modelo de dominio

### Entidades

- **User**: representa a un participante del chat.
- **ChatRoom**: representa una sala de conversación.
- **Message**: representa un mensaje enviado en una sala.

### Relaciones

- Un `User` puede enviar muchos `Messages`.
- Un `ChatRoom` contiene muchos `Messages`.

### Interfaces

- `IMessageRepository` define las operaciones que la aplicación requiere para gestionar mensajes, separando la lógica de dominio de la infraestructura.

> Aplicamos aquí los principios:
> - **Single Responsibility**: cada entidad y repositorio tiene una sola responsabilidad.
> - **Dependency Inversion**: la aplicación depende de interfaces, no implementaciones concretas.

---

## 🧠 Capa de Aplicación (`AWS_Application`)

### Objetivo
Define los casos de uso del sistema sin depender de tecnologías específicas (como bases de datos o HTTP). Solo conoce interfaces.

### Estructura

```
AWS_Application/
│
├── DTOs/
│   └── MessageDto.cs       → Datos de entrada/salida
│
├── Interfaces/
│   └── IMessageService.cs  → Contrato de servicios de mensajes
│
└── Services/
    └── MessageService.cs   → Implementación de la lógica de envío/lectura
```

### Principios aplicados
- **Single Responsibility**: Separación entre lógica de aplicación y lógica de dominio.
- **Dependency Inversion**: Usa `IMessageRepository` sin conocer su implementación.
- **Open/Closed**: Puedes extender servicios o agregar validaciones sin modificar la API o infraestructura.

--- 

## 🗃️ Capa de Infraestructura (`AWS_Infrastructure` con Dapper)

### Objetivo
Implementa acceso a datos con Dapper, desacoplado mediante interfaces del dominio.

### Estructura

 ```
AWS_Infrastructure/
│
├── Repositories/
│   └── MessageRepository.cs        → Implementación de IMessageRepository usando Dapper
│
└── Configuration/
    └── DapperConnectionFactory.cs  → Encapsula creación de conexiones a PostgreSQL
 ```
 
### Características

- Se utiliza Dapper por su rendimiento y simplicidad.
- Se mantiene el contrato con `IMessageRepository`.
- Se centraliza la creación de conexiones con `DapperConnectionFactory`.

### Principios aplicados

- **Dependency Inversion**: `Application` depende de interfaces, no de Dapper directamente.
- **Open/Closed**: Esta implementación puede ser reemplazada por una basada en EF Core (`AWS_Infrastructure_EF`) sin afectar al resto del sistema.

---

## 🌐 Capa API (`AWS_API`)

### Objetivo
Define los endpoints HTTP (y más adelante SignalR) para interactuar con el microservicio.

### Configuración en `Program.cs`

- Registra `DapperConnectionFactory` como singleton.
- Inyecta `IMessageRepository` y `IMessageService`.
- Usa `Swagger` para probar los endpoints.
- Lee la cadena de conexión desde `appsettings.json`.

### Fragmento clave

```csharp
builder.Services.AddSingleton<DapperConnectionFactory>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
```

### Principios aplicados
- Inversión de dependencias: Los controladores reciben servicios sin conocer detalles de infraestructura.
- Configuración limpia: Separamos la configuración de la lógica.

---

## 🛠️ Configuración de la base de datos (PostgreSQL)

### Creación de tablas

Este microservicio usa PostgreSQL como base de datos relacional. Las siguientes tablas deben crearse previamente:

```sql
CREATE TABLE IF NOT EXISTS Users (
    Id UUID PRIMARY KEY,
    Username TEXT NOT NULL,
    ConnectedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS ChatRooms (
    Id UUID PRIMARY KEY,
    Name TEXT NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Messages (
    Id UUID PRIMARY KEY,
    Content TEXT NOT NULL,
    SentAt TIMESTAMP NOT NULL,
    UserId UUID NOT NULL REFERENCES Users(Id),
    ChatRoomId UUID NOT NULL REFERENCES ChatRooms(Id)
);
```

### Conexión

La cadena de conexión debe estar definida en appsettings.json dentro de AWS_API:

```json
"ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=db_testing;Username=testing;Password=testing"
}
```

---

## 📡 Endpoints REST

El controlador `MessagesController` expone los siguientes endpoints REST:

### GET `/api/messages/{chatRoomId}`

Obtiene todos los mensajes de una sala.

**Response:**
```json
[
  {
    "id": "uuid",
    "content": "texto",
    "sentAt": "2025-07-04T10:15:00Z",
    "userId": "uuid",
    "chatRoomId": "uuid"
  }
]
```

### POST /api/messages
Envía un nuevo mensaje a una sala.

Body:

```json
{
  "chatRoomId": "uuid",
  "userId": "uuid",
  "content": "Hola mundo"
}
```

Principios aplicados:

- Responsabilidad única: el controlador solo orquesta, la lógica vive en Application.
- Desacoplamiento: el controlador no conoce la infraestructura.

---

## 🔄 Datos de prueba

Antes de probar los endpoints, asegúrate de tener al menos un usuario y una sala de chat en la base de datos:

```sql
-- Usuario
INSERT INTO Users (Id, Username, ConnectedAt)
VALUES ('11112222-3333-4444-5555-666677778888', 'Juan', CURRENT_TIMESTAMP);

-- Sala
INSERT INTO ChatRooms (Id, Name, CreatedAt)
VALUES ('a1b2c3d4-e5f6-7890-abcd-1234567890ab', 'General', CURRENT_TIMESTAMP);
```

---

## ✅ Prueba exitosa

Primer mensaje insertado correctamente mediante `POST /api/messages`.

### Respuesta ejemplo:

```json
{
  "id": "7dc4808d-11ea-436e-aff6-18963d77fac3",
  "content": "¡Hola desde REST!",
  "sentAt": "2025-07-05T02:41:42.2320829Z",
  "userId": "11112222-3333-4444-5555-666677778888",
  "user": null,
  "chatRoomId": "a1b2c3d4-e5f6-7890-abcd-1234567890ab",
  "chatRoom": null
}
```

---

## 🧭 Estrategia de usuarios en el microservicio

### Fase inicial (sin autenticación)

- Los usuarios se registran al momento de ingresar su nombre en el cliente (Angular o JS).
- Se crea un nuevo usuario en la base de datos (`Users`).
- El ID generado (GUID) se guarda en el navegador (localStorage).
- Este ID se usa para enviar y recibir mensajes.

### Futuro: autenticación JWT

- Registro de usuarios con email y contraseña.
- Tokens JWT para proteger endpoints y conectar a SignalR.
- Chats privados y moderación.

---

## 🔌 SignalR (Chat en tiempo real)

El servicio utiliza SignalR para la comunicación bidireccional entre cliente y servidor.

### 📡 Endpoints de SignalR

- `/chatHub`

### 📘 Métodos del Hub

- `SendMessage(MessageDto)` → Envía y guarda el mensaje.
- `JoinRoom(Guid chatRoomId)` → Une al usuario a un grupo específico.
- `LeaveRoom(Guid chatRoomId)` → Abandona el grupo.

### 🔗 Conexión desde el cliente

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5001/chatHub")
    .build();

await connection.start();
```

---

## 🆕 Nuevos endpoints REST: Usuarios y Salas de chat
Agregado: 2025-07-05 (o ajusta la fecha según tu commit)

Se implementaron nuevos endpoints para permitir el registro dinámico de usuarios y salas directamente desde el cliente.

DTOs usados

```csharp
// CreateUserDto.cs
public class CreateUserDto
{
    public string Username { get; set; } = default!;
}

// CreateChatRoomDto.cs
public class CreateChatRoomDto
{
    public string Name { get; set; } = default!;
}
```

### 📨 Endpoints disponibles

POST /api/users
Registra un nuevo usuario.

Request:

```json
{
  "username": "maria42"
}
```

POST /api/chatrooms
Crea una nueva sala de chat.

Request:

```json
{
  "name": "Sala General"
}
```

### 🧼 Principios aplicados
- SRP (Single Responsibility): Cada clase y endpoint cumple una única función.
- DIP (Dependency Inversion Principle): La lógica de aplicación depende de interfaces (IUserRepository, IChatRoomRepository) y no de implementaciones concretas.
- CLEAN Architecture: División clara entre controladores, servicios, dominio y acceso a datos.



