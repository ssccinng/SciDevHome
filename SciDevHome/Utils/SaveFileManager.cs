using System.Text.Json;

namespace SciDevHome.Utils
{
    public class SaveFileManager
    {
        public static async Task SaveAsync(string fileName, DevHomeClientSave devHomeClientSave)
        {
            await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(devHomeClientSave));
        }

        public static async Task<DevHomeClientSave> LoadAsync(string fileName)
        {
            try
            {
                return JsonSerializer.Deserialize<DevHomeClientSave>(File.ReadAllText(fileName));

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