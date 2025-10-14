using System;

namespace TournamentTable.Players;

public class EliminatedPlayer : Player
{
  public EliminatedPlayer(int id, string name, int health, int place = 0, List<Opponent> foughts = null!)
    : base(id, name, health, place, foughts) { }
  // public EliminatedPlayer(){}
}
