namespace Shop.Application.UserAddresses;

public class AddressDto
{
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string Province { get; set; } = "";
    public string District { get; set; } = "";
    public string Ward { get; set; } = "";
    public bool IsDefault { get; set; }
}