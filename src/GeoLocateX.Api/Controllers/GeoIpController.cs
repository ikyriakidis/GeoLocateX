using GeoLocateX.Api.Models;
using GeoLocateX.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GeoLocateX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class GeoIpController : ControllerBase
{
    private readonly IGeoIpService _geoIpService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoIpController"/> class.
    /// </summary>
    /// <param name="geoIpService">The GeoIP service.</param>
    public GeoIpController(
        IGeoIpService geoIpService)
    {
        _geoIpService = geoIpService;
    }

    /// <summary>
    /// Gets GeoIP information for a specific IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to look up.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Returns GeoIP information.</returns>
    [HttpGet("{ipAddress}")]
    [ProducesResponseType(typeof(GeoIpResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetGeoIp([FromRoute] string ipAddress, CancellationToken cancellationToken)
    {
        if (!IPAddress.TryParse(ipAddress, out var ip))
        {
            ModelState.AddModelError("IpAddress", "Invalid IP address format.");
            return BadRequest(ModelState);
        }

        var result = await _geoIpService.GetGeoIPAsync(ipAddress, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Starts a batch process for a list of IP addresses.
    /// </summary>
    /// <param name="requestModel">The request model containing IP addresses.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Returns the result of the batch process start.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> StartBatchProcess(
        [FromBody] StartBatchProcessRequestModel requestModel,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _geoIpService.StartBatchProcess(requestModel.IpAddresses, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets the status of a specific batch process.
    /// </summary>
    /// <param name="batchId">The ID of the batch process.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Returns the status of the batch process.</returns>
    [HttpGet("status/{batchId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBatchStatus(Guid batchId, CancellationToken cancellationToken)
    {
        var result = await _geoIpService.GetBatchStatus(batchId, cancellationToken);

        return Ok(result);
    }
}
