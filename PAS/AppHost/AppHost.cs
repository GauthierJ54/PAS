var builder = DistributedApplication.CreateBuilder(args);

var rabbitMqUsername = builder.AddParameter(
    "rabbitmq-username",
    secret: true);

var rabbitMqPassword = builder.AddParameter(
    "rabbitmq-password",
    secret: true);

var rabbitMq = builder
    .AddRabbitMQ(
        "messaging",
        rabbitMqUsername,
        rabbitMqPassword)
    .WithDataVolume("pas-rabbitmq-data")
    .WithManagementPlugin();

var assetApi = builder
    .AddProject<Projects.PAS_Asset_Api>("pas-asset-api")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

var calculationApi = builder
    .AddProject<Projects.PAS_Calculation_Api>("pas-calculation-api")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

builder.Build().Run();