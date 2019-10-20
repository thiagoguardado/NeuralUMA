using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ViewerCanvas : MonoBehaviour
{
  public Dropdown genderDropdown;
  public Dropdown epochDropdown;
  public RectTransform loadingPanel;
  public Text loadingPanelText;
  public delegate void EpochChanged(DNA_Item[] dnaItens);
  public event EpochChanged EpochChange;
  private List<EpochList> dnaFolders = new List<EpochList>();
  private MLData mLData;

  void Start()
  {
    genderDropdown.onValueChanged.AddListener(delegate { OnGenderDropDownSelect(genderDropdown); });
    epochDropdown.onValueChanged.AddListener(delegate { OnEpochDropDownSelect(epochDropdown); });

    SetText("Querying database...");
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
    newValue = Mathf.Clamp(newValue, 0, dropdown.options.Count - 1);
    if (newValue != dropdown.value)
    {
      dropdown.value = newValue;
    }
  }

  public void Setup(MLData mlData)
  {
    // hide loading
    loadingPanel.gameObject.SetActive(false);

    this.mLData = mlData;

    LoadGenderDropdown();
    LoadEpochesDropdown();

    OnEpochDropDownSelect(epochDropdown);
  }

  public void SetText(string text)
  {
    loadingPanelText.text = text;
    loadingPanel.gameObject.SetActive(true);
  }
  public void LoadGenderDropdown()
  {
    genderDropdown.ClearOptions();
    List<string> races = new List<string>();
    foreach (RaceData race in mLData.data)
    {
      races.Add(race.key);
    }
    genderDropdown.AddOptions(races);
    genderDropdown.value = 0;
  }

  private void LoadEpochesDropdown()
  {
    epochDropdown.ClearOptions();
    List<string> epoches = new List<string>();
    foreach (EpochData epoch in mLData.data[genderDropdown.value].values)
    {
      epoches.Add(epoch.key);
    }
    epochDropdown.AddOptions(epoches);
    epochDropdown.value = 0;
  }


  void OnGenderDropDownSelect(Dropdown dropdown)
  {
    LoadEpochesDropdown();
  }

  void OnEpochDropDownSelect(Dropdown dropdown)
  {
    if (EpochChange != null)
      EpochChange.Invoke(mLData.data[genderDropdown.value].values[epochDropdown.value].values);
  }

  [System.Serializable]
  public class EpochList
  {
    public string raceKey;
    public List<string> epochesKeys;

    public EpochList(string raceKey, List<string> epochesKeys)
    {
      this.raceKey = raceKey;
      this.epochesKeys = epochesKeys;
    }
  }
}
