namespace Shop.Application.UserAddresses;

public interface IUserAddressRepository
{
    Task<IEnumerable<object>> GetAllAsync(int userId);
    Task CreateAsync(int userId, AddressDto dto);
    Task<bool> UpdateAsync(int id, int userId, AddressDto dto);
    Task<bool> DeleteAsync(int id, int userId);
}