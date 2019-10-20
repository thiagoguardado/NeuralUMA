using System;
using System.Collections;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using UnityEngine;

public class DBGetter : MonoBehaviour
{

  public void GetLast(Action<string> callback)
  {
    StartCoroutine(GetFromMongo(callback));
  }

  public IEnumerator GetFromMongo(Action<string> callback)
  {
    var client = new MongoClient("mongodb+srv://admin:admin@cluster0-vto77.gcp.mongodb.net/admin?retryWrites=true&w=majority");
    var database = client.GetDatabase("gan_output");
    var collection = database.GetCollection<BsonDocument>("output");

    yield return collection.Find(FilterDefinition<BsonDocument>.Empty)
      .Sort("{_id:-1}")
      .Limit(1)
      .ForEachAsync(document =>
      {
        var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
        string json = document.ToJson(jsonWriterSettings);
        callback.Invoke(json);
      });
  }

}
