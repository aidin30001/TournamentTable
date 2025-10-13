using System;
using TournamentTable.Players;
using Newtonsoft.Json;
using System.Text;
using TournamentTable.Data.Players;
using System.Runtime.Serialization.Json;
using TournamentTable.Tournament;
using TournamentTable.Data.Tournament;

namespace TournamentTable.Data.FileController;

public static class DataManager
{
  private static string pathPlayers_Json = "";
  private static string pathFight_Json = "";
  private static string pathBracket_Json = "";
  public static string Path
  {
    set
    {
      pathPlayers_Json = System.IO.Path.Combine(value, "Players.json");
      pathFight_Json = System.IO.Path.Combine(value, "Fight.json");
      pathBracket_Json = System.IO.Path.Combine(value, "Bracket.json");
    }
  }
  public static void CreateNewPlayers(this List<string> playersName, int health)
  {
    List<Player> players = new List<Player>();
    int id = 0;
    players.AddRange(playersName.Select(item => new Player(id += 1, item, health)));
    players.PlayerCreate();
  }

  public static IEnumerable<PlayersDeserialize> PlayerDeserialize()
  {
    string json = File.ReadAllText(pathPlayers_Json, Encoding.UTF8);
    var serialize = new DataContractJsonSerializer(typeof(List<PlayersDeserialize>));

    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
    {
      return (List<PlayersDeserialize>)serialize.ReadObject(ms)!;
    }
  }

  public static void PlayerCreate(this List<Player> players)
  {
    using (var fs = new FileStream(pathPlayers_Json, FileMode.Create))
    {
      var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(players, Formatting.None));
      fs.Write(bytes, 0, bytes.Length);
    }
  }

  public static void PlayerUpdate(this List<Player> players, Player playerFirst, Player playerSecond)
  {
    // Локальный метод для обновления одного игрока
    void UpdateSingle(Player updatedPlayer)
    {
      // Найти игрока по Id
      int index = players.FindIndex(p => p.Id == updatedPlayer.Id);

      if (index >= 0)
      {
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
      else
      {
        // Игрока нет — добавляем нового
        players.Add(updatedPlayer);
      }
    }

    // Обновляем обоих игроков
    UpdateSingle(playerFirst);
    UpdateSingle(playerSecond);

    players.PlayerCreate();
  }



  public static IEnumerable<BattleDeserialize> FightDeserialize()
  {
    string json = File.ReadAllText(pathFight_Json, Encoding.UTF8);
    var serialize = new DataContractJsonSerializer(typeof(List<BattleDeserialize>));

    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
    {
      return (List<BattleDeserialize>)serialize.ReadObject(ms)!;
    }
  }

  public static void FightUpdate(this Battle battle)
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

  public static void FightCreate(this Battle battle)
  {
    using (var fs = new FileStream(pathFight_Json, FileMode.Create))
    {
      var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(battle, Formatting.None));
      fs.Write(bytes, 0, bytes.Length);
    }
  }
}
