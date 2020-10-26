using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GemTracker.Shared.Services
{
    public interface IFileService
    {
        Task SetAsync<T>(string fileName, T objectToSerialize);
        Task<T> GetAsync<T>(string fileName);
    }

    public class FileService : IFileService
    {
        private readonly JsonSerializerOptions _options;
        public FileService()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        public async Task<T> GetAsync<T>(string fileName)
        {
            try
            {
                using FileStream fs = File.OpenRead(fileName);
                return await JsonSerializer.DeserializeAsync<T>(fs, _options);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SetAsync<T>(string fileName, T objectToSerialize)
        {
            try
            {
                using FileStream fs = File.Create(fileName);
                await JsonSerializer.SerializeAsync(fs, objectToSerialize, options: _options);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}