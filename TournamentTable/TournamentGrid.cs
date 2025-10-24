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
    List<UpcomingFight> upcomings = new List<UpcomingFight>();
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
    var _upcoming = DataManager.UpcomingFightDeserialize().ToList();

    int index = _upcoming.FindIndex(u => !u.IsCompleted);

    if (index == -1) return false;

    _upcoming[index].IsCompleted = true;
    _upcoming.ConvertListUpcomingFight().Update();

    if (_upcoming[index].PlayerSecondId == 0 || _upcoming[index].PlayerFirstId == 0)
    {
      int _id = _upcoming[index].PlayerFirstId != 0 ? _upcoming[index].PlayerSecondId : _upcoming[index].PlayerSecondId;

      new MatchTour().Battle(_id);
      return true;
    }

    new MatchTour().Battle(_upcoming[index].PlayerFirstId, _upcoming[index].PlayerSecondId);
    return true;
  }
  
  public void Battle()
  {
    
  }
}
