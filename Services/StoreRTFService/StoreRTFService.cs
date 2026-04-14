using Domain.Services;

namespace Services.StoreRTFService
{
    public class StoreRTFService : IStoreRTFService
    {
        string photosPath = @"..\..\..\RTFs";

        public string StoreRTF(byte[] rtfData)
        {
            try
            {
                string fileName = "volcano_" + DateTime.UtcNow.Microsecond + ".rtf";
                string rtfFolderPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, photosPath));
                string destinationPath = Path.Combine(rtfFolderPath, fileName);

                File.WriteAllBytes(destinationPath, rtfData);

                return destinationPath;
            }
            catch
            {
                return String.Empty;
            }
        }
        public bool UpdateRTF(byte[] rtfData, string path)
        {
            try
            {
                File.WriteAllBytes(path, rtfData);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
