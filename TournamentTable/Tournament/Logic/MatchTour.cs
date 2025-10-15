using System;
using TournamentTable.Data.FileController;
using TournamentTable.Data.Players;
using TournamentTable.Players;

namespace TournamentTable.Tournament.Logic;

public class MatchTour
{
  static List<PlayersDeserialize>? players;
  static List<EliminatedPlayer>? eliminateds = new List<EliminatedPlayer>();
  static Random random = new Random();
  static PlayersDeserialize? playerFirst;
  static PlayersDeserialize? playersSecond;
  static int fightRound;
  static int indexFirstPlayer;
  static int indexSecondPlayer;
  public void StartBattle()
  {
    eliminateds = DataManager.EliminatedPlayerDeserilialize().ConvertListEliminated();
    players = DataManager.PlayerDeserialize().ToList();

    var shuffledPlayers = players.OrderBy(
      p => random.Next()).ToList();
    int i = 0;
    for (; i < shuffledPlayers.Count; i += 2)
    {
      if (i + 1 < shuffledPlayers.Count)
        Battle(shuffledPlayers[i].Id, shuffledPlayers[i + 1].Id);
      else
        Battle(shuffledPlayers[i].Id);
    }
  }

  public void Battle(int id1, int id2)
  {
    eliminateds = DataManager.EliminatedPlayerDeserilialize().ConvertListEliminated();
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
    battle.PlayerSecond.Foughts = SetOpponent(playersSecond.ConvertPlayer(), fightRound, playerFirst.Id);

    if (res.Lose == battle.PlayerFirst.Id)
      battle.PlayerFirst.Health -= 1;
    else
      battle.PlayerSecond.Health -= 1;

    battle.Update();

    bool isLoser = battle.PlayerFirst.Health <= 0 || battle.PlayerSecond.Health <= 0;
    if (isLoser)
    {
      var player = battle.PlayerFirst.Health <= 0 ? battle.PlayerFirst : battle.PlayerSecond;
      eliminateds!.Add(new EliminatedPlayer(player.Id, player.Name!, player.Health, players.Count, player.Foughts));
      eliminateds!.Update();
      var removePlayers = players.ConvertListPlayer();
      removePlayers.RemoveAll(p => p.Id == player.Id);
      removePlayers.Update(battle.PlayerFirst, battle.PlayerSecond);
      return;
    }
    players.ConvertListPlayer().Update(battle.PlayerFirst, battle.PlayerSecond);
  }

  public void Battle(int id)
  {
    players = DataManager.PlayerDeserialize().ToList();

    int index = players.FindIndex(p => p.Id == id);
    fightRound = DataManager.FightDeserialize().ToList().Count + 1;

    Battle battle = new Battle
    {
      Round = fightRound,
      PlayerFirst = players[index].ConvertPlayer(),
      WinId = id
    };

    battle.Update();

    if (players.Count <= 1)
    {
      var removePlayers = players.ConvertListPlayer();
      removePlayers.RemoveAll(p => p.Id == id);
      removePlayers.PlayerCreate();
      var playerEliminated = battle.PlayerFirst;
      eliminateds!.Add(new EliminatedPlayer(playerEliminated.Id, playerEliminated.Name, playerEliminated.Health, players.Count, playerEliminated.Foughts));
      eliminateds.Update();
      return;
    }
    players.ConvertListPlayer().Update(battle.PlayerFirst);
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
