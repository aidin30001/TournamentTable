using System;
using System.Runtime.Serialization;
using TournamentTable.Data.Players;

namespace TournamentTable.Data.Tournament;

[DataContract]
public class BattleDeserialize
{
  [DataMember]
  public int Round { get; set; }
  [DataMember]
  public PlayersDeserialize? PlayerFirst { get; set; }
  [DataMember]
  public PlayersDeserialize? PlayerSecond { get; set; }
  [DataMember]
  public int WinId { get; set; }
  [DataMember]
  public int LoseId { get; set; }
}
