using Amazon;
using Amazon.Lambda;
using Amazon.XRay.Recorder.Handlers.AspNetCore;
using Amazon.Runtime;
using FCG.Api.Payments.Consumers;
using FCG.Api.Payments.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Payment service
builder.Services.AddScoped<IPaymentService, SimulatedPaymentService>();

// AWS Lambda client
builder.Services.AddSingleton<IAmazonLambda>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var accessKey = config["AWS:AccessKeyId"];
    var secretKey = config["AWS:SecretAccessKey"];
    var sessionToken = config["AWS:SessionToken"];
    var region = config["AWS:Region"] ?? "us-east-1";

    AWSCredentials credentials;
    if (!string.IsNullOrEmpty(sessionToken))
        credentials = new SessionAWSCredentials(accessKey, secretKey, sessionToken);
    else if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
        credentials = new BasicAWSCredentials(accessKey, secretKey);
    else
        credentials = FallbackCredentialsFactory.GetCredentials();

    return new AmazonLambdaClient(credentials, RegionEndpoint.GetBySystemName(region));
});

builder.Services.AddScoped<ILambdaNotificationService, LambdaNotificationService>();

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedEventConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var user = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var pass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(user);
            h.Password(pass);
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.UseXRay("fcg-payments-api");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "payments-api" }));

app.Run();
