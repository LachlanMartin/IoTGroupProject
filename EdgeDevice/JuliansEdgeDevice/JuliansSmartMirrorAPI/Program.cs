using Microsoft.OpenApi.Models;
using SmartMirror.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSingleton<MqttService>(); 
builder.Services.AddHostedService<MqttService>(provider => provider.GetService<MqttService>());
builder.Services.AddSingleton<ArduinoService>(); 
builder.Services.AddHostedService<ArduinoService>(provider => provider.GetService<ArduinoService>());
builder.Services.AddLogging();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartMirror API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartMirror API v1");
    });
}
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();