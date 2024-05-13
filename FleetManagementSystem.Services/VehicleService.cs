using FleetManagementSystem.Db.Interfaces;
using FleetManagementSystem.Services.Dtos;
using Npgsql;
using System.Collections.Concurrent;
using System.Data;

namespace FleetManagementSystem.Services;

public class VehicleService
{
    private readonly IDatabaseConnection _databaseConnection;

    public VehicleService(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public GVAR GetVehiclesGvar()
    {
        var gvar = new GVAR();
        try
        {
            var dt = new DataTable();
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT \"VehicleID\", \"VehicleNumber\", \"VehicleType\" FROM \"Vehicles\"", connection))
                using (var reader = command.ExecuteReader())
                {
                    var vehicleIdColumn = new DataColumn("VehicleID", typeof(string));
                    var vehicleNumberColumn = new DataColumn("VehicleNumber", typeof(string));
                    var vehicleTypeColumn = new DataColumn("VehicleType", typeof(string));
                    dt.Columns.AddRange(new DataColumn[] { vehicleIdColumn, vehicleNumberColumn, vehicleTypeColumn });

                    while (reader.Read())
                    {
                        var row = dt.NewRow();
                        row[vehicleIdColumn] = reader["VehicleID"].ToString();
                        row[vehicleNumberColumn] = reader["VehicleNumber"].ToString();
                        row[vehicleTypeColumn] = reader["VehicleType"].ToString();
                        dt.Rows.Add(row);
                    }
                }
            }
            gvar.DicOfDT["Vehicles"] = dt;

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
            gvar.DicOfDT["Vehicles"] = new DataTable();  // Ensure structure consistency
                                                         // Consider logging the exception here
        }
        return gvar;
    }

    public ResponseResult AddVehicle(GVAR gvar)
    {
        try
        {
            if (!gvar.DicOfDic.TryGetValue("Tags", out var vehicleDetails))
            {
                return new ResponseResult(Success: false, Message: "Vehicle details are missing.");
            }

            // Convert VehicleNumber to long since the database expects a bigint
            if (!long.TryParse(vehicleDetails["VehicleNumber"], out long vehicleNumber))
            {
                return new ResponseResult(Success: false, Message: "VehicleNumber is invalid or not present.");
            }

            string vehicleType = vehicleDetails["VehicleType"];

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO \"Vehicles\" (\"VehicleNumber\", \"VehicleType\") VALUES (@VehicleNumber, @VehicleType)", connection))
                {
                    command.Parameters.AddWithValue("@VehicleNumber", vehicleNumber);
                    command.Parameters.AddWithValue("@VehicleType", vehicleType);
                    command.ExecuteNonQuery();
                }
            }

            return new ResponseResult(Success: true, Message: "Vehicle added successfully.");
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Error adding vehicle.");
            return new ResponseResult(Success: false, Message: $"Error adding vehicle: {ex.Message}");
        }
    }

    public GVAR DeleteVehicle(GVAR gvar)
    {
        var gvarResponse = new GVAR();
        gvarResponse.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        try
        {
            if (!gvar.DicOfDic.TryGetValue("Tags", out var vehicleDetails))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                //_logger.LogError("Tags not provided in the request.");
                return gvarResponse;
            }

            if (!vehicleDetails.TryGetValue("VehicleID", out string vehicleIDString) || !long.TryParse(vehicleIDString, out long vehicleID))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                //_logger.LogError("VehicleID is either not provided, is invalid, or could not be parsed as long.");
                return gvarResponse;
            }

            //_logger.LogInformation($"Attempting to delete vehicle with ID: {vehicleID}");

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM \"Vehicles\" WHERE \"VehicleID\" = @VehicleID", connection))
                {
                    command.Parameters.AddWithValue("@VehicleID", vehicleID);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                        //_logger.LogWarning($"No vehicle found with ID: {vehicleID}");
                        return gvarResponse;
                    }
                }
            }

            gvarResponse.DicOfDic["Tags"]["STS"] = "1";
            //_logger.LogInformation($"Vehicle with ID: {vehicleID} deleted successfully.");
            return gvarResponse;
        }
        catch (Exception ex)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            //_logger.LogError(ex, "Error while deleting vehicle.");
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
                //_logger.LogError("Vehicle details are missing.");
                return gvarResponse;
            }

            if (!vehicleDetails.TryGetValue("VehicleID", out string vehicleIDString) || !long.TryParse(vehicleIDString, out long vehicleID))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                //_logger.LogError("VehicleID is invalid or not present.");
                return gvarResponse;
            }

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();

                // Check for unique VehicleNumber if provided and parse it
                if (vehicleDetails.TryGetValue("VehicleNumber", out string newVehicleNumberString) && long.TryParse(newVehicleNumberString, out long newVehicleNumber))
                {
                    using (var checkCommand = new NpgsqlCommand("SELECT COUNT(*) FROM \"Vehicles\" WHERE \"VehicleNumber\" = @NewVehicleNumber AND \"VehicleID\" <> @VehicleID", connection))
                    {
                        checkCommand.Parameters.AddWithValue("@NewVehicleNumber", newVehicleNumber);
                        checkCommand.Parameters.AddWithValue("@VehicleID", vehicleID);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                        if (count > 0)
                        {
                            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                            //_logger.LogError("New VehicleNumber is not unique.");
                            return gvarResponse;
                        }
                    }

                    // Update VehicleNumber if provided and unique
                    using (var updateCommand = new NpgsqlCommand("UPDATE \"Vehicles\" SET \"VehicleNumber\" = @VehicleNumber WHERE \"VehicleID\" = @VehicleID", connection))
                    {
                        updateCommand.Parameters.AddWithValue("@VehicleNumber", newVehicleNumber);
                        updateCommand.Parameters.AddWithValue("@VehicleID", vehicleID);
                        updateCommand.ExecuteNonQuery();
                    }
                }

                // Update VehicleType if provided
                if (vehicleDetails.TryGetValue("VehicleType", out string vehicleType))
                {
                    using (var updateCommand = new NpgsqlCommand("UPDATE \"Vehicles\" SET \"VehicleType\" = @VehicleType WHERE \"VehicleID\" = @VehicleID", connection))
                    {
                        updateCommand.Parameters.AddWithValue("@VehicleType", vehicleType ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@VehicleID", vehicleID);
                        updateCommand.ExecuteNonQuery();
                    }
                }

                gvarResponse.DicOfDic["Tags"]["STS"] = "1";
                //_logger.LogInformation($"Vehicle with ID: {vehicleID} updated successfully.");
                return gvarResponse;
            }
        }
        catch (Exception ex)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            //_logger.LogError(ex, "Error updating vehicle.");
            return gvarResponse;
        }
    }
}
