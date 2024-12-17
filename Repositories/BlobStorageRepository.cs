﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageRepository(BlobServiceClient blobServiceClient, string containerName)
        {
            _blobServiceClient = blobServiceClient;
            _containerName = containerName;
        }

        /// <summary>
        /// Sube un archivo al contenedor de Azure Blob Storage.
        /// </summary>
        /// <param name="fileStream">Stream del archivo a subir.</param>
        /// <param name="fileName">Nombre del archivo.</param>
        /// <param name="contentType">Tipo de contenido (MIME type).</param>
        /// <returns>URL pública del archivo subido.</returns>
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, int year, long id)
        {
            try
            {

                // Generar la ruta de la carpeta basada en el año y el identificador
                var folderPath = $"{year}/{id}";

                // Combinar la ruta de la carpeta y el nombre del archivo
                var blobName = $"{folderPath}/{fileName}";

                // Obtener el cliente del contenedor
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

                // Crear el contenedor si no existe
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                // Obtener cliente del blob
                var blobClient = containerClient.GetBlobClient(blobName);

                // Configurar encabezados del blob
                var headers = new BlobHttpHeaders
                {
                    ContentType = contentType
                };

                // Subir el archivo
                await blobClient.UploadAsync(fileStream, headers);

                return blobClient.Uri.ToString(); // Devolver la URL del archivo subido
            }
            catch (Exception ex)
            {
                // Manejo de errores
                throw new Exception($"Error al subir el archivo: {ex.Message}", ex);

            }
        }

        /// <summary>
        /// Descarga un archivo desde Azure Blob Storage.
        /// </summary>
        /// <param name="fileName">Nombre del archivo a descargar.</param>
        /// <returns>Stream del archivo descargado.</returns>
        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            // Obtener el cliente del contenedor
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Obtener cliente del blob
            var blobClient = containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                // Descargar el archivo
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }

            throw new FileNotFoundException($"El archivo {fileName} no existe en el Blob Storage.");
        }

        /// <summary>
        /// Elimina un archivo de Azure Blob Storage.
        /// </summary>
        /// <param name="fileName">Nombre del archivo a eliminar.</param>
        /// <returns>Tarea completada.</returns>
        public async Task DeleteFileAsync(string fileName)
        {
            // Obtener el cliente del contenedor
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Obtener cliente del blob
            var blobClient = containerClient.GetBlobClient(fileName);

            // Eliminar el archivo si existe
            await blobClient.DeleteIfExistsAsync();
        }
    }
}