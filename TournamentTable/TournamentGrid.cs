using System;
using System.Diagnostics.Contracts;
using Newtonsoft.Json;
using TournamentTable.Data.FileController;
using TournamentTable.Data.Players;
using TournamentTable.Data.Tournament;
using TournamentTable.Players;
using TournamentTable.Tournament;
using TournamentTable.Tournament.Info;
using TournamentTable.Tournament.Logic;

namespace TournamentTable;

public class TournamentGrid
{
  static int playerCount = 0;
  static List<UpcomingFight>? upcomings;
  static Random random = new Random();
  

  public TournamentGrid(List<string> playersName, string path, int health = 2)
  {
    playerCount = playersName.Count;
    DataManager.Path = path;
    playersName.PlayersNewCreate(health);

    new Battle().Create();

    DataManager.ResultNewCreate();

    new List<UpcomingFight>().Create();
  }

  public void FullMatch()
  {
    Info();
    new MatchTour().StartBattle();
  }

  public void Info()
  {
     var shuffledPlayers = DataManager.PlayerDeserialize().OrderBy(
      p => random.Next()).ToList();
    upcomings = new List<UpcomingFight>();
    var startId = DataManager.UpcomingFightDeserialize().ToList().Count;

    for (int i = 0; i < shuffledPlayers.Count; i+=2)
    {
      if (i + 1 < shuffledPlayers.Count)
      {
        upcomings.Add(new UpcomingFight(startId + upcomings.Count + 1)
        {
          PlayerFirstId = shuffledPlayers[i].Id,
          PlayerSecondId = shuffledPlayers[i + 1].Id,
          IsCompleted = false
        });
        continue;
      }

      upcomings.Add(new UpcomingFight(startId + upcomings.Count + 1)
      {
        PlayerFirstId = shuffledPlayers[i].Id,
        IsCompleted = false
      });
    }

    upcomings.Update();
  }

  public bool Next()
  {
    if (upcomings is null) Info();
    int index = upcomings!.FindIndex(u => !u.IsCompleted);

    if (index < 0) return false;
    
    if (upcomings[index].PlayerSecondId == 0)
    {
      new MatchTour().Battle(upcomings[index].PlayerFirstId);
      upcomings[index].IsCompleted = true;
      upcomings.Update();
      return true;
    }

    new MatchTour().Battle(upcomings[index].PlayerFirstId, upcomings[index].PlayerSecondId);
    upcomings[index].IsCompleted = true;
    upcomings.Update();
    return true;
  }
  
  public bool IsCompleted()
  {
    var _upcoming = DataManager.UpcomingFightDeserialize().ToList();
    return _upcoming[_upcoming.Count - 1].IsCompleted;
  }
}
