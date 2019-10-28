using System;
using System.Collections;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using UnityEngine;

public class DBGetter : MonoBehaviour
{

  public void GetLast(Action<string> callback,string database, string collection)
  {
    StartCoroutine(GetFromMongo(callback,database,collection));
  }

  public IEnumerator GetFromMongo(Action<string> callback,string _database, string _collection)
  {
    var client = new MongoClient("mongodb+srv://admin:admin@cluster0-vto77.gcp.mongodb.net/admin?retryWrites=true&w=majority");
    var database = client.GetDatabase(_database);
    var collection = database.GetCollection<BsonDocument>(_collection);

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
