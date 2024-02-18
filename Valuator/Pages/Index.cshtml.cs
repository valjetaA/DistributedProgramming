using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _connectionMultiplexer = connectionMultiplexer;
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        if (text == null)
        {
            return Redirect("/");
        }
        
        IDatabase db = _connectionMultiplexer.GetDatabase();
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();
        
        string similarityKey = "SIMILARITY-" + id;
        //посчитать similarity и сохранить в БД по ключу similarityKey
        var keys = _connectionMultiplexer.GetServer("localhost:6379").Keys();
        int similarity = 0;
        foreach (var key in keys)
        {
            RedisValue value = db.StringGet(key);
            if (text == value.ToString())
            {
                similarity = 1;
                break;
            }
        }
        db.StringSet(similarityKey, similarity.ToString());

        string textKey = "TEXT-" + id;
        //сохранить в БД text по ключу textKey
        db.StringSet(textKey, text);

        string rankKey = "RANK-" + id;
        //посчитать rank и сохранить в БД по ключу rankKey
        double count = 0;
        foreach (var ch in text)
        {
            if (!char.IsLetter(ch))
            {
                count += 1;
            }
        }

        double result = count / text.Length;
        db.StringSet(rankKey, result.ToString());
        
        return Redirect($"summary?id={id}");
    }
}
