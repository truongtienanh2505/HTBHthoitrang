namespace Shop.Application.Articles;

public interface IArticleRepository
{
    Task<IEnumerable<object>> GetPublishedArticlesAsync();
    Task<object?> GetArticleDetailAsync(string slug);
}