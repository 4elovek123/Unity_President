using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;

public class Factor_Slot : MonoBehaviour, IDropHandler
{
    private string DragItemName;
    public JSONController jSONController;
    private bool _FactorOnDrop = false;

    public string _name = "";
    public int _level = 0;
    public string _climate = ""; // ������ ���������� 
    public int _buff_diplomation = 0;
    public int _buff_diplomation_delta = 0;
    public int _buff_fortune = 0;
    public int _buff_fortune_delta = 0;
    public int _buff_protection = 0;
    public int _buff_protection_delta = 0;
    public int _buff_attack = 0;
    private int _BUFF_Initial = 10; // ������������� ��� �������� ����� 10 
    public int _buff_attack_delta = 0;
    public string _factor_materials = ""; 
    public int _materials_ability_protect = 0; 
    public string _factor_economic = "";
    public int _economic_ability_protect = 0; 
    public int _economic_ability_attack = 0; 
    public string _factor_health = "";
    public int _health_ability_protect = 0; 
    public string _factor_food = "";
    public int _food_ability_protect = 0; 
    public string enterFactor = ""; // ������, ������� �������� ����� ��� �������������� 
    private Text[] texts; // ������ ��������� ����������� � ���������� 
    JSONController.ItemList myItemList;
    private int counter_player=0; // ������� ����� ���������� �� ������� ����������� (j � �����) 

    public int _BUFFmaterials;
    public int _BUFFeconomic;
    public int _BUFFhealth;
    public int _BUFFfood;


    public void OnDrop(PointerEventData eventData) // ��� �������������� �������
    {
        DragItemName = eventData.pointerDrag.transform.name; // ���������� ��� ������� (�������) 
        _FactorOnDrop = true; 
        calc(counter_player);
    }

void calc(int k)
    {
        if (!_FactorOnDrop) // ���� �� �������� �������, �� �������� ��������� ���������� ���������� ������
        {
            if (myItemList.player[k].factor_materials != "") // ���� �������� � ����� �� ������, �� ������������ ��� 
            {
                Materials(counter_player);
            } 

            if (myItemList.player[k].factor_economic != "") // ���������� 
            {
                Economic(counter_player);
            }  

            if (myItemList.player[k].factor_health != "")
            {
                Health(counter_player);
            }  

            if (myItemList.player[k].factor_food != "")
            {
                Food(counter_player);
            }
        }
        else // ���� ���������� �������, �� �������� ������ ���������� ������ 
        {
            if (myItemList.player[k].factor_materials != "" && DragItemName == "Materials")
            {
                Materials(counter_player);
            } 

            if (myItemList.player[k].factor_economic != "" && DragItemName == "Economic")
            {
                Economic(counter_player);
            }  

            if (myItemList.player[k].factor_health != "" && DragItemName == "Health")
            {
                Health(counter_player);
            } 

            if (myItemList.player[k].factor_food != "" && DragItemName == "Food")
            {
                Food(counter_player);
            }    
        }

        // ������� � ���������� ������ ������ �������� ����������. ����� ���� ������������ ��� �� ������ texts[].text, ��, ������� ������� ��������� ������, ����� �������� ������ ��������� 
        this.transform.Find("Text_MaterialProtect").GetComponent<Text>().text = "" + _materials_ability_protect;
        this.transform.Find("Text_EconomicProtect").GetComponent<Text>().text = "" + _economic_ability_protect; 
        this.transform.Find("Text_EconomicAttack").GetComponent<Text>().text = "" + _economic_ability_attack;
        this.transform.Find("Text_HealthProtect").GetComponent<Text>().text = "" + _health_ability_protect;
        this.transform.Find("Text_FoodProtect").GetComponent<Text>().text = "" + _food_ability_protect;
        this.transform.Find("Text_Level").GetComponent<Text>().text = "" + _level;
        this.transform.Find("Text_Climate").GetComponent<Text>().text = "" + _climate;
        this.transform.Find("Text_NamePresident").GetComponent<Text>().text = "" + _name;
        this.transform.Find("Text_BufDiplomationDelta").GetComponent<Text>().text = "" + _buff_diplomation_delta;
        this.transform.Find("Text_BufFortuneDelta").GetComponent<Text>().text = "" + _buff_fortune_delta;
        this.transform.Find("Text_BufProtectionDelta").GetComponent<Text>().text = "" + _buff_protection_delta;
        this.transform.Find("Text_BufAttackDelta").GetComponent<Text>().text = "" + _buff_attack_delta; 
        this.transform.Find("Text_BufDiplomation").GetComponent<Text>().text = "" + _buff_diplomation;
        this.transform.Find("Text_BufFortune").GetComponent<Text>().text = "" + _buff_fortune; 
        this.transform.Find("Text_BufProtection").GetComponent<Text>().text = "" + _buff_protection;
        this.transform.Find("Text_BufAttack").GetComponent<Text>().text = "" + _buff_attack;

    }
    
