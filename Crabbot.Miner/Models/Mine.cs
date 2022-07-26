namespace Crabbot.Miner;

    public class Process
    {
        public string action { get; set; }
        public int transaction_time { get; set; }
    }

    public class Mine
    {
        public int game_id { get; set; }
        public int start_time { get; set; }
        public int end_time { get; set; }
        public object cra_reward { get; set; }
        public string tus_reward { get; set; }
        public object miner_cra_reward { get; set; }
        public string miner_tus_reward { get; set; }
        public long looter_cra_reward { get; set; }
        public object looter_tus_reward { get; set; }
        public object estimate_looter_win_cra { get; set; }
        public string estimate_looter_win_tus { get; set; }
        public object estimate_looter_lose_cra { get; set; }
        public string estimate_looter_lose_tus { get; set; }
        public object estimate_miner_lose_cra { get; set; }
        public string estimate_miner_lose_tus { get; set; }
        public object estimate_miner_win_cra { get; set; }
        public string estimate_miner_win_tus { get; set; }
        public int? round { get; set; }
        public int team_id { get; set; }
        public string owner { get; set; }
        public int defense_point { get; set; }
        public int defense_mine_point { get; set; }
        public int? attack_team_id { get; set; }
        public string attack_team_owner { get; set; }
        public int? attack_point { get; set; }
        public int? winner_team_id { get; set; }
        public string status { get; set; }
        public List<Process> process { get; set; }
        public int crabby_id_1 { get; set; }
        public int crabby_id_2 { get; set; }
        public int crabby_id_3 { get; set; }
        public int mine_point_modifier { get; set; }
        public string crabby_1_photo { get; set; }
        public string crabby_2_photo { get; set; }
        public string crabby_3_photo { get; set; }
        public string faction { get; set; }
        public int defense_crabby_number { get; set; }
    }

    public class Result
    {
        public int totalRecord { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public List<Mine> data { get; set; }
    }

    public class MineResponseRoot
    {
        public object error_code { get; set; }
        public object message { get; set; }
        public Result result { get; set; }
    }



