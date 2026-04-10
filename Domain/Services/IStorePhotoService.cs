namespace Domain.Services
{
    public interface IStorePhotoService
    {
        public string CopyPhotoToPath(long volcanoID, string sourcePath);
    }
}
