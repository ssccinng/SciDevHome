﻿using System.Text.Json;
namespace SciDevHome.Utils
{
    public class SaveFileManager
    {
        public static async Task SaveAsync(string fileName, DevHomeClientSave devHomeClientSave)
        {
            //fileName = AppDomain.CurrentDomain.BaseDirectory + fileName;

            await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(devHomeClientSave));
        }

        public static async Task<DevHomeClientSave> LoadAsync(string fileName)
        {

            //fileName = Environment. + fileName;
            try
            {

                var json = File.ReadAllText(fileName);
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