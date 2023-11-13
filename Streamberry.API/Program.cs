using Microsoft.EntityFrameworkCore;
using Streamberry.API.Data;
using Streamberry.API.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});

builder.Services.AddScoped<IMovieRepository, MovieRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<DataContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

try
{
    //Aplica quaisquer migrations pendentes do contexto ao banco de dados. Irá criar o banco de dados se ainda não existir.
    context.Database.Migrate();
    DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    logger.LogError(ex, "Um problema ocorreu durante a migration");
}

app.Run();
