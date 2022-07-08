using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightRules : MonoBehaviour
{
    public Factor_Item[] _FactorItemPresident = new Factor_Item[3];
    //public GameObject[] _President = new GameObject[3];
    //public GameObject[] _EnemyPresident = new GameObject[3];

    int[] MoralePresident = new int[3];
    int[] MoralePresidentEnemy = new int[3];
    int _totalMoralePresident = 0;
    public Canvas _canvasCamera;
    public Transform[] _Presidents_before;
    public Transform[] _PresidentsEnemy_before;
    public Transform[] _Place_after; 
    public Transform[] _PlaceEnemy_after;

    public Transform _FactorEconomic;
    public Transform _FactorHealth;
    public Transform _FactorMaterials; 
    public Transform _FactorFood;

    private Factor_Item[] _scriptFactorItem;

    //Transform _rootGO;
    //private Transform _son; // сын 

    void Start()
    {

    }

    public void StartButton()
    {
        for (int i = 0; i < _Presidents_before.Length; i++)
        {
            _Presidents_before[i].transform.SetParent(_Place_after[i].transform);
            _Presidents_before[i].transform.localPosition = Vector3.zero;
            _Presidents_before[i].transform.localRotation = Quaternion.identity;
            _Presidents_before[i].transform.localScale = Vector3.one;
            _Presidents_before[i].transform.GetComponent<Image>().enabled = false;
            _Presidents_before[i].transform.Find("Text_Factor").GetComponent<Text>().enabled = false;
            _Presidents_before[i].transform.Find("Text_Abilities").GetComponent<Text>().enabled = false;
            _Presidents_before[i].transform.Find("Card").GetComponent<CanvasGroup>().blocksRaycasts = false; //временное решение. Выключаем канвасгрупп рейкаст, чтобы не смогли попасть по UI президентов, когда сидим за столом 
        }

        for (int i = 0; i < _PresidentsEnemy_before.Length; i++)
        {
            _PresidentsEnemy_before[i].transform.SetParent(_PlaceEnemy_after[i].transform);
            _PresidentsEnemy_before[i].transform.localPosition = Vector3.zero;
            _PresidentsEnemy_before[i].transform.localRotation = Quaternion.identity;
            _PresidentsEnemy_before[i].transform.localScale = Vector3.one;
            _PresidentsEnemy_before[i].transform.GetComponent<Image>().enabled = false;
            _PresidentsEnemy_before[i].transform.Find("Text_Factor").GetComponent<Text>().enabled = false;
            _PresidentsEnemy_before[i].transform.Find("Text_Abilities").GetComponent<Text>().enabled = false;
            _PresidentsEnemy_before[i].transform.Find("Card").GetComponent<CanvasGroup>().blocksRaycasts = false; //временное решение. Выключаем канвасгрупп рейкаст, чтобы не смогли попасть по UI президентов, когда сидим за столом 
        }

        ReadyFight();
        /*
        for (int i = 0; i < _PresidentsEnemy_before.Length; i++)
        {
            if (_scriptFactorItem[i]._enterFactor == "Economic")
            { 
                
            }
        }
        */
        
    }
    public void ReadyFight() // рассчитывается при нажатии кнопки "Ready" 
    {
        for (int i = 0; i < 3; i++) // считаем мораль 
        {
            MoralePresident[i] = _FactorItemPresident[i]._BUFFmaterials + _FactorItemPresident[i]._BUFFeconomic + _FactorItemPresident[i]._BUFFhealth + _FactorItemPresident[i]._BUFFfood;
            _totalMoralePresident += MoralePresident[i];
        }

        _canvasCamera.transform.Find("Text_TotalMorale").GetComponent<Text>().text = "You morale " + _totalMoralePresident;
        _canvasCamera.transform.Find("Text_TotalMoraleEnemy").GetComponent<Text>().text = "Enemy morale "; // + _totalMoralePresident;
    } 
} 