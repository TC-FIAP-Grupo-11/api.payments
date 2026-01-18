using FCG.Api.Payments.Services;
using FCG.Api.Payments.Consumers;
using FCG.Lib.Shared.Messaging.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPaymentService, SimulatedPaymentService>();

// Configurar messaging - consumer e publisher
builder.Services.AddMessagingConsumers(builder.Configuration, consumers =>
{
    consumers.AddConsumer<OrderPlacedEventConsumer>();
}, "payments");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
