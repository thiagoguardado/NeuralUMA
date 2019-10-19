using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ViewerCanvas : MonoBehaviour
{
  public Dropdown genderDropdown;
  public Dropdown epochDropdown;
  public delegate void ValueChange(string newValue);
  public event ValueChange DropDownChange;
  private List<DNAFolder> dnaFolders = new List<DNAFolder>();
  private DNAFolder currentParentFolder;

  void Start()
  {
    genderDropdown.onValueChanged.AddListener(delegate { OnGenderDropDownSelect(genderDropdown); });
    epochDropdown.onValueChanged.AddListener(delegate { OnEpochDropDownSelect(epochDropdown); });
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      ScrollDropdown(epochDropdown, false);
    }
    else if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      ScrollDropdown(epochDropdown, true);
    }

    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      ScrollDropdown(genderDropdown, false);
    }
    else if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      ScrollDropdown(genderDropdown, true);
    }
  }

  private void ScrollDropdown(Dropdown dropdown, bool forward)
  {
    int newValue = dropdown.value + (forward ? 1 : -1);
    newValue = Mathf.Clamp(newValue, 0, dropdown.options.Count);
    if (newValue != dropdown.value)
      dropdown.value = newValue;
  }

  public void AddFoldersToDropdown(string folder)
  {

    List<string> parentPaths = GetPaths(Path.Combine(Application.dataPath, folder));
    List<string> parentShortPaths = new List<string>();
    foreach (string parentPath in parentPaths)
    {
      string[] splittedParent = parentPath.Split('\\');
      string parentShort = splittedParent[splittedParent.Length - 1];
      parentShortPaths.Add(parentShort);

      List<string> childrenPaths = GetPaths(parentPath);
      List<string> childShort = new List<string>();
      string[] splittedChild;
      foreach (string childPath in childrenPaths)
      {
        splittedChild = childPath.Split('\\');
        childShort.Add(splittedChild[splittedChild.Length - 1]);

      }
      DNAFolder dnaFolder = new DNAFolder(parentShort, childrenPaths, childShort);
      dnaFolders.Add(dnaFolder);
    }

    genderDropdown.AddOptions(parentShortPaths);
    ChangeParentFolder(0);
  }

  List<string> GetPaths(string folder)
  {
    DirectoryInfo info = new DirectoryInfo(folder);
    DirectoryInfo[] dirInfo = info.GetDirectories();
    List<string> paths = new List<string>();
    foreach (DirectoryInfo dir in dirInfo)
    {
      paths.Add(dir.FullName);
    }
    return paths;
  }

  void ChangeParentFolder(int index)
  {
    currentParentFolder = dnaFolders[index];
    epochDropdown.ClearOptions();
    epochDropdown.AddOptions(currentParentFolder.childrenFolders);
    epochDropdown.value = 0;
    OnEpochDropDownSelect(epochDropdown);
  }


  void OnGenderDropDownSelect(Dropdown dropdown)
  {
    ChangeParentFolder(dropdown.value);
  }

  void OnEpochDropDownSelect(Dropdown dropdown)
  {
    if (DropDownChange != null)
      DropDownChange.Invoke(currentParentFolder.childrenFolderFull[dropdown.value]);
  }

  [System.Serializable]
  public class DNAFolder
  {
    public string parentFolder;
    public List<string> childrenFolderFull;
    public List<string> childrenFolders;

    public DNAFolder(string parentFolder, List<string> childrenFolderFull, List<string> childrenFolders)
    {
      this.parentFolder = parentFolder;
      this.childrenFolderFull = childrenFolderFull;
      this.childrenFolders = childrenFolders;
    }
  }
}
