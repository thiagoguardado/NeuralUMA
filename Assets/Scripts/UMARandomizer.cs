using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UMARandomizer : MonoBehaviour
{
  public UMA.CharacterSystem.DynamicCharacterAvatar targetMaleAvatar;
  public UMA.CharacterSystem.DynamicCharacterAvatar targetFemaleAvatar;
  private UMA.CharacterSystem.DynamicCharacterAvatar currentAvatar;
  private string[] races = new string[] { "HumanMale", "HumanFemale" };
  private string currentRace;
  public string CurrentRace { get => currentRace; }
  private Dictionary<string, float> currentDNA;
  public Dictionary<string, float> CurrentDNA { get => currentDNA; }
  public Transform anchorPoint;
  public bool getFromDB = false;

  private Trait[] possibleValues = new Trait[] {
      new Trait("skinGreenness",false),
      new Trait("skinBlueness", false),
      new Trait("skinRedness", false),

      new Trait("height", true, 0.5f, 0.9f),

      new Trait("headSize", true, 0.3f, 0.9f),
      new Trait("headWidth", true, 0f, 1f),
      new Trait("neckThickness", true, 0f, 1f),

      new Trait("armLength", true, 0.3f, 0.7f),
      new Trait("forearmLength", true, 0.3f, 0.8f),
      new Trait("armWidth", true, 0.1f, 0.9f),
      new Trait("forearmWidth", true, 0f, 1f),
      new Trait("handsSize", true, 0f, 0.8f),

      new Trait("legsSize", true, 0.2f, 0.5f),
      new Trait("legSeparation", true, 0.2f, 1f),
      new Trait("feetSize", true, 0.3f, 0.8f),

      new Trait("upperMuscle", true, 0f, 1f),
      new Trait("lowerMuscle", true, 0f, 1f),
      new Trait("upperWeight", true, 0f, 1f),
      new Trait("lowerWeight", true, 0f, 1f),

      new Trait("belly", true, 0f, 1f),
      new Trait("waist", true, 0f, 1f),
      new Trait("gluteusSize", true, 0f, 1f),

      new Trait("earsSize", false),
      new Trait("earsPosition", false),
      new Trait("earsRotation", false),
      new Trait("noseSize", false),
      new Trait("noseCurve", false),
      new Trait("noseWidth", false),
      new Trait("noseInclination", false),
      new Trait("nosePosition", false),
      new Trait("nosePronounced", false),
      new Trait("noseFlatten", false),
      new Trait("chinSize", false),
      new Trait("chinPronounced", false),
      new Trait("chinPosition", false),
      new Trait("mandibleSize", false),
      new Trait("jawsSize", false),
      new Trait("jawsPosition", false),
      new Trait("cheekSize", false),
      new Trait("cheekPosition", false),
      new Trait("lowCheekPronounced", false),
      new Trait("lowCheekPosition", false),
      new Trait("foreheadSize", false),
      new Trait("foreheadPosition", false),
      new Trait("lipsSize", false),
      new Trait("mouthSize", false),
      new Trait("eyeRotation", false),
      new Trait("eyeSize", false),
      new Trait("breastSize", true),
      new Trait("eyeSpacing", false)
    };


  public void StartValidations(UMA.UMAData umaData)
  {
    targetMaleAvatar.gameObject.SetActive(false);
    targetFemaleAvatar.gameObject.SetActive(false);

    Randomize();
  }

  public void Randomize()
  {
    if (getFromDB)
    {
      RandomizeFromDB();
    }
    else
    {
      RandomizeLocal();
    }
  }


  public void RandomizeFromDB()
  {
    int index;
    float newValue;
    Dictionary<string, float> randomizedDNA = new Dictionary<string, float>();

    //possible values
    RaceFinalData[] races = RandomizerGetFromDB.instance.FinalData.finalData;
    RaceFinalData race = races[UnityEngine.Random.Range(0, races.Length)];
    DNA_Item[] raceItens = race.values;
    DNA_Item item = raceItens[UnityEngine.Random.Range(0, raceItens.Length)];

    // randomize gender
    UMA.CharacterSystem.DynamicCharacterAvatar nextAvatar = race.key == "HumanMale" ? targetMaleAvatar : targetFemaleAvatar;
    if (currentAvatar != nextAvatar)
    {
      if (currentAvatar) StartCoroutine(ActivateAvatar(currentAvatar.gameObject, false));
      StartCoroutine(ActivateAvatar(nextAvatar.gameObject, true));
      nextAvatar.transform.position = anchorPoint.position;
    }
    currentAvatar = nextAvatar;

    currentRace = currentAvatar.activeRace.name;

    // change traits
    UMA.UMADnaBase[] DNA = currentAvatar.GetAllDNA();
    UMA.UMADnaBase dnaData = DNA[0];
    string[] _dnaFieldNames = dnaData.Names;

    foreach (var trait in item.fields)
    {
      index = Array.IndexOf(_dnaFieldNames, trait.field);
      newValue = trait.value;
      dnaData.SetValue(index, newValue);
      randomizedDNA.Add(trait.field, newValue);
    }

    currentDNA = randomizedDNA;

    currentAvatar.ForceUpdate(true, false, false);
  }

  public void RandomizeLocal()
  {
    int index;
    float newValue;
    Dictionary<string, float> randomizedDNA = new Dictionary<string, float>();

    // randomize gender
    UMA.CharacterSystem.DynamicCharacterAvatar nextAvatar = UnityEngine.Random.Range(0, 1f) > 0.5f ? targetMaleAvatar : targetFemaleAvatar;
    if (currentAvatar != nextAvatar)
    {
      if (currentAvatar) StartCoroutine(ActivateAvatar(currentAvatar.gameObject, false));
      StartCoroutine(ActivateAvatar(nextAvatar.gameObject, true));
      nextAvatar.transform.position = anchorPoint.position;
    }
    currentAvatar = nextAvatar;

    currentRace = currentAvatar.activeRace.name;

    // change traits
    UMA.UMADnaBase[] DNA = currentAvatar.GetAllDNA();
    UMA.UMADnaBase dnaData = DNA[0];
    string[] _dnaFieldNames = dnaData.Names;


    foreach (var trait in possibleValues)
    {
      if (trait.Available)
      {
        index = Array.IndexOf(_dnaFieldNames, trait.Name);
        newValue = UnityEngine.Random.Range(trait.MinValue, trait.MaxValue);
        dnaData.SetValue(index, newValue);
        randomizedDNA.Add(trait.Name, newValue);
      }
    }

    currentDNA = randomizedDNA;

    currentAvatar.ForceUpdate(true, false, false);

  }

  IEnumerator ActivateAvatar(GameObject avatar, bool activate)
  {
    yield return null;
    yield return null;
    if (avatar) avatar.gameObject.SetActive(activate);
  }

  public void Save()
  {
    UMADataLogger.instance.Save();
  }
}


public class Trait
{
  string name;
  bool available;
  float minValue;
  float maxValue;

  public Trait(string name, bool available, float minValue = 0f, float maxValue = 1f)
  {
    this.name = name;
    this.available = available;
    this.minValue = minValue;
    this.maxValue = maxValue;
  }

  public string Name { get => name; }
  public bool Available { get => available; }
  public float MinValue { get => minValue; }
  public float MaxValue { get => maxValue; }
}