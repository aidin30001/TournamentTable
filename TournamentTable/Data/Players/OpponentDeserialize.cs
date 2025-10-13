using System;
using System.Runtime.Serialization;

namespace TournamentTable.Data.Players;

[DataContract]
public class OpponentDeserialize
{
  [DataMember]
  public int OpponentId { get; set; }
  [DataMember]
  public int Round { get; set; }
}