    void Start()
    {
        myItemList = jSONController.myItemList; // ���������� ���� �� ������� 
        for (int j = 0; j < myItemList.player.Length; j++) // ����������� �� ���� ����������� �� JSON-����� 
        {
            if (this.name == myItemList.player[j].id) // ����� ���������� 
            {
                counter_player = j; // ���������, � ����� ������ ������� ������ player (���������)

                texts = this.GetComponentsInChildren<Text>(); // ��������� ��������� ���������� � ���������� ���������� 

                // ����� � ���������� �� �� JSON 
                _name = myItemList.player[j].name;
                _level = myItemList.player[j].level;
                _climate = myItemList.player[j].climate;
                _buff_diplomation_delta = myItemList.player[j].buff_diplomation;
                _buff_fortune_delta = myItemList.player[j].buff_fortune;
                _buff_protection_delta = myItemList.player[j].buff_protection;
                _buff_attack_delta = myItemList.player[j].buff_attack;
                _buff_diplomation = _BUFF_Initial + _buff_diplomation_delta;
                _buff_fortune = _BUFF_Initial + _buff_fortune_delta;
                _buff_protection = _BUFF_Initial + _buff_protection_delta;
                _buff_attack = _BUFF_Initial + _buff_attack_delta;
                _factor_materials = myItemList.player[j].factor_materials; 
                _factor_economic = myItemList.player[j].factor_economic;
                _factor_health = myItemList.player[j].factor_health;
                _factor_food = myItemList.player[j].factor_food;

                calc(counter_player);

                _BUFFmaterials = (_buff_fortune + _buff_attack)/2; 
                _BUFFeconomic = (_buff_attack + _buff_diplomation)/2; 
                _BUFFhealth = (_buff_protection + _buff_diplomation)/2; 
                _BUFFfood = (_buff_protection + _buff_fortune)/2; 
            }
        }
    }

    void Materials(int j)
    {
        this.transform.Find("Text_Factor").GetComponent<Text>().text = "�����"; 
        this.transform.Find("Text_Abilities").GetComponent<Text>().text = ""+ _factor_materials; //������ � ��������� �������� Abilities
        enterFactor = "Materials"; // ���� ����� �� �������� �� ������ �������, ������, ������, ������� ������� ����� ��� �������������� - ��������� 
        _materials_ability_protect = myItemList.player[j].materials_ability_protect;
        _economic_ability_protect = 0; // ��������, �.�. ������� ����� �� ����� 
        _economic_ability_attack = 0; // ��������, �.�. ������� ����� �� ����� 
        _health_ability_protect = 0;// ��������, �.�. ������� ����� �� ����� 
        _food_ability_protect = 0; // ��������, �.�. ������� ����� �� ����� 
    }

    void Economic(int j)
    {
        //texts[0].text = "���������";
        this.transform.Find("Text_Factor").GetComponent<Text>().text = "���������";
        //texts[1].text = _factor_economic;
        this.transform.Find("Text_Abilities").GetComponent<Text>().text = _factor_economic; 
        enterFactor = "Economic"; // �� ����� ����� ������ 
        _materials_ability_protect = 0;
        _economic_ability_protect = myItemList.player[j].economic_ability_protect; // ��� ������ 
        _economic_ability_attack = myItemList.player[j].economic_ability_attack; // ��� �����
        _health_ability_protect = 0;
        _food_ability_protect = 0;
    } 

    void Health(int j)
    {
        enterFactor = "Health"; // �� ����� ����� ������
        //texts[0].text = "���������������";
        this.transform.Find("Text_Factor").GetComponent<Text>().text = "���������������";
        //texts[1].text = _factor_health;
        this.transform.Find("Text_Abilities").GetComponent<Text>().text = _factor_health; 
        _materials_ability_protect = 0;
        _economic_ability_protect = 0;
        _economic_ability_attack = 0;
        _health_ability_protect = myItemList.player[j].health_ability_protect;
        _food_ability_protect = 0;
    } 

    void Food(int j)
    {
        enterFactor = "Food"; // �� ����� ����� ������ 
        //texts[0].text = "��������������";
        this.transform.Find("Text_Factor").GetComponent<Text>().text = "��������������"; 
        //texts[1].text = _factor_food;
        this.transform.Find("Text_Abilities").GetComponent<Text>().text = _factor_food; 
        _materials_ability_protect = 0;
        _economic_ability_protect = 0;
        _economic_ability_attack = 0;
        _health_ability_protect = 0;
        _food_ability_protect = myItemList.player[j].food_ability_protect; 
    }

    void Update()
    {

    }
}