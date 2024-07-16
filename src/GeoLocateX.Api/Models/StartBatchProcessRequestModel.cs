using GeoLocateX.Api.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GeoLocateX.Api.Models;

public class StartBatchProcessRequestModel
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one IP address is required.")]
    [IpAddressList(ErrorMessage = "One or more IP addresses are invalid.")]
    public IEnumerable<string> IpAddresses { get; set; }
}
