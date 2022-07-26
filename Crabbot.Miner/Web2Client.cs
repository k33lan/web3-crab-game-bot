using System.Net;
using Newtonsoft.Json;

namespace Crabbot.Miner;
using Microsoft.AspNetCore.WebUtilities;

public class Web2Client
{
    private ConfigOptions _options = null;
    public Web2Client(ConfigOptions options)
    {
        _options = options;
    }
    public async Task<List<Mine>> getMines()
    {
        try
        {
            List<Mine> myMines = null;
            string url = _options.Web2BaseUrl + "/mines";
        
            var query = new Dictionary<string, string>()
            {
                ["user_address"] = _options.UserAddress.ToLower(),
                ["page"] = "1",
                ["status"] = "open",
                ["limit"] = "6"
            };
        
            var uri = QueryHelpers.AddQueryString(url, query);
        
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Other");
            var task = client.GetAsync(uri)
                .ContinueWith((taskwithresponse) =>
                {
                    var response = taskwithresponse.Result.Content.ReadAsStringAsync();
                    response.Wait();
                    MineResponseRoot serializedRes = JsonConvert.DeserializeObject<MineResponseRoot>(response.Result);

                    if (serializedRes!=null)
                    {
                        if (serializedRes.result!=null
                            &&serializedRes.result.data!=null
                            &&serializedRes.result.data.Count() > 0)
                        {
                            myMines = serializedRes.result.data;
                        }
                        else
                        {
                            Console.WriteLine(DateTime.Now + " " + serializedRes.error_code + " " +  serializedRes.message);
                        }
                    }
                });
            task.Wait();

            return myMines;
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e);
            return null;
        }
    }
    public async Task<List<Team>> getTeams()
    {
        try
        {
            List<Team> myTeams = null;
            string url =  _options.Web2BaseUrl + "/teams";
        
            var query = new Dictionary<string, string>()
            {
                ["user_address"] =  _options.UserAddress.ToLower(),
                ["page"] = "1",
                ["is_team_available"] = "1",
                ["limit"] = "6"
            };
        
            var uri = QueryHelpers.AddQueryString(url, query);
        
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Other");
            var task = client.GetAsync(uri)
                .ContinueWith((taskwithresponse) =>
                {
                    var response = taskwithresponse.Result.Content.ReadAsStringAsync();
                    response.Wait();
                
                    TeamResponseRoot serializedRes = JsonConvert.DeserializeObject<TeamResponseRoot>(response.Result);
                
                    if (serializedRes!=null)
                    {
                        if (serializedRes.result!=null
                            &&serializedRes.result.data!=null
                            &&serializedRes.result.data.Count() > 0)
                        {
                            myTeams = serializedRes.result.data;
                        }
                        else
                        {
                            Console.WriteLine(DateTime.Now + " " + serializedRes.error_code + " " +  serializedRes.message);
                        }
                    }
                });
            task.Wait();

            // only available teams setup in config
            List<Team> availableTeams = new List<Team>();
            if (myTeams != null)
            {
                foreach (var team in myTeams)
                {
                    if (_options.TeamIds.Contains(team.team_id))
                        availableTeams.Add(team);
                }

                return availableTeams;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e);
        }

        return null;
    }

    public async Task<Mine> findSomethingToLoot()
    {
        Mine targetMine = null;
        List<Mine> mines = null;
        string url = _options.Web2BaseUrl + "/mines";
        
        var query = new Dictionary<string, string>()
        {
            ["page"] = "1",
            ["status"] = "open",
            ["looter_address"] = _options.UserAddress.ToLower(),
            ["can_loot"] = "1",
            ["limit"] = "8"
        };
        
        var uri = QueryHelpers.AddQueryString(url, query);
        
        var client = new HttpClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Other");
        var task = client.GetAsync(uri)
            .ContinueWith((taskwithresponse) =>
            {
                var response = taskwithresponse.Result.Content.ReadAsStringAsync();
                response.Wait();
                MineResponseRoot serializedRes = JsonConvert.DeserializeObject<MineResponseRoot>(response.Result);

                if (serializedRes!=null)
                {
                    if (serializedRes.result!=null
                        &&serializedRes.result.data!=null
                        &&serializedRes.result.data.Count() > 0)
                    {
                        mines = serializedRes.result.data;

                        foreach (Mine mine in mines)
                        {
                            if ((mine.faction == "ABYSS" || mine.faction == "TRENCH") &&
                                mine.attack_team_id == null)
                            {
                                // valid target
                                targetMine = mine;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(DateTime.Now + " " + serializedRes.error_code + " " +  serializedRes.message);
                    }
                }
            });
        task.Wait();

        return targetMine;
    }

    public async Task<List<Crab>> getCrabs()
    {
        try
        {
            List<Crab> availableCrabs = null;
            string url =  _options.Web2BaseUrl + "/crabbys/lending";
        
            var query = new Dictionary<string, string>()
            {
                ["orderBy"] =  "price",
                ["order"] =  "asc",
                ["page"] = "1",
                ["limit"] = "10"
            };
        
            var uri = QueryHelpers.AddQueryString(url, query);
        
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Other");
            var task = client.GetAsync(uri)
                .ContinueWith((taskwithresponse) =>
                {
                    var response = taskwithresponse.Result.Content.ReadAsStringAsync();
                    response.Wait();
                
                    CrabRoot serializedRes = JsonConvert.DeserializeObject<CrabRoot>(response.Result);
                
                    if (serializedRes!=null)
                    {
                        if (serializedRes.result!=null
                            &&serializedRes.result.data!=null
                            &&serializedRes.result.data.Count() > 0)
                        {
                            availableCrabs = serializedRes.result.data;
                        }
                        else
                        {
                            Console.WriteLine(DateTime.Now + " " + serializedRes.error_code + " " +  serializedRes.message);
                        }
                    }
                });
            task.Wait();

            return availableCrabs;
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e);
        }

        return null;
    }
}