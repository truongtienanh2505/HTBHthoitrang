namespace Shop.Application.Articles;

public class ArticleService
{
    private readonly IArticleRepository _repository;
    public ArticleService(IArticleRepository repository) => _repository = repository;

    public Task<IEnumerable<object>> GetListAsync() => _repository.GetPublishedArticlesAsync();
    public Task<object?> GetDetailAsync(string slug) => _repository.GetArticleDetailAsync(slug);
}