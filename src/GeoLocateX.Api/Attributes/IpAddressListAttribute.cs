using System.ComponentModel.DataAnnotations;
using System.Net;

namespace GeoLocateX.Api.Attributes;

public class IpAddressListAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is IEnumerable<string> ipAddresses)
        {
            foreach (var ipAddress in ipAddresses)
            {
                if (!IPAddress.TryParse(ipAddress, out _))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }

        return new ValidationResult("Invalid IP address list.");
    }
}
