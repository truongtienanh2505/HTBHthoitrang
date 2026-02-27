namespace Shop.Api.BackgroundJobs;

public sealed class PromotionCacheOptions
{
    public bool Enabled { get; set; } = true;
    public bool RunOnStartup { get; set; } = true;
    public int IntervalSeconds { get; set; } = 300;
}