using System;
using System.Diagnostics.Contracts;
using Newtonsoft.Json;
using TournamentTable.Data.FileController;
using TournamentTable.Players;
using TournamentTable.Tournament;
using TournamentTable.Tournament.Logic;

namespace TournamentTable;

public class TournamentGrid
{
  public TournamentGrid(List<string> playersName, string path, int health = 2)
  {
    DataManager.Path = path;
    playersName.CreateNewPlayers(health);

    new Battle().FightCreate();

    DataManager.EliminatedPlayerNewCreate();
  }

  public void Start()
  {
    MatchTour match = new MatchTour();
    match.StartBattle();
  }
}
