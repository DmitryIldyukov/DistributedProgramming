using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        if (text == null)
            return Redirect($"index");

        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string similarityKey = "SIMILARITY-" + id;
        SetSimilarity(similarityKey, text);
        
        string textKey = "TEXT-" + id;
        _db.StringSet(textKey, text);
        
        string rankKey = "RANK-" + id;
        SetRank(rankKey, text);

        return Redirect($"summary?id={id}");
    }

    private void SetSimilarity(string key, string text)
    {
        var keys = _redis.GetServer(_redis.GetEndPoints().First()).Keys();
        List<string> values = keys.Select(_ => _db.StringGet(_).ToString()).ToList();
        int similarityValue = values.Any(_ => _ == text) ? 1 : 0;
        _db.StringSet(key, similarityValue);
    }

    private void SetRank(string key, string text)
    {
        double rankValue = (double)text.Count(_ => !char.IsLetter(_))/text.Length;
        _db.StringSet(key, rankValue.ToString());
    }
}
