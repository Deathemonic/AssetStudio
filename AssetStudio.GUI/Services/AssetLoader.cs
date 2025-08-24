using System;
using System.Threading.Tasks;

namespace AssetStudio.GUI.Services;

public class AssetLoader(AssetsManager assetsManager)
{
    public async Task<bool> LoadFileAsync(string filePath)
    {
        try
        {
            await Task.Run(() => { assetsManager.LoadFilesAndFolders(filePath); });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> LoadFilesAsync(string[] filePaths)
    {
        try
        {
            await Task.Run(() => { assetsManager.LoadFilesAndFolders(filePaths); });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
