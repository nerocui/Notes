using CommunityToolkit.WinUI.Helpers;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace Notes.Helpers
{
    internal static class StorageHelper
    {
        private static StorageFolder _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private const string _fileName = "appdata.json";
        public static async Task<bool> SaveFile<T>(T item)
        {
            try
            {
                var json = JsonSerializer.Serialize<T>(item);
                var file = await _localFolder.WriteTextToFileAsync(json, _fileName, CreationCollisionOption.ReplaceExisting);
                return file != null;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static async Task<T> GetFile<T>()
        {
            try
            {
                var file = await _localFolder.GetFileAsync(_fileName);
                var json = await Windows.Storage.FileIO.ReadTextAsync(file);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch(Exception e)
            {
                return default(T);
            }
        }
    }
}
