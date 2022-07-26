using Nethereum.RPC.Eth.DTOs;
namespace Crabbot.Miner;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ConfigOptions _options;
    private List<Mine> _activeMines = new List<Mine>();

    public Worker(ILogger<Worker> logger, ConfigOptions options)
    {
        _logger = logger;
        _options = options;
    }

    private int _errorCount = 0;
    private int _minesClosed = 0;
    private int _wins = 0;
    private int _losses = 0;
    private double _tusMade = 0;
    private double _craMade = 0;
    private int _winPercentage = 0;
    
    private bool _hasShownDaily = false;
    private int _dailyminesClosed = 0;
    private int _dailywins = 0;
    private int _dailylosses = 0;
    private double _dailytusMade = 0;
    private double _dailycraMade = 0;
    private int _dailywinPercentage = 0;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{time} Crabbot v"+_options.Version+" Initialized, EU:"+_options.IsEuropeTimezone+", REIN:"+_options.IsReinforcementEnabled, DateTimeOffset.Now);
        
        // init
        _activeMines = await checkForMines();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_options.DelayTime, stoppingToken);
                
                printDaily();
                
                // for each mine we have active, check if any need reinforcing and reinforce if needed
                if (_options.IsReinforcementEnabled)
                    await tryReinforceMines();

                //for each mine we have active, check the expiry and claim if expired then remove from the list
                 await tryCloseMines();
                
                 if (_activeMines.Count() < _options.TeamIds.Count())
                 {
                     // if less than configured mines, a team MUST be available
                     // check if 30 mins passed the last start time
                     _logger.LogInformation("{time} Under mine count threshold, checking if start window valid....", DateTimeOffset.Now);
                     bool isValidWindow = false;
                     if (_activeMines.Count() == 0)
                     {
                         isValidWindow = true;
                         // valid window to start new mine
                         _logger.LogInformation("{time} No mines, for valid window check, checking for teams...", DateTimeOffset.Now);
                     }
                     else
                     {
                         int lastStartTime = _activeMines.Max(m => m.start_time);
                         DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                         DateTime startTime = startDate.AddSeconds(lastStartTime).AddHours(1);
                         DateTime startTimePlus30 = startTime.AddMinutes(30);
                         DateTime now = (_options.IsEuropeTimezone) ? DateTime.Now.AddHours(-1) : DateTime.Now;

                         if (now > startTimePlus30)
                         {
                             isValidWindow = true;
                             // valid window to start new mine
                             _logger.LogInformation("{time} Last mine started @ {start}, valid window, checking for teams...", DateTimeOffset.Now, startTime);
                         }
                             
                     }
                    
                     if (isValidWindow)
                     {
                         List<Team> availableTeams = await checkForAvailableTeams();
                         if (availableTeams != null && availableTeams.Count() > 0)
                         {
                             // teams available, send one on a new mine
                             _logger.LogInformation("{time} Sending team out...", DateTimeOffset.Now);
                             // send team on to mine
                             await sendTeam(availableTeams.FirstOrDefault());
                             _logger.LogInformation("{time} Team Sent.", DateTimeOffset.Now);
                         }
                         else
                             _logger.LogInformation("{time} No available teams.", DateTimeOffset.Now);

                         // wait for the mines to update
                         _logger.LogInformation("{time} Waiting....", DateTimeOffset.Now);
                         await Task.Delay(_options.DelayTime, stoppingToken);
                     
                         // refresh mines
                         _logger.LogInformation("{time} Refreshing mines...", DateTimeOffset.Now);
                         _activeMines = await checkForMines();
                     }
                 }
            }
            catch (Exception e)
            {
                _errorCount++;
                _logger.LogCritical("{time} ERROR: {error}", DateTimeOffset.Now, e.Message);
                _logger.LogCritical("Error Count: {_errorCount}", _errorCount);
            }
        }
    }

    // Teams
    private async Task<List<Team>> checkForAvailableTeams()
    {
        _logger.LogInformation("{time} Checking for claimable mines...", DateTimeOffset.Now);
        Web2Client client = new Web2Client(_options);
        List<Team> teams =  await client.getTeams();
        
        if (teams != null && teams.Count()>0)
            _logger.LogInformation("{time} Found at least 1 available team...", DateTimeOffset.Now);
        else
            _logger.LogInformation("{time} No available teams found.", DateTimeOffset.Now);

        return teams;
    }
    
    private async Task sendTeam(Team availableTeam)
    {
        try
        {
            Web3Client client = new Web3Client(_options);
        
            _logger.LogInformation("{time} Sending team id: {teamid}", DateTimeOffset.Now, availableTeam.team_id);
            TransactionReceipt? resp =  await client.sendTeam(availableTeam.team_id);

            if (resp != null)
            {
                _logger.LogInformation("{time} Team sent id: {teamid} tx: {tx}", DateTimeOffset.Now, availableTeam.team_id, resp.TransactionHash);
            }
            else
            {
                throw new Exception(DateTimeOffset.Now + " Error sending team id: " + availableTeam.team_id);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical("{time} ERROR: {error}", DateTimeOffset.Now, e.Message);
            _errorCount++;
            _logger.LogCritical("Error Count: {_errorCount}", _errorCount);
        }
    }

    private void printDaily()
    {
        DateTime now = DateTime.Now;
        if (now.Hour == 23 && now.Minute > 50 && now.Minute < 59)
        {
            // last 15 mins of the day
            if (!_hasShownDaily)
            {
                _logger.LogInformation("{time} ==== TODAY ({today}) MINES({mines}) WINS({wins}) LOSSES({losses}) W/L%({wl}) TUS({tus}) CRA({cra}) ====", DateTimeOffset.Now, now.Day, _dailyminesClosed, _dailywins, _dailylosses, _dailywinPercentage, _dailytusMade, _dailycraMade);
                
                _dailyminesClosed = 0;
                _dailylosses = 0;
                _dailywins = 0;
                _dailycraMade = 0;
                _dailytusMade = 0;
                _dailywinPercentage = 0;
                
                _hasShownDaily = true;
            }
        }
        else if (now.Hour == 0 && now.Minute > 0 && now.Minute < 10)
        {
            _hasShownDaily = false;
        }
    }
    
    private async Task sendTeams(List<Team> availableTeams)
    {
        try
        {
            Web3Client client = new Web3Client(_options);
        
            foreach (var team in availableTeams)
            {
                _logger.LogInformation("{time} Sending team id: {teamid}", DateTimeOffset.Now, team.team_id);
                TransactionReceipt? resp =  await client.sendTeam(team.team_id);

                if (resp != null)
                {
                    _logger.LogInformation("{time} Team sent id: {teamid} tx: {tx}", DateTimeOffset.Now, team.team_id, resp.TransactionHash);
                }
                else
                {
                    throw new Exception(DateTimeOffset.Now + " Error sending team id: " + team.team_id);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical("{time} ERROR: {error}", DateTimeOffset.Now, e.Message);
            _errorCount++;
            _logger.LogCritical("Error Count: {_errorCount}", _errorCount);
        }
    }

    // Mines
    private async Task<List<Mine>> checkForMines()
    {
        _logger.LogInformation("{time} Checking for mines...", DateTimeOffset.Now);
        Web2Client client = new Web2Client(_options);
        List<Mine> mines = await client.getMines();

        if (mines != null && mines.Count()>0)
            _logger.LogInformation("{time} Found {total} mines...", DateTimeOffset.Now, mines.Count());
        else
            _logger.LogInformation("{time} No mines found.", DateTimeOffset.Now);
        
        return mines ?? new List<Mine>();
    }
    private async Task tryCloseMines()
    {
        List<Mine> tempMines = new List<Mine>(_activeMines);

        try
        {
            foreach (Mine mine in tempMines)
            {
                DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                DateTime time = startDate.AddSeconds(mine.end_time).AddHours(1);
                DateTime now = (_options.IsEuropeTimezone) ? DateTime.Now.AddHours(-1) : DateTime.Now;
                if (now > time)
                {
                    // can close this mine -> close it
                    Web3Client client = new Web3Client(_options);
                    _logger.LogInformation("{time} Closing mine id: {game}", DateTimeOffset.Now, mine.game_id);
                    TransactionReceipt? resp =  await client.closeGame(mine.game_id);

                    if (resp != null)
                    {
                        _logger.LogInformation("{time} Mine closed id: {game} tx: {tx}", DateTimeOffset.Now, mine.game_id, resp.TransactionHash);
                        _activeMines.Remove(mine);
                        calculateStats(mine);
                        _logger.LogInformation("{time} total mines closed: {total}", DateTimeOffset.Now, _minesClosed);
                    }
                    else
                    {
                        // Refresh mines (prevents game invalid)
                        _activeMines = await checkForMines(); 
                        throw new Exception(DateTimeOffset.Now + " Error closing mine id: " + mine.game_id + " Refreshed mines...");
                    }
                }
                else
                {
                    _logger.LogInformation("{time} Can't close mine {gameid} yet, expires: {endtime}", DateTimeOffset.Now, mine.game_id, time);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical("{time} ERROR: {error}", DateTimeOffset.Now, e.Message);
            _errorCount++;
            _logger.LogCritical("Error Count: {_errorCount}", _errorCount);
        }
    }

    private void calculateStats(Mine mine)
    {
        _minesClosed++;
        _dailyminesClosed++;

        // all at least 1 prime in team
        double tusWinRate = 334.12;
        double craWinRate = 4.12;
        double tusLossRate = 136.68;
        double craLossRate = 1.68;
        
        if (mine.winner_team_id != mine.team_id)
        {
            // lost
            _tusMade = _tusMade + tusLossRate;
            _craMade = _craMade + craLossRate;
            _losses++;
            _winPercentage = (int)Math.Round((double)(100 * _wins) / _minesClosed);
            
            _dailytusMade = _dailytusMade + tusLossRate;
            _dailycraMade = _dailycraMade + craLossRate;
            _dailylosses++;
            _dailywinPercentage = (int)Math.Round((double)(100 * _dailywins) / _dailyminesClosed);
        }
        else
        {
            // won
            _tusMade = _tusMade + tusWinRate;
            _craMade = _craMade + craWinRate;
            _wins++;
            _winPercentage = (int)Math.Round((double)(100 * _wins) / _minesClosed);
            
            _dailytusMade = _dailytusMade + tusWinRate;
            _dailycraMade = _dailycraMade + craWinRate;
            _dailywins++;
            _dailywinPercentage = (int)Math.Round((double)(100 * _dailywins) / _dailyminesClosed);
        }
        
        _logger.LogInformation("{time} ALLTIME CLOSED: {total} -- WON: {} -- LOST: {} -- W/L: {} -- TUS: {} -- CRA: {} ", DateTimeOffset.Now, _minesClosed, _wins, _losses, _winPercentage, _tusMade, _craMade);
        _logger.LogInformation("{time} TODAY CLOSED: {total} -- WON: {} -- LOST: {} -- W/L: {} -- TUS: {} -- CRA: {} ", DateTimeOffset.Now, _dailyminesClosed, _dailywins, _dailylosses, _dailywinPercentage, _dailytusMade, _dailycraMade);
    }

    private async Task tryReinforceMines()
    {
        try
        {
            // check activemines, if time is within 30 mins of starting any mine, do a full check
            bool canCheckToReinforce = false;
            foreach (var mine in _activeMines)
            {
                DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                DateTime startTime = startDate.AddSeconds(mine.start_time).AddHours(1);
                DateTime startTimePlus30 = startTime.AddMinutes(30);
                DateTime now = (_options.IsEuropeTimezone) ? DateTime.Now.AddHours(-1) : DateTime.Now;

                if (now < startTimePlus30)
                {
                    canCheckToReinforce = true;
                    break;
                }  
            }

            if (canCheckToReinforce)
            {
                // get all open mines of user
                List<Mine> openMines = await checkForMines();

                // check if mine can be reinforced
                foreach (Mine openMine in openMines)
                {
                    if (canReinforceDefense(openMine))
                    {
                        _logger.LogInformation("{time} Can reinforce mine id: {game}", DateTimeOffset.Now, openMine.game_id);
                        int crabId = getReinforcementCrabId(openMine);
                        if (crabId!=0)
                        {
                            _logger.LogInformation("{time} Sending crab #: {id} to reinforce {game}", DateTimeOffset.Now, crabId, openMine.game_id);
                            Web3Client client = new Web3Client(_options);
                            TransactionReceipt? resp = await client.reinforceDefense(openMine.game_id, crabId, 0);

                            if (resp != null)
                            {
                                _logger.LogInformation("{time} mine {game} reinforced with crab {crab} tx: {tx}", DateTimeOffset.Now, openMine.game_id, crabId, resp.TransactionHash);
                            }
                            else
                            {
                                var msg = DateTimeOffset.Now + " Error reinforcing: " + openMine.game_id +
                                          " with crab " + crabId;
                                _logger.LogCritical(msg);
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("{time} No mines in reinforce window. Skipping reinforcements.", DateTimeOffset.Now);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical("{time} ERROR: {error}", DateTimeOffset.Now, e.Message);
            _errorCount++;
            _logger.LogCritical("Error Count: {_errorCount}", _errorCount);
        }
    }

    private int getReinforcementCrabId(Mine mine)
    {
        int crabId = 0;
        switch (getMinerReinforcementStatus(mine))
        {
            case 0:
                break;
            case 1:
                crabId = _options.ReinforcementCrabIds[0];
                break;
            case 2:
                crabId = _options.ReinforcementCrabIds[1];
                break;
            default:
                break;
        }

        return crabId;
    }

    // reinforce
    private bool canReinforceDefense(Mine openMine)
    { 
        /*Return True if, in the given game, the miner (the defense) can
        reinforce at this moment, regardless of whether its the first or the
        second time*/

        if (getMinerReinforcementStatus(openMine) != 0)
            return true;

        return false;
    }
    private int getMinerReinforcementStatus(Mine openMine)
    {
        /*Determines whether the game can be reinforced and
        at which reinforcement stage we are.

            Returns:
            - 0 if the mine cannot be reinforced
            - 1 if the mine can be reinforced the first time
            - 2 if the mine can be reinforced the second time*/
        
        if (minerCanReinforceForTheFirstTime(openMine))
            return 1;
        
        if (minerCanReinforceForTheSecondTime(openMine))
            return 2;
        
        return 0;

    }
    private bool isMineSettled(Mine mine)
    {
        if (mine.winner_team_id != null && mine.winner_team_id != 0)
            return true;

        return false;
    }
    private bool isMineOpen(Mine mine)
    {
        if (mine.status == "open")
            return true;

        else return false;
    }
    private bool isMineAttacked(Mine mine)
    {
        if (mine.attack_team_id != null && mine.attack_team_id != 0)
            return true;

        return false;
    }
    private bool minerCanReinforceForTheFirstTime(Mine openMine)
    {
        /*
         * Return True if, in the given game, the miner (the defense) can
            reinforce at this moment for the first time
         */
        
        // mine is not settled
        // mine has been attacked
        // mine is open
        // mine round is 0
        if (!isMineSettled(openMine) && isMineAttacked(openMine) && isMineOpen(openMine) && openMine.round == 0)
            return true;

        return false;

    }
    private bool minerCanReinforceForTheSecondTime(Mine openMine)
    {
        /*
         * Return True if, in the given game, the miner (the defense) can
            reinforce at this moment for the second time
         */
        
        // mine is not settled
        // mine has been attacked
        // mine is open
        // mine round is 0
        if (!isMineSettled(openMine) && isMineAttacked(openMine) && isMineOpen(openMine) && openMine.round == 2)
            return true;

        return false;
    }
    
    // Looting
    private async Task checkForLoots()
    {
        _logger.LogInformation("{time} Checking for lootable targets...", DateTimeOffset.Now);
        Web2Client client = new Web2Client(_options);
        Mine lootableMine = await client.findSomethingToLoot();
        if (lootableMine != null)
        {
            // attack!
            // can close this mine -> close it
            Web3Client web3client = new Web3Client(_options);
            _logger.LogInformation("{time} Attacking mine id: {game}", DateTimeOffset.Now, lootableMine.game_id);
            TransactionReceipt? resp =  await web3client.attack(lootableMine.game_id, 9000);

            if (resp != null)
            {
                _logger.LogInformation("{time} Mine attacked id: {game} tx: {tx}", DateTimeOffset.Now, lootableMine.game_id, resp.TransactionHash);
            }
            else
            {
                throw new Exception(DateTimeOffset.Now + " Error attacking mine id: " + lootableMine.game_id + " ");
            }
        }
    }
}