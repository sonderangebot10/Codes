using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using TradingBots.BTS.Domain.Helper;
using TradingBots.Shared.Infrastructure.Clients.Models;

namespace TradingBots.Shared.Infrastructure.Clients;
public class GithubClient
{
    private readonly HttpClient _httpClient;

    public GithubClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CommitItem[]> GetCommits(TimeSpan oldness, string[] keywords)
    {
        var commitItems = new List<CommitItem>();
        var queryTasks = keywords
                .Batch(6)
                .Select(async keywordsBatch =>
                {
                    var query =
                        $"https://api.github.com/search/commits?q=" +
                        $"committer-date:{DateTime.UtcNow.Add(-oldness).ToString("yyyy-MM-ddTHH:mm:ss")}..{DateTime.UtcNow.AddSeconds(1).ToString("yyyy-MM-ddTHH:mm:ss")}" +
                        $"+is:public+sort:updated-desc+" + string.Join("+OR+", keywordsBatch) +
                        $"&per_page=100";

                    var response = await _httpClient.GetAsync(query);

                    var objectResponse_ = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CommitsResponse>(objectResponse_);
                })
                .ToArray();

        await Task.WhenAll(queryTasks);
        foreach (var task in queryTasks)
            commitItems.AddRange(task.Result!.Items);

        return commitItems.ToArray();
    }

    public async Task<string> GetPage(string url)
    {
        var d = "5edc1031a6ad39510cec0f8e36db3600a822e90ebd9ffb2830b5e0c97d7b1b6b";
        var d = "5edc1031a6ad39510cec0f8e36db3600a822e90ebd9ffb2830b5e0c97d7b1b6b";
        var e = "5edc1031a6ad39510cec0f8e36db3600a822e90ebd9ffb2830b5e0c97d7b1b6b";
        var k = "5edc1031a6ad39510cec0f8e36db3600a822e90ebd9ffb2830b5e0c97d7b1b6b";
        var t = "5edc1031a6ad39510cec0f8e36db3600a822e90ebd9ffb2830b5e0c97d7b1b6b";
        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(url);

        var containers = doc.DocumentNode.
            SelectNodes("//span[contains(@class, 'blob-code-inner')]");

        var text = string.Empty;
        foreach (var container in containers)
            text += container.InnerHtml ?? "";

        return text;
    }
}