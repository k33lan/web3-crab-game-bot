namespace Crabbot.Miner;

public class ConfigOptions
{
    public const string Config = "Config";

    public string Web2BaseUrl { get; set; } = String.Empty;
    public string Key { get; set; } = String.Empty;
    public string UserAddress { get; set; } = String.Empty;
    public string Web3BaseUrl { get; set; } = String.Empty;
    public string ContractAddress { get; set; } = String.Empty;
    public string Version { get; set; } = String.Empty;
    public int DelayTime { get; set; } = 0;
    public int SecondaryDelayTime { get; set; } = 0;
    public int ChainId { get; set; }
    public bool IsEuropeTimezone { get; set; } = false;
    public bool IsReinforcementEnabled { get; set; } = false;
    public List<int> TeamIds { get; set; } = null;
    public List<int> ReinforcementCrabIds { get; set; } = null;
}