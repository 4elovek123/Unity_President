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
    private Transform _parent; // отец, президент
    private Transform _praparent; // дед, канвас 
    private Transform _factors; // "папка" с факторами
    Transform[] childrenFactors; 
    private string _DragFactorName;
    private bool _FactorOnDrop = false;

    public JSONController jSONController;
    JSONController.ItemList myItemList;
    public string _name = "";
    public int _level = 0;
    public string _climate = ""; // климат президента 
    public int _buff_diplomation = 0;
    public int _buff_diplomation_delta = 0;
    public int _buff_fortune = 0;
    public int _buff_fortune_delta = 0;
    public int _buff_protection = 0;
    public int _buff_protection_delta = 0;
    public int _buff_attack = 0;
    private int _BUFF_Initial = 10; // Первоначально все атрибуты равны 10 
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

    public string _enterFactor = ""; // фактор, который выбирает игрок при перетаскивании 
    private Text[] texts; // массив текстовых компонентов у Президента 
    private int counter_player = 0; // передаём номер президента из массива презадентов (j в цикле) 
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

        for (int i = 0; i < _factors.childCount; i++) // забиваем массив childrenFactors факторами сцены. Можно было использовать foreach
        {
            childrenFactors[i] = _factors.GetChild(i).transform;
        }

        ////////// Обрабатываем JSON и все, что с ним связано 

        myItemList = jSONController.myItemList; // вытягиваем лист со скрипта 
        for (int j = 0; j < myItemList.player.Length; j++) // пробежались по всем президентам из JSON-файла 
        {
            if (_parent.name == myItemList.player[j].id) // нашли совпадения обладателя этого скрипта с президентами и JSON
            {
                counter_player = j; // запомнили, в какой ячейке массива нужный player (президент)

                texts = this.GetComponentsInChildren<Text>(); // запомнили текстовые компоненты у найденного президента     

                // пишем в переменные всё из JSON 
                _name = myItemList.player[j].name;
                _level = myItemList.player[j].level;
                _climate = myItemList.player[j].climate; 

                _buff_diplomation_delta = myItemList.player[j].buff_diplomation;
                _buff_protection_delta = myItemList.player[j].buff_protection;
                _buff_attack_delta = myItemList.player[j].buff_attack;
                _buff_fortune_delta = myItemList.player[j].buff_fortune;

                CalcClimate(); // расчет воздействия климата 

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



        WriteVariables(); // запись текущих значений при старте 

        //////////////////////// 


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.transform.SetParent(_praparent); // меняем родителя - дед стал родителем
        _canvasGroup.blocksRaycasts = false;

        for (int n = 0; n < _factors.childCount; n++) // пробегаемся по массиву Факторов
        {
            childrenFactors[n].GetComponent<Factor_Slot>()._targetDragIsTrue = false; // и обнуляем их статус нажатия 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _mainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(_parent); // меняем родителя обратно, отец стал родителем 
        transform.localPosition = Vector3.zero;
        _canvasGroup.blocksRaycasts = true;

        for (int i = 0; i < _factors.childCount; i++) // пробегаемся по массиву Факторов
        {
            if (childrenFactors[i].GetComponent<Factor_Slot>()._targetDragIsTrue == true) // Если Фактор в скрипте говорит, что на него пришел pointerDrag 
            {
                _DragFactorName = childrenFactors[i].name; // запомнили имя Фактора, на который набросили Президента 
                _FactorOnDrop = true;
                calc(counter_player);
            }
        }        
    } 

    void calc(int k)
    {
        if (!_FactorOnDrop) // если ещё не выбирали Факторы
        {
            if (myItemList.player[k].factor_materials != "") // если описание у сырья не пустое, то обрабатываем его 
            {
                Materials(counter_player);
            }

            if (myItemList.player[k].factor_economic != "") // аналогично 
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
        else // если накидывали Факторы, то выдается нужный допустимый фактор, иначе сброс 
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

        WriteVariables(); // запись текущих значений  

    }

    void Materials(int j)
    {
        _mainHeader = "Сырье";
        _descriptionFactor = "" + _factor_materials; //задали в текстовом описание Abilities
        _enterFactor = "Materials"; // если дошли по условиям до данной строчки, значит, фактор, который накинул игрок при перетаскивании - Материалы 
        _materials_ability_protect = myItemList.player[j].materials_ability_protect;
        _economic_ability_protect = 0; // обнуляем, т.к. выбрали бонус по Сырью 
        _economic_ability_attack = 0; // обнуляем, т.к. выбрали бонус по Сырью 
        _health_ability_protect = 0;// обнуляем, т.к. выбрали бонус по Сырью 
        _food_ability_protect = 0; // обнуляем, т.к. выбрали бонус по Сырью 
        _food_ability_attack = 0;
    }

    void Economic(int j)
    {
        //texts[0].text = "Экономика";
        _mainHeader = "Экономика";
        //texts[1].text = _factor_economic;
        _descriptionFactor = _factor_economic;
        _enterFactor = "Economic"; // на какую сферу влияет 
        _materials_ability_protect = 0;
        _economic_ability_protect = myItemList.player[j].economic_ability_protect; // баф защиты 
        _economic_ability_attack = myItemList.player[j].economic_ability_attack; // баф атаки
        _health_ability_protect = 0;
        _food_ability_protect = 0;
        _food_ability_attack = 0;
    }

    void Health(int j)
    {

        //texts[0].text = "Здравоохранение";
        _mainHeader = "Здравоохранение";
        //texts[1].text = _factor_health;
        _descriptionFactor = _factor_health;
        _enterFactor = "Health"; // на какую сферу влияет
        _materials_ability_protect = 0;
        _economic_ability_protect = 0;
        _economic_ability_attack = 0;
        _health_ability_protect = myItemList.player[j].health_ability_protect;
        _food_ability_protect = 0;
        _food_ability_attack = 0;
    }

    void Food(int j)
    {
        //texts[0].text = "Продовольствие";
        _mainHeader = "Продовольствие";
        _descriptionFactor = _factor_food;
        _enterFactor = "Food"; // на какую сферу влияет 
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
        _descriptionFactor = ""; //задали в текстовом описание Abilities
        _enterFactor = ""; // если дошли по условиям до данной строчки, значит, фактор сброшен  
        _materials_ability_protect = 0; 
        _economic_ability_protect = 0; 
        _economic_ability_attack = 0; 
        _health_ability_protect = 0;
        _food_ability_protect = 0;
        _food_ability_attack = 0; 
    }

    void WriteVariables()
    {
        // передаём в компоненты текста нужные значения переменных. Можно было использовать тот же массив texts[].text, но, изменив порядок текстовых блоков, можно получить другие сообщения 
        _parent.transform.Find("Text_Factor").GetComponent<Text>().text = _mainHeader;
        _parent.transform.Find("Text_Abilities").GetComponent<Text>().text = _descriptionFactor; //задали в текстовом описание Abilities 
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


















        ColorRect(); // рисуем подчеркивания 
    }

    void ColorRect()
    {
        if (_buff_attack_delta > 0) // если параметр, влияющий на итоговый баф, положительный
        {
            ImageBuf[0].GetComponent<Image>().color = new Color(0, 1, 0); // зелёный 
        }
        else
        {
            if (_buff_attack_delta == 0) // если 0
            {
                ImageBuf[0].GetComponent<Image>().color = new Color(1, 1, 1); // белый
            }
            else // если он отрицательный 
            {
                ImageBuf[0].GetComponent<Image>().color = new Color(1, 0, 0); //красный
            }
        }
        
        if (_buff_protection_delta > 0) // если баф положительный
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

        if (_buff_fortune_delta > 0) // если баф положительный
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
        
        if (_buff_diplomation_delta > 0) // если баф положительный
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

    void CalcClimate() // коррекция изменения бафа из-за климата 
    {
        string _TotalClimate = DataHolder._GeneralClimate; // достаём глобальное поле
        if (_climate != _TotalClimate) // Климат на совпал 
        {
            _buff_attack_delta += -1;
            _buff_diplomation_delta += -1;
            _buff_fortune_delta += -1;
            _buff_protection_delta += -1;
        }
        else
        {
            _buff_attack_delta += 2; // Климат совпал
            _buff_diplomation_delta += 2;
            _buff_fortune_delta += 2;
        _buff_protection_delta += 2;
        }
    }

    void Update()
    {

    }
} 