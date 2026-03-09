namespace Shop.Application.Banners;
public class BannerService
{
    private readonly IBannerRepository _repo;
    public BannerService(IBannerRepository repo) => _repo = repo;
    public Task<IEnumerable<object>> GetBannersAsync() => _repo.GetActiveBannersAsync();
}