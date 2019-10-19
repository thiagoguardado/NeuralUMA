using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UMADataReader : MonoBehaviour
{
  public static UMADataReader instance;
  void Awake()
  {
    instance = this;
  }

  public string[] ReadFolder(string relativePath)
  {
    List<string> result = new List<string>();
    string fullPath = Path.Combine(Application.dataPath, relativePath);

    if (Directory.Exists(fullPath))
    {

      string[] filesPaths = Directory.GetFiles(fullPath);
      foreach (var filePath in filesPaths)
      {
        if (Path.GetExtension(filePath) == ".json")
        {
          string fileContents = File.ReadAllText(filePath);
          result.Add(fileContents);
        }
      }
    }
    
    return result.ToArray();
  }


}
