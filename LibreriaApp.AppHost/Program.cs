var builder = DistributedApplication.CreateBuilder(args);

// Base de datos SQL Server (orquestada por Aspire en desarrollo)
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume("libreria-sqlserver-data");

var libreriaDb = sqlServer.AddDatabase("LibreriaDB");

// LibreriaAPI: servicio principal de la aplicación
var libreriaApi = builder.AddProject<Projects.LibreriaAPI>("libreria-api")
    .WithReference(libreriaDb)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
