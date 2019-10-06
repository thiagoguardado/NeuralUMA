using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidationUI : MonoBehaviour
{
  public UMARandomizer umaRandomizer;

  public GameObject startPanel;
  public GameObject thanksPanel;

  void Start()
  {
    startPanel.SetActive(true);
    thanksPanel.SetActive(false);
  }

  public void StartButton()
  {
    startPanel.SetActive(false);
    umaRandomizer.StartValidations(null);
  }
  public void SaveButton()
  {
    umaRandomizer.Save();
    thanksPanel.SetActive(true);
  }

  public void ContinueButton(){
    thanksPanel.SetActive(false);
  }

  public void AcceptDNA()
  {
    // AnalyticsReport.ReportPlausible(currentDNA);
    DataLogger.instance.LogDNAItem(true, umaRandomizer.CurrentDNA,umaRandomizer.CurrentRace);
    umaRandomizer.Randomize();
  }

  public void RejectDNA()
  {
    // AnalyticsReport.ReportNotPlausible(currentDNA);
    DataLogger.instance.LogDNAItem(false, umaRandomizer.CurrentDNA,umaRandomizer.CurrentRace);
    umaRandomizer.Randomize();
  }

}
