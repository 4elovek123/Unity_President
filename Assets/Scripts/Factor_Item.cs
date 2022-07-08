using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using System.IO;
using UnityEngine.UI;

public class Factor_Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup _canvasGroup; 
    private Canvas _mainCanvas; 
    private RectTransform _rectTransform;
    private Transform _parent; // ����, ���������
    private Transform _praparent; // ���, ������ 
    private Transform _factors; // "�����" � ���������
    Transform[] childrenFactors; 
    private string _DragFactorName;
    private bool _FactorOnDrop = false;

    public JSONController jSONController;
    JSONController.ItemList myItemList;
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
    public int _food_ability_attack = 0;

    public string _enterFactor = ""; // ������, ������� �������� ����� ��� �������������� 
    private Text[] texts; // ������ ��������� ����������� � ���������� 
    private int counter_player = 0; // ������� ����� ���������� �� ������� ����������� (j � �����) 
    private string _descriptionFactor = "";
    private string _mainHeader = ""; 
    public int _BUFFmaterials;
    public int _BUFFeconomic;
    public int _BUFFhealth;
    public int _BUFFfood;

    public Image[] ImageBuf = new Image[3];

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>(); 
        _canvasGroup = GetComponent<CanvasGroup>();
        _parent = transform.parent;//.transform;
        _mainCanvas = _parent.GetComponentInParent<Canvas>(); 
        _praparent = _parent.transform.parent;
        _factors = _praparent.transform.Find("Factors");
        childrenFactors = new Transform[_factors.childCount]; 

        for (int i = 0; i < _factors.childCount; i++) // �������� ������ childrenFactors ��������� �����. ����� ���� ������������ foreach
        {
            childrenFactors[i] = _factors.GetChild(i).transform;
        }

        ////////// ������������ JSON � ���, ��� � ��� ������� 

        myItemList = jSONController.myItemList; // ���������� ���� �� ������� 
        for (int j = 0; j < myItemList.player.Length; j++) // ����������� �� ���� ����������� �� JSON-����� 
        {
            if (_parent.name == myItemList.player[j].id) // ����� ���������� ���������� ����� ������� � ������������ � JSON
            {
                counter_player = j; // ���������, � ����� ������ ������� ������ player (���������)

                texts = this.GetComponentsInChildren<Text>(); // ��������� ��������� ���������� � ���������� ����������     

                // ����� � ���������� �� �� JSON 
                _name = myItemList.player[j].name;
                _level = myItemList.player[j].level;
                _climate = myItemList.player[j].climate; 

                _buff_diplomation_delta = myItemList.player[j].buff_diplomation;
                _buff_protection_delta = myItemList.player[j].buff_protection;
                _buff_attack_delta = myItemList.player[j].buff_attack;
                _buff_fortune_delta = myItemList.player[j].buff_fortune;

                CalcClimate(); // ������ ����������� ������� 

                _buff_diplomation = _BUFF_Initial + _buff_diplomation_delta;
                _buff_fortune = _BUFF_Initial + _buff_fortune_delta;
                _buff_protection = _BUFF_Initial + _buff_protection_delta;
                _buff_attack = _BUFF_Initial + _buff_attack_delta;
                _factor_materials = myItemList.player[j].factor_materials;
                _factor_economic = myItemList.player[j].factor_economic;
                _factor_health = myItemList.player[j].factor_health;
                _factor_food = myItemList.player[j].factor_food; 
                _BUFFmaterials = (_buff_fortune + _buff_attack) / 2;
                _BUFFeconomic = (_buff_attack + _buff_diplomation) / 2;
                _BUFFhealth = (_buff_protection + _buff_diplomation) / 2;
                _BUFFfood = (_buff_protection + _buff_fortune) / 2;
            }
        } 



        WriteVariables(); // ������ ������� �������� ��� ������ 

        //////////////////////// 


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.transform.SetParent(_praparent); // ������ �������� - ��� ���� ���������
        _canvasGroup.blocksRaycasts = false;

        for (int n = 0; n < _factors.childCount; n++) // ����������� �� ������� ��������
        {
            childrenFactors[n].GetComponent<Factor_Slot>()._targetDragIsTrue = false; // � �������� �� ������ ������� 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _mainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(_parent); // ������ �������� �������, ���� ���� ��������� 
        transform.localPosition = Vector3.zero;
        _canvasGroup.blocksRaycasts = true;

        for (int i = 0; i < _factors.childCount; i++) // ����������� �� ������� ��������
        {
            if (childrenFactors[i].GetComponent<Factor_Slot>()._targetDragIsTrue == true) // ���� ������ � ������� �������, ��� �� ���� ������ pointerDrag 
            {
                _DragFactorName = childrenFactors[i].name; // ��������� ��� �������, �� ������� ��������� ���������� 
                _FactorOnDrop = true;
                calc(counter_player);
            }
        }        
    } 

    void calc(int k)
    {
        if (!_FactorOnDrop) // ���� ��� �� �������� �������
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
        else // ���� ���������� �������, �� �������� ������ ���������� ������, ����� ����� 
        {
            if (myItemList.player[k].factor_materials != "" && _DragFactorName == "Materials")
            {
                Materials(counter_player);
            } else

            if (myItemList.player[k].factor_economic != "" && _DragFactorName == "Economic")
            {
                Economic(counter_player);
            } else

            if (myItemList.player[k].factor_health != "" && _DragFactorName == "Health")
            {
                Health(counter_player);
            } else

            if (myItemList.player[k].factor_food != "" && _DragFactorName == "Food")
            {
                Food(counter_player);
            } else Reset();
            
        }

        WriteVariables(); // ������ ������� ��������  

    }

    void Materials(int j)
    {
        _mainHeader = "�����";
        _descriptionFactor = "" + _factor_materials; //������ � ��������� �������� Abilities
        _enterFactor = "Materials"; // ���� ����� �� �������� �� ������ �������, ������, ������, ������� ������� ����� ��� �������������� - ��������� 
        _materials_ability_protect = myItemList.player[j].materials_ability_protect;
        _economic_ability_protect = 0; // ��������, �.�. ������� ����� �� ����� 
        _economic_ability_attack = 0; // ��������, �.�. ������� ����� �� ����� 
        _health_ability_protect = 0;// ��������, �.�. ������� ����� �� ����� 
        _food_ability_protect = 0; // ��������, �.�. ������� ����� �� ����� 
        _food_ability_attack = 0;
    }

    void Economic(int j)
    {
        //texts[0].text = "���������";
        _mainHeader = "���������";
        //texts[1].text = _factor_economic;
        _descriptionFactor = _factor_economic;
        _enterFactor = "Economic"; // �� ����� ����� ������ 
        _materials_ability_protect = 0;
        _economic_ability_protect = myItemList.player[j].economic_ability_protect; // ��� ������ 
        _economic_ability_attack = myItemList.player[j].economic_ability_attack; // ��� �����
        _health_ability_protect = 0;
        _food_ability_protect = 0;
        _food_ability_attack = 0;
    }

    void Health(int j)
    {

        //texts[0].text = "���������������";
        _mainHeader = "���������������";
        //texts[1].text = _factor_health;
        _descriptionFactor = _factor_health;
        _enterFactor = "Health"; // �� ����� ����� ������
        _materials_ability_protect = 0;
        _economic_ability_protect = 0;
        _economic_ability_attack = 0;
        _health_ability_protect = myItemList.player[j].health_ability_protect;
        _food_ability_protect = 0;
        _food_ability_attack = 0;
    }

    void Food(int j)
    {
        //texts[0].text = "��������������";
        _mainHeader = "��������������";
        _descriptionFactor = _factor_food;
        _enterFactor = "Food"; // �� ����� ����� ������ 
        _materials_ability_protect = 0;
        _economic_ability_protect = 0;
        _economic_ability_attack = 0;
        _health_ability_protect = 0;
        _food_ability_protect = myItemList.player[j].food_ability_protect;
        _food_ability_attack = myItemList.player[j].food_ability_attack; ///////////////////
    }

    void Reset()
    {
        _mainHeader = "";
        _descriptionFactor = ""; //������ � ��������� �������� Abilities
        _enterFactor = ""; // ���� ����� �� �������� �� ������ �������, ������, ������ �������  
        _materials_ability_protect = 0; 
        _economic_ability_protect = 0; 
        _economic_ability_attack = 0; 
        _health_ability_protect = 0;
        _food_ability_protect = 0;
        _food_ability_attack = 0; 
    }

    void WriteVariables()
    {
        // ������� � ���������� ������ ������ �������� ����������. ����� ���� ������������ ��� �� ������ texts[].text, ��, ������� ������� ��������� ������, ����� �������� ������ ��������� 
        _parent.transform.Find("Text_Factor").GetComponent<Text>().text = _mainHeader;
        _parent.transform.Find("Text_Abilities").GetComponent<Text>().text = _descriptionFactor; //������ � ��������� �������� Abilities 
        _parent.transform.Find("Text_MaterialProtect").GetComponent<Text>().text = "" + _materials_ability_protect;
        _parent.transform.Find("Text_EconomicProtect").GetComponent<Text>().text = "" + _economic_ability_protect;
        _parent.transform.Find("Text_EconomicAttack").GetComponent<Text>().text = "" + _economic_ability_attack;
        _parent.transform.Find("Text_HealthProtect").GetComponent<Text>().text = "" + _health_ability_protect;
        _parent.transform.Find("Text_FoodProtect").GetComponent<Text>().text = "" + _food_ability_protect;
        this.transform.Find("Text_Level").GetComponent<Text>().text = "" + _level;
        this.transform.Find("Text_Climate").GetComponent<Text>().text = "" + _climate;
        this.transform.Find("Text_NamePresident").GetComponent<Text>().text = "" + _name;
        this.transform.Find("Text_BufDiplomation").GetComponent<Text>().text = "" + _buff_diplomation;
        this.transform.Find("Text_BufFortune").GetComponent<Text>().text = "" + _buff_fortune;
        this.transform.Find("Text_BufProtection").GetComponent<Text>().text = "" + _buff_protection;
        this.transform.Find("Text_BufAttack").GetComponent<Text>().text = "" + _buff_attack;

        if (_buff_diplomation_delta > 0) this.transform.Find("Text_BufDiplomationDelta").GetComponent<Text>().text = "+" + _buff_diplomation_delta;
        else this.transform.Find("Text_BufDiplomationDelta").GetComponent<Text>().text = "" + _buff_diplomation_delta;


        if (_buff_fortune_delta > 0) this.transform.Find("Text_BufFortuneDelta").GetComponent<Text>().text = "+" + _buff_fortune_delta;
        else this.transform.Find("Text_BufFortuneDelta").GetComponent<Text>().text = "" + _buff_fortune_delta;


        if (_buff_protection_delta > 0) this.transform.Find("Text_BufProtectionDelta").GetComponent<Text>().text = "+" + _buff_protection_delta;
        else this.transform.Find("Text_BufProtectionDelta").GetComponent<Text>().text = "" + _buff_protection_delta; 


        if (_buff_attack_delta > 0) this.transform.Find("Text_BufAttackDelta").GetComponent<Text>().text = "+" + _buff_attack_delta;
        else this.transform.Find("Text_BufAttackDelta").GetComponent<Text>().text = "" + _buff_attack_delta;


















        ColorRect(); // ������ ������������� 
    }

    void ColorRect()
    {
        if (_buff_attack_delta > 0) // ���� ��������, �������� �� �������� ���, �������������
        {
            ImageBuf[0].GetComponent<Image>().color = new Color(0, 1, 0); // ������ 
        }
        else
        {
            if (_buff_attack_delta == 0) // ���� 0
            {
                ImageBuf[0].GetComponent<Image>().color = new Color(1, 1, 1); // �����
            }
            else // ���� �� ������������� 
            {
                ImageBuf[0].GetComponent<Image>().color = new Color(1, 0, 0); //�������
            }
        }
        
        if (_buff_protection_delta > 0) // ���� ��� �������������
        {
            ImageBuf[1].GetComponent<Image>().color = new Color(0, 1, 0);
        }
        else
        {
            if (_buff_attack_delta == 0)
            {
                ImageBuf[1].GetComponent<Image>().color = new Color(1, 1, 1);
            }
            else
            {
                ImageBuf[1].GetComponent<Image>().color = new Color(1, 0, 0);
            }
        }

        if (_buff_fortune_delta > 0) // ���� ��� �������������
        {
            ImageBuf[2].GetComponent<Image>().color = new Color(0, 1, 0);
        }
        else
        {
            if (_buff_attack_delta == 0)
            {
                ImageBuf[2].GetComponent<Image>().color = new Color(1, 1, 1);
            }
            else
            {
                ImageBuf[2].GetComponent<Image>().color = new Color(1, 0, 0);
            }
        }
        
        if (_buff_diplomation_delta > 0) // ���� ��� �������������
        {
            ImageBuf[3].GetComponent<Image>().color = new Color(0, 1, 0);
        }
        else
        {
            if (_buff_attack_delta == 0)
            {
                ImageBuf[3].GetComponent<Image>().color = new Color(1, 1, 1);
            }
            else
            {
                ImageBuf[3].GetComponent<Image>().color = new Color(1, 0, 0);
            }
        }
    }

    void CalcClimate() // ��������� ��������� ���� ��-�� ������� 
    {
        string _TotalClimate = DataHolder._GeneralClimate; // ������ ���������� ����
        if (_climate != _TotalClimate) // ������ �� ������ 
        {
            _buff_attack_delta += -1;
            _buff_diplomation_delta += -1;
            _buff_fortune_delta += -1;
            _buff_protection_delta += -1;
        }
        else
        {
            _buff_attack_delta += 2; // ������ ������
            _buff_diplomation_delta += 2;
            _buff_fortune_delta += 2;
        _buff_protection_delta += 2;
        }
    }

    void Update()
    {

    }
} 