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
using LinqToDB;

namespace Daley_Demo.API.Controllers
{
    [Route("api/devicetypes")]
    public class DeviceTypeController : ApiController
    {
        private readonly string _connectionString;

        public DeviceTypeController()
        {
            // Retrieve the connection string from your config
            _connectionString = ConfigurationManager.ConnectionStrings["IoTDatabase"].ConnectionString;
        }


        // GET: api/devicetypes
        [HttpGet]
        public IHttpActionResult GetDeviceTypes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetAllDeviceTypes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var deviceTypes = new List<object>();
                        while (reader.Read())
                        {
                            deviceTypes.Add(new
                            {
                                DeviceTypeID = reader["DeviceTypeID"],
                                DeviceTypeName = reader["DeviceTypeName"]
                            });
                        }
                        return Ok(deviceTypes);
                    }
                }
            }
        }

        // POST: api/devicetypes
        [HttpPost]
        public IHttpActionResult CreateDeviceType([FromBody] DeviceType DeviceType)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertDeviceType", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DeviceTypeName", DeviceType.DeviceTypeName);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return Ok();
        }
    




    [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateDeviceType(int id, [FromBody] DeviceType DeviceType)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateDeviceType", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DeviceTypeID", id);
                    cmd.Parameters.AddWithValue("@DeviceTypeName", DeviceType.DeviceTypeName);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return Ok();
                }
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteDeviceType(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteDeviceType", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DeviceTypeID", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return Ok();
                }
            }
        }
    }
}
