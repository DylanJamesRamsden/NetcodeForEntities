using Unity.NetCode;
using UnityEngine.Scripting;

// Preserves this class from being stripped when building
[Preserve]
public class NetcodeBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        // 7777 Autoconnects us so we don't have to specify the exact port
        AutoConnectPort = 7777;
        return base.Initialize(defaultWorldName);
    }
}
