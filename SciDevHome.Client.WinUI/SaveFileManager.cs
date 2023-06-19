using System.Text.Json;
using Windows.Storage;

namespace SciDevHome.Utils
{
    public class SaveFileManager
    {
       static  Windows.Storage.StorageFolder localFolder =
    Windows.Storage.ApplicationData.Current.LocalFolder;
        public static async Task SaveAsync(string fileName, DevHomeClientSave devHomeClientSave)
        {
            StorageFile sampleFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

            await FileIO.WriteTextAsync(sampleFile, JsonSerializer.Serialize(devHomeClientSave));
        }

        public static async Task<DevHomeClientSave> LoadAsync(string fileName)
        {

            StorageFile sampleFile = await localFolder.CreateFileAsync(fileName,
      CreationCollisionOption.OpenIfExists);

            try
            {
                var json = await FileIO.ReadTextAsync(sampleFile);
                return JsonSerializer.Deserialize<DevHomeClientSave>(json);

            }
            catch (Exception)
            {

                var res = new DevHomeClientSave();

                await SaveAsync(fileName, res);
                return res;
            }
        }

    }
}