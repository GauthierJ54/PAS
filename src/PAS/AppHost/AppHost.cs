var builder = DistributedApplication.CreateBuilder(args);

var rabbitMqUsername = builder.AddParameter("rabbitmq-username", secret: true);

var rabbitMqPassword = builder.AddParameter("rabbitmq-password", secret: true);

var rabbitMq = builder
    .AddRabbitMQ("messaging", rabbitMqUsername, rabbitMqPassword)
    .WithDataVolume("pas-rabbitmq-data")
    .WithManagementPlugin();

var keycloak = builder
    .AddKeycloak(name: "keycloak", port: 8080)
    .WithDataVolume("pas-keycloak-data");

var sqlServer = builder.AddSqlServer("sqlServer")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("pas-sqlServer-data")
    .WithDbGate();

var pasDatabase = sqlServer.AddDatabase(name: "pas", databaseName: "PAS");

var databaseUpdater = builder
    .AddProject<Projects.PAS_DatabaseUpdater>("pas-database-updater")
    .WithReference(pasDatabase)
    .WaitFor(pasDatabase);

var assetApi = builder
    .AddProject<Projects.PAS_Asset_Api>("pas-asset-api")
    .WithReference(rabbitMq)
    .WithReference(keycloak)
    .WithReference(pasDatabase)
    .WaitFor(rabbitMq)
    //.WaitFor(keycloak)
    .WaitFor(pasDatabase)
    .WaitForCompletion(databaseUpdater);

var calculationApi = builder
    .AddProject<Projects.PAS_Calculation_Api>("pas-calculation-api")
    .WithReference(rabbitMq)
    .WithReference(keycloak)
    .WithReference(pasDatabase)
    .WaitFor(rabbitMq)
    //.WaitFor(keycloak)
    .WaitFor(pasDatabase)
    .WaitForCompletion(databaseUpdater);

var frontend = builder
    .AddViteApp("pas-frontend", "../frontend")
    .WithReference(assetApi)
    .WithReference(calculationApi)
    .WithReference(keycloak)
    .WaitFor(assetApi)
    .WaitFor(calculationApi);
    //.WaitFor(keycloak);

builder.Build().Run();