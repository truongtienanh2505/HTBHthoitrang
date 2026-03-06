namespace Shop.Application.Reviews;

public class ReviewDto
{
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; } = "";
}