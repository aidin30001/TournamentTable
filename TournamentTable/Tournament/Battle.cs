using System;
using TournamentTable.Players;

namespace TournamentTable.Tournament;

public class Battle
{
  public int Round { get; set; }
  public Player? PlayerFirst { get; set; }
  public Player? PlayerSecond { get; set; }
  public int WinId { get; set; }
  public int LoseId { get; set; }
}
