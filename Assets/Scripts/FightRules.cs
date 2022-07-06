using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightRules : MonoBehaviour
{
    public Factor_Slot[] _FactorSlotPresident = new Factor_Slot[3];
    public GameObject[] _President = new GameObject[3];
    public GameObject[] _EnemyPresident = new GameObject[3];

    int[] MoralePresident = new int[3];
    int[] MoralePresidentEnemy = new int[3];
    int _totalMoralePresident = 0;
    public Canvas _canvasCamera;

    string _TotalClimate = "";
    string[] _climatePresident = new string[3];
    void Start()
    {
        
    }

    public void CalcClimate()
    {
        _TotalClimate = DataHolder._GeneralClimate; 

        for (int i = 0; i < 3; i++)
        {
            _climatePresident[i] = _FactorSlotPresident[i]._climate;
            if (_climatePresident[i] != _TotalClimate)
            {
                Debug.Log("Климат не совпал");
            }
            else 
            {
                Debug.Log("Климат совпал"); 
            }
        }
    }

    public void ReadyFight() // рассчитывается при нажатии кнопки "Ready" 
    {
        for (int i = 0; i < 3; i++)
        {
            MoralePresident[i] = _FactorSlotPresident[i]._buff_attack + _FactorSlotPresident[i]._buff_diplomation + _FactorSlotPresident[i]._buff_fortune + _FactorSlotPresident[i]._buff_protection;
            _totalMoralePresident += MoralePresident[i];
        }
        Debug.Log("Общая мораль - " + _totalMoralePresident);
        _canvasCamera.transform.Find("Text_TotalMorale").GetComponent<Text>().text = "You morale " + _totalMoralePresident;
        _canvasCamera.transform.Find("Text_TotalMoraleEnemy").GetComponent<Text>().text = "Enemy morale " + _totalMoralePresident;
    }
} 