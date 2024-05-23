﻿using FleetManagementSystem.Db.Interfaces;
using FleetManagementSystem.Services.Dtos;
using Npgsql;
using System.Collections.Concurrent;
using System.Data;

namespace FleetManagementSystem.Services.Services;

public class VehicleService
{
    private readonly IDatabaseConnection _databaseConnection;

    public VehicleService(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public GVAR GetVehicles()
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
}
