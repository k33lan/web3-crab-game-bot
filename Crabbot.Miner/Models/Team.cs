namespace Crabbot.Miner;

    public class Team
    {
        public int team_id { get; set; }
        public string owner { get; set; }
        public int crabby_id_1 { get; set; }
        public string crabby_1_photo { get; set; }
        public int crabby_1_hp { get; set; }
        public int crabby_1_speed { get; set; }
        public int crabby_1_armor { get; set; }
        public int crabby_1_damage { get; set; }
        public int crabby_1_critical { get; set; }
        public int crabby_1_is_origin { get; set; }
        public int crabby_1_is_genesis { get; set; }
        public int crabby_1_legend_number { get; set; }
        public int crabby_id_2 { get; set; }
        public string crabby_2_photo { get; set; }
        public int crabby_2_hp { get; set; }
        public int crabby_2_speed { get; set; }
        public int crabby_2_armor { get; set; }
        public int crabby_2_damage { get; set; }
        public int crabby_2_critical { get; set; }
        public int crabby_2_is_origin { get; set; }
        public int crabby_2_is_genesis { get; set; }
        public int crabby_2_legend_number { get; set; }
        public int crabby_id_3 { get; set; }
        public string crabby_3_photo { get; set; }
        public int crabby_3_hp { get; set; }
        public int crabby_3_speed { get; set; }
        public int crabby_3_armor { get; set; }
        public int crabby_3_damage { get; set; }
        public int crabby_3_critical { get; set; }
        public int crabby_3_is_origin { get; set; }
        public int crabby_3_is_genesis { get; set; }
        public int crabby_3_legend_number { get; set; }
        public int battle_point { get; set; }
        public int time_point { get; set; }
        public int mine_point { get; set; }
        public object game_type { get; set; }
        public object mine_start_time { get; set; }
        public object mine_end_time { get; set; }
        public object game_id { get; set; }
        public object game_start_time { get; set; }
        public object game_end_time { get; set; }
        public object process_status { get; set; }
        public object game_round { get; set; }
        public string status { get; set; }
        public int crabby_1_class { get; set; }
        public int crabby_2_class { get; set; }
        public int crabby_3_class { get; set; }
        public int crabby_1_type { get; set; }
        public int crabby_2_type { get; set; }
        public int crabby_3_type { get; set; }
    }

    public class TeamResult
    {
        public int totalRecord { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public List<Team> data { get; set; }
        public int team_size { get; set; }
    }

    public class TeamResponseRoot
    {
        public object error_code { get; set; }
        public object message { get; set; }
        public TeamResult result { get; set; }
    }





