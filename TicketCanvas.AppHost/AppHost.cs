var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var databasePassword = builder.AddParameter("database-password", secret: true);

var sqlServer = builder.AddSqlServer("database", databasePassword, 5432)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint("direct-connection", endpoint =>
    {
        endpoint.Port = 2345;
        endpoint.TargetPort = 1433;
        endpoint.IsProxied = false;
    });
var showDb = sqlServer.AddDatabase("show-db");
var ticketDb = sqlServer.AddDatabase("ticket-db");
var paymentDb = sqlServer.AddDatabase("payment-db");
var userDb = sqlServer.AddDatabase("user-db");

var showMigration = builder.AddProject<Projects.TicketCanvas_Show_MigrationService>("show-migration")
    .WithReference(showDb)
    .WaitFor(showDb);

var showApi = builder.AddProject<Projects.TicketCanvas_Show_Api>("show-api")
    .WithReference(showDb)
    .WaitFor(showDb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitForCompletion(showMigration)
    .WithHttpHealthCheck("/health");

var ticketMigration = builder.AddProject<Projects.TicketCanvas_Ticket_MigrationService>("ticket-migration")
    .WithReference(ticketDb)
    .WaitFor(ticketDb);

var ticketApi = builder.AddProject<Projects.TicketCanvas_Ticket_Api>("ticket-api")
    .WithReference(ticketDb)
    .WaitFor(ticketDb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitForCompletion(ticketMigration)
    .WithHttpHealthCheck("/health");

var paymentMigration = builder.AddProject<Projects.TicketCanvas_Payment_MigrationService>("payment-migration")
    .WithReference(paymentDb)
    .WaitFor(paymentDb);

var paymentApi = builder.AddProject<Projects.TicketCanvas_Payment_Api>("payment-api")
    .WithReference(paymentDb)
    .WaitFor(paymentDb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitForCompletion(paymentMigration)
    .WithHttpHealthCheck("/health");

var userMigration = builder.AddProject<Projects.TicketCanvas_User_MigrationService>("user-migration")
    .WithReference(userDb)
    .WaitFor(userDb);

var userApi = builder.AddProject<Projects.TicketCanvas_User_Api>("user-api")
    .WithReference(userDb)
    .WaitFor(userDb)
    .WaitForCompletion(userMigration)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
