using FleetManagementSystem.Db.Interfaces;
using FleetManagementSystem.Services.utils;
using Npgsql;
using System.Collections.Concurrent;
using System.Data;

namespace FleetManagementSystem.Services.Services;

public class VehicleInformationService
{
    //private readonly ILogger _logger;
    private readonly IDatabaseConnection _databaseConnection;

    public VehicleInformationService(IDatabaseConnection databaseConnection)
    //, ILogger<VehicleInformationService> logger)
    {
        //_logger = logger;
        _databaseConnection = databaseConnection;
    }

    public GVAR AddVehicleInfo(GVAR gvar)
    {
        var gvarResponse = new GVAR();
        gvarResponse.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            if (!gvar.DicOfDic.TryGetValue("Tags", out var vehicleInfoDetails))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            // Ensure VehicleID is provided and valid
            if (!long.TryParse(vehicleInfoDetails["VehicleID"], out long vehicleID))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            // Optional fields, handle non-provision gracefully
            vehicleInfoDetails.TryGetValue("DriverID", out string driverIDString);
            long.TryParse(driverIDString, out long driverID);
            string vehicleMake = vehicleInfoDetails.GetValueOrDefault("VehicleMake", null);
            string vehicleModel = vehicleInfoDetails.GetValueOrDefault("VehicleModel", null);
            string purchaseDateString = vehicleInfoDetails.GetValueOrDefault("PurchaseDate", null);
            long purchaseDateEpoch = 0;
            if (!string.IsNullOrEmpty(purchaseDateString) && long.TryParse(purchaseDateString, out purchaseDateEpoch))
            {
                purchaseDateEpoch = Convert.ToInt64(purchaseDateString);
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO \"VehiclesInformations\" (\"VehicleID\", \"DriverID\", \"VehicleMake\", \"VehicleModel\", \"PurchaseDate\") VALUES (@VehicleID, @DriverID, @VehicleMake, @VehicleModel, @PurchaseDate)", connection))
                {
                    command.Parameters.AddWithValue("@VehicleID", vehicleID);
                    command.Parameters.AddWithValue("@DriverID", driverID != 0 ? driverID : DBNull.Value); // Handle nullable bigint
                    command.Parameters.AddWithValue("@VehicleMake", vehicleMake ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VehicleModel", vehicleModel ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PurchaseDate", purchaseDateEpoch != 0 ? purchaseDateEpoch : DBNull.Value);

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

    public GVAR UpdateVehicleInfo(GVAR gvar)
    {
        var gvarResponse = new GVAR();
        gvarResponse.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            if (!gvar.DicOfDic.TryGetValue("Tags", out var vehicleDetails))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            // Ensure the ID of the VehiclesInformations record is provided
            if (!vehicleDetails.TryGetValue("ID", out string idString) || !long.TryParse(idString, out long id))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();

                // Check if the ID exists
                using (var checkCommand = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM \"VehiclesInformations\" WHERE \"ID\" = @ID)", connection))
                {
                    checkCommand.Parameters.AddWithValue("@ID", id);
                    bool exists = (bool)checkCommand.ExecuteScalar();
                    if (!exists)
                    {
                        gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                        return gvarResponse;
                    }
                }

                var updateParts = new List<string>();
                var parameters = new Dictionary<string, object>();

                // Process each field dynamically, excluding the primary key "ID"
                foreach (var detail in vehicleDetails)
                {
                    if (detail.Key != "ID" && detail.Value != null)  // Exclude the PK from updates
                    {
                        string paramName = "@" + detail.Key;
                        object value = detail.Value;

                        // Handle type conversion for specific fields
                        if ((detail.Key == "VehicleID" || detail.Key == "DriverID" || detail.Key == "PurchaseDate") && long.TryParse(detail.Value, out long longValue))
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
                    string sql = $"UPDATE \"VehiclesInformations\" SET {string.Join(", ", updateParts)} WHERE \"ID\" = @ID";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                        command.Parameters.AddWithValue("@ID", id);

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

    public GVAR GetVehiclesInformations()
    {
        var gvar = new GVAR();
        try
        {
            var dt = new DataTable();
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT \"ID\", \"VehicleID\", \"DriverID\", \"VehicleMake\", \"VehicleModel\", \"PurchaseDate\" FROM \"VehiclesInformations\"", connection))
                using (var reader = command.ExecuteReader())
                {
                    dt.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("ID", typeof(string)),
                        new DataColumn("VehicleID", typeof(string)),
                        new DataColumn("DriverID", typeof(string)),
                        new DataColumn("VehicleMake", typeof(string)),
                        new DataColumn("VehicleModel", typeof(string)),
                        new DataColumn("PurchaseDate", typeof(string))
                    });

                    while (reader.Read())
                    {
                        var row = dt.NewRow();
                        row["ID"] = reader["ID"].ToString();
                        row["VehicleID"] = reader["VehicleID"].ToString();
                        row["DriverID"] = reader["DriverID"].ToString();
                        row["VehicleMake"] = reader["VehicleMake"].ToString();
                        row["VehicleModel"] = reader["VehicleModel"].ToString();

                        // Convert Unix time in milliseconds to DateTime
                        long purchaseDateMillis = Convert.ToInt64(reader["PurchaseDate"]);
                        DateTime purchaseDate = DateTimeOffset.FromUnixTimeMilliseconds(purchaseDateMillis).DateTime;
                        row["PurchaseDate"] = purchaseDate.ToString("o"); // ISO 8601 format

                        dt.Rows.Add(row);
                    }
                }
            }
            gvar.DicOfDT["VehiclesInformations"] = dt;

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
            gvar.DicOfDT["VehiclesInformations"] = new DataTable();  // Ensure structure consistency
        }
        return gvar;
    }

    public GVAR DeleteVehicleInfo(GVAR gvar)
    {
        var gvarResponse = new GVAR();
        gvarResponse.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            if (!gvar.DicOfDic.TryGetValue("Tags", out var vehicleDetails))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            if (!vehicleDetails.TryGetValue("ID", out string idString) || !long.TryParse(idString, out long id))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM \"VehiclesInformations\" WHERE \"ID\" = @ID", connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
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
        catch (Exception ex)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            return gvarResponse;
        }
    }
}
