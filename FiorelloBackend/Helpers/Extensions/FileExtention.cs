using Azure.Core;

namespace FiorelloBackend.Helpers.Extensions
{
    public static class FileExtention
    {
        public static bool CheckFileType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }

        public static bool CheckFileSize(this IFormFile file, long size)
        {
            return file.Length/1024 < size;
        }

        public static string GenerateFilePath(this IWebHostEnvironment env, string folder, string fileName)
        { 
            return Path.Combine(env.WebRootPath, folder, fileName);
        }

        public static void DeleteFile(this string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }

        public static string GenerateFileName(this IFormFile file)
        {
            return Guid.NewGuid().ToString() + "-" + file.FileName;
        }
    }
}
