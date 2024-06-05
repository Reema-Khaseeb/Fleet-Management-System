using System.Collections.Concurrent;

namespace FleetManagementSystem.Services.utils;

public static class GVARUtility
{
    public static GVAR InitializeGVARResponse()
    {
        var gvar = new GVAR();
        gvar.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();
        return gvar;
    }
}
