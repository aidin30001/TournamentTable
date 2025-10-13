using System;
using System.Security.Cryptography.X509Certificates;

namespace TournamentTable.Players;

public class Player
{
  public readonly int Id;
  public string Name = "";
  public int Place;
  public int Health;
  public List<Opponent> Foughts { get; set; }

  public Player(int id, string name, int health, int place = 0, List<Opponent> foughts = null!)
  {
    Id = id;
    Name = name;
    Health = health;
    Place = place;
    Foughts = foughts == null ? new List<Opponent>() : foughts;
  }
}
