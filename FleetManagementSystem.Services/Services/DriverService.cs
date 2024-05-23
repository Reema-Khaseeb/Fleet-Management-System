using FleetManagementSystem.Db.Interfaces;
using FleetManagementSystem.Services.utils;
using Npgsql;
using System.Collections.Concurrent;
using System.Data;

namespace FleetManagementSystem.Services;

public class DriverService
{
    private readonly IDatabaseConnection _databaseConnection;

    public DriverService(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public GVAR AddDriver(GVAR gvar)
    {
        var gvarResponse = new GVAR();
        gvarResponse.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            if (!gvar.DicOfDic.TryGetValue("Tags", out var driverDetails))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            string driverName = driverDetails.GetValueOrDefault("DriverName", null);
            string phoneNumberString = driverDetails.GetValueOrDefault("PhoneNumber", null);
            long phoneNumber = 0;
            if (!string.IsNullOrEmpty(phoneNumberString) && long.TryParse(phoneNumberString, out long phone))
            {
                phoneNumber = phone;
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO \"Driver\" (\"DriverName\", \"PhoneNumber\") VALUES (@DriverName, @PhoneNumber)", connection))
                {
                    command.Parameters.AddWithValue("@DriverName", driverName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber != 0 ? (object)phoneNumber : DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }

            gvarResponse.DicOfDic["Tags"]["STS"] = "1";
            return gvarResponse;
        }
        catch (Exception)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            return gvarResponse;
        }
    }

    public GVAR UpdateDriver(GVAR gvar)
    {
        var gvarResponse = new GVAR();
        gvarResponse.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            if (!gvar.DicOfDic.TryGetValue("Tags", out var driverDetails))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            // Ensure the ID of the Driver record is provided
            if (!driverDetails.TryGetValue("DriverID", out string driverIdString) || !long.TryParse(driverIdString, out long driverId))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();

                // Check if the DriverID exists
                using (var checkCommand = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM \"Driver\" WHERE \"DriverID\" = @DriverID)", connection))
                {
                    checkCommand.Parameters.AddWithValue("@DriverID", driverId);
                    bool exists = (bool)checkCommand.ExecuteScalar();
                    if (!exists)
                    {
                        gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                        return gvarResponse;
                    }
                }

                var updateParts = new List<string>();
                var parameters = new Dictionary<string, object>();

                // Process each field dynamically, excluding the primary key "DriverID"
                foreach (var detail in driverDetails)
                {
                    if (detail.Key != "DriverID" && detail.Value != null)  // Exclude the PK from updates
                    {
                        string paramName = "@" + detail.Key;
                        object value = detail.Value;

                        // Handle type conversion for specific fields
                        if (detail.Key == "PhoneNumber" && long.TryParse(detail.Value, out long longValue))
                        {
                            value = longValue; // Store as long for database insertion
                        }

                        updateParts.Add($"\"{detail.Key}\" = {paramName}");
                        parameters[paramName] = value;
                    }
                }

                // Only proceed if there are fields to update
                if (updateParts.Count > 0)
                {
                    string sql = $"UPDATE \"Driver\" SET {string.Join(", ", updateParts)} WHERE \"DriverID\" = @DriverID";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                        command.Parameters.AddWithValue("@DriverID", driverId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                            return gvarResponse;
                        }
                    }
                }
                
                gvarResponse.DicOfDic["Tags"]["STS"] = "1";
                return gvarResponse;
            }
        }
        catch (Exception)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            return gvarResponse;
        }
    }

    public GVAR GetDrivers()
    {
        var gvar = new GVAR();
        try
        {
            var dt = new DataTable();
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT \"DriverID\", \"DriverName\", \"PhoneNumber\" FROM \"Driver\"", connection))
                using (var reader = command.ExecuteReader())
                {
                    dt.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("DriverID", typeof(string)),
                        new DataColumn("DriverName", typeof(string)),
                        new DataColumn("PhoneNumber", typeof(string))
                    });

                    while (reader.Read())
                    {
                        var row = dt.NewRow();
                        row["DriverID"] = reader["DriverID"].ToString();
                        row["DriverName"] = reader["DriverName"].ToString();
                        row["PhoneNumber"] = reader["PhoneNumber"].ToString();
                        dt.Rows.Add(row);
                    }
                }
            }
            gvar.DicOfDT["Driver"] = dt;

            if (!gvar.DicOfDic.ContainsKey("Tags"))
            {
                gvar.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();
            }
            gvar.DicOfDic["Tags"]["STS"] = "1";  // Indicate success
        }
        catch (Exception ex)
        {
            if (!gvar.DicOfDic.ContainsKey("Tags"))
            {
                gvar.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();
            }
            gvar.DicOfDic["Tags"]["STS"] = "0";  // Indicate failure
            gvar.DicOfDT["Driver"] = new DataTable();  // Ensure structure consistency
        }
        return gvar;
    }

    public GVAR DeleteDriver(GVAR gvar)
    {
        var gvarResponse = new GVAR();
        gvarResponse.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            if (!gvar.DicOfDic.TryGetValue("Tags", out var driverDetails))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            // Ensure the ID of the Driver record is provided
            if (!driverDetails.TryGetValue("DriverID", out string driverIdString) || !long.TryParse(driverIdString, out long driverId))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM \"Driver\" WHERE \"DriverID\" = @DriverID", connection))
                {
                    command.Parameters.AddWithValue("@DriverID", driverId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                        return gvarResponse;
                    }
                }
            }

            gvarResponse.DicOfDic["Tags"]["STS"] = "1";
            return gvarResponse;
        }
        catch (Exception)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            return gvarResponse;
        }
    }
}
