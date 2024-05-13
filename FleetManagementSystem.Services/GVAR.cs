using System.Collections.Concurrent;
using System.Data;

namespace FleetManagementSystem.Services;

public class GVAR
{
    public ConcurrentDictionary<string, ConcurrentDictionary<string, string>> DicOfDic { get; set; }
    public ConcurrentDictionary<string, DataTable> DicOfDT { get; set; }

    public GVAR()
    {
        DicOfDic = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        DicOfDT = new ConcurrentDictionary<string, DataTable>();
    }
}
