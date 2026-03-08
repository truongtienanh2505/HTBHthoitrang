namespace Shop.Application.UserAddresses;

public class UserAddressService
{
    private readonly IUserAddressRepository _repository;

    public UserAddressService(IUserAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<object>> GetAddressesAsync(int userId)
    {
        return await _repository.GetAllAsync(userId);
    }

    public async Task CreateAddressAsync(int userId, AddressDto dto)
    {
        await _repository.CreateAsync(userId, dto);
    }

    public async Task<bool> UpdateAddressAsync(int id, int userId, AddressDto dto)
    {
        return await _repository.UpdateAsync(id, userId, dto);
    }

    public async Task<bool> DeleteAddressAsync(int id, int userId)
    {
        return await _repository.DeleteAsync(id, userId);
    }
}