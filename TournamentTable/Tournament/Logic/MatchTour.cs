using System;
using TournamentTable.Data.FileController;
using TournamentTable.Data.Players;
using TournamentTable.Players;

namespace TournamentTable.Tournament.Logic;

public class MatchTour
{
  static List<PlayersDeserialize>? players;
  static Random random = new Random();
  static PlayersDeserialize? playerFirst;
  static PlayersDeserialize? playersSecond;
  static int fightRound;
  static int indexFirstPlayer;
  static int indexSecondPlayer;
  public void StartBattle()
  {
    players = DataManager.PlayerDeserialize().ToList();
    var shuffledPlayers = players.OrderBy(
      p => random.Next()).ToList();

    for (int i = 0; i < players.Count() - 1; i += 2)
      Battle(shuffledPlayers[i].Id, shuffledPlayers[i + 1].Id);
  }

  public void Battle(int id1, int id2)
  {
    players = DataManager.PlayerDeserialize().ToList();

    indexFirstPlayer = players.FindIndex(p => p.Id == id1);
    indexSecondPlayer = players.FindIndex(p => p.Id == id2);

    playerFirst = players[indexFirstPlayer];
    playersSecond = players[indexSecondPlayer];

    fightRound = DataManager.FightDeserialize().ToList().Count + 1;
    var res = Randoms(playerFirst!.Id, playersSecond!.Id);

    Battle battle = new Battle()
    {
      Round = fightRound,
      PlayerFirst = playerFirst!.ConvertPlayer(),
      PlayerSecond = playersSecond!.ConvertPlayer(),
      WinId = res.Win,
      LoseId = res.Lose
    };
    
    battle.PlayerFirst.Foughts = SetOpponent(playerFirst.ConvertPlayer(), fightRound, playersSecond.Id);
    battle.PlayerSecond.Foughts = SetOpponent(playersSecond.ConvertPlayer(),fightRound, playerFirst.Id);

    if (res.Lose == battle.PlayerFirst.Id)
      battle.PlayerFirst.Health -= 1;
    else
      battle.PlayerSecond.Health -= 1;

    battle.FightUpdate();
    players.ConvertListPlayer().PlayerUpdate(battle.PlayerFirst, battle.PlayerSecond);
  }

  public (int Win, int Lose) Randoms(int id1, int id2)
  {
    int Win;
    int Lose;
    if (random.NextDouble() >= 0.5d)
    {
      Win = id1;
      Lose = id2;
    }
    else
    {
      Win = id2;
      Lose = id1;
    }

    return (Win, Lose);
  }

  private List<Opponent> SetOpponent(Player player, int round, int id)
  {
    var oldFought = new List<Opponent>();

    oldFought.AddRange(player.Foughts.Select(f => new Opponent
    {
      OpponentId = f.OpponentId,
      Round = f.Round
    }));

    oldFought.Add(new Opponent()
    {
      Round = round,
      OpponentId = id
    });

    return oldFought;
  }
}
