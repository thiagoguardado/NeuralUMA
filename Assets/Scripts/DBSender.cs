using System.Collections;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using UnityEngine;
using UnityEngine.Networking;

public class DBSender : MonoBehaviour
{
  public static DBSender instance;
  private void Awake()
  {
    instance = this;
  }

  public void SendData(List<DNA_Item> itens, string databaseName)
  {
    if (itens.Count >= 0)
    {
      List<string> jsonItens = new List<string>();
      foreach (var item in itens)
      {
        jsonItens.Add(JsonUtility.ToJson(item));
      }
      StartCoroutine(SendToMongo(jsonItens.ToArray(), databaseName));
    }

  }
  public IEnumerator SendToMongo(string[] jsons, string databaseName)
  {
    System.Threading.Tasks.Task task = null;
    try
    {
      string keyJson = File.ReadAllText(Application.streamingAssetsPath + "/Key/key.json");
      DBKeyFile key = JsonUtility.FromJson<DBKeyFile>(keyJson);
      var client = new MongoClient(key.connURL);
      var database = client.GetDatabase(databaseName);
      var collection = database.GetCollection<BsonDocument>(SystemInfo.deviceName);

      List<BsonDocument> bsons = new List<BsonDocument>();
      foreach (var json in jsons)
      {
        bsons.Add(BsonSerializer.Deserialize<BsonDocument>(json));
      }

      task = collection.InsertManyAsync(bsons);

      Debug.Log("enviou");
    }
    catch (System.Exception)
    {
      Debug.LogError("nao enviou para mongo db");
    }

    yield return task;
  }
}
