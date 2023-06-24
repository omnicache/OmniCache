using MinimalistDemo.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddWebUIServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationInitialiser>();
    await initializer.InitialiseAsync();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

