using System;

namespace TournamentTable.Tournament.Info;

public class UpcomingFight
{
  public readonly int Round;
  public int PlayerFirstId;
  public int PlayerSecondId;
  public bool IsCompleted;

  public UpcomingFight(int round) => Round = round;
}
