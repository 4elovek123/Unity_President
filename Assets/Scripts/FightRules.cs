using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class FightRules : MonoBehaviour
{
    public Transform _Table;
    public GameObject _Arrow;
    public Factor_Item[] _FactorItemPresident = new Factor_Item[3];
    public Factor_Item[] _FactorItemPresidentEnemy = new Factor_Item[3];
    //public GameObject[] _President = new GameObject[3];
    //public GameObject[] _EnemyPresident = new GameObject[3];

    int[] MoralePresident = new int[3];
    int[] MoralePresidentEnemy = new int[3];
    public int _totalMoralePresident = 0;
    public int _totalMoralePresidentEnemy = 0;
    public Canvas _canvasCamera;
    public Camera _camera;

    public Transform[] _Presidents_before;
    public Transform[] _PresidentsEnemy_before;
    public Transform[] _Place_after;
    public Transform[] _PlaceEnemy_after;

    public Transform[] _PlaceFactor;
    public Transform[] _PlaceFactorEnemy;
    public Transform _FactorEconomic;
    public Transform _FactorHealth;
    public Transform _FactorMaterials;
    public Transform _FactorFood;

    public Transform _FactorEconomicEnemy;
    public Transform _FactorHealthEnemy;
    public Transform _FactorMaterialsEnemy;
    public Transform _FactorFoodEnemy;

    public int _ourBUFFmaterial = 0;
    public int _ourBUFFhealth = 0;
    public int _ourBUFFfood = 0;
    public int _ourBUFFeconomic = 0;

    public int _enemyBUFFmaterial = 0;
    public int _enemyBUFFhealth = 0;
    public int _enemyBUFFfood = 0;
    public int _enemyBUFFeconomic = 0;

    public JSONController_Card jSONControllerCard;
    JSONController_Card.ItemListCard myItemListCard;
    private Transform[] _FightCardInScene;
    public Transform[] _FightCardOnTable; // Массив боевых карт на столе 
    private Transform[] _FightCardPlace;
    public GameObject Folder_FightCardPlace;
    public GameObject Folder_FightCardInScene;
    private int counter_card = 0; // передаём номер карты из массива карт (j в цикле) 
    private int _costCard = 0;
    private int _materialsCard = 0;
    private int _economicCard = 0;
    private int _healthCard = 0;
    private int _foodCard = 0;
    private int _attackCard = 0;
    private int _protectCard = 0;
    private int _diplomationCard = 0;
    private int _fortuneCard = 0;
    private int _deltamorale_positive = 0;
    private int _deltamorale_negative = 0;

    private bool _helperMain = true;
    private bool _helper = true;
    private bool _helper101 = false; ///////////////
    private bool _helper105 = false; ///////////////
    private bool _helperScaleCardIsTrue = false;
    private bool _boolOutlineToFactor = false;

    private string[] _FactorsOnFigtCard;

    Transform _FirstSelectionFightCard = null;
    Transform _factorForAnimLogo;

    private Transform _dragFactor = null; // Фактор, который выбрали мы
    private Transform _dragFactorEnemy = null; // Фактор, который выбрал враг 

    private int _materials_ability_protect = 0;
    private int _economic_ability_protect = 0;
    private int _economic_ability_attack = 0;
    private int _health_ability_protect = 0;
    private int _food_ability_protect = 0;

    [TextArea]
    private string _testText;  // ВРЕМЕННО, ЛОГ НА ЭКРАНЕ 
    public Transform _scrollViewContent; // ВРЕМЕННО, ЛОГ НА ЭКРАНЕ 
    private AnimationClip PresidentAnimation;

    private Transform[] ImageBuf = new Transform[3];
    private Transform _hitLast = null;

    public Material _redBoom;
    public Material _greenBoom;
    public Material _OtherMatBoom;

    void Start()
    {

    }

    void StartAnimation()
    {
        for (int i = 0; i < _Presidents_before.Length; i++) // анимация президентов 
        {
            Animation anim = _Presidents_before[i].transform.GetComponent<Animation>();
            anim.Play("PresidentAnimation");
        }

        for (int i = 0; i < _PresidentsEnemy_before.Length; i++) // анимация соперника  
        {
            Animation anim = _PresidentsEnemy_before[i].transform.GetComponent<Animation>();
            anim.Play("PresidentAnimationEnemy");
        }

        for (int i = 0; i < _FightCardOnTable.Length; i++) // анимация боевых карточек
        {
            Animation anim = _FightCardOnTable[i].transform.GetComponent<Animation>();
            anim.Play("FightCardStartAnimation");
        }
        
        Animation animTable = _Table.transform.GetComponent<Animation>();
        animTable.Play("TableRotateAnimation");
        
    }

    public void RandomFightCardToPlace() // заносим в массивы все карты сцены, места для карт за столом, выбираем рандомные, помещаем за стол. Рассчитывается при нажатии кнопки "Ready" 
    {

        _FightCardInScene = new Transform[Folder_FightCardInScene.transform.childCount];
        for (int i = 0; i < _FightCardInScene.Length; i++) // пробежались по всем боевым картам в сцене в папке FightCardInScene (16 штук сейчас) 
        {
            _FightCardInScene[i] = Folder_FightCardInScene.transform.GetChild(i).transform; // занесли их в массив _FightCardInScene[]
        }

        _FightCardPlace = new Transform[Folder_FightCardPlace.transform.childCount];
        _FightCardOnTable = new Transform[_FightCardPlace.Length];

        int[] _random = new int[_FightCardPlace.Length]; // формируем упорядоченный массив с 0 до _FightCardPlace.Length (от 0 до 9 включительно)
        for (int i = 0; i < _random.Length; i++) { _random[i] = i; }

        int[] Mix(int[] num) // перемешиваем его 
        {
            for (int i = 0; i < num.Length; i++)
            {
                int currentValue = num[i];
                int randomIndex = Random.Range(i, num.Length);
                num[i] = num[randomIndex];
                num[randomIndex] = currentValue;
            }
            return num;
        }

        int[] MixArray = Mix(_random); // перемешанный массив для выбора случайных карт из _FightCardInScene

        myItemListCard = jSONControllerCard.myListFightCard; // вытягиваем лист со скрипта jSON 
        for (int i = 0; i < _FightCardPlace.Length; i++) // пробежались по всем Place будущих карт 
        {
            _FightCardPlace[i] = Folder_FightCardPlace.transform.GetChild(i).transform; // занесли их в массив _FightCardPlace[] 
            _FightCardOnTable[i] = _FightCardInScene[MixArray[i]]; // заполнили массив случайными картами
            _FightCardOnTable[i].transform.SetParent(_FightCardPlace[i]); // поместили боевые карты в Place[] 
            _FightCardOnTable[i].transform.localPosition = Vector3.zero; // сбросили позицию 
            _FightCardOnTable[i].transform.localRotation = Quaternion.identity; // сбросили вращение 

            for (int j = 0; j < myItemListCard.fight_card.Length; j++) // пробежались по всем картам из JSON-файла 
            {
                if (_FightCardOnTable[i].name == myItemListCard.fight_card[j].id) // нашли совпадения с картой из JSON 
                {
                    _FightCardOnTable[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentInChildren<Text>().text = "" + myItemListCard.fight_card[j].cost; // прописали цену для каждой карты за столом 
                }
            }
        }

        StartAnimation();

    }

    public void StartButton() // выставляем карты президентов 
    {
        for (int i = 0; i < _Presidents_before.Length; i++) // копируем свойства UI президентов на 3D карты президентов 
        {
            _Presidents_before[i].transform.SetParent(_Place_after[i]);
            _Presidents_before[i].transform.localPosition = Vector3.zero;
            _Presidents_before[i].transform.localRotation = Quaternion.identity;
            //_Presidents_before[i].transform.localScale = Vector3.one;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_Level").GetComponent<Text>().text = "" + _FactorItemPresident[i]._level;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufAttack").GetComponent<Text>().text = "" + _FactorItemPresident[i]._buff_attack;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufFortune").GetComponent<Text>().text = "" + _FactorItemPresident[i]._buff_fortune;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufProtection").GetComponent<Text>().text = "" + _FactorItemPresident[i]._buff_protection;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufDiplomation").GetComponent<Text>().text = "" + _FactorItemPresident[i]._buff_diplomation;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufAttackDelta").GetComponent<Text>().text = "" + _FactorItemPresident[i]._buff_attack_delta;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufFortuneDelta").GetComponent<Text>().text = "" + _FactorItemPresident[i]._buff_fortune_delta;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufProtectionDelta").GetComponent<Text>().text = "" + _FactorItemPresident[i]._buff_protection_delta;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufDiplomationDelta").GetComponent<Text>().text = "" + _FactorItemPresident[i]._buff_diplomation_delta;

            _ourBUFFmaterial += _FactorItemPresident[i]._BUFFmaterials;
            _ourBUFFhealth += _FactorItemPresident[i]._BUFFhealth;
            _ourBUFFfood += _FactorItemPresident[i]._BUFFfood;
            _ourBUFFeconomic += _FactorItemPresident[i]._BUFFeconomic;
            _materials_ability_protect += _FactorItemPresident[i]._materials_ability_protect;
            _economic_ability_protect += _FactorItemPresident[i]._economic_ability_protect;
            _economic_ability_attack += _FactorItemPresident[i]._economic_ability_attack;
            _health_ability_protect += _FactorItemPresident[i]._health_ability_protect;
            _food_ability_protect += _FactorItemPresident[i]._food_ability_protect;

            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[0].color = _FactorItemPresident[i].ImageBuf[0].GetComponent<Image>().color;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[1].color = _FactorItemPresident[i].ImageBuf[1].GetComponent<Image>().color;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[2].color = _FactorItemPresident[i].ImageBuf[2].GetComponent<Image>().color;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[0].color = _FactorItemPresident[i].ImageBuf[3].GetComponent<Image>().color;
        }

        for (int i = 0; i < _PresidentsEnemy_before.Length; i++) // копируем свойства UI президентов на 3D карты президентов-противников 
        {
            _PresidentsEnemy_before[i].transform.SetParent(_PlaceEnemy_after[i]);
            _PresidentsEnemy_before[i].transform.localPosition = Vector3.zero;
            _PresidentsEnemy_before[i].transform.localRotation = Quaternion.identity;
            //_PresidentsEnemy_before[i].transform.localScale = Vector3.one;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_Level").GetComponent<Text>().text = "" + _FactorItemPresidentEnemy[i]._level;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufAttack").GetComponent<Text>().text = "" + _FactorItemPresidentEnemy[i]._buff_attack;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufFortune").GetComponent<Text>().text = "" + _FactorItemPresidentEnemy[i]._buff_fortune;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufProtection").GetComponent<Text>().text = "" + _FactorItemPresidentEnemy[i]._buff_protection;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufDiplomation").GetComponent<Text>().text = "" + _FactorItemPresidentEnemy[i]._buff_diplomation;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufAttackDelta").GetComponent<Text>().text = "" + _FactorItemPresidentEnemy[i]._buff_attack_delta;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufFortuneDelta").GetComponent<Text>().text = "" + _FactorItemPresidentEnemy[i]._buff_fortune_delta;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufProtectionDelta").GetComponent<Text>().text = "" + _FactorItemPresidentEnemy[i]._buff_protection_delta;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.Find("Text_BufDiplomationDelta").GetComponent<Text>().text = "" + _FactorItemPresidentEnemy[i]._buff_diplomation_delta;

            _enemyBUFFmaterial += _FactorItemPresidentEnemy[i]._BUFFmaterials;
            _enemyBUFFhealth += _FactorItemPresidentEnemy[i]._BUFFhealth;
            _enemyBUFFfood += _FactorItemPresidentEnemy[i]._BUFFfood;
            _enemyBUFFeconomic += _FactorItemPresidentEnemy[i]._BUFFeconomic;

            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[0].color = _FactorItemPresidentEnemy[i].ImageBuf[0].GetComponent<Image>().color;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[1].color = _FactorItemPresidentEnemy[i].ImageBuf[1].GetComponent<Image>().color;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[2].color = _FactorItemPresidentEnemy[i].ImageBuf[2].GetComponent<Image>().color;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[3].color = _FactorItemPresidentEnemy[i].ImageBuf[3].GetComponent<Image>().color;
        }
        ReadyFight();
        ReadyFight2();
    }

    void calcLocationFactorsOur() // расставляем наши факторы, пока топорно без зависимости от выбора президента
    {
        _FactorMaterials.transform.SetParent(_PlaceFactor[0]);
        _FactorMaterials.transform.localPosition = Vector3.zero;
        _FactorMaterials.transform.localRotation = Quaternion.identity;
        _FactorMaterials.transform.localScale = new Vector3(1f, 1f, 1f);
        _PlaceFactor[0].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFmaterial; // ХП Сырья

        _FactorFood.transform.SetParent(_PlaceFactor[1]);
        _FactorFood.transform.localPosition = Vector3.zero;
        _FactorFood.transform.localRotation = Quaternion.identity;
        _FactorFood.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactor[1].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFfood; // хп продовольствия

        _FactorEconomic.transform.SetParent(_PlaceFactor[2]);
        _FactorEconomic.transform.localPosition = Vector3.zero;
        _FactorEconomic.transform.localRotation = Quaternion.identity;
        _FactorEconomic.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactor[2].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFeconomic; // ХП экономики

        _FactorHealth.transform.SetParent(_PlaceFactor[3]);
        _FactorHealth.transform.localPosition = Vector3.zero;
        _FactorHealth.transform.localRotation = Quaternion.identity;
        _FactorHealth.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactor[3].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFhealth; // хп здравоохранения 
    }

    void calcLocationFactorsEnemy() // расставляем факторы врага 
    {
        _FactorMaterialsEnemy.transform.SetParent(_PlaceFactorEnemy[0]);
        _FactorMaterialsEnemy.transform.localPosition = Vector3.zero;
        _FactorMaterialsEnemy.transform.localRotation = Quaternion.identity;
        _FactorMaterialsEnemy.transform.localScale = new Vector3(1f, 1f, 1f);
        _PlaceFactorEnemy[0].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFmaterial; // ХП Сырья 

        _FactorFoodEnemy.transform.SetParent(_PlaceFactorEnemy[1]);
        _FactorFoodEnemy.transform.localPosition = Vector3.zero;
        _FactorFoodEnemy.transform.localRotation = Quaternion.identity;
        _FactorFoodEnemy.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactorEnemy[1].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFfood; // хп продовольствия

        _FactorEconomicEnemy.transform.SetParent(_PlaceFactorEnemy[2]);
        _FactorEconomicEnemy.transform.localPosition = Vector3.zero;
        _FactorEconomicEnemy.transform.localRotation = Quaternion.identity;
        _FactorEconomicEnemy.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactorEnemy[2].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFeconomic; // хп экономики

        _FactorHealthEnemy.transform.SetParent(_PlaceFactorEnemy[3]);
        _FactorHealthEnemy.transform.localPosition = Vector3.zero;
        _FactorHealthEnemy.transform.localRotation = Quaternion.identity;
        _FactorHealthEnemy.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactorEnemy[3].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFhealth; // хп здравоохранения 
    }

    void Update()
    {
        if (_helperMain == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1)); // рейкаст, вычисляем наведение мышки на боевую карту 
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity) && _hit.collider.tag == "FightCard") // проверяем объект по тэгу, чтобы лишний раз цикл for не пускать 
            {
                for (int k = 0; k < _FightCardOnTable.Length; k++) // перебираем, попали ли по карте и по какой 
                {
                    if (_hit.transform.name == _FightCardOnTable[k].name) // выяснили имя карты, по которой попали 
                    {
                        if (_FirstSelectionFightCard != _hit.transform && _FirstSelectionFightCard != null && _helper105 == false) // если перескочили с карты на карту 
                        {
                            _helper105 = true;
                            AnimationScaleCardClose(_FirstSelectionFightCard); // закрываем предыдущую 
                        }
                        if (_helper101 == false)
                        {
                            _helper101 = true; // хелпер, чтобы не гонять постоянно в Update
                            counter_card = k;
                            AnimationScaleCardOpen(_FightCardOnTable[counter_card]); // анимация открытия текущей карты 
                            FightCard(counter_card);
                        }
                        if (_hit.transform == _hitLast) // если выбрали снова карту, которая была на предыдущем шаге, то не пускаем дальше 
                        {
                            _helper = false;
                        }
                        else _helper = true; 
                    }
                }
            }
            else
            {
                if (_helperScaleCardIsTrue == true && _helper101 == true)
                {
                    AnimationScaleCardClose(_FirstSelectionFightCard);
                    _FirstSelectionFightCard = null;
                }
                _helper101 = false;
            }
        }

        if (_helper101 && Input.GetMouseButtonDown(0) && _helper == true) // если мышка наведена и при этом нажали на клавишу, то обрабатываем далее. _helper101 true - значит, прошли предыдущий пункт. _helper - для текущей остановки в Update 
        {
            _helper = false;
            _helperMain = false; // выключаем, чтобы не обрабатывать 1 этап (рейкаст и тп при наведении мышки на боевые карты) 
            _FactorsOnFigtCard = new string[4]; // факторы, которые есть на выбранной боевой карте, используются далее 
            AnimationTransformCard(_FightCardOnTable[counter_card]); // анимация вылета выбранной карты на середину колоды 
        }

        if (_boolOutlineToFactor == true) // рейкаст по нужным факторам 
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity)) // рейкаст, вычисляем наведение мышки на фактор 
            {
                for (int k = 0; k < _FactorsOnFigtCard.Length; k++)
                {
                    if (_hit.transform.name == _FactorsOnFigtCard[k] && Input.GetMouseButtonDown(0)) // если мы нажали на нужный фактор 
                    {
                         _boolOutlineToFactor = false; // выключили обработку в Update 
                        _dragFactor = _hit.transform; // узнали, какой фактор выбрали 

                        AnimationArrow(_dragFactor); // вызываем анимацию стрелки 
                    }
                }
            }

            else
            {
                _dragFactor = null;
            }


            /*if (Input.GetMouseButtonUp(0) == true)
            {
                _helper = true;
                _helper3 = true;s
                _helper2 = false;
                _helper4 = true;

                if (_dragFactor != null)
                { 
                    CalcOurFight();
                    _dragFactor = null;
                }
            }
            
        }*/ 
        }
    }

    void FightCard(int k) // запоминаем параметры выбранной боевой карты для расчёта 
    {
        myItemListCard = jSONControllerCard.myListFightCard; // вытягиваем лист со скрипта 
        for (int j = 0; j < myItemListCard.fight_card.Length; j++) // пробежались по всем картам из JSON-файла 
        {
            if (_FightCardOnTable[k].name == myItemListCard.fight_card[j].id) // нашли совпадения с картой из JSON
            {
                _costCard = myItemListCard.fight_card[j].cost;
                _materialsCard = myItemListCard.fight_card[j].materials;
                _economicCard = myItemListCard.fight_card[j].economic;
                _healthCard = myItemListCard.fight_card[j].health;
                _foodCard = myItemListCard.fight_card[j].food;
                _attackCard = myItemListCard.fight_card[j].attack;
                _protectCard = myItemListCard.fight_card[j].protect;
                _diplomationCard = myItemListCard.fight_card[j].diplomation;
                _fortuneCard = myItemListCard.fight_card[j].fortune;
                _deltamorale_positive = myItemListCard.fight_card[j].deltamorale_positive;
                _deltamorale_negative = myItemListCard.fight_card[j].deltamorale_negative;
            }
        }
    }

    void AnimationScaleCardOpen(Transform _y) // анимация размера боевой карты, которая проигрывается при наведении мыши 
    {
        _FirstSelectionFightCard = _y;
        Animation animOpen = _y.transform.GetComponent<Animation>();
        animOpen.Play("FightCardSelectOpen"); 
        _helperScaleCardIsTrue = true; // хелпер переключился, мы знаем, что объект заскейлен 
        _helper105 = false;
    }


    void AnimationScaleCardClose(Transform _y) // анимация размера боевой карты, которая проигрывается при наведении мыши 
    {
        Animation animClose = _y.transform.GetComponent<Animation>();
        animClose.Play("FightCardSelectClose"); 
        _helperScaleCardIsTrue = false;
        _helper101 = false;
    }

    void AnimationTransformCard(Transform _y) // анимация вылета боевой карты на центр, которая проигрывается при нажатии кнопк мыши, и других 
    {
        _y.transform.position = new Vector3(_camera.ViewportToWorldPoint(new Vector3(.5f, .5f, 0)).x, _y.transform.position.y, _y.transform.position.z); // карта на центр колоды 
        _helper101 = false; 

        if(_attackCard == 1 || _diplomationCard == 1)  // включаем анимацию логотипов у нужных факторов (смотрим на данные выбранной карты) 
        {                                              // пока у нас жесткая привязка, на Place0 - Materials, 1 - Food, 2 - Economic, 3 - Health 
            if (_materialsCard == 1)
            {
                AnimationFactors(_PlaceFactorEnemy[0], "Enemy_Materials"); // атака и дипломатия ориентирована на факторы противника 
                _FactorsOnFigtCard[0] = "Enemy_Materials";
            }
            if (_foodCard == 1)
            {
                AnimationFactors(_PlaceFactorEnemy[1], "Enemy_Food");
                _FactorsOnFigtCard[1] = "Enemy_Food";
            }
            if (_economicCard == 1)
            {
                AnimationFactors(_PlaceFactorEnemy[2], "Enemy_Economic");
                _FactorsOnFigtCard[2] = "Enemy_Economic";
            }
            if (_healthCard == 1)
            {
                AnimationFactors(_PlaceFactorEnemy[3], "Enemy_Health");
                _FactorsOnFigtCard[3] = "Enemy_Health";
            }
        }
        
        if (_protectCard == 1 || _fortuneCard == 1) // защита и удача ориентирована на свои факторы
        {
            if (_materialsCard == 1)
            {
                AnimationFactors(_PlaceFactor[0], "Our_Materials");
                _FactorsOnFigtCard[0] = "Our_Materials";
            }
            if (_foodCard == 1)
            {
                AnimationFactors(_PlaceFactor[1], "Our_Food");
                _FactorsOnFigtCard[1] = "Our_Food";
            }
            if (_economicCard == 1)
            {
                AnimationFactors(_PlaceFactor[2], "Our_Economic");
                _FactorsOnFigtCard[2] = "Our_Economic";
            }
            if (_healthCard == 1)
            {
                AnimationFactors(_PlaceFactor[3], "Our_Health");
                _FactorsOnFigtCard[3] = "Our_Health";
            }
        }


    } 

    void AnimationFactors(Transform _factorForAnim, string _name) // запускаем анимацию Лого факторов 
    {
        _factorForAnimLogo = _factorForAnim.transform.Find(_name).transform.Find("Logo"); // нашли Логотип фактора 
        Animator animatorFactor = _factorForAnimLogo.transform.GetComponent<Animator>(); // достали аниматор 
        animatorFactor.SetBool ("StartRotate", true); // включили вращение логотипа 
        _boolOutlineToFactor = true; // отмечаем, что прошли этап, в Update нужно рейкастить фактор, по которому щелкнем мышью
    }


    void AnimationArrow(Transform _dragFactorForArrow) // вычисляем, по какому фактору нужна анимация стрелки 
    {
        Cursor.lockState = CursorLockMode.Locked; // отключаем курсор 
        _Arrow.SetActive(true); // включаем стрелку 

        Animator _arrowanim = _Arrow.transform.GetComponent<Animator>(); // достали аниматор стрелки 

        if (_dragFactorForArrow.name == "Our_Materials")
        {
            _arrowanim.SetInteger("_selectArrow", 1); // в зависимости от фактора, запускаем анимацию стрелки 
        }   
        if (_dragFactorForArrow.name == "Our_Food")
        {
            _arrowanim.SetInteger("_selectArrow", 2);
        }
        if (_dragFactorForArrow.name == "Our_Economic")
        {
            _arrowanim.SetInteger("_selectArrow", 3);
        }
        if (_dragFactorForArrow.name == "Our_Health")
        {
            _arrowanim.SetInteger("_selectArrow", 4);
        }
        if (_dragFactorForArrow.name == "Enemy_Materials")
        {
            _arrowanim.SetInteger("_selectArrow", 5);
        }
        if (_dragFactorForArrow.name == "Enemy_Food")
        {
            _arrowanim.SetInteger("_selectArrow", 6);
        }
        if (_dragFactorForArrow.name == "Enemy_Economic")
        {
            _arrowanim.SetInteger("_selectArrow", 7);
        }
        if (_dragFactorForArrow.name == "Enemy_Health")
        {
            _arrowanim.SetInteger("_selectArrow", 8);
        }

        StartCoroutine(Pause()); // вызываем паузу для прохождения анимации стрелки 

    }

    IEnumerator Pause() 
    {
        yield return new WaitForSeconds (2); // пока так, в секундах. Через 2 сек закончилась анимация стрелки 
        StopAnimationFactors(); // остановили все логотипы 
        _Arrow.SetActive(false); // выключили стрелку 

        _dragFactor.Find("Boom").gameObject.SetActive(true); // включили фейерверк 
        StartCoroutine(Pause2()); 

    } 

    void StopAnimationFactors() // пока так топорно и с Find останавливаем анимацию Лого , потом убрать 
    {
        for (int i = 0; i < _PlaceFactor.Length; i++)
        {
            _PlaceFactor[i].transform.Find(_PlaceFactor[i].name).transform.Find("Logo").transform.GetComponent<Animator>().SetBool("StartRotate", false);
        }
        for (int i = 0; i < _PlaceFactorEnemy.Length; i++)
        {
            _PlaceFactorEnemy[i].transform.Find(_PlaceFactorEnemy[i].name).transform.Find("Logo").transform.GetComponent<Animator>().SetBool("StartRotate", false);
        }
    } 

    IEnumerator Pause2()
    {
        yield return new WaitForSeconds(1); // пока так, в секундах. Через 1 сек закончилась анимация взрыва 
        ResetAnimationCard(); 
    } 

    void ResetAnimationCard()//(Transform y)
    {
        _factorForAnimLogo.parent.Find("Boom").gameObject.SetActive(false); // выключили фейерверк 
        _FightCardOnTable[counter_card].transform.localScale = Vector3.one; // вернули карту на место 
        _FightCardOnTable[counter_card].transform.localPosition = Vector3.zero;
        Debug.Log("OK ");
        CalcOurFight(); // вызываем расчёт очков в бою 
    }

    void CalcOurFight() // Расчёт очков в бою после нашего хода 
    {
        Debug.Log("CalcOurFight!"); 
        if (_dragFactor != null)
        {
            _testText = " We used the card " + _FightCardOnTable[counter_card].name + " to " + _dragFactor.name + "\n" + _testText;
            /*
            _materials_ability_protect
            _economic_ability_protect
            _economic_ability_attack
            _health_ability_protect
            _food_ability_protect
            
            _ourBUFFeconomic = 0;
            _ourBUFFfood = 0;
            _ourBUFFhealth = 0;
            _ourBUFFmaterial = 0;

            _enemyBUFFeconomic = 0;
            _enemyBUFFfood = 0;
            _enemyBUFFhealth = 0;
            _enemyBUFFmaterial = 0; 

            */
            Debug.Log("_enemyBUFFmaterial After OUR" + _enemyBUFFmaterial);
            if (_attackCard == 1)
            {
                if (_materialsCard == 1)// && _dragFactor.name == "Our_Materials")
                {
                    _enemyBUFFmaterial = _enemyBUFFmaterial - _deltamorale_positive;
                    _totalMoralePresident = _totalMoralePresident - _costCard;
                }
                else if (_economicCard == 1)// && _dragFactor.name == "Our_Economic")
                {
                    _enemyBUFFeconomic = _enemyBUFFeconomic - _deltamorale_positive;
                    _totalMoralePresident = _totalMoralePresident - _costCard;
                }
                else if (_healthCard == 1)// && _dragFactor.name == "Our_Health")
                {
                    _enemyBUFFhealth = _enemyBUFFhealth - _deltamorale_positive;
                    _totalMoralePresident = _totalMoralePresident - _costCard;
                }
                else if (_foodCard == 1)// && _dragFactor.name == "Our_Food")
                {
                    _enemyBUFFfood = _enemyBUFFfood - _deltamorale_positive;
                    _totalMoralePresident = _totalMoralePresident - _costCard;
                }
                else
                {
                    _totalMoralePresident = _totalMoralePresident - _costCard; // на всякий, случиться не должно 
                }
            }
            else
            {
                if (_protectCard == 1)
                {
                    if (_materialsCard == 1)// && _dragFactor.name == "Our_Materials")
                    {
                        _ourBUFFmaterial = _ourBUFFmaterial - _deltamorale_negative - _costCard;
                    }
                    else if (_economicCard == 1)// && _dragFactor.name == "Our_Economic")
                    {
                        _ourBUFFeconomic = _ourBUFFeconomic - _deltamorale_negative - _costCard;
                    }
                    else if (_healthCard == 1) // && _dragFactor.name == "Our_Health")
                    {
                        _ourBUFFhealth = _ourBUFFhealth - _deltamorale_negative - _costCard;
                    }
                    else if (_foodCard == 1) // && _dragFactor.name == "Our_Food")
                    {
                        _ourBUFFfood = _ourBUFFfood - _deltamorale_negative - _costCard;
                    }
                    else
                    { 
                        _totalMoralePresident = _totalMoralePresident - _costCard;  // на всякий, случиться не должно 
                    }
                }
                else
                {
                    if (_diplomationCard == 1) // по дипломатии пока нет функционала 
                    {
                        if (_materialsCard == 1)// && _dragFactor.name == "Our_Materials")
                        {
                            _totalMoralePresident = _totalMoralePresident - _costCard;
                        }
                        else if (_economicCard == 1) //&& _dragFactor.name == "Our_Economic")
                        {
                            _totalMoralePresident = _totalMoralePresident - _costCard;
                        }
                        else if (_healthCard == 1) // && _dragFactor.name == "Our_Health")
                        {
                            _totalMoralePresident = _totalMoralePresident - _costCard;
                        }
                        else if (_foodCard == 1) // && _dragFactor.name == "Our_Food")
                        {
                            _totalMoralePresident = _totalMoralePresident - _costCard;
                        }
                        else _totalMoralePresident = _totalMoralePresident - _costCard; // на всякий, случиться не должно 
                    }
                    else
                    {
                        if (_fortuneCard == 1)
                        {
                            if (_materialsCard == 1) // && _dragFactor.name == "Our_Materials")
                            {
                                int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                                _ourBUFFmaterial = _ourBUFFmaterial + _randomDeltaMorale[Random.Range(0, 1)]; // если +, то суммируем, если -, то вычитаем 
                                _totalMoralePresident  = _totalMoralePresident - _costCard; 
                            }
                            else if (_economicCard == 1) // && _dragFactor.name == "Our_Economic")
                            {
                                int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                                _ourBUFFeconomic = _ourBUFFeconomic + _randomDeltaMorale[Random.Range(0, 1)]; // если +, то суммируем, если -, то вычитаем 
                                _totalMoralePresident = _totalMoralePresident - _costCard;
                            }
                            else if (_healthCard == 1) //  && _dragFactor.name == "Our_Health")
                            {
                                int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                                _ourBUFFhealth = _ourBUFFhealth + _randomDeltaMorale[Random.Range(0, 1)]; // если +, то суммируем, если -, то вычитаем 
                                _totalMoralePresident = _totalMoralePresident - _costCard;
                            }
                            else if (_foodCard == 1) // && _dragFactor.name == "Our_Food")
                            {
                                int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                                _ourBUFFfood = _ourBUFFfood + _randomDeltaMorale[Random.Range(0, 1)]; // если +, то суммируем, если -, то вычитаем 
                                _totalMoralePresident = _totalMoralePresident - _costCard;
                            }
                            else _totalMoralePresident = _totalMoralePresident - _costCard; // на всякий, случиться не должно 
                        }
                    }
                }
            }
        }

        if (_ourBUFFmaterial < 0) _ourBUFFmaterial = 0; // очки факторов не могут быть отрицательными 
        if (_ourBUFFeconomic < 0) _ourBUFFeconomic = 0; 
        if (_ourBUFFhealth < 0) _ourBUFFhealth = 0; 
        if (_ourBUFFfood < 0) _ourBUFFfood = 0; 

        Debug.Log("_enemyBUFFmaterial_2 After OUR" + _enemyBUFFmaterial);
        ReadyFight2(); // Вызываем вывод нужных данных на экран 
        StartCoroutine(PauseOurCoroutine());
    }

    IEnumerator PauseOurCoroutine() // Пауза после нашего хода 
    {
        Debug.Log("PauseOurCoroutine!");
        yield return new WaitForSeconds(0.1f);
        CalcEnemyFight(); // Ход соперника 
    }

    void CalcEnemyFight() // Соперник "Выбирает" карту 
    {
        Debug.Log("CalcEnemyFight!");
        int _enemy = Random.Range(0, _FightCardOnTable.Length - 1); // выбираем карту (за столом) наугад 
        FightCard(_enemy); // получаем её данные 
        //string _dragFactorEnemy = "";
        if (_materialsCard == 1) _dragFactorEnemy = _FactorMaterialsEnemy;
        else
        {
            if (_economicCard == 1) _dragFactorEnemy = _FactorEconomicEnemy;
            else
            {
                if (_healthCard == 1) _dragFactorEnemy = _FactorHealthEnemy;
                else
                {
                    if (_foodCard == 1) _dragFactorEnemy = _FactorFoodEnemy;
                }
            }
        }

        _testText = " The opponent used the card: " + _FightCardOnTable[_enemy].name + " to " + _dragFactorEnemy.name + "\n" + _testText;
        StartCoroutine(PauseEnemyCoroutine()); // пауза после хода соперника 
    }
    IEnumerator PauseEnemyCoroutine() // Пауза 
    {
        Debug.Log("PauseEnemyCoroutine!");
        yield return new WaitForSeconds(2);
        CalcEnemyFight2();
    }

    void CalcEnemyFight2() // расчёт очков от хода соперника 
    {
        _OtherMatBoom.CopyPropertiesFromMaterial(_redBoom); // меняем цвета у частиц 
        _redBoom.CopyPropertiesFromMaterial(_greenBoom);
        _greenBoom.CopyPropertiesFromMaterial(_OtherMatBoom); 

        Debug.Log("_enemyBUFFmaterial After ENEMY" + _enemyBUFFmaterial);
        if (_attackCard == 1) 
        {
            if (_materialsCard == 1)
            {
                _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                _ourBUFFmaterial = _ourBUFFmaterial - _deltamorale_positive;
                _FactorMaterials.Find("Boom").gameObject.SetActive(true); // включили фейерверк 
                
            }
            else if (_economicCard == 1)
            {
                _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                _ourBUFFeconomic = _ourBUFFeconomic - _deltamorale_positive;
                _FactorEconomic.Find("Boom").gameObject.SetActive(true);
            }
            else if (_healthCard == 1)
            {
                _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                _ourBUFFhealth = _ourBUFFhealth - _deltamorale_positive;
                _FactorHealth.Find("Boom").gameObject.SetActive(true);
            }
            else if (_foodCard == 1)
            {
                _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                _ourBUFFfood = _ourBUFFfood - _deltamorale_positive;
                _FactorFood.Find("Boom").gameObject.SetActive(true);
            }
            else _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard; // на всякий, случиться не должно 
        }
        else
        {
            if (_protectCard == 1)
            {
                if (_materialsCard == 1)
                {
                    _enemyBUFFmaterial = _enemyBUFFmaterial - _deltamorale_negative;
                    _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                    _FactorMaterialsEnemy.Find("Boom").gameObject.SetActive(true);
                }
                else if (_economicCard == 1)
                {
                    _enemyBUFFeconomic = _enemyBUFFeconomic - _deltamorale_negative;
                    _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                    _FactorEconomicEnemy.Find("Boom").gameObject.SetActive(true);
                }
                else if (_healthCard == 1)
                {
                    _enemyBUFFhealth = _enemyBUFFhealth - _deltamorale_negative;
                    _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                    _FactorHealthEnemy.Find("Boom").gameObject.SetActive(true); 
                }
                else if (_foodCard == 1)
                {
                    _enemyBUFFfood = _enemyBUFFfood - _deltamorale_negative; 
                    _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                    _FactorFoodEnemy.Find("Boom").gameObject.SetActive(true);
                }
                else _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard; // на всякий, случиться не должно 
            }
            else
            {
                if (_diplomationCard == 1) // по дипломатии пока нет функционала 
                {
                    if (_materialsCard == 1)
                    {
                        _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                        _FactorMaterials.Find("Boom").gameObject.SetActive(true);
                    }
                    else if (_economicCard == 1)
                    {
                        _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                        _FactorEconomic.Find("Boom").gameObject.SetActive(true);
                    }
                    else if (_healthCard == 1)
                    {
                        _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                        _FactorHealth.Find("Boom").gameObject.SetActive(true); 
                    }
                    else if (_foodCard == 1)
                    {
                        _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                        _FactorFood.Find("Boom").gameObject.SetActive(true);
                    }
                    else _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard; // на всякий, случиться не должно 
                }
                else
                {
                    if (_fortuneCard == 1)
                    {
                        if (_materialsCard == 1)
                        {
                            int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                            _enemyBUFFmaterial = _enemyBUFFmaterial + _randomDeltaMorale[Random.Range(0, 1)];
                            _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                            _FactorMaterialsEnemy.Find("Boom").gameObject.SetActive(true);
                        }
                        else if (_economicCard == 1)
                        {
                            int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                            _enemyBUFFeconomic = _enemyBUFFeconomic + _randomDeltaMorale[Random.Range(0, 1)];
                            _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                            _FactorEconomicEnemy.Find("Boom").gameObject.SetActive(true);
                        }
                        else if (_healthCard == 1)
                        {
                            int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                            _enemyBUFFhealth = _enemyBUFFhealth + _randomDeltaMorale[Random.Range(0, 1)];
                            _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                            _FactorHealthEnemy.Find("Boom").gameObject.SetActive(true);
                        } 
                        else if (_foodCard == 1)
                        {
                            int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                            _enemyBUFFfood = _enemyBUFFfood + _randomDeltaMorale[Random.Range(0, 1)];
                            _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                            _FactorFoodEnemy.Find("Boom").gameObject.SetActive(true);
                        }
                        else _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard; // на всякий, случиться не должно 
                    }
                }
            }
        }

        if (_enemyBUFFmaterial < 0) _enemyBUFFmaterial = 0; // очки факторов не могут быть отрицательными 
        if (_enemyBUFFeconomic < 0) _enemyBUFFeconomic = 0;
        if (_enemyBUFFhealth < 0) _enemyBUFFhealth = 0;
        if (_enemyBUFFfood < 0) _enemyBUFFfood = 0;

        _hitLast = _FightCardOnTable[counter_card]; // записали, что в этом ходу была выбрана такая-то карта 
        ReadyFight2(); // вывод на экран расчётов 

        StartCoroutine(Pause3()); 
    }

    IEnumerator Pause3()
    {
        yield return new WaitForSeconds(2);
        _FactorMaterials.Find("Boom").gameObject.SetActive(false); // Выключили фейерверки у врага, пока так топорно, с дорогим Find
        _FactorEconomic.Find("Boom").gameObject.SetActive(false);
        _FactorHealth.Find("Boom").gameObject.SetActive(false);
        _FactorFood.Find("Boom").gameObject.SetActive(false);
        _FactorMaterialsEnemy.Find("Boom").gameObject.SetActive(false);
        _FactorEconomicEnemy.Find("Boom").gameObject.SetActive(false);
        _FactorHealthEnemy.Find("Boom").gameObject.SetActive(false);
        _FactorFoodEnemy.Find("Boom").gameObject.SetActive(false);

        _OtherMatBoom.CopyPropertiesFromMaterial(_redBoom); // меняем цвета у частиц обратно 
        _redBoom.CopyPropertiesFromMaterial(_greenBoom);
        _greenBoom.CopyPropertiesFromMaterial(_OtherMatBoom);

        Cursor.lockState = CursorLockMode.None; // включаем курсор только после хода соперника 
        _helperMain = true; // возвращаем хелперы в рабочее состояние 
        _helper = true; 
    }
    

