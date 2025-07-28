using AWS_ChatService_API.Hubs;
using AWS_ChatService_Application.Interfaces;
using AWS_ChatService_Application.Services;
using AWS_ChatService_Domain.Interfaces;
using AWS_ChatService_Infrastructure.Configuration;
using AWS_ChatService_Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Agregar configuración para PostgreSQL (desde appsettings.json)
builder.Services.AddSingleton<DapperConnectionFactory>();

// 2. Repositorios e interfaces
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();

// 3. Servicios de aplicación
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatRoomService, ChatRoomService>();

// 4. Servicios de SignalR
builder.Services.AddSignalR();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5 CORS
var allowedOrigins = "_allowedOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins,
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "http://127.0.0.1:3000",
                    "http://localhost:4200",    // Añade tu puerto de Angular
                    "http://127.0.0.1:4200"     // Añade tu puerto de Angular
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true);
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 6 Enable CORS
//app.UseRouting();
app.UseCors(allowedOrigins);

app.UseAuthorization();

// 7. ChatHub Endpint
app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.Run();
