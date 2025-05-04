var builder = DistributedApplication.CreateBuilder(args);
//var postgres = builder.AddPostgres("postgres")
//    .WithImage("ankane/pgvector")
//    .WithImageTag("latest")
//    .WithLifetime(ContainerLifetime.Persistent);

//var catalogDb = postgres.AddDatabase("catalogdb");
builder.Build().Run();
