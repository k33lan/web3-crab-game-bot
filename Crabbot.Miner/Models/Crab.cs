namespace Crabbot.Miner;

public class Crab
{
    public int crabby_id { get; set; }
    public int id { get; set; }
    public string price { get; set; }
    public string crabby_name { get; set; }
    public string lender { get; set; }
    public int is_being_borrowed { get; set; }
    public string borrower { get; set; }
    public int? game_id { get; set; }
    public int crabby_type { get; set; }
    public int crabby_class { get; set; }
    public int class_id { get; set; }
    public string class_name { get; set; }
    public int is_origin { get; set; }
    public int is_genesis { get; set; }
    public int legend_number { get; set; }
    public int pure_number { get; set; }
    public string photo { get; set; }
    public int hp { get; set; }
    public int speed { get; set; }
    public int damage { get; set; }
    public int critical { get; set; }
    public int armor { get; set; }
    public int battle_point { get; set; }
    public int time_point { get; set; }
    public int mine_point { get; set; }
}

public class CrabResult
{
    public int totalRecord { get; set; }
    public int totalPages { get; set; }
    public int page { get; set; }
    public int limit { get; set; }
    public List<Crab> data { get; set; }
}

public class CrabRoot
{
    public object error_code { get; set; }
    public object message { get; set; }
    public CrabResult result { get; set; }
}

