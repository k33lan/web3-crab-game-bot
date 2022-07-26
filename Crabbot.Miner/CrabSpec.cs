using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace Crabbot.Miner
{

public partial class AddcrabbyToTeamFunction : AddcrabbyToTeamFunctionBase
{
}

[Function("addcrabbyToTeam")]
public class AddcrabbyToTeamFunctionBase : FunctionMessage
{
    [Parameter("uint256", "teamId", 1)] public virtual BigInteger TeamId { get; set; }
    [Parameter("uint256", "position", 2)] public virtual BigInteger Position { get; set; }
    [Parameter("uint256", "crabbyId", 3)] public virtual BigInteger crabbyId { get; set; }
}

public partial class AttackFunction : AttackFunctionBase
{
}

[Function("attack")]
public class AttackFunctionBase : FunctionMessage
{
    [Parameter("uint256", "gameId", 1)] public virtual BigInteger GameId { get; set; }

    [Parameter("uint256", "attackTeamId", 2)]
    public virtual BigInteger AttackTeamId { get; set; }
}

public partial class CloseGameFunction : CloseGameFunctionBase
{
}

[Function("closeGame")]
public class CloseGameFunctionBase : FunctionMessage
{
    [Parameter("uint256", "gameId", 1)] public virtual BigInteger GameId { get; set; }
}

public partial class CreateTeamFunction : CreateTeamFunctionBase
{
}

[Function("createTeam")]
public class CreateTeamFunctionBase : FunctionMessage
{
    [Parameter("uint256", "crabbyId1", 1)]
    public virtual BigInteger crabbyId1 { get; set; }

    [Parameter("uint256", "crabbyId2", 2)]
    public virtual BigInteger crabbyId2 { get; set; }

    [Parameter("uint256", "crabbyId3", 3)]
    public virtual BigInteger crabbyId3 { get; set; }
}

public partial class DepositFunction : DepositFunctionBase
{
}

[Function("deposit")]
public class DepositFunctionBase : FunctionMessage
{
    [Parameter("uint256[]", "crabbyIds", 1)]
    public virtual List<BigInteger> crabbyIds { get; set; }
}

public partial class ReinforceAttackFunction : ReinforceAttackFunctionBase
{
}

[Function("reinforceAttack")]
public class ReinforceAttackFunctionBase : FunctionMessage
{
    [Parameter("uint256", "gameId", 1)] public virtual BigInteger GameId { get; set; }
    [Parameter("uint256", "crabbyId", 2)] public virtual BigInteger crabbyId { get; set; }

    [Parameter("uint256", "borrowPrice", 3)]
    public virtual BigInteger BorrowPrice { get; set; }
}

public partial class ReinforceDefenseFunction : ReinforceDefenseFunctionBase
{
}

[Function("reinforceDefense")]
public class ReinforceDefenseFunctionBase : FunctionMessage
{
    [Parameter("uint256", "gameId", 1)] public virtual BigInteger GameId { get; set; }
    [Parameter("uint256", "crabbyId", 2)] public virtual BigInteger crabbyId { get; set; }

    [Parameter("uint256", "borrowPrice", 3)]
    public virtual BigInteger BorrowPrice { get; set; }
}

public partial class RemovecrabbyFromTeamFunction : RemovecrabbyFromTeamFunctionBase
{
}

[Function("removecrabbyFromTeam")]
public class RemovecrabbyFromTeamFunctionBase : FunctionMessage
{
    [Parameter("uint256", "teamId", 1)] public virtual BigInteger TeamId { get; set; }
    [Parameter("uint256", "position", 2)] public virtual BigInteger Position { get; set; }
}

public partial class SetLendingPriceFunction : SetLendingPriceFunctionBase
{
}

[Function("setLendingPrice")]
public class SetLendingPriceFunctionBase : FunctionMessage
{
    [Parameter("uint256", "crabbyId", 1)] public virtual BigInteger crabbyId { get; set; }
    [Parameter("uint256", "price", 2)] public virtual BigInteger Price { get; set; }
}

public partial class SettleGameFunction : SettleGameFunctionBase
{
}

[Function("settleGame")]
public class SettleGameFunctionBase : FunctionMessage
{
    [Parameter("uint256", "gameId", 1)] public virtual BigInteger GameId { get; set; }
}

public partial class StartGameFunction : StartGameFunctionBase
{
}

[Function("startGame")]
public class StartGameFunctionBase : FunctionMessage
{
    [Parameter("uint256", "teamId", 1)] public virtual BigInteger TeamId { get; set; }
}

public partial class WithdrawFunction : WithdrawFunctionBase
{
}

[Function("withdraw")]
public class WithdrawFunctionBase : FunctionMessage
{
    [Parameter("address", "to", 1)] public virtual string To { get; set; }

    [Parameter("uint256[]", "crabbyIds", 2)]
    public virtual List<BigInteger> crabbyIds { get; set; }
}

public partial class StartGameEventDTO : StartGameEventDTOBase
{
}

[Event("StartGame")]
public class StartGameEventDTOBase : IEventDTO
{
    [Parameter("uint256", "gameId", 1, false)]
    public virtual BigInteger GameId { get; set; }

    [Parameter("uint256", "teamId", 2, false)]
    public virtual BigInteger TeamId { get; set; }

    [Parameter("uint256", "gameDuration", 3, false)]
    public virtual BigInteger GameDuration { get; set; }

    [Parameter("uint256", "craReward", 4, false)]
    public virtual BigInteger CraReward { get; set; }

    [Parameter("uint256", "tusReward", 5, false)]
    public virtual BigInteger TusReward { get; set; }
}

public partial class AttackEventDTO : AttackEventDTOBase
{
}

[Event("Attack")]
public class AttackEventDTOBase : IEventDTO
{
    [Parameter("uint256", "gameId", 1, false)]
    public virtual BigInteger GameId { get; set; }

    [Parameter("uint256", "unknown1", 2, false)]
    public virtual BigInteger Unknown1 { get; set; }

    [Parameter("uint256", "attackTeamId", 3, false)]
    public virtual BigInteger AttackTeamId { get; set; }

    [Parameter("uint256", "defenseTeamId", 4, false)]
    public virtual BigInteger DefenseTeamId { get; set; }

    [Parameter("uint256", "unknown2", 5, false)]
    public virtual BigInteger Unknown2 { get; set; }

    [Parameter("uint256", "unknown3MaybeTimeBeforeNextStep", 6, false)]
    public virtual BigInteger Unknown3MaybeTimeBeforeNextStep { get; set; }

    [Parameter("uint256", "attackPoint", 7, false)]
    public virtual BigInteger AttackPoint { get; set; }

    [Parameter("uint256", "defensePoint", 8, false)]
    public virtual BigInteger DefensePoint { get; set; }
}
}




