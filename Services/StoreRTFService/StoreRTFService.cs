using Domain.Services;
using System.IO;

namespace Services.StoreRTFService
{
    public class StoreRTFService : IStoreRTFService
    {
        string rtfsPath = @"..\..\..\RTFs";

        public string StoreRTF(byte[] rtfData)
        {
            try
            {
                string fileName = "volcano_" + DateTime.UtcNow.Microsecond + ".rtf";
                string dest = Path.Combine(rtfsPath, fileName);

                File.WriteAllBytes(dest, rtfData);

                return dest;
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
