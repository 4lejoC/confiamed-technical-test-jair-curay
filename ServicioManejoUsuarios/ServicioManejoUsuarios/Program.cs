using Microsoft.EntityFrameworkCore;
using ServicioManejoUsuarios.Data;
using ServicioManejoUsuarios.Models;
using ServicioManejoUsuarios.Repositories;
using ServicioManejoUsuarios.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

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
    var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

    context.Database.EnsureCreated(); //Crea la base SQLite y la tabla si no existen

    if (!context.Users.Any()) //Evita insertar duplicados cada vez que arranca
    {
        context.Users.AddRange( //Carga usuarios semilla para pruebas rápidas
            new User { Username = "jcuray", FullName = "Jair Curay", IsActive = true },
            new User { Username = "mlopez", FullName = "Maria Lopez", IsActive = true },
            new User { Username = "agarcia", FullName = "Ana Garcia", IsActive = true },
            new User { Username = "rperez", FullName = "Raul Perez", IsActive = true }
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