public void ReadyFight() // рассчитывается 1 раз в начале при нажатии кнопки "Ready" 
    {
        calcLocationFactorsOur();
        calcLocationFactorsEnemy();

        for (int i = 0; i < 3; i++) // считаем мораль 
        {
            MoralePresident[i] = _FactorItemPresident[i]._BUFFmaterials + _FactorItemPresident[i]._BUFFeconomic + _FactorItemPresident[i]._BUFFhealth + _FactorItemPresident[i]._BUFFfood;
            _totalMoralePresident += MoralePresident[i];
            MoralePresidentEnemy[i] = _FactorItemPresidentEnemy[i]._BUFFmaterials + _FactorItemPresidentEnemy[i]._BUFFeconomic + _FactorItemPresidentEnemy[i]._BUFFhealth + _FactorItemPresidentEnemy[i]._BUFFfood;
            _totalMoralePresidentEnemy += MoralePresidentEnemy[i];
        }
    }

    public void ReadyFight2() // Вывод нужных данных на экран 
    {

        _canvasCamera.transform.Find("Text_TotalMorale").GetComponent<Text>().text = "You morale " + _totalMoralePresident; // наша мораль 
        _canvasCamera.transform.Find("Text_TotalMoraleEnemy").GetComponent<Text>().text = "Enemy morale " + _totalMoralePresidentEnemy; // мораль противника 

        _PlaceFactor[0].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFmaterial; // ХП Сырья 
        _PlaceFactor[1].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFfood; // ХП продовольствия
        _PlaceFactor[2].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFeconomic; // хп экономики
        _PlaceFactor[3].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFhealth; // хп здравоохранения 
        _PlaceFactorEnemy[0].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFmaterial; // ХП Сырья
        _PlaceFactorEnemy[1].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFfood; // ХП продовольствия
        _PlaceFactorEnemy[2].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFeconomic; // хп экономики
        _PlaceFactorEnemy[3].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFhealth; // хп здравоохранения 

        if (_totalMoralePresidentEnemy <= 0) // если враг проиграл
        {
            Cursor.lockState = CursorLockMode.None; // включаем курсор 
            DataHolder._winnerHolder = true;
            DataHolder._moralePresidentHolder = _totalMoralePresident;
            SceneManager.LoadScene(3);
            SaveTXT();
        }
        else
            if (_totalMoralePresident <= 0) // если мы проиграли 
        {
            Cursor.lockState = CursorLockMode.None; // включаем курсор 
            DataHolder._winnerHolder = false;
            SceneManager.LoadScene(3);
            SaveTXT();
        }

        _testText = "\n OurMorale " + _totalMoralePresident + "\n MoraleEnemy " + _totalMoralePresidentEnemy + "\n" + _testText; // вывод данных 
        _scrollViewContent.transform.GetComponent<Text>().text = _testText;
    }
    void SaveTXT() // пишем Лог 
    {
        string path = Application.streamingAssetsPath + "/" + "log.txt";
        File.WriteAllText(path, _testText);
    }
}