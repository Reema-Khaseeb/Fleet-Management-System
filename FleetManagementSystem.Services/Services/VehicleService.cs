using FleetManagementSystem.Db.Interfaces;
using FleetManagementSystem.Services.Interfaces;
using FleetManagementSystem.Services.utils;
using Npgsql;
using System.Collections.Concurrent;
using System.Data;
using Serilog;

namespace FleetManagementSystem.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDatabaseConnection _databaseConnection;
    private readonly ILogger _logger;

    public VehicleService(IDatabaseConnection databaseConnection,
        ILogger logger, IVehicleRepository vehicleRepository)
    {
        _databaseConnection = databaseConnection;
        _logger = logger;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<GVAR> AddVehicleAsync(GVAR gvar, CancellationToken cancellationToken)
    {
        var gvarResponse = GVARUtility.InitializeGVARResponse();

        try
        {
            if (!TryGetVehicleDetails(gvar, gvarResponse, out var vehicleDetails))
            {
                _logger.Error("Failed to get vehicle details");
                return gvarResponse;
            }

            if (!TryGetVehicleNumber(vehicleDetails, out long vehicleNumber, gvarResponse))
            {
                return gvarResponse;
            }

            var vehicleType = vehicleDetails["VehicleType"];
            await _vehicleRepository.AddVehicleAsync(vehicleNumber, vehicleType, cancellationToken);

            gvarResponse.DicOfDic["Tags"]["STS"] = "1";
            _logger.Information($"Successfully added vehicle with number: {vehicleNumber}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while adding vehicle");
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
        }

            return gvarResponse;
        }

    public async Task<GVAR> DeleteVehicleAsync(GVAR gvar, CancellationToken cancellationToken)
    {
        var gvarResponse = GVARUtility.InitializeGVARResponse();

        try
        {
            if (!TryGetVehicleDetails(gvar, gvarResponse, out var vehicleDetails))
            {
                _logger.Error("Failed to get vehicle details");
                return gvarResponse;
            }

            if (!TryGetVehicleId(vehicleDetails, out long vehicleId, gvarResponse))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            await _vehicleRepository.DeleteVehicleAsync(vehicleId, cancellationToken);

            gvarResponse.DicOfDic["Tags"]["STS"] = "1";
            _logger.Information($"Successfully deleted vehicle with ID: {vehicleId}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while deleting vehicle");
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
        }

            return gvarResponse;
        }
    }

    public GVAR UpdateVehicle(GVAR gvar)
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

            if (!vehicleDetails.TryGetValue("VehicleID", out string vehicleIDString) || !long.TryParse(vehicleIDString, out long vehicleID))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();

                // Check if the VehicleID exists
                using (var checkCommand = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM \"Vehicles\" WHERE \"VehicleID\" = @VehicleID)", connection))
                {
                    checkCommand.Parameters.AddWithValue("@VehicleID", vehicleID);
                    bool exists = (bool)checkCommand.ExecuteScalar();
                    if (!exists)
                    {
                        gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                        return gvarResponse;
                    }
                }

                var updateParts = new List<string>();
                var parameters = new Dictionary<string, object>();

                // Process each field dynamically, excluding the primary key "VehicleID"
                foreach (var detail in vehicleDetails)
                {
                    if (detail.Key != "VehicleID" && detail.Value != null)
                    {
                        string paramName = "@" + detail.Key;
                        object value = detail.Value;

                        // Handle type conversion for VehicleNumber and check for uniqueness
                        if (detail.Key == "VehicleNumber" && long.TryParse(detail.Value, out long newVehicleNumber))
                        {
                            // Check for unique VehicleNumber if provided
                            using (var checkCommand = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM \"Vehicles\" WHERE \"VehicleNumber\" = @NewVehicleNumber AND \"VehicleID\" <> @VehicleID)", connection))
                            {
                                checkCommand.Parameters.AddWithValue("@NewVehicleNumber", newVehicleNumber);
                                checkCommand.Parameters.AddWithValue("@VehicleID", vehicleID);
                                bool exists = (bool)checkCommand.ExecuteScalar();
                                if (exists)
                                {
                                    gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                                    return gvarResponse;
                                }
                            }

                            value = newVehicleNumber;
                        }

                        updateParts.Add($"\"{detail.Key}\" = {paramName}");
                        parameters[paramName] = value;
                    }
                }

                // Only proceed if there are fields to update
                if (updateParts.Count > 0)
                {
                    string sql = $"UPDATE \"Vehicles\" SET {string.Join(", ", updateParts)} WHERE \"VehicleID\" = @VehicleID";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                        command.Parameters.AddWithValue("@VehicleID", vehicleID);

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

    public GVAR GetVehicleDetails(long vehicleID)
    {
        var gvar = new GVAR();
        gvar.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(@"
                    SELECT 
                        V.""VehicleID"",
                        V.""VehicleNumber"",
                        V.""VehicleType"",
                        D.""DriverName"",
                        D.""PhoneNumber"",
                        VI.""VehicleMake"",
                        VI.""VehicleModel"",
                        RH.""Latitude"" || ', ' || RH.""Longitude"" AS ""LastPosition"",
                        RH.""Epoch"" AS ""LastGPSTime"",
                        RH.""VehicleSpeed"" AS ""LastGPSSpeed"",
                        RH.""Address"" AS ""LastAddress""
                    FROM 
                        ""Vehicles"" V
                    LEFT JOIN 
                        ""VehiclesInformations"" VI ON V.""VehicleID"" = VI.""VehicleID""
                    LEFT JOIN 
                        ""Driver"" D ON VI.""DriverID"" = D.""DriverID""
                    LEFT JOIN 
                        (
                            SELECT 
                                ""VehicleID"",
                                ""Latitude"",
                                ""Longitude"",
                                ""Epoch"",
                                ""VehicleSpeed"",
                                ""Address""
                            FROM 
                                ""RouteHistory""
                            WHERE 
                                ""VehicleID"" = @VehicleID
                            ORDER BY 
                                ""Epoch"" DESC
                            LIMIT 1
                        ) RH ON V.""VehicleID"" = RH.""VehicleID""
                    WHERE
                        V.""VehicleID"" = @VehicleID;", connection))
                {
                    command.Parameters.AddWithValue("@VehicleID", vehicleID);

                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Columns.AddRange(new DataColumn[]
                        {
                            new DataColumn("VehicleID", typeof(long)),
                            new DataColumn("VehicleNumber", typeof(long)),
                            new DataColumn("VehicleType", typeof(string)),
                            new DataColumn("DriverName", typeof(string)),
                            new DataColumn("PhoneNumber", typeof(long)),
                            new DataColumn("VehicleMake", typeof(string)),
                            new DataColumn("VehicleModel", typeof(string)),
                            new DataColumn("LastPosition", typeof(string)),
                            new DataColumn("LastGPSTime", typeof(long)),
                            new DataColumn("LastGPSSpeed", typeof(string)),
                            new DataColumn("LastAddress", typeof(string))
                        });

                        if (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["VehicleID"] = reader["VehicleID"];
                            row["VehicleNumber"] = reader["VehicleNumber"];
                            row["VehicleType"] = reader["VehicleType"];
                            row["DriverName"] = reader["DriverName"]?.ToString();
                            row["PhoneNumber"] = reader["PhoneNumber"];
                            row["VehicleMake"] = reader["VehicleMake"]?.ToString();
                            row["VehicleModel"] = reader["VehicleModel"]?.ToString();
                            row["LastPosition"] = reader["LastPosition"]?.ToString();
                            row["LastGPSTime"] = reader["LastGPSTime"];
                            row["LastGPSSpeed"] = reader["LastGPSSpeed"]?.ToString();
                            row["LastAddress"] = reader["LastAddress"]?.ToString();
                            dataTable.Rows.Add(row);

                            gvar.DicOfDT["VehicleInformation"] = dataTable;
                            gvar.DicOfDic["Tags"]["STS"] = "1"; // Indicate success
                        }
                        else
                        {
                            gvar.DicOfDic["Tags"]["STS"] = "0"; // Indicate failure
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            gvar.DicOfDic["Tags"]["STS"] = "0"; // Indicate failure
        }

        return gvar;
    }

    public GVAR GetVehicles()
    {
        var gvar = new GVAR();
        gvar.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(@"
                        WITH LatestHistory AS (
                            SELECT 
                                ""VehicleID"",
                                ""VehicleDirection"",
                                ""Status"",
                                ""Address"",
                                ""Latitude"",
                                ""Longitude"",
                                ROW_NUMBER() OVER (PARTITION BY ""VehicleID"" ORDER BY ""Epoch"" DESC) AS rn
                            FROM 
                                ""RouteHistory""
                        )
                        SELECT 
                            V.""VehicleID"",
                            V.""VehicleNumber"",
                            V.""VehicleType"",
                            LH.""VehicleDirection"" AS ""LastDirection"",
                            LH.""Status"" AS ""LastStatus"",
                            LH.""Address"" AS ""LastAddress"",
                            LH.""Latitude"" AS ""LastLatitude"",
                            LH.""Longitude"" AS ""LastLongitude""
                        FROM 
                            ""Vehicles"" V
                        LEFT JOIN 
                            LatestHistory LH ON V.""VehicleID"" = LH.""VehicleID"" AND LH.rn = 1;", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Columns.AddRange(new DataColumn[]
                        {
                                new DataColumn("VehicleID", typeof(long)),
                                new DataColumn("VehicleNumber", typeof(long)),
                                new DataColumn("VehicleType", typeof(string)),
                                new DataColumn("LastDirection", typeof(int)),
                                new DataColumn("LastStatus", typeof(string)),
                                new DataColumn("LastAddress", typeof(string)),
                                new DataColumn("LastLatitude", typeof(float)),
                                new DataColumn("LastLongitude", typeof(float))
                        });

                        while (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["VehicleID"] = reader["VehicleID"];
                            row["VehicleNumber"] = reader["VehicleNumber"];
                            row["VehicleType"] = reader["VehicleType"];
                            row["LastDirection"] = reader["LastDirection"] == DBNull.Value ? (object)DBNull.Value : (int)reader["LastDirection"];
                            row["LastStatus"] = reader["LastStatus"]?.ToString();
                            row["LastAddress"] = reader["LastAddress"]?.ToString();
                            row["LastLatitude"] = reader["LastLatitude"] == DBNull.Value ? (object)DBNull.Value : (float)reader["LastLatitude"];
                            row["LastLongitude"] = reader["LastLongitude"] == DBNull.Value ? (object)DBNull.Value : (float)reader["LastLongitude"];
                            dataTable.Rows.Add(row);
                        }

                        gvar.DicOfDT["Vehicles"] = dataTable;
                        gvar.DicOfDic["Tags"]["STS"] = "1"; // Indicate success
                    }
                }
            }
        }
        catch (Exception)
        {
            gvar.DicOfDic["Tags"]["STS"] = "0"; // Indicate failure
        }

        return gvar;
    }

    public GVAR GetVehicleRouteHistory(long vehicleID, long startEpoch, long endEpoch)
    {
        var gvar = new GVAR();
        gvar.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(@"
                        SELECT 
                            V.""VehicleID"",
                            V.""VehicleNumber"",
                            RH.""Address"",
                            RH.""Status"",
                            RH.""Latitude"",
                            RH.""Longitude"",
                            RH.""VehicleDirection"",
                            RH.""VehicleSpeed"" AS ""GPSSpeed"",
                            RH.""Epoch"" AS ""GPSTime""
                        FROM 
                            ""Vehicles"" V
                        JOIN 
                            ""RouteHistory"" RH ON V.""VehicleID"" = RH.""VehicleID""
                        WHERE 
                            V.""VehicleID"" = @VehicleID AND
                            RH.""Epoch"" BETWEEN @StartEpoch AND @EndEpoch
                        ORDER BY 
                            RH.""Epoch"" ASC;", connection))
                {
                    command.Parameters.AddWithValue("@VehicleID", vehicleID);
                    command.Parameters.AddWithValue("@StartEpoch", startEpoch);
                    command.Parameters.AddWithValue("@EndEpoch", endEpoch);

                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Columns.AddRange(new DataColumn[]
                        {
                                new DataColumn("VehicleID", typeof(long)),
                                new DataColumn("VehicleNumber", typeof(long)),
                                new DataColumn("Address", typeof(string)),
                                new DataColumn("Status", typeof(string)),
                                new DataColumn("Latitude", typeof(float)),
                                new DataColumn("Longitude", typeof(float)),
                                new DataColumn("VehicleDirection", typeof(int)),
                                new DataColumn("GPSSpeed", typeof(string)),
                                new DataColumn("GPSTime", typeof(long))
                        });

                        while (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["VehicleID"] = reader["VehicleID"];
                            row["VehicleNumber"] = reader["VehicleNumber"];
                            row["Address"] = reader["Address"]?.ToString();
                            row["Status"] = reader["Status"]?.ToString();
                            row["Latitude"] = reader["Latitude"] == DBNull.Value ? (object)DBNull.Value : (float)reader["Latitude"];
                            row["Longitude"] = reader["Longitude"] == DBNull.Value ? (object)DBNull.Value : (float)reader["Longitude"];
                            row["VehicleDirection"] = reader["VehicleDirection"] == DBNull.Value ? (object)DBNull.Value : (int)reader["VehicleDirection"];
                            row["GPSSpeed"] = reader["GPSSpeed"]?.ToString();
                            row["GPSTime"] = reader["GPSTime"];
                            dataTable.Rows.Add(row);
                        }

                        gvar.DicOfDT["RouteHistory"] = dataTable;
                        gvar.DicOfDic["Tags"]["STS"] = "1"; // Indicate success
                    }
                }
            }
        }
        catch (Exception)
        {
            gvar.DicOfDic["Tags"]["STS"] = "0"; // Indicate failure

    private bool TryGetVehicleDetails(GVAR gvar, GVAR gvarResponse, 
        out ConcurrentDictionary<string, string> vehicleDetails)
    {
        if (!gvar.DicOfDic.TryGetValue("Tags", out vehicleDetails))
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            _logger.Error("Failed to get vehicle details");
            return false;
        }
        return true;
    }

    private bool TryGetVehicleNumber(ConcurrentDictionary<string, string> vehicleDetails, 
        out long vehicleNumber, GVAR gvarResponse)
    {
        if (!long.TryParse(vehicleDetails["VehicleNumber"], out vehicleNumber))
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            _logger.Error($"Failed to parse vehicle number: {vehicleDetails["VehicleNumber"]}");
            return false;
        }
        return true;
        }

    private bool TryGetVehicleId(ConcurrentDictionary<string, string> vehicleDetails, out long vehicleId, GVAR gvarResponse)
    {
        vehicleId = 0;
        if (!vehicleDetails.TryGetValue("VehicleID", out string vehicleIdString) || !long.TryParse(vehicleIdString, out vehicleId))
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            _logger.Error($"Failed to parse vehicle ID: {vehicleIdString}");
            return false;
        }
        return true;
    }
}
