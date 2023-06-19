using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciDevHome.Data;

namespace SciDevHome.Utils;
public class ZQDHelper
{
    /// <summary>
    /// 获取实际路径名字
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetPath(IEnumerable<string> path)
    {
        return string.Join("/", path);
    }
    /// <summary>
    /// 获取实际路径名字
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetPath(IEnumerable<Folder> folders)
    {
    
        return string.Join("/", folders.Select(s => s.Name));
    }

    // 同为可枚举，亦有其不同，一为前路之列表，二位所经之路径
    // 后期可能考虑linux
    public IEnumerable<Folder> GetRootPath()
    {
        List<Folder> folders = new List<Folder>();
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            folders.Add(new Folder { Name = drive.Name });
        }
        return folders;
    }




}
