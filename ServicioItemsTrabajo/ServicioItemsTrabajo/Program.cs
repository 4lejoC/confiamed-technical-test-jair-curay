using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ServicioItemsTrabajo.Clients;
using ServicioItemsTrabajo.Data;
using ServicioItemsTrabajo.Models;
using ServicioItemsTrabajo.Repositories;
using ServicioItemsTrabajo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<WorkItemsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<DistributionOptions>(
    builder.Configuration.GetSection("DistributionSettings"));

string? userManagementBaseUrl = builder.Configuration["UserManagementService:BaseUrl"];

if (string.IsNullOrWhiteSpace(userManagementBaseUrl))
{
    throw new InvalidOperationException("La URL del microservicio de usuarios es obligatoria.");
}

builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();
builder.Services.AddScoped<IWorkItemService, WorkItemService>();
builder.Services.AddHttpClient<IUserManagementClient, UserManagementClient>(client =>
{
    client.BaseAddress = new Uri(userManagementBaseUrl);
});

builder.Services.AddEndpointsApiExplorer(); //Permite descubrir endpoints para documentarlos
builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
}); //Genera la documentación OpenAPI

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkItemsDbContext>();

    context.Database.EnsureCreated(); //Crea la base SQLite y la tabla si no existen

    if (!context.WorkItems.Any()) //Evita insertar duplicados cada vez que arranca
    {
        context.WorkItems.AddRange(
            new WorkItem
            {
                Title = "Revisar contratos",
                Description = "Revisar contratos pendientes del día",
                Relevance = WorkItemRelevance.High,
                DueDate = DateTime.UtcNow.Date.AddDays(1),
                Status = WorkItemStatus.Pending,
                AssignedUsername = "jcuray",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                AssignedAt = DateTime.UtcNow.AddDays(-2),
                PendingOrder = 1
            },
            new WorkItem
            {
                Title = "Actualizar afiliados",
                Description = "Actualizar información general de afiliados",
                Relevance = WorkItemRelevance.Low,
                DueDate = DateTime.UtcNow.Date.AddDays(5),
                Status = WorkItemStatus.Pending,
                AssignedUsername = "mlopez",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                AssignedAt = DateTime.UtcNow.AddDays(-1),
                PendingOrder = 1
            },
            new WorkItem
            {
                Title = "Generar reporte diario",
                Description = "Generar reporte operativo del día",
                Relevance = WorkItemRelevance.High,
                DueDate = DateTime.UtcNow.Date.AddDays(4),
                Status = WorkItemStatus.Pending,
                AssignedUsername = "agarcia",
                CreatedAt = DateTime.UtcNow.AddHours(-12),
                AssignedAt = DateTime.UtcNow.AddHours(-12),
                PendingOrder = 1
            },
            new WorkItem
            {
                Title = "Validar documentación",
                Description = "Validar documentos de soporte recibidos",
                Relevance = WorkItemRelevance.Low,
                DueDate = DateTime.UtcNow.Date.AddDays(2),
                Status = WorkItemStatus.Pending,
                AssignedUsername = "rperez",
                CreatedAt = DateTime.UtcNow.AddHours(-10),
                AssignedAt = DateTime.UtcNow.AddHours(-10),
                PendingOrder = 1
            },
            new WorkItem
            {
                Title = "Cerrar caso clínico",
                Description = "Cerrar caso que ya fue resuelto",
                Relevance = WorkItemRelevance.High,
                DueDate = DateTime.UtcNow.Date.AddDays(-1),
                Status = WorkItemStatus.Completed,
                AssignedUsername = "jcuray",
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                AssignedAt = DateTime.UtcNow.AddDays(-4),
                CompletedAt = DateTime.UtcNow.AddDays(-1),
                PendingOrder = 0
            }
        );

        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); //Expone el JSON de Swagger
    app.UseSwaggerUI(); //Muestra la interfaz web navegable
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
