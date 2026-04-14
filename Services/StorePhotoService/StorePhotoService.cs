using Domain.Services;
using System.IO;

namespace Services.SetPhotoService
{
    public class StorePhotoService : IStorePhotoService
    {
        string photosPath = @"..\..\..\Photos";

        public StorePhotoService() { }

        public string CopyPhotoToPath(long volcanoID, string sourcePath)
        {
            try
            {
                string fileName = "volcano_" + volcanoID + Path.GetExtension(sourcePath);
                string photoFolderPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, photosPath));
                string destinationPath = Path.Combine(photoFolderPath, fileName);

                File.Copy(sourcePath, destinationPath, true);

                return destinationPath;
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}
