namespace Domain.Services
{
    public interface IStoreRTFService
    {
        public string StoreRTF(byte[] rtfData);
        public bool UpdateRTF(byte[] rtfData, string path);
    }
}
