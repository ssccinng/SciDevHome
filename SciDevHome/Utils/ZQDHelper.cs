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
    public static string GetPath(IEnumerable<string> path)
    {
        return string.Join("/", path);
    }
    /// <summary>
    /// 获取实际路径名字
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetPath(IEnumerable<Folder> folders)
    {

        return string.Join("/", folders.Select(s => s.Name));
    }

    // 同为可枚举，亦有其不同，一为前路之列表，二位所经之路径
    // 后期可能考虑linux
    public static IEnumerable<Folder> GetRootPath()
    {
        List<Folder> folders = new List<Folder>();
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            folders.Add(new Folder { Name = drive.Name });
        }

        // 加上环境
        return folders;
    }



    static string[] s_names = { "临流", "揽镜", "双魂", "scixing", "zqd", "linliu", "风骤暖", "dgz", "itachi", "guy", "madara", "obito", "veyle", "celine", "moob" };
    public static string GetRandomName()
    {
        return s_names[Random.Shared.Next(0, s_names.Length)];
    }

}
