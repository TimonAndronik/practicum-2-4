var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

var usersDb = sqlServer.AddDatabase("usersdb");
var catalogDb = sqlServer.AddDatabase("catalogdb");

builder.AddProject<Projects.ModuShop_Web>("webapi")
    .WithReference(usersDb)
    .WithReference(catalogDb)
    .WaitFor(usersDb)
    .WaitFor(catalogDb);

builder.Build().Run();