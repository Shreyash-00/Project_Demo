using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

public class TelemetryController : ApiController
{
    private readonly TelemetryRepository _repository;

    public TelemetryController()
    {
        _repository = new TelemetryRepository();
    }

    [HttpGet]
    [Route("api/telemetry/deviceTypes")]
    public IHttpActionResult GetDeviceTypes()
    {
        var deviceTypes = _repository.GetDeviceTypes();
        return Ok(deviceTypes);
    }

    [HttpPost]
    [Route("api/telemetry/fields")]
    public IHttpActionResult GetFieldsByDeviceType([FromBody] string deviceTypeName)
    {
        if (string.IsNullOrEmpty(deviceTypeName))
            return BadRequest("Device type name cannot be null or empty.");

        var fields = _repository.GetFieldsByDeviceType(deviceTypeName);
        return Ok(fields);
    }

    [HttpPost]
    [Route("api/telemetry/data")]
    public IHttpActionResult GetTelemetryData([FromBody] TelemetryRequest request)
    {
        if (request == null || request.SelectedFields == null )
            return BadRequest("Invalid request.");

        var dataTable = _repository.GetTelemetryData(request.DeviceTypeName, request.SelectedFields);
        var data = dataTable.AsEnumerable().Select(row => request.SelectedFields.ToDictionary(field => field, field => row[field])).ToList();
        return Ok(data);
    }
}

public class TelemetryRequest
{
    public string DeviceTypeName { get; set; }
    public List<string> SelectedFields { get; set; }
}