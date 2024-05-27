using System.Collections.Concurrent;
using System.Data;

namespace FleetManagementSystem.Services.utils;

public class GVAR
{
    public ConcurrentDictionary<string, ConcurrentDictionary<string, string>> DicOfDic { get; set; }
    public ConcurrentDictionary<string, DataTable> DicOfDT { get; set; }

    public GVAR()
    {
        DicOfDic = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        DicOfDT = new ConcurrentDictionary<string, DataTable>();
    }

    // Method to initialize or add a new dictionary to DicOfDic
    public void InitializeOrAddDictionary(string key)
    {
        DicOfDic.TryAdd(key, new ConcurrentDictionary<string, string>());
    }

    // Method to add or update a key-value pair in a sub-dictionary within DicOfDic
    public void AddOrUpdateInDicOfDic(string dictionaryKey, string key, string value)
    {
        if (DicOfDic.TryGetValue(dictionaryKey, out var subDictionary))
        {
            subDictionary[key] = value;
        }
        else
        {
            // Initialize the sub-dictionary if it does not exist
            var newSubDictionary = new ConcurrentDictionary<string, string>();
            newSubDictionary[key] = value;
            DicOfDic[dictionaryKey] = newSubDictionary;
        }
    }

    // Method to initialize or add a new DataTable to DicOfDT
    public void InitializeOrAddDataTable(string key)
    {
        DicOfDT.TryAdd(key, new DataTable());
    }

    // Method to update a DataTable in DicOfDT
    public void UpdateDataTable(string key, DataTable dataTable)
    {
        DicOfDT[key] = dataTable;
    }
}
