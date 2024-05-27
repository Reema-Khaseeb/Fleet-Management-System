using FleetManagementSystem.Db.Interfaces;
using FleetManagementSystem.Services.utils;
using Npgsql;
using System.Collections.Concurrent;

namespace FleetManagementSystem.Services.Services;

public class RouteHistoryService
{
    private readonly IDatabaseConnection _databaseConnection;

    public RouteHistoryService(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }
    public GVAR AddHistoricalData(GVAR gvar)
    {
        var gvarResponse = new GVAR();
        gvarResponse.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        if (!gvar.DicOfDic.TryGetValue("Tags", out var historicalData) ||
            !TryParseHistoricalData(historicalData, out var parsedData) ||
            !VehicleExists(parsedData.VehicleID))
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            return gvarResponse;
        }

        try
        {
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO \"RouteHistory\" (\"VehicleID\", \"VehicleDirection\", \"Status\", \"VehicleSpeed\", \"Epoch\", \"Address\", \"Latitude\", \"Longitude\") VALUES (@VehicleID, @VehicleDirection, @Status, @VehicleSpeed, @Epoch, @Address, @Latitude, @Longitude)", connection))
                {
                    command.Parameters.AddWithValue("@VehicleID", parsedData.VehicleID);
                    command.Parameters.AddWithValue("@VehicleDirection", parsedData.VehicleDirection);
                    command.Parameters.AddWithValue("@Status", parsedData.Status);
                    command.Parameters.AddWithValue("@VehicleSpeed", parsedData.VehicleSpeed);
                    command.Parameters.AddWithValue("@Epoch", parsedData.Epoch);
                    command.Parameters.AddWithValue("@Address", parsedData.Address);
                    command.Parameters.AddWithValue("@Latitude", parsedData.Latitude);
                    command.Parameters.AddWithValue("@Longitude", parsedData.Longitude);
                    command.ExecuteNonQuery();
                }
            }

            gvarResponse.DicOfDic["Tags"]["STS"] = "1";
        }
        catch (Exception)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
        }

        return gvarResponse;
    }

    private bool VehicleExists(long vehicleID)
    {
        using (var connection = _databaseConnection.GetConnection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM \"Vehicles\" WHERE \"VehicleID\" = @VehicleID)", connection))
            {
                command.Parameters.AddWithValue("@VehicleID", vehicleID);
                return (bool)command.ExecuteScalar();
            }
        }
    }

    private bool TryParseHistoricalData(ConcurrentDictionary<string, string> historicalData, out (long VehicleID, int VehicleDirection, string Status, string VehicleSpeed, long Epoch, string Address, float Latitude, float Longitude) parsedData)
    {
        parsedData = default;

        if (!historicalData.TryGetValue("VehicleID", out string vehicleIDString) || !long.TryParse(vehicleIDString, out long vehicleID) ||
            !historicalData.TryGetValue("VehicleDirection", out string vehicleDirectionString) || !int.TryParse(vehicleDirectionString, out int vehicleDirection) ||
            !historicalData.TryGetValue("Status", out string status) ||
            !historicalData.TryGetValue("VehicleSpeed", out string vehicleSpeed) ||
            !historicalData.TryGetValue("Epoch", out string epochString) || !long.TryParse(epochString, out long epoch) ||
            !historicalData.TryGetValue("Address", out string address) ||
            !historicalData.TryGetValue("Latitude", out string latitudeString) || !float.TryParse(latitudeString, out float latitude) ||
            !historicalData.TryGetValue("Longitude", out string longitudeString) || !float.TryParse(longitudeString, out float longitude))
        {
            return false;
        }

        parsedData = (vehicleID, vehicleDirection, status, vehicleSpeed, epoch, address, latitude, longitude);
        return true;
    }
}
