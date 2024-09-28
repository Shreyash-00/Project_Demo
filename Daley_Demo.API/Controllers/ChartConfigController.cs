using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Daley_Demo.API.Models;
using Newtonsoft.Json;

namespace Daley_Demo.API.Controllers
{
    [Route("api/chartconfigs")]
    public class ChartConfigController : ApiController
    {
        private readonly string _connectionString;

        public ChartConfigController()
        {
            // Retrieve the connection string from your config
            _connectionString = ConfigurationManager.ConnectionStrings["IoTDatabase"].ConnectionString;
        }

        [HttpGet]
        [Route("api/chartconfigs/{id}")]
        public IHttpActionResult GetChartConfigById(int id)
        {

            // Assuming you have a stored procedure or direct query to get by ID
            var chartConfig = GetChartConfigFromDb(id); // This method should fetch the chart config from the DB
            if (chartConfig == null)
            {
                return NotFound();
            }
            return Ok(chartConfig);
        }

        private object GetChartConfigFromDb(int id)
        {
            // Implement the logic to call the stored procedure for fetching a specific chart config by ID.
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetChartConfigById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DeviceChartConfig
                        {
                            ID = reader.GetInt32(0),
                            ChartHeading = reader.GetString(1),
                            DeviceTypeID = reader.GetInt32(2),
                            ChartType = reader.GetString(3),
                            Fields = JsonConvert.DeserializeObject<List<string>>(reader["Fields"].ToString()),
                            Options = JsonConvert.DeserializeObject<ChartOptions>(reader["ChartOptions"].ToString()),
                            
                            Status = reader.GetInt32(6)
                        };
                    }
                }
            }
            return null;
        }



        // GET: api/chartconfigs
        [HttpGet]
        public IHttpActionResult GetChartConfigs()
        {
            List<DeviceChartConfig> chartConfigs = new List<DeviceChartConfig>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT cc.ID, cc.ChartHeading, dt.DeviceTypeName, cc.ChartType, cc.Fields, cc.ChartOptions, cc.Status
            FROM ChartConfigs cc
            JOIN DeviceTypes dt ON cc.DeviceTypeID = dt.DeviceTypeID", conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        chartConfigs.Add(new DeviceChartConfig
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            ChartHeading = reader["ChartHeading"].ToString(),
                            DeviceTypeName = reader["DeviceTypeName"].ToString(),  // Return DeviceTypeName
                            ChartType = reader["ChartType"].ToString(),
                            Fields = JsonConvert.DeserializeObject<List<string>>(reader["Fields"].ToString()),
                            Options = JsonConvert.DeserializeObject<ChartOptions>(reader["ChartOptions"].ToString()),
                            Status = Convert.ToInt32(reader["Status"])
                        });
                    }
                }
            }

            return Ok(chartConfigs);
        }

        // POST: api/chartconfigs
        [HttpPost]
        public IHttpActionResult CreateChartConfig([FromBody] DeviceChartConfig deviceChartConfig)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Get DeviceTypeID from DeviceTypeName
                int deviceTypeID = GetDeviceTypeIDByName(deviceChartConfig.DeviceTypeName, conn);
                if (deviceTypeID == 0) return BadRequest("Invalid DeviceTypeName");

                using (SqlCommand cmd = new SqlCommand("InsertChartConfig", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChartHeading", deviceChartConfig.ChartHeading);
                    cmd.Parameters.AddWithValue("@DeviceTypeID", deviceTypeID);
                    cmd.Parameters.AddWithValue("@ChartType", deviceChartConfig.ChartType);
                    cmd.Parameters.AddWithValue("@Fields", JsonConvert.SerializeObject(deviceChartConfig.Fields));
                    cmd.Parameters.AddWithValue("@ChartOptions", JsonConvert.SerializeObject(deviceChartConfig.Options));
                    cmd.Parameters.AddWithValue("@Status", deviceChartConfig.Status);

                    cmd.ExecuteNonQuery();
                }
            }

            return Ok();
        }

        // Helper function to get DeviceTypeID from DeviceTypeName
        private int GetDeviceTypeIDByName(string deviceTypeName, SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT DeviceTypeID FROM DeviceTypes WHERE DeviceTypeName = @DeviceTypeName", conn))
            {
                cmd.Parameters.AddWithValue("@DeviceTypeName", deviceTypeName);
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }




        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateChartConfig(int id, [FromBody] DeviceChartConfig deviceChartConfig)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Get DeviceTypeID from DeviceTypeName
                int deviceTypeID = GetDeviceTypeIDByName(deviceChartConfig.DeviceTypeName, conn);
                if (deviceTypeID == 0) return BadRequest("Invalid DeviceTypeName");

                using (SqlCommand cmd = new SqlCommand("UpdateChartConfig", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@ChartHeading", deviceChartConfig.ChartHeading);
                    cmd.Parameters.AddWithValue("@DeviceTypeID", deviceTypeID);  // Use resolved DeviceTypeID
                    cmd.Parameters.AddWithValue("@ChartType", deviceChartConfig.ChartType);
                    cmd.Parameters.AddWithValue("@Fields", JsonConvert.SerializeObject(deviceChartConfig.Fields));
                    cmd.Parameters.AddWithValue("@ChartOptions", JsonConvert.SerializeObject(deviceChartConfig.Options));
                    cmd.Parameters.AddWithValue("@Status", deviceChartConfig.Status);

                    cmd.ExecuteNonQuery();
                }
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteChartConfig(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteChartConfig", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return Ok();
                }
            }
        }
    }


}
