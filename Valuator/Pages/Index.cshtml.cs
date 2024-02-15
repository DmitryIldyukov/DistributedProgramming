using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _redis;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);
        IDatabase db = _redis.GetDatabase();

        string id = Guid.NewGuid().ToString();

        string similarityKey = "SIMILARITY-" + id;
        var keys = _redis.GetServer(_redis.GetEndPoints().First()).Keys();
        List<string> values = keys.Select(_ => db.StringGet(_).ToString()).ToList();
        int similarityValue = values.Any(_ => _ == text) ? 1 : 0;
        db.StringSet(similarityKey, similarityValue);
        
        string textKey = "TEXT-" + id;
        db.StringSet(textKey, text);
        
        string rankKey = "RANK-" + id;
        double rankValue = (double)text.Count(_ => !char.IsLetter(_))/text.Length;
        db.StringSet(rankKey, rankValue.ToString());

        return Redirect($"summary?id={id}");
    }
}
