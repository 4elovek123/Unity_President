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

    public void CalcClimate() // рассчитывается при нажатии кнопки "Ready" 
    {
        _TotalClimate = DataHolder._GeneralClimate; // достаём глобальное поле
        
        for (int i = 0; i < 3; i++)
        {
            _climatePresident[i] = _FactorSlotPresident[i]._climate;
            if (_climatePresident[i] != _TotalClimate)
            {   
                // Климат на совпал 
                _FactorSlotPresident[i]._buff_attack += -1;
                _FactorSlotPresident[i]._buff_diplomation += -1;
                _FactorSlotPresident[i]._buff_fortune += -1;
                _FactorSlotPresident[i]._buff_protection += -1;
                Debug.Log("_111");

            }
            else 
            {
                // Климат совпал
                _FactorSlotPresident[i]._buff_attack += 2;
                _FactorSlotPresident[i]._buff_diplomation += 2;
                _FactorSlotPresident[i]._buff_fortune += 2;
                _FactorSlotPresident[i]._buff_protection += 2;
            }
        }
    }

    public void ReadyFight() // рассчитывается при нажатии кнопки "Ready" 
    {
        for (int i = 0; i < 3; i++)
        {
            MoralePresident[i] = _FactorSlotPresident[i]._BUFFmaterials + _FactorSlotPresident[i]._BUFFeconomic + _FactorSlotPresident[i]._BUFFhealth + _FactorSlotPresident[i]._BUFFfood;
            _totalMoralePresident += MoralePresident[i];
        }
        Debug.Log("Общая мораль - " + _totalMoralePresident);
        _canvasCamera.transform.Find("Text_TotalMorale").GetComponent<Text>().text = "You morale " + _totalMoralePresident;
        _canvasCamera.transform.Find("Text_TotalMoraleEnemy").GetComponent<Text>().text = "Enemy morale "; // + _totalMoralePresident;
    }
} 