using System;
using TournamentTable.Data.Players;
using TournamentTable.Data.Tournament;
using TournamentTable.Players;
using TournamentTable.Tournament;

namespace TournamentTable.Data.FileController;

public static class ConvertToObject
{
  public static BattleDeserialize ConvertBattleDes(this Battle battle)
  {
    return new BattleDeserialize
    {
      Round = battle.Round,
      PlayerFirst = battle.PlayerFirst!.ConvertPlayersDes(),
      PlayerSecond = battle.PlayerSecond!.ConvertPlayersDes(),
      WinId = battle.WinId,
      LoseId = battle.LoseId
    };
  }

  public static PlayersDeserialize ConvertPlayersDes(this Player player)
  {
    if (player == null) return new PlayersDeserialize();
    return new PlayersDeserialize()
    {
      Id = player.Id,
      Name = player.Name,
      Place = player.Place,
      Health = player.Health,
      Foughts = player.Foughts.ConvertListFoughtsDes(),
    };
  }

  public static List<PlayersDeserialize> ConvertListPlayersDes(this List<Player> player)
  {
    return player.Select(p => new PlayersDeserialize
    {
      Id = p.Id,
      Name = p.Name,
      Place = p.Place,
      Health = p.Health,
      Foughts = p.Foughts.ConvertListFoughtsDes()
    }).ToList();
  }

  public static List<OpponentDeserialize> ConvertListFoughtsDes(this List<Opponent> foughts)
  {
    return foughts.Select(i => new OpponentDeserialize
    {
      OpponentId = i.OpponentId,
      Round = i.Round
    }).ToList();
  }

  public static List<Player> ConvertListPlayer(this List<PlayersDeserialize> player)
  {
    return player.Select(p => new Player(p.Id, p.Name!, p.Health)
    {
      Foughts = p.Foughts!.ConvertFoughts()
    }).ToList();
  }

  public static List<Player> ConvertListPlayer(this PlayersDeserialize player)
  {
    return new List<Player>
    {
      new Player(player.Id, player.Name!, player.Health)
      {
        Foughts = player.Foughts!.ConvertFoughts()
      }
    };
  }

  public static Player ConvertPlayer(this PlayersDeserialize player)
  {
    return new Player(player.Id, player.Name!, player.Health)
    {
      Foughts = player.Foughts!.ConvertFoughts()
    };
  }

  public static List<EliminatedPlayer> ConvertListEliminated(this List<PlayersDeserialize> players)
  {
    return players.Select(p => new EliminatedPlayer(p.Id, p.Name!, p.Health, p.Place, p.Foughts!.ConvertFoughts())).ToList();
  }

  public static List<Opponent> ConvertFoughts(this List<OpponentDeserialize> foughts)
  {
    return foughts.Select(i => new Opponent
    {
      OpponentId = i.OpponentId,
      Round = i.Round
    }).ToList();
  }

  public static Opponent ConvertFoughts(this OpponentDeserialize foughts)
  {
    return new Opponent
    {
      OpponentId = foughts.OpponentId,
      Round = foughts.Round
    };
  }
}
