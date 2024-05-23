using FleetManagementSystem.Db.Interfaces;
using FleetManagementSystem.Services.utils;
using Npgsql;
using System.Collections.Concurrent;
using System.Data;

namespace FleetManagementSystem.Services.Services;
public class GeofenceService
{
    private readonly IDatabaseConnection _databaseConnection;

    public GeofenceService(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public GVAR GetAllGeofences()
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
                            ""GeofenceID"",
                            ""GeofenceType"",
                            ""AddedDate"",
                            ""StrockColor"" AS ""StrokeColor"",
                            ""StrockOpacity"" AS ""StrokeOpacity"",
                            ""StrockWeight"" AS ""StrokeWeight"",
                            ""FillColor"",
                            ""FillOpacity""
                        FROM 
                            ""Geofences"";", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Columns.AddRange(new DataColumn[]
                        {
                            new DataColumn("GeofenceID", typeof(long)),
                            new DataColumn("GeofenceType", typeof(string)),
                            new DataColumn("AddedDate", typeof(long)),
                            new DataColumn("StrokeColor", typeof(string)),
                            new DataColumn("StrokeOpacity", typeof(float)),
                            new DataColumn("StrokeWeight", typeof(float)),
                            new DataColumn("FillColor", typeof(string)),
                            new DataColumn("FillOpacity", typeof(float))
                        });

                        while (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["GeofenceID"] = reader["GeofenceID"];
                            row["GeofenceType"] = reader["GeofenceType"];
                            row["AddedDate"] = reader["AddedDate"];
                            row["StrokeColor"] = reader["StrokeColor"];
                            row["StrokeOpacity"] = reader["StrokeOpacity"];
                            row["StrokeWeight"] = reader["StrokeWeight"];
                            row["FillColor"] = reader["FillColor"];
                            row["FillOpacity"] = reader["FillOpacity"];
                            dataTable.Rows.Add(row);
                        }

                        gvar.DicOfDT["Geofences"] = dataTable;
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

    public GVAR GetCircularGeofences()
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
                            ""GeofenceID"",
                            ""Radius"",
                            ""Latitude"",
                            ""Longitude""
                        FROM 
                            ""CircleGeofence"";", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Columns.AddRange(new DataColumn[]
                        {
                                new DataColumn("GeofenceID", typeof(long)),
                                new DataColumn("Radius", typeof(long)),
                                new DataColumn("Latitude", typeof(float)),
                                new DataColumn("Longitude", typeof(float))
                        });

                        while (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["GeofenceID"] = reader["GeofenceID"];
                            row["Radius"] = reader["Radius"];
                            row["Latitude"] = reader["Latitude"];
                            row["Longitude"] = reader["Longitude"];
                            dataTable.Rows.Add(row);
                        }

                        gvar.DicOfDT["CircularGeofences"] = dataTable;
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

    public GVAR GetRectangularGeofences()
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
                            ""GeofenceID"",
                            ""North"",
                            ""East"",
                            ""West"",
                            ""South""
                        FROM 
                            ""RectangleGeofence"";", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Columns.AddRange(new DataColumn[]
                        {
                                new DataColumn("GeofenceID", typeof(long)),
                                new DataColumn("North", typeof(float)),
                                new DataColumn("East", typeof(float)),
                                new DataColumn("West", typeof(float)),
                                new DataColumn("South", typeof(float))
                        });

                        while (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["GeofenceID"] = reader["GeofenceID"];
                            row["North"] = reader["North"];
                            row["East"] = reader["East"];
                            row["West"] = reader["West"];
                            row["South"] = reader["South"];
                            dataTable.Rows.Add(row);
                        }

                        gvar.DicOfDT["RectangularGeofences"] = dataTable;
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

    public GVAR GetPolygonGeofences()
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
                            ""GeofenceID"",
                            ""Latitude"",
                            ""Longitude""
                        FROM 
                            ""PolygonGeofence"";", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Columns.AddRange(new DataColumn[]
                        {
                                new DataColumn("GeofenceID", typeof(long)),
                                new DataColumn("Latitude", typeof(float)),
                                new DataColumn("Longitude", typeof(float))
                        });

                        while (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["GeofenceID"] = reader["GeofenceID"];
                            row["Latitude"] = reader["Latitude"];
                            row["Longitude"] = reader["Longitude"];
                            dataTable.Rows.Add(row);
                        }

                        gvar.DicOfDT["PolygonGeofences"] = dataTable;
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
}

