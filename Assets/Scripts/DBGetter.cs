using System;
using System.Collections;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using UnityEngine;

public class DBGetter : MonoBehaviour
{

  public void GetLast(Action<string> callback, string database, string collection)
  {
    StartCoroutine(GetFromMongo(callback, database, collection));
  }

  public IEnumerator GetFromMongo(Action<string> callback, string _database, string _collection)
  {
    string keyJson = File.ReadAllText(Application.streamingAssetsPath + "/Key/key.json");
    DBKeyFile key = JsonUtility.FromJson<DBKeyFile>(keyJson);
    var client = new MongoClient(key.connURL);
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

[Serializable]
public class DBKeyFile
{
  public string connURL;
}