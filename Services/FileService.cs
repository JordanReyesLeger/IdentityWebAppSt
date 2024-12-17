using Models.Files;
using Repositories;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class FileService : IFileService
    {
        private readonly IFileRecordRepository _fileRecordRepository;
        private readonly IParentRecordRepository _parentRecordRepository;
        private readonly IBlobStorageRepository _blobStorageRepository;

        public FileService(IFileRecordRepository fileRecordRepository, IParentRecordRepository parentRecordRepository, IBlobStorageRepository blobStorageRepository)
        {
            _fileRecordRepository = fileRecordRepository;
            _parentRecordRepository = parentRecordRepository;
            _blobStorageRepository = blobStorageRepository;
        }

        // Implementación de otros métodos...

        public async Task<(string FileUrl, string FileName, string Description)> UploadFileAsync(Stream fileStream, string fileName, string contentType, string description, int year, long consecutivo)
        {
            if (fileStream == null || fileStream.Length == 0)
            {
                throw new ArgumentException("El archivo no puede estar vacío.");
            }

            // Crear el registro padre
            var parentRecord = new ParentRecord
            {
                PartitionKey = year.ToString(),
                RowKey =consecutivo.ToString(),
                Description = description,
                // Otros campos necesarios
            };
            await CreateParentRecordAsync(parentRecord);

            // Subir el archivo al Blob Storage
            var fileUrl = await _blobStorageRepository.UploadFileAsync(fileStream, fileName, contentType, year, consecutivo);

            // Crear el registro del archivo
            var fileRecord = new FileRecord
            {
                PartitionKey = $"{year.ToString()}-{consecutivo}",
                RowKey = Guid.NewGuid().ToString(),
                FileName = fileName,
                FileSize = fileStream.Length,
                Url = fileUrl
                // Otros campos necesarios
            };
            await CreateFileRecordAsync(fileRecord);

            return (fileUrl, fileName, description);
        }

        public async Task CreateParentRecordAsync(ParentRecord parentRecord)
        {
            await _parentRecordRepository.CreateAsync(parentRecord);
        }

        public async Task CreateFileRecordAsync(FileRecord fileRecord)
        {
            await _fileRecordRepository.CreateAsync(fileRecord);
        }
    }
}
