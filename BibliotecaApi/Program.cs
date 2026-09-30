using System.Reflection;
using Asp.Versioning;
using BibliotecaApi.Data;
using BibliotecaApi.Middleware;
using BibliotecaApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Banco de dados (SQLite via EF Core); o arquivo fica sempre na pasta do projeto,
// independentemente do diretório de onde a API é executada
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!
    .Replace("Data Source=", $"Data Source={builder.Environment.ContentRootPath}{Path.DirectorySeparatorChar}");
builder.Services.AddDbContext<BibliotecaDbContext>(options => options.UseSqlite(connectionString));

// Camada de serviços
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILivroService, LivroService>();

// Tratamento global de erros
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

// Versionamento de endpoints: /api/v1/...
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Biblioteca API",
        Version = "v1",
        Description = "API RESTful para gestão do acervo de uma biblioteca (autores e livros)."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

var app = builder.Build();

// Aplica as migrations pendentes ao iniciar, criando o banco se necessário
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BibliotecaDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Biblioteca API v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
