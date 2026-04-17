var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

var usersDb = sqlServer.AddDatabase("usersdb");
var catalogDb = sqlServer.AddDatabase("catalogdb");
var ordersDb = sqlServer.AddDatabase("ordersdb");

builder.AddProject<Projects.ModuShop_Web>("webapi")
    .WithReference(usersDb)
    .WithReference(catalogDb)
    .WithReference(ordersDb)
    .WaitFor(usersDb)
    .WaitFor(catalogDb)
    .WaitFor(ordersDb);

builder.Build().Run();