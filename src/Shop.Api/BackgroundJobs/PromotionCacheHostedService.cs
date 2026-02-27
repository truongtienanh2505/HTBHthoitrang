using Microsoft.Extensions.Options;
using Shop.Application.Promotions;

namespace Shop.Api.BackgroundJobs;

public sealed class PromotionCacheHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PromotionCacheHostedService> _logger;
    private readonly PromotionCacheOptions _opt;

    public PromotionCacheHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<PromotionCacheHostedService> logger,
        IOptions<PromotionCacheOptions> opt)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _opt = opt.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_opt.Enabled)
        {
            _logger.LogInformation("PromotionCache: disabled");
            return;
        }

        async Task RunOnce()
        {
            using var scope = _scopeFactory.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<PromotionCacheService>();
            await svc.RebuildAsync(stoppingToken);
        }

        if (_opt.RunOnStartup)
        {
            try { await RunOnce(); }
            catch (Exception ex) { _logger.LogError(ex, "PromotionCache: startup rebuild failed"); }
        }

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Max(10, _opt.IntervalSeconds)));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try { await RunOnce(); }
            catch (Exception ex) { _logger.LogError(ex, "PromotionCache: rebuild failed"); }
        }
    }
}