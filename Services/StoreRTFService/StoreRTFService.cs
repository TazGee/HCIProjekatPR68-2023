using Domain.Services;

namespace Services.StoreRTFService
{
    public class StoreRTFService : IStoreRTFService
    {
        string photosPath = @"..\..\..\RTFs";

        public string StoreRTF(long volcanoID, string sourcePath)
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
