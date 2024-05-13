using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FleetManagementSystem.Services;

public class DataTableConverter : JsonConverter<DataTable>
{
    public override DataTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialization of DataTable is not supported.");
    }

    public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (DataRow row in value.Rows)
        {
            writer.WriteStartObject();
            foreach (DataColumn column in value.Columns)
            {
                writer.WritePropertyName(column.ColumnName);
                JsonSerializer.Serialize(writer, row[column], options);
            }
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
