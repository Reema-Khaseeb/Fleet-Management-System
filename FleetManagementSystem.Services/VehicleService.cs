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
        }
        return gvar;
    }

    public GVAR AddVehicle(GVAR gvar)
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

            // Convert VehicleNumber to long since the database expects a bigint
            if (!long.TryParse(vehicleDetails["VehicleNumber"], out long vehicleNumber))
            {
                gvarResponse.DicOfDic["Tags"]["STS"] = "0";
                return gvarResponse;
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

            gvarResponse.DicOfDic["Tags"]["STS"] = "1";
            return gvarResponse;
        }
        catch (Exception ex)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            return gvarResponse;
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
                using (var command = new NpgsqlCommand("DELETE FROM \"Vehicles\" WHERE \"VehicleID\" = @VehicleID", connection))
                {
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
        catch (Exception ex)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
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
                return gvarResponse;
            }
        }
        catch (Exception ex)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            return gvarResponse;
        }
    }
}
