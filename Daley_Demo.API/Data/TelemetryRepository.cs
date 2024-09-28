using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;

public class TelemetryRepository
{
    private readonly string _connectionString;

    public TelemetryRepository()
    {
        // Retrieve the connection string from your config
        _connectionString = ConfigurationManager.ConnectionStrings["IoTDatabase"].ConnectionString;
    }

    // Method to get all DeviceTypeNames
    public List<string> GetDeviceTypes()
    {
        var deviceTypes = new List<string>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT DeviceTypeName FROM DeviceTypes", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deviceTypes.Add(reader.GetString(0));
                    }
                }
            }
        }

        return deviceTypes;
    }

    // Method to get fields based on DeviceTypeName
    public List<string> GetFieldsByDeviceType(string deviceTypeName)
    {
        var fields = new List<string>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand("GetFieldsByDeviceType", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@DeviceTypeName", deviceTypeName);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fields.Add(reader.GetString(0)); // Adjust based on your SP output
                    }
                }
            }
        }

        return fields;
    }

    // Method to get telemetry data based on DeviceTypeName and selected fields
    public DataTable GetTelemetryData(string deviceTypeName, List<string> selectedFields)
    {
        string fieldQuery = string.Join(", ", selectedFields.Select(f => $"JSON_VALUE(Payload, '$.{f}') AS {f}"));

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand($@"
                SELECT Timestamp, {fieldQuery}
                FROM TelemetryData
                WHERE DeviceTypeID = (SELECT DeviceTypeID FROM DeviceTypes WHERE DeviceTypeName = @DeviceTypeName)
                ORDER BY Timestamp", connection))
            {
                command.Parameters.AddWithValue("@DeviceTypeName", deviceTypeName);
                var adapter = new SqlDataAdapter(command);
                var resultTable = new DataTable();
                adapter.Fill(resultTable);

                return resultTable;
            }
        }
    }
}

