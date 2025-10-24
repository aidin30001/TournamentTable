using System;
using TournamentTable.Players;
using Newtonsoft.Json;
using System.Text;
using TournamentTable.Data.Players;
using System.Runtime.Serialization.Json;
using TournamentTable.Tournament;
using TournamentTable.Data.Tournament;
using TournamentTable.Tournament.Info;

namespace TournamentTable.Data.FileController;

public static class DataManager
{
  private static string pathPlayers_Json = "";
  private static string pathFight_Json = "";
  private static string pathResult_Json = "";
  private static string pathUpcomingFight_Json = "";
  public static string Path
  {
    set
    {
      pathPlayers_Json = System.IO.Path.Combine(value, "Players.json");
      pathFight_Json = System.IO.Path.Combine(value, "Fight.json");
      pathResult_Json = System.IO.Path.Combine(value, "Result.json");
      pathUpcomingFight_Json = System.IO.Path.Combine(value, "UpcomingFight.json");
    }
  }
  public static void PlayersNewCreate(this List<string> playersName, int health)
  {
    List<Player> players = new List<Player>();
    int id = 0;
    players.AddRange(playersName.Select(item => new Player(id += 1, item, health)));
    players.Create();
  }
  public static void ResultNewCreate()
  {
    using (var fs = new FileStream(pathResult_Json, FileMode.Create))
    {
      var bytes = Encoding.UTF8.GetBytes("{}");
      fs.Write(bytes, 0, bytes.Length);
    }
  }

  public static IEnumerable<PlayersDeserialize> PlayerDeserialize()
  {
    string json = File.ReadAllText(pathPlayers_Json, Encoding.UTF8);
    var serialize = new DataContractJsonSerializer(typeof(List<PlayersDeserialize>));

    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
      return (List<PlayersDeserialize>)serialize.ReadObject(ms)!;
  }

  public static IEnumerable<BattleDeserialize> FightDeserialize()
  {
    string json = File.ReadAllText(pathFight_Json, Encoding.UTF8);
    var serialize = new DataContractJsonSerializer(typeof(List<BattleDeserialize>));

    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
      return (List<BattleDeserialize>)serialize.ReadObject(ms)!;
  }

  public static IEnumerable<UpcomingFightDeserialize> UpcomingFightDeserialize()
  {
    string json = File.ReadAllText(pathUpcomingFight_Json, Encoding.UTF8);
    var serialize = new DataContractJsonSerializer(typeof(List<UpcomingFightDeserialize>));

    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
      return (List<UpcomingFightDeserialize>)serialize.ReadObject(ms)!;
  }

  public static IEnumerable<PlayersDeserialize> ResultDeserilialize()
  {
    string json = File.ReadAllText(pathResult_Json, Encoding.UTF8);
    var serialize = new DataContractJsonSerializer(typeof(List<PlayersDeserialize>));

    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
      return (List<PlayersDeserialize>)serialize.ReadObject(ms)!;
  }

  public static void Create(this List<Player> players)
  {
    using (var fs = new FileStream(pathPlayers_Json, FileMode.Create))
    {
      var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(players, Formatting.None));
      fs.Write(bytes, 0, bytes.Length);
    }
  }

  public static void Create(this List<UpcomingFight> upcoming)
  {
    using (var fs = new FileStream(pathUpcomingFight_Json, FileMode.Create))
    {
      var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(upcoming, Formatting.None));
      fs.Write(bytes, 0, bytes.Length);
    }
  }

  public static void Create(this List<Result> players)
  {
    using (var fs = new FileStream(pathResult_Json, FileMode.Create))
    {
      var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(players, Formatting.None));
      fs.Write(bytes, 0, bytes.Length);
    }
  }

  public static void Create(this Battle battle)
  {
    using (var fs = new FileStream(pathFight_Json, FileMode.Create))
    {
      var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(battle, Formatting.None));
      fs.Write(bytes, 0, bytes.Length);
    }
  }

  public static void Update(this List<Player> players, params Player[] updatedPlayer)
  {
    // Локальный метод для обновления одного игрока
    void UpdateSingle(Player updatedPlayer)
    {
      // Найти игрока по Id
      int index = players.FindIndex(p => p.Id == updatedPlayer.Id);

      if (index < 0) return;

      // Копируем старые Foughts
      var oldFoughts = players[index].Foughts != null
          ? new List<Opponent>(players[index].Foughts)
          : new List<Opponent>();

      // Добавляем новые Foughts из updatedPlayer (без дубликатов по OpponentId + Round)
      foreach (var f in updatedPlayer.Foughts)
      {
        if (!oldFoughts.Any(o => o.OpponentId == f.OpponentId && o.Round == f.Round))
        {
          oldFoughts.Add(f);
        }
      }

      // Обновляем игрока с новым здоровьем и Foughts
      players[index] = new Player(updatedPlayer.Id)
      {
        Name = updatedPlayer.Name!,
        Health = updatedPlayer.Health,
        Foughts = oldFoughts
      };
    }

    // Обновляем обоих игроков
    foreach (var player in updatedPlayer)
    {
      UpdateSingle(player);
    }

    players.Create();
  }

  public static void Update(this List<Result> players)
  {
    var readResulst = new List<Result>();

    if (File.Exists(pathResult_Json))
      readResulst = ResultDeserilialize().ToList().ConvertListResult();

    players.ForEach(p =>
    {
      var existing = readResulst.FirstOrDefault(e => e.Id == p.Id);

      if (existing == null)
        readResulst.Add(p);
      else
      {
        existing.Health = p.Health;
        existing.Place = p.Place;
        existing.Foughts = p.Foughts;
      }
    });

    readResulst.Create();
  }

  public static void Update(this List<UpcomingFight> upcomings)
  {
    var readResUpcoming = new List<UpcomingFight>();

    if (File.Exists(pathUpcomingFight_Json))
      readResUpcoming = UpcomingFightDeserialize().ToList().ConvertListUpcomingFight();

    upcomings.ForEach(u =>
    {
      var existing = readResUpcoming.FirstOrDefault(_u => _u.Round == u.Round);
      if (existing == null)
        readResUpcoming.Add(u);
      else
      {
        existing.PlayerFirstId = u.PlayerFirstId;
        existing.PlayerSecondId = u.PlayerSecondId;
        existing.IsCompleted = u.IsCompleted;
      }
    });

    readResUpcoming.Create();
  }

  public static void Update(this Battle battle)
  {
    var readResBattle = new List<BattleDeserialize>();

    if (File.Exists(pathFight_Json))
      readResBattle = FightDeserialize().ToList();

    var existing = readResBattle.FirstOrDefault(b =>
    b.Round == battle.Round);

    if (existing == null)
      readResBattle.Add(battle.ConvertBattleDes());

    using (var fs = new FileStream(pathFight_Json, FileMode.Create))
    {
      var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(readResBattle, Formatting.None));
      fs.Write(bytes, 0, bytes.Length);
    }
  }
}
