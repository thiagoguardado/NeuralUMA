using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerGetFromDB : MonoBehaviour
{
  public static RandomizerGetFromDB instance;
  public UMADataReader reader;
  public GameObject loadingPanel;
  private MLFinalData finalData;
  public MLFinalData FinalData { get => finalData; set => finalData = value; }

  void Awake()
  {
    instance = this;

    loadingPanel.SetActive(true);
  }

  void Start()
  {
    StartCoroutine(Load());
  }

  IEnumerator Load()
  {

    reader.ReadMongoFinalData();

    while (!reader.IsDone) yield return null;

    finalData = reader.MlFinalData;
    loadingPanel.SetActive(false);
  }

}
