namespace Shop.Application.Banners;
public interface IBannerRepository { Task<IEnumerable<object>> GetActiveBannersAsync(); }