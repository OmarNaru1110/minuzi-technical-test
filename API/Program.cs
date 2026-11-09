using System.Text.Json.Serialization;
using Menuzi.TechnicalTest.Core.Interfaces;
using Menuzi.TechnicalTest.Core.Services;
using Menuzi.TechnicalTest.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IProductCatalog, InMemoryProductCatalog>();
builder.Services.AddSingleton<DeliverySlotService>();

var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Menuzi Technical Test API v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
