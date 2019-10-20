using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ViewerController : MonoBehaviour
{
  public UMASpawner spawner;
  public UMADataReader reader;
  public ViewerCanvas viewerCanvas;
  public string outputFile;
  public bool useLocalData = false;

  void Awake()
  {
    viewerCanvas.EpochChange += OnDropDownSelect;
  }

  void Start()
  {
    StartCoroutine(Load());
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.R))
    {
      viewerCanvas.SetText("Reloading...");
      SceneManager.LoadSceneAsync("Viewer");
    }
  }

  IEnumerator Load()
  {
    if (useLocalData)
    {
      reader.ReadOutputData(Application.streamingAssetsPath + "/GanData/" + outputFile);
    }
    else
    {
      reader.ReadMongoData();
    }
    
    while (!reader.IsDone) yield return null;

    MLData mlData = reader.MlData;
    viewerCanvas.Setup(mlData);
  }

  void OnDropDownSelect(DNA_Item[] itens)
  {
    spawner.SpawnItens(itens);
  }
}
