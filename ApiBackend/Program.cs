using Azure.Identity;
using Azure.Storage.Blobs;
using Repositories;
using Repositories.Interfaces;
using Services;
using Services.Interfaces;

namespace ApiBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurar Key Vault si está habilitado
            var keyVaultUri = builder.Configuration["KeyVault:Uri"];
            bool useIdentity = bool.Parse(builder.Configuration["UseIdentity"]);

            if (useIdentity)
            {
                // Settings con Identity
                if (!string.IsNullOrEmpty(keyVaultUri))
                {
                    builder.Configuration.AddAzureKeyVault(
                        new Uri(keyVaultUri),
                        new DefaultAzureCredential());
                }

                // Crear el BlobServiceClient usando Managed Identity
                builder.Services.AddSingleton(provider =>
                {
                    var blobServiceUri = new Uri(builder.Configuration["AzureStorage:BlobServiceUri"]);
                    return new BlobServiceClient(blobServiceUri, new DefaultAzureCredential());
                });
            }
            else // Settings con cadena de conexión
            {
                // Configurar Key Vault utilizando Client ID y Client Secret
                var keyVaultConfig = builder.Configuration.GetSection("AzureKeyVault");
                var clientId = keyVaultConfig["ClientId"];
                var clientSecret = keyVaultConfig["ClientSecret"];
                var tenantId = keyVaultConfig["TenantId"];

                if (!string.IsNullOrEmpty(keyVaultUri) && !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(tenantId))
                {
                    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), credential);
                }

                // Registrar servicios
                builder.Services.AddSingleton(provider =>
                {
                    var connectionString = builder.Configuration["BlobStorageConnectionString"];
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new Exception("La cadena de conexión del Blob Storage no está configurada.");
                    }

                    return new BlobServiceClient(connectionString); // Usando Azure.Storage.Blobs
                });
            }

            // Registrar servicios de aplicación
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IFileRecordRepository, FileRecordRepository>(provider =>
            {
                var connectionString = builder.Configuration["TableStorageConnectionString"];
                var tableName = builder.Configuration["FileRecordTableName"];
                return new FileRecordRepository(connectionString, tableName);
            });
            builder.Services.AddScoped<IParentRecordRepository, ParentRecordRepository>(provider =>
            {
                var connectionString = builder.Configuration["TableStorageConnectionString"];
                var tableName = builder.Configuration["ParentRecordTableName"];
                return new ParentRecordRepository(connectionString, tableName);
            });
            builder.Services.AddScoped<IBlobStorageRepository, BlobStorageRepository>(provider =>
            {
                var blobServiceClient = provider.GetRequiredService<BlobServiceClient>();
                var containerName = builder.Configuration["BlobContainerName"];
                return new BlobStorageRepository(blobServiceClient, containerName);
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
