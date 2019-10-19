using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class UMADataLogger : MonoBehaviour
{
  public static UMADataLogger instance;
  private string path;
  private List<DNA_Item> dnaItens = new List<DNA_Item>();

  public List<DNA_Item> DnaItens { get => dnaItens; }

  private void Awake()
  {
    instance = this;
  }

  private void Start()
  {
    path = Path.Combine(Application.persistentDataPath, "DNAlog.json");
  }

  public void LogDNAItem(bool plausible, Dictionary<string, float> currentDNA, string currentRace)
  {
    List<DNA_field> keys = new List<DNA_field>();
    foreach (var dna in currentDNA)
    {
      keys.Add(new DNA_field(dna.Key, dna.Value));
    }
    dnaItens.Add(new DNA_Item(plausible, keys.ToArray(), currentRace));
  }

  private DNA_Log Read()
  {
    if (File.Exists(path))
    {

      string json = File.ReadAllText(path);
      return JsonUtility.FromJson<DNA_Log>(json);
    }

    return null;
  }


  public void Save()
  {
    // add to current
    DNA_Log previousLog = Read();
    List<DNA_Item> accumulated_itens = new List<DNA_Item>();
    if (previousLog != null)
    {
      foreach (var dna in previousLog.DNAs)
      {
        accumulated_itens.Add(dna);
      }
    }
    foreach (var dna in dnaItens)
    {
      accumulated_itens.Add(dna);
    }

    // save locally
    DNA_Log accumulatedLog = new DNA_Log(accumulated_itens.ToArray());
    string json = JsonUtility.ToJson(accumulatedLog, true);
    File.WriteAllText(path, json);
    Debug.Log("Saved at " + path);

    // try to save new ones at mongo db
    DBSender.instance.SendData(dnaItens);

    // clear
    dnaItens.Clear();
  }

}


[Serializable]
public class DNA_Log
{
  public DNA_Item[] DNAs;

  public DNA_Log(DNA_Item[] DNAs)
  {
    this.DNAs = DNAs;
  }
  public void Add(DNA_Item[] DNAs)
  {

  }
}

[Serializable]
public class DNA_Item
{
  public bool plausible;
  public DNA_field[] fields;
  public string race;

  public DNA_Item(bool plausible, DNA_field[] fields, string race)
  {
    this.race = race;
    this.plausible = plausible;
    this.fields = fields;
  }
}

[Serializable]
public class DNA_field
{
  public string field;
  public float value;

  public DNA_field(string field, float value)
  {
    this.field = field;
    this.value = value;
  }
}
