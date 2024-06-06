namespace FleetManagementSystem.Common;

public static class DatabaseConstants
{
    public static class GVARKeys
    {
        public const string Tags = "Tags";
        public const string Status = "STS";
    }

    public static class Tables
    {
        public const string Vehicles = "Vehicles";
        public const string VehiclesInformations = "VehiclesInformations";
        public const string Driver = "Driver";
        public const string RouteHistory = "RouteHistory";
        public const string Geofences = "Geofences";
        public const string CircleGeofence = "CircleGeofence";
        public const string RectangleGeofence = "RectangleGeofence";
        public const string PolygonGeofence = "PolygonGeofence";
    }

    public static class VehiclesFields
    {
        public const string VehicleID = "VehicleID";
        public const string VehicleNumber = "VehicleNumber";
        public const string VehicleType = "VehicleType";
    }

    public static class VehiclesInformationsFields
    {
        public const string ID = "ID";
        public const string VehicleID = "VehicleID";
        public const string DriverID = "DriverID";
        public const string VehicleMake = "VehicleMake";
        public const string VehicleModel = "VehicleModel";
        public const string PurchaseDate = "PurchaseDate";
    }

    public static class DriverFields
    {
        public const string DriverID = "DriverID";
        public const string DriverName = "DriverName";
        public const string PhoneNumber = "PhoneNumber";
    }

    public static class RouteHistoryFields
    {
        public const string RouteHistoryID = "RouteHistoryID";
        public const string VehicleID = "VehicleID";
        public const string VehicleDirection = "VehicleDirection";
        public const string Status = "Status";
        public const string VehicleSpeed = "VehicleSpeed";
        public const string Epoch = "Epoch";
        public const string Address = "Address";
        public const string Latitude = "Latitude";
        public const string Longitude = "Longitude";
    }

    public static class GeofencesFields
    {
        public const string GeofenceID = "GeofenceID";
        public const string GeofenceType = "GeofenceType";
        public const string AddedDate = "AddedDate";
        public const string StrokeColor = "StrokeColor";
        public const string StrokeOpacity = "StrokeOpacity";
        public const string StrokeWeight = "StrokeWeight";
        public const string FillColor = "FillColor";
        public const string FillOpacity = "FillOpacity";
    }

    public static class CircleGeofenceFields
    {
        public const string ID = "ID";
        public const string GeofenceID = "GeofenceID";
        public const string Radius = "Radius";
        public const string Latitude = "Latitude";
        public const string Longitude = "Longitude";
    }

    public static class RectangleGeofenceFields
    {
        public const string ID = "ID";
        public const string GeofenceID = "GeofenceID";
        public const string North = "North";
        public const string East = "East";
        public const string West = "West";
        public const string South = "South";
    }

    public static class PolygonGeofenceFields
    {
        public const string ID = "ID";
        public const string GeofenceID = "GeofenceID";
        public const string Latitude = "Latitude";
        public const string Longitude = "Longitude";
    }
}
