using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class UMADataReader : MonoBehaviour
{
  public DBGetter getter;
  private bool isDone = false;
  private string[] result;
  public bool IsDone { get => isDone; }
  public string[] Result { get => result; }
  private MLData mlData;
  private MLFinalData mlFinalData;
  public MLData MlData { get => mlData; }
  public MLFinalData MlFinalData { get => mlFinalData; }
  public string epochesDatabase = "gan_output";
  public string epochesCollection = "output";
  public string finalDataDatabase = "gan_final_output";
  public string finalDataCollection = "output";

  public void ReadOutputData(string path)
  {
    isDone = false;
    result = null;
    StartCoroutine(Read(path));
  }

  public void ReadMongoData()
  {
    isDone = false;
    result = null;
    getter.GetLast((json) =>
    {
      MLData data = JsonUtility.FromJson<MLData>(json);
      mlData = data;

      isDone = true;
    }, epochesDatabase, epochesCollection);
  }

  public void ReadMongoFinalData(){
    isDone = false;
    result = null;
    getter.GetLast((json) =>
    {
      MLFinalData data = JsonUtility.FromJson<MLFinalData>(json);
      mlFinalData = data;

      isDone = true;
    }, finalDataDatabase, finalDataCollection);
  }

  private IEnumerator Read(string path)
  {
    string outputFileContents;
    if (path.Contains("://") || path.Contains(":///"))
    {
      UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
      yield return www.SendWebRequest();
      outputFileContents = www.downloadHandler.text;
    }
    else
    {
      outputFileContents = File.ReadAllText(path);
    }

    
    MLData data = JsonUtility.FromJson<MLData>(outputFileContents);
    mlData = data;

    isDone = true;

    yield return null;
  }

}

[Serializable]
public class MLData
{
  public RaceData[] data;
}
[Serializable]
public class MLFinalData
{
  public RaceFinalData[] finalData;
}
[Serializable]
public class RaceData
{
  public string key;
  public EpochData[] values;
}
[Serializable]
public class RaceFinalData
{
  public string key;
  public DNA_Item[] values;
}
[Serializable]
public class EpochData
{
  public string key;
  public DNA_Item[] values;
}