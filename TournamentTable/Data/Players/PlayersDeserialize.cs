using System;
using System.Runtime.Serialization;

namespace TournamentTable.Data.Players;

[DataContract]
public class PlayersDeserialize
{
  [DataMember]
  public int Id { get; set; }
  [DataMember]
  public string? Name { get; set; }
  [DataMember]
  public int Place { get; set; }
  [DataMember]
  public int Health { get; set; }
  [DataMember]
  public List<OpponentDeserialize>? Foughts { get; set; }
}
