using System.Collections;
using System.Collections.Generic;
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
      StartCoroutine(SendToMongo(jsonItens.ToArray(),databaseName));
    }

  }
  public IEnumerator SendToMongo(string[] jsons, string databaseName)
  {
    System.Threading.Tasks.Task task = null;
    try
    {
      var client = new MongoClient("mongodb+srv://admin:admin@cluster0-vto77.gcp.mongodb.net/admin?retryWrites=true&w=majority");
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
