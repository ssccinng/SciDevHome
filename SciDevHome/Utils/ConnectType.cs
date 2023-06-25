namespace SciDevHome.Utils;

public class ConnectType
{
    public readonly string Command;
    public ConnectType(string command)
    {
        Command = command;
    }

    public readonly ConnectType GetPathInfoType = new ConnectType("GetPathInfo");
    
}