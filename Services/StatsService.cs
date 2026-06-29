using Microsoft.JSInterop;

namespace QuickMath.Services;

public class GameStats
{
    public string Username { get; set; } = "Player";
    public int TotalXp { get; set; }
    public int GamesPlayed { get; set; }
    public int TotalCorrect { get; set; }
    public int TotalWrong { get; set; }
    public int BestScore { get; set; }
    public int BestStreak { get; set; }
}

public class StatsService
{
    const string Key = "quickmath-stats";
    readonly IJSRuntime JS;

    GameStats? _cached;

    public StatsService(IJSRuntime js) => JS = js;

    public async Task<GameStats> GetAsync()
    {
        if (_cached is not null) return _cached;
        try
        {
            var json = await JS.InvokeAsync<string>("__storage.get", new object[] { Key });
            if (!string.IsNullOrEmpty(json))
                _cached = System.Text.Json.JsonSerializer.Deserialize<GameStats>(json);
        }
        catch { }
        _cached ??= new GameStats();
        return _cached;
    }

    public async Task SaveAsync(GameStats stats)
    {
        _cached = stats;
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(stats);
            await JS.InvokeVoidAsync("__storage.set", Key, json);
        }
        catch { }
    }

    public async Task UpdatePartialAsync(int correctInc, int wrongInc, int xpInc, int currentScore, int currentStreak)
    {
        var s = await GetAsync();
        s.TotalCorrect += correctInc;
        s.TotalWrong += wrongInc;
        s.TotalXp += xpInc;
        if (currentScore > s.BestScore) s.BestScore = currentScore;
        if (currentStreak > s.BestStreak) s.BestStreak = currentStreak;
        await SaveAsync(s);
    }

    public async Task IncrementGamesPlayedAsync()
    {
        var s = await GetAsync();
        s.GamesPlayed++;
        await SaveAsync(s);
    }

    public async Task UpdateUsernameAsync(string username)
    {
        var s = await GetAsync();
        s.Username = username;
        await SaveAsync(s);
    }

    public async Task ResetAsync()
    {
        _cached = new GameStats();
        try { await JS.InvokeVoidAsync("__storage.remove", Key); }
        catch { }
    }
}
