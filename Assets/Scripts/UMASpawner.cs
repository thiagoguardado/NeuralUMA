using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMASpawner : MonoBehaviour
{
  public Transform spawnPoint;
  public GameObject maleUmaPrefab;
  public GameObject femaleUmaPrefab;
  public float gridSize;
  private Vector3 initialPos;

  void Awake()
  {
    initialPos = transform.localPosition;
  }

  public void SpawnItens(DNA_Item[] itens)
  {
    // delete todos os umas atuais
    Clear();

    // calcula formato do grid
    int gridSlots = 0;
    while (itens.Length > gridSlots * gridSlots) gridSlots++;

    int instantiated = 0;
    for (int i = 0; i < itens.Length; i++)
    {
      Vector3 pos = initialPos + (Vector3.right * gridSize * (instantiated % gridSlots)) + (Vector3.forward * gridSize * (float)Math.Floor((float)instantiated / gridSlots));
      ProcessDNAItem(itens[i], pos);
      instantiated++;
    }

    // desloca spawn point para centralizar tudo
    spawnPoint.localPosition = initialPos + new Vector3(-1, 0, -1) * (gridSlots - 1) * gridSize / 2;
  }

  void Clear()
  {
    foreach (Transform child in spawnPoint.transform)
    {
      GameObject.Destroy(child.gameObject);
    }
  }


  void ProcessDNAItem(DNA_Item item, Vector3 spawnPos)
  {
    GameObject objToInstantiate = (item.race == "HumanMale" ? maleUmaPrefab : (item.race == "HumanFemale" ? femaleUmaPrefab : null));

    if (objToInstantiate == null) return;

    GameObject go = Instantiate(objToInstantiate, spawnPoint, false);
    go.transform.localPosition = spawnPos;
    go.transform.rotation = Quaternion.Euler(0, 180, 0);

    UMA.CharacterSystem.DynamicCharacterAvatar avatar = go.GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>();

    StartCoroutine(SetupUMA(avatar, item));

  }

  IEnumerator SetupUMA(UMA.CharacterSystem.DynamicCharacterAvatar avatar, DNA_Item item)
  {
    yield return null;
    yield return null;
    yield return null;

    UMA.UMADnaBase[] DNA = avatar.GetAllDNA();
    UMA.UMADnaBase dnaData = DNA[0];
    string[] _dnaFieldNames = dnaData.Names;

    int traitIndex;
    foreach (var trait in item.fields)
    {
      traitIndex = Array.IndexOf(_dnaFieldNames, trait.field);
      dnaData.SetValue(traitIndex, trait.value);
    }

    avatar.ForceUpdate(true, false, false);

  }
}
