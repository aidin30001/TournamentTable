using System;
using System.Runtime.Serialization;

namespace TournamentTable.Data.Tournament;

[DataContract]
public class UpcomingFightDeserialize
{
  [DataMember]
  public int Round { get; set; }
  [DataMember]
  public int PlayerFirstId { get; set; }
  [DataMember]
  public int PlayerSecondId { get; set; }
  [DataMember]
  public bool IsCompleted { get; set; }
}
