using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ViewerController : MonoBehaviour
{
  public ViewerCanvas viewerCanvas;
  public string parentDataPath;

  void Start()
  {
    viewerCanvas.DropDownChange += OnDropDownSelect;

    ListFolders(parentDataPath);
  }

  void ListFolders(string folder)
  {
    viewerCanvas.AddFoldersToDropdown(folder);
  }

  void OnDropDownSelect(string folder)
  {
    UMASpawner.instance.SpawnFromFolder(folder);
  }
}
