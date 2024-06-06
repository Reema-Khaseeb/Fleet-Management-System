using System.Collections.Concurrent;
using static FleetManagementSystem.Common.DatabaseConstants;

namespace FleetManagementSystem.Services.utils;

public static class GVARUtility
{
    public static GVAR InitializeGVARResponse()
    {
        var gvar = new GVAR();
        gvar.DicOfDic[GVARKeys.Tags] = new ConcurrentDictionary<string, string>();
        return gvar;
    }

    public static void SetStatus(GVAR gvar, string status)
    {
        gvar.DicOfDic[GVARKeys.Tags][GVARKeys.Status] = status;
    }

    public static string GetStatus(GVAR gvar)
    {
        return gvar.DicOfDic[GVARKeys.Tags]
                   .TryGetValue(GVARKeys.Status, out var status) ? status : null;
    }
}
