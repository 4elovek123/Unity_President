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
    private string _idCard = "";
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

    private int _ourBuffAttack = 0; // общая атака всех президентов 
    private int _enemyBuffAttack = 0; // общая атака всех президентов 

    private int _ourBuffFortune = 0; // общая атака всех президентов 
    private int _enemyBuffFortune = 0; // общая атака всех президентов 

    private int _ourBuffDiplomation = 0; // общая атака всех президентов 
    private int _enemyBuffDiplomation = 0; // общая атака всех президентов 

    private bool _helperMain = true;
    private bool _helper = true;
    private bool _helper101 = false; ///////////////
    private bool _helper105 = false; ///////////////
    private bool _helperScaleCardIsTrue = false;
    private bool _boolOutlineToFactor = false;

    Transform _FirstSelectionFightCard = null;
    Transform _factorForAnimLogo;

    private Transform _dragFactor = null; // Фактор, который выбрали мы
    // private Transform _dragFactorEnemy = null; // Фактор, который выбрал враг 
    private string _dragFactorName;

    private int _materials_ability_protect = 0;
    private int _economic_ability_protect = 0;
    private int _economic_ability_attack = 0;
    private int _health_ability_protect = 0;
    private int _food_ability_protect = 0;

    [TextArea]
    private string _testText1;  // ВРЕМЕННО, ЛОГ НА ЭКРАНЕ 
    private string _testText2;  // ВРЕМЕННО, ЛОГ НА ЭКРАНЕ 
    public Transform _scrollViewContent1; // ВРЕМЕННО, ЛОГ НА ЭКРАНЕ 
    public Transform _scrollViewContent2; // ВРЕМЕННО, ЛОГ НА ЭКРАНЕ 
    private AnimationClip PresidentAnimation;

    private Transform[] ImageBuf = new Transform[3];
    private Transform _hitLast = null;

    public Material _redBoom;
    public Material _greenBoom;
    public Material _MaterialBoom; 

    private int _multiplyBlockOurEconomic = 1; // множители для эффекта от карт защит. Если 1, то никакого действия не оказывает. Если 0, то блокируется урон при подсчёте 
    private int _multiplyBlockOurMaterials = 1;
    private int _multiplyBlockOurFood = 1;
    private int _multiplyBlockOurHealth = 1;

    private int _multiplyBlockEnemyEconomic = 1; // множители для эффекта от карт защит. Если 1, то никакого действия не оказывает. Если 0, то блокируется урон при подсчёте 
    private int _multiplyBlockEnemyMaterials = 1;
    private int _multiplyBlockEnemyFood = 1;
    private int _multiplyBlockEnemyHealth = 1;

    private string path = Application.streamingAssetsPath + "/" + "log.txt"; // создали лог-файл 

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
            // Animation anim = _FightCardOnTable[i].transform.GetComponent<Animation>();
            // anim.Play("FightCardStartAnimation"); 

            Animator animatorOpen = _FightCardOnTable[i].transform.GetComponent<Animator>();
            animatorOpen.SetBool("Start", true);

        }

        Animation animTable = _Table.transform.GetComponent<Animation>(); // анимируется стол 
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
            //_FightCardOnTable[i].transform.localScale = Vector3.one;

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
            _ourBuffAttack = _ourBuffAttack + _FactorItemPresident[i]._buff_attack; // суммируем атаки всех президентов
            _ourBuffFortune = _ourBuffFortune + _FactorItemPresident[i]._buff_fortune; // суммируем атрибуты удачи всех президентов
            _ourBuffDiplomation = _ourBuffDiplomation + _FactorItemPresident[i]._buff_diplomation; // суммируем атрибуты дипломатии всех президентов

            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[0].color = _FactorItemPresident[i].ImageBuf[0].GetComponent<Image>().color;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[1].color = _FactorItemPresident[i].ImageBuf[1].GetComponent<Image>().color;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[2].color = _FactorItemPresident[i].ImageBuf[2].GetComponent<Image>().color;
            _Presidents_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[3].color = _FactorItemPresident[i].ImageBuf[3].GetComponent<Image>().color;
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
            _enemyBuffAttack = _enemyBuffAttack + _FactorItemPresidentEnemy[i]._buff_attack; // суммируем атаки всех президентов 
            _enemyBuffFortune = _enemyBuffFortune + _FactorItemPresidentEnemy[i]._buff_fortune; // суммируем атрибуты удачи всех президентов
            _enemyBuffDiplomation = _enemyBuffDiplomation + _FactorItemPresidentEnemy[i]._buff_diplomation; // суммируем атрибуты дипломатии всех президентов

            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[0].color = _FactorItemPresidentEnemy[i].ImageBuf[0].GetComponent<Image>().color;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[1].color = _FactorItemPresidentEnemy[i].ImageBuf[1].GetComponent<Image>().color;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[2].color = _FactorItemPresidentEnemy[i].ImageBuf[2].GetComponent<Image>().color;
            _PresidentsEnemy_before[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Image>()[3].color = _FactorItemPresidentEnemy[i].ImageBuf[3].GetComponent<Image>().color;
        }
        ReadyFight();
        ReadyFight2();

        // path = Application.streamingAssetsPath + "/" + "log.txt"; // создали лог-файл 
    }

    void calcLocationFactors() // расставляем факторы, пока топорно без зависимости от выбора президента
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

                        if (_hit.transform != _hitLast) // если выбрали карту, которая НЕ была на предыдущем шаге
                        {
                            _helper = true; // разрешение щелкнуть мышкой по карте 

                            if (_helper101 == false)
                            {
                                _helper101 = true; // хелпер, чтобы не гонять постоянно в Update
                                counter_card = k;
                                AnimationScaleCardOpen(_FightCardOnTable[counter_card]); // анимация увеличения текущей карты 
                                FightCard(counter_card);
                            }
                        }
                        else _helper = false; // иначе не разрешаем щелкать по ней 
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

            if (_hitLast != null)
            {
                _hitLast.transform.localRotation = Quaternion.identity; // и перевернули закрытую карту рубашкой обратно 
            }

            AnimationTransformCard(_FightCardOnTable[counter_card]); // анимация вылета выбранной карты на середину колоды 

        }

        if (_boolOutlineToFactor == true) // рейкаст по нужным факторам, когда нажали на боевую карту 
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity) && _hit.collider.tag == "Factors") // рейкаст, вычисляем наведение мышки на фактор 
            {
                if (_attackCard == 1 || _diplomationCard == 1) // атака и дипломатия ориентированы на факторы противника 
                {
                    if (Input.GetMouseButtonDown(0)) 
                    {
                        if (_hit.transform.name == "Enemy_Materials" && _enemyBUFFmaterial > 0 || // условие, что только при положительном ХП факторов будет реакция на нажатие 
                            _hit.transform.name == "Enemy_Food" && _enemyBUFFfood > 0 ||
                            _hit.transform.name == "Enemy_Economic" && _enemyBUFFeconomic > 0 ||
                            _hit.transform.name == "Enemy_Health" && _enemyBUFFhealth > 0)
                        {
                            _boolOutlineToFactor = false; // выключили обработку в Update 
                            _dragFactor = _hit.transform; // узнали, какой фактор выбрали 
                            AnimationArrow(_dragFactor); // вызываем анимацию стрелки 
                        }
                    }
                }

                else if (_protectCard == 1 || _fortuneCard == 1)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_hit.transform.name == "Our_Materials" && _ourBUFFmaterial > 0 || // условие, что только при положительном ХП факторов будет реакция на нажатие 
                            _hit.transform.name == "Our_Food" && _ourBUFFfood > 0 ||
                            _hit.transform.name == "Our_Economic" && _ourBUFFeconomic > 0 ||
                            _hit.transform.name == "Our_Health" && _ourBUFFhealth > 0)
                        //   if (_hit.transform.name == _PlaceFactor[k].name && Input.GetMouseButtonDown(0)) // если мы нажали на нужный фактор 
                        {
                            _boolOutlineToFactor = false; // выключили обработку в Update 
                            _dragFactor = _hit.transform; // узнали, какой фактор выбрали 
                            AnimationArrow(_dragFactor); // вызываем анимацию стрелки 
                        }
                    }
                }
            }
            else
            {
                _dragFactor = null;
            }
        }
    }

    void FightCard(int k) // запоминаем параметры выбранной боевой карты для расчёта 
    {
        myItemListCard = jSONControllerCard.myListFightCard; // вытягиваем лист со скрипта 
        for (int j = 0; j < myItemListCard.fight_card.Length; j++) // пробежались по всем картам из JSON-файла 
        {
            if (_FightCardOnTable[k].name == myItemListCard.fight_card[j].id) // нашли совпадения с картой из JSON
            {
                _idCard = myItemListCard.fight_card[j].id;
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

        _y.localPosition = new Vector3(_y.localPosition.x, 0.05f, _y.localPosition.z); // если использовать ротатор для смещения карты на нас (чтобы избежать Z файтинга, когда карта увеличивается), получается глюк, когда карта из раза в раз постепенно смещается от 0,0,0. Поэтому задаем смещение карты на себя без анимации, "железно" 

        Animator animatorOpen = _y.transform.GetComponent<Animator>();
        animatorOpen.SetBool("Open", true);
        _helperScaleCardIsTrue = true; // хелпер переключился, мы знаем, что объект заскейлен 
        _helper105 = false;
    }

    void AnimationScaleCardClose(Transform _y) // анимация размера боевой карты, которая проигрывается при убирании мыши 
    {
        _y.localPosition = new Vector3(_y.localPosition.x, 0, _y.localPosition.z); // если использовать ротатор для смещения карты на нас (чтобы избежать Z файтинга, когда карта увеличивается), получается глюк, когда карта из раза в раз постепенно смещается от 0,0,0. Поэтому задаем смещение карты на себя без анимации, "железно"

        Animator animatorOpen = _y.transform.GetComponent<Animator>();
        animatorOpen.SetBool("Open", false);

        _helperScaleCardIsTrue = false;
        _helper101 = false;
    }

    void AnimationTransformCard(Transform _y) // анимация вылета боевой карты на центр, которая проигрывается при нажатии кнопк мыши, и других 
    {
        _y.transform.position = new Vector3(_camera.ViewportToWorldPoint(new Vector3(.5f, .5f, 0)).x, _y.transform.position.y, _y.transform.position.z); // карта на центр колоды 
        _helper101 = false;
        // включаем анимацию логотипов у нужных факторов 
        if (_attackCard == 1 || _diplomationCard == 1) // атака и дипломатия ориентированы на факторы противника 
        {
            for (int i = 0; i < _PlaceFactorEnemy.Length; i++)
            {
                if (_PlaceFactorEnemy[i].name == "Enemy_Materials" && _enemyBUFFmaterial > 0 || // условие, что только при положительном ХП факторов будет крутиться лого 
                    _PlaceFactorEnemy[i].name == "Enemy_Food" && _enemyBUFFfood > 0 ||
                    _PlaceFactorEnemy[i].name == "Enemy_Economic" && _enemyBUFFeconomic > 0 ||
                    _PlaceFactorEnemy[i].name == "Enemy_Health" && _enemyBUFFhealth > 0)
                { 
                    AnimationFactors(_PlaceFactorEnemy[i]); // запускаем анимацию всех логотипов, предлагая игроку выбрать фактор 
                }
            }
        }

        if (_protectCard == 1 || _fortuneCard == 1) // защита и удача ориентированы на свои факторы
        {
            for (int i = 0; i < _PlaceFactor.Length; i++)
            {
                if (_PlaceFactor[i].name == "Our_Materials" && _ourBUFFmaterial > 0 ||
                    _PlaceFactor[i].name == "Our_Food" && _ourBUFFfood > 0 ||
                    _PlaceFactor[i].name == "Our_Economic" && _ourBUFFeconomic > 0 ||
                    _PlaceFactor[i].name == "Our_Health" && _ourBUFFhealth > 0)
                
                {
                    AnimationFactors(_PlaceFactor[i]); // запускаем анимацию всех логотипов, предлагая игроку выбрать фактор 
                }
            }
        }
    }

    void AnimationFactors(Transform _factorForAnim) // запускаем анимацию Лого факторов 
    {
        _factorForAnimLogo = _factorForAnim.transform.Find(_factorForAnim.name).transform.Find("Logo"); // нашли Логотип фактора 
        Animator animatorFactor = _factorForAnimLogo.transform.GetComponent<Animator>(); // достали аниматор 
        animatorFactor.SetBool("StartRotate", true); // включили вращение логотипа 
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

    IEnumerator Pause() // пауза анимации стрелки
    {
        yield return new WaitForSeconds(1.25f); // пока так, в секундах. Через 2 сек закончилась анимация стрелки 
        StopAnimationFactors(); // остановили все логотипы 
        _Arrow.SetActive(false); // выключили стрелку 

        if (_attackCard == 1 || _diplomationCard == 1) // если атака или диломатия
        {
            _MaterialBoom.CopyPropertiesFromMaterial(_redBoom); // меняем цвета у частиц на красный 
        }
        else if (_protectCard == 1 || _fortuneCard == 1) // если защита или удача 
        {
            _MaterialBoom.CopyPropertiesFromMaterial(_greenBoom); // меняем цвета у частиц на зеленый  
        }

        _dragFactor.Find("Boom").gameObject.SetActive(true); // включили фейерверк 
        StartCoroutine(Pause2());
    }

    void StopAnimationFactors() // пока так топорно и с Find останавливаем анимацию Лого, потом убрать 
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

    IEnumerator Pause2() // пауза анимации нашего фейерверка 
    {
        yield return new WaitForSeconds(1); // пока так, в секундах. Через 1 сек закончилась анимация взрыва 
        ResetAnimationCard();
    }

    void ResetAnimationCard()
    { 
        _dragFactor.Find("Boom").gameObject.SetActive(false); // выключили фейерверк 

        AnimationScaleCardClose(_FightCardOnTable[counter_card]); // сбросили анимацию карты 
        _FightCardOnTable[counter_card].transform.localPosition = Vector3.zero; // вернули карту на место 
        //_FightCardOnTable[counter_card].transform.Rotate(180, 180, 0); // и перевернули рубашкой вверх 
        _FightCardOnTable[counter_card].transform.localEulerAngles = new Vector3(0, 0, 180); // и перевернули рубашкой вверх 
        CalcOurFight(); // вызываем расчёт очков в бою 
    }

    void CalcOurFight() // Расчёт очков в бою после нашего хода 
    {
        if (_dragFactor != null)
        {
            RenameFactors(); 
            _testText1 = " We used the card " + _FightCardOnTable[counter_card].name + " to " + _dragFactorName + "\n" + _testText1;
            _testText2 = " We used the card " + _FightCardOnTable[counter_card].name + " to " + _dragFactorName + "\n" + _testText2;

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

            if (_attackCard == 1)
            { 
                CalcDamageAtack(true); // пересчитали урон на мораль или факторы врага 
                CalcCoastFactors(true); // вычли цену карты из фактора 
            }
            else
            {
                if (_protectCard == 1)
                {
                    CalcDamageProtect(true);
                    CalcCoastFactors(true); // вычли цену карты из фактора  
                }
                else
                {
                    if (_diplomationCard == 1) // по дипломатии пока нет функционала 
                    {
                        CalcDamageDiplomation(true);
                        CalcCoastFactors(true); // вычли цену карты из фактора 
                    }
                    else
                    {
                        if (_fortuneCard == 1)
                        {
                            CalcCoastFactors(true); // вычли цену карты из фактора
                            CalcDamageFortune(true); 
                        }
                    }
                }
            }
        }

        ReadyFight2(); // Вызываем вывод нужных данных на экран 
        Reset_multiplyBlock(false); // обнуляем эффект от карты соперника "Защита" (на факторы соперника) после нашего хода 
        CalcEnemyFight(); // Ход соперника 
    }

    void CalcEnemyFight() // Соперник "Выбирает" карту 
    {
        int _enemy = Random.Range(0, _FightCardOnTable.Length); // выбираем карту (за столом) наугад 
        FightCard(_enemy); // получаем её данные 

        if (_attackCard == 1 || _diplomationCard == 1) // атака и дипломатия ориентированы на факторы противника 
        {
            int i = Random.Range(0, 4); // int инклюзивный, 4 не входит, выбор от 0 до 3 
            _dragFactor = _PlaceFactor[i].Find(_PlaceFactor[i].name); 
        }

        if (_protectCard == 1 || _fortuneCard == 1) // защита и удача ориентированы на свои факторы
        {
            int i = Random.Range(0, 4); // int инклюзивный, 4 не входит, выбор от 0 до 3 
            _dragFactor = _PlaceFactorEnemy[i].Find(_PlaceFactorEnemy[i].name); 
        }

        RenameFactors();
        _testText1 = " The opponent used the card " + _FightCardOnTable[_enemy].name + " to " + _dragFactorName + "\n" + _testText1;
        _testText2 = " The opponent used the card " + _FightCardOnTable[_enemy].name + " to " + _dragFactorName + "\n" + _testText2; 

        StartCoroutine(PauseEnemyCoroutine()); // пауза после хода соперника 
    }
    IEnumerator PauseEnemyCoroutine() // пауза после выбора карты соперником 
    {
        yield return new WaitForSeconds(1.5f);
        CalcEnemyFight2();
    }

    void RenameFactors()
    {
        if (_dragFactor.name == "Enemy_Materials" || _dragFactor.name == "Our_Materials") _dragFactorName = "Material";
        if (_dragFactor.name == "Enemy_Economic" || _dragFactor.name == "Our_Economic") _dragFactorName = "Economic";
        if (_dragFactor.name == "Enemy_Health" || _dragFactor.name == "Our_Health") _dragFactorName = "Health";
        if (_dragFactor.name == "Enemy_Food" || _dragFactor.name == "Our_Food") _dragFactorName = "Food";
    }


    ////////////////////
    
    void CalcDamageAtack(bool _isOurAttack) // считаем урон от атаки в зависимости от типа карты (и нашего или противника Атрибута атаки) 
    {
        void calcDamage(int _damage)
        {
            if (_dragFactor.name == "Enemy_Materials")
            {
                _enemyBUFFmaterial = _enemyBUFFmaterial - _damage * _multiplyBlockEnemyMaterials; // Множители для эффекта от карт защит. Если 1, то никакого действия не оказывает. Если 0, то блокируется урон от атаки при подсчёте 
            }
            if (_dragFactor.name == "Enemy_Economic") _enemyBUFFeconomic = _enemyBUFFeconomic - _damage * _multiplyBlockEnemyEconomic; 
            if (_dragFactor.name == "Enemy_Health") _enemyBUFFhealth = _enemyBUFFhealth - _damage * _multiplyBlockEnemyHealth; 
            if (_dragFactor.name == "Enemy_Food") _enemyBUFFfood = _enemyBUFFfood - _damage * _multiplyBlockEnemyFood; 

            if (_dragFactor.name == "Our_Materials") _ourBUFFmaterial = _ourBUFFmaterial - _damage * _multiplyBlockOurMaterials; 
            if (_dragFactor.name == "Our_Economic") _ourBUFFeconomic = _ourBUFFeconomic - _damage * _multiplyBlockOurEconomic;
            if (_dragFactor.name == "Our_Health") _ourBUFFhealth = _ourBUFFhealth - _damage * _multiplyBlockOurHealth;
            if (_dragFactor.name == "Our_Food") _ourBUFFfood = _ourBUFFfood - _damage * _multiplyBlockOurFood; 
        }

        int _damage;
        if (_isOurAttack) // если мы атакуем 
        {
            if (_idCard == "airStrike")
            {
                _damage = 3 + _ourBuffAttack;
                calcDamage(_damage);
            }
            else if (_idCard == "intelligenceData")
            {
                _damage = 2 + _ourBuffAttack / 2;
                calcDamage(_damage);
            }
            else if (_idCard == "sunction")
            {
                _damage = _ourBuffAttack / 2;
                calcDamage(_damage);
            }

            else if (_idCard == "isolation")
            {
                _damage = _ourBuffAttack / 3;
                _enemyBUFFmaterial = _enemyBUFFmaterial - _damage * _multiplyBlockEnemyMaterials;
                _enemyBUFFeconomic = _enemyBUFFeconomic - _damage * _multiplyBlockEnemyEconomic;
                _enemyBUFFhealth = _enemyBUFFhealth - _damage * _multiplyBlockEnemyHealth;
                _enemyBUFFfood = _enemyBUFFfood - _damage * _multiplyBlockEnemyFood;
            }
        }
        else // если враг атакует 
        {
            if (_idCard == "airStrike")
            {
                _damage = 3 + _ourBuffAttack;
                calcDamage(_damage);
            }
            else if (_idCard == "intelligenceData")
            {
                _damage = 2 + _ourBuffAttack / 2;
                calcDamage(_damage);
            }
            else if (_idCard == "sunction")
            {
                _damage = _ourBuffAttack / 2;
                calcDamage(_damage);
            }

            else if (_idCard == "isolation")
            {
                _damage = _ourBuffAttack / 3;
                _ourBUFFmaterial = _ourBUFFmaterial - _damage * _multiplyBlockOurMaterials; 
                _ourBUFFeconomic = _ourBUFFeconomic - _damage * _multiplyBlockOurEconomic;
                _ourBUFFhealth = _ourBUFFhealth - _damage * _multiplyBlockOurHealth;
                _ourBUFFfood = _ourBUFFfood - _damage * _multiplyBlockOurFood;
            }
        }
    }

    void CalcDamageProtect(bool _isOurAttack) // считаем эффект от защиты в зависимости от типа карты (и нашего или противника Атрибута атаки) 
    {
        if (_isOurAttack) // если мы ходим  
        {
            if (_idCard == "customsReform")
            {
                _totalMoralePresident = _totalMoralePresident - 5;
                _multiplyBlockOurEconomic = 0;
            }
            else if (_idCard == "militaryPosition")
            {
                _totalMoralePresident = _totalMoralePresident - 5;
                _multiplyBlockOurMaterials = 0;
            }
            else if (_idCard == "pestControl")
            {
                _totalMoralePresident = _totalMoralePresident - 5;
                _multiplyBlockOurFood = 0;
            }
            else if (_idCard == "accession")
            {
                _totalMoralePresident = _totalMoralePresident - 10;
                _multiplyBlockOurEconomic = 0;
                _multiplyBlockOurMaterials = 0;
                _multiplyBlockOurFood = 0;
                _multiplyBlockOurHealth = 0;
            }
        }
        else // если враг атакует 
        {
            if (_idCard == "customsReform")
            {
                _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - 5;
                _multiplyBlockEnemyEconomic = 0;
            }
            else if (_idCard == "militaryPosition")
            {
                _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - 5;
                _multiplyBlockEnemyMaterials = 0;
            }
            else if (_idCard == "pestControl")
            {
                _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - 5;
                _multiplyBlockEnemyFood = 0;
            }

            else if (_idCard == "accession")
            {
                _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - 10;
                _multiplyBlockEnemyEconomic = 0;
                _multiplyBlockEnemyMaterials = 0;
                _multiplyBlockEnemyFood = 0;
                _multiplyBlockEnemyHealth = 0;
            }
        }
    }

    void CalcDamageDiplomation(bool _isOurAttack)
    {
        int _damageOur(int _addBufDiplomation) // расчёт _multiplyBlockOur в зависимости от шансов (прокачанная дипломатия) 
        {
            int result = 0;
            if (Random.Range(0, 101) >= _ourBuffDiplomation + _addBufDiplomation) // выбираем рандомное значение от 0 до 100% с вероятностью выбора в _ourBuffDiplomation + _addBufDiplomation процентов 
            {
                result = 0; // успешная блокировка урона 
            }
            else
            {
                result = 1; // безуспешная блокировка урона 
            }
            return result;
        }

        int _damageEnemy(int _addBufDiplomation) // расчёт _multiplyBlockOur в зависимости от шансов (прокачанная дипломатия) 
        {
            int result = 0;
            if (Random.Range(0, 101) >= _enemyBuffDiplomation + _addBufDiplomation) // выбираем рандомное значение от 0 до 100% с вероятностью выбора в _ourBuffDiplomation + _addBufDiplomation процентов 
            {
                result = 0; // успешная блокировка урона 
            }
            else
            {
                result = 1; // безуспешная блокировка урона 
            }
            return result;
        }

        if (_isOurAttack) // если мы ходим  
        {
            if (_idCard == "patronage")
            {
                if (_materialsCard == 1) _multiplyBlockOurMaterials = _damageOur(0);
                else if (_economicCard == 1) _multiplyBlockOurEconomic = _damageOur(0);
                else if (_healthCard == 1) _multiplyBlockOurHealth = _damageOur(0);
                else if (_foodCard == 1) _multiplyBlockOurFood = _damageOur(0);
            }
            if (_idCard == "diplomaticImmunty")
            {
                if (_damageOur(0) == 1) // в patronage фактор не получает урон с верояностью _ourBuffDiplomation + _addBufDiplomation, а здесь наоборот, Вероятность снижена на _ourBuffDiplomation + _addBufDiplomation, поэтому "инвертируем" result 
                {
                    _multiplyBlockOurMaterials = 1;
                    _multiplyBlockOurFood = 1;
                }
                else if (_damageOur(0) == 0)
                {
                    _multiplyBlockOurMaterials = 0;
                    _multiplyBlockOurFood = 0; 
                }
            }
            if (_idCard == "strategicLoan") // не проработал функционал, пока так 
            {
                _multiplyBlockOurEconomic = _damageOur(0);
                _multiplyBlockOurHealth = _damageOur(0);
            }
            if (_idCard == "energyExpansion")
            {
                _multiplyBlockOurMaterials = _damageOur(40);
                _multiplyBlockOurFood = _damageOur(40);
            }
        }
        else // если враг ходит 
        {
            if (_idCard == "patronage")
            {
                if (_materialsCard == 1) _multiplyBlockEnemyMaterials = _damageEnemy(0);
                else if (_economicCard == 1) _multiplyBlockEnemyEconomic = _damageEnemy(0);
                else if (_healthCard == 1) _multiplyBlockEnemyHealth = _damageEnemy(0);
                else if (_foodCard == 1) _multiplyBlockEnemyFood = _damageEnemy(0);
            }
            if (_idCard == "diplomaticImmunty")
            {
                if (_damageEnemy(0) == 1) // в patronage фактор не получает урон с верояностью _ourBuffDiplomation + _addBufDiplomation, а здесь наоборот, Вероятность снижена на _ourBuffDiplomation + _addBufDiplomation, поэтому "инвертируем" result 
                {
                    _multiplyBlockEnemyMaterials = 1;
                    _multiplyBlockEnemyFood = 1;
                }
                else if (_damageEnemy(0) == 0)
                {
                    _multiplyBlockEnemyMaterials = 0;
                    _multiplyBlockEnemyFood = 0;
                }
            }
            if (_idCard == "strategicLoan") // не проработал функционал, пока так 
            {
                _multiplyBlockEnemyEconomic = _damageEnemy(0);
                _multiplyBlockEnemyHealth = _damageEnemy(0);
            }
            if (_idCard == "energyExpansion")
            {
                _multiplyBlockEnemyMaterials = _damageEnemy(40);
                _multiplyBlockEnemyFood = _damageEnemy(40);
            }
        }
    }

    void CalcDamageFortune(bool _isOurAttack)
    {
        int _fortuneDamage = 0;
        if (_isOurAttack) // если мы ходим  
        {
            if (_idCard == "harvest")
            {
                _fortuneDamage = 10 + _ourBuffFortune;
            }
            else if (_idCard == "elections")
            {
                _fortuneDamage = 10 + _ourBUFFhealth;
            }
            else if (_idCard == "techological")
            {
                _fortuneDamage = 10 + _ourBUFFfood; // решение не такое, но пока оставим 
            }
            else if (_idCard == "educationalInfrastructure")
            {
                _fortuneDamage = 10 + _ourBUFFmaterial; // решение не такое, но пока оставим
            }
            
            if (Random.Range(0, 101) >= _fortuneDamage) // выбираем рандомное значение от 0 до 100% с вероятностью выбора в _fortuneDamage процентов 
            {
                _totalMoralePresident = _totalMoralePresident + _deltamorale_positive;
            }
            else _totalMoralePresident = _totalMoralePresident + _deltamorale_negative; // в JSON'е _deltamorale_negative отрицательное, поэтому прибавляем 

        }
        else
        {
            if (_idCard == "harvest") _fortuneDamage = 10 + _enemyBuffFortune;
            else if (_idCard == "elections") _fortuneDamage = 10 + _enemyBUFFhealth;
            else if (_idCard == "techological") _fortuneDamage = 10 + _enemyBUFFfood; // решение не такое, но пока оставим 
            else if (_idCard == "educationalInfrastructure") _fortuneDamage = 10 + _enemyBUFFmaterial; // решение не такое, но пока оставим
          
            if (Random.Range(0, 101) >= _fortuneDamage) // выбираем рандомное значение от 0 до 100% с вероятностью выбора в _fortuneDamage процентов 
            {
                _totalMoralePresidentEnemy = _totalMoralePresidentEnemy + _deltamorale_positive;
            }
            else _totalMoralePresidentEnemy = _totalMoralePresidentEnemy + _deltamorale_negative;
        }
    }

    void CalcCoastFactors(bool _isOurAttack) // вычитаем цену карты из факторов и морали 
    {
        if (_isOurAttack) // если мы атакуем 
        {
            _totalMoralePresident = _totalMoralePresident - _costCard; // пока будем дублировать вычитаение цены карты в Морали, потому что не можем в игровом процессе пересчитывать мораль как сумму факторов. Потому что в процессе игры на мораль влияют эффекты, которые не затрагивают факторы.  

            if (_materialsCard == 1) _ourBUFFmaterial = _ourBUFFmaterial - _costCard; 
            if (_economicCard == 1) _ourBUFFeconomic = _ourBUFFeconomic - _costCard; 
            if (_healthCard == 1) _ourBUFFhealth = _ourBUFFhealth - _costCard; 
            if (_foodCard == 1) _ourBUFFfood = _ourBUFFfood - _costCard; 
        }
        else // если враг атакует 
        {
            _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard; // пока будем дублировать вычитание цены карты в Морали, потому что не можем в игровом процессе пересчитывать мораль как сумму факторов. Потому что в процессе игры на мораль влияют эффекты, которые не затрагивают факторы 

            if (_materialsCard == 1) _enemyBUFFmaterial = _enemyBUFFmaterial - _costCard; 
            if (_economicCard == 1) _enemyBUFFeconomic = _enemyBUFFeconomic - _costCard; 
            if (_healthCard == 1) _enemyBUFFhealth = _enemyBUFFhealth - _costCard;
            if (_foodCard == 1) _enemyBUFFfood = _enemyBUFFfood - _costCard;
        } 
    }

    void CalcEnemyFight2() // расчёт очков от хода соперника 
    {
        if (_attackCard == 1) 
        {
            _MaterialBoom.CopyPropertiesFromMaterial(_redBoom); // меняем цвета у частиц на красный
            CalcDamageAtack(false); // считаем урон Атаки  
            CalcCoastFactors(false); // пересчитываем факторы исходя из цены 
        }
        else
        {
            if (_protectCard == 1)
            {
                _MaterialBoom.CopyPropertiesFromMaterial(_greenBoom); // меняем цвета у частиц на красный
                CalcDamageProtect(false);
                CalcCoastFactors(false); // пересчитываем факторы исходя из цены 
            }
            else
            {
                if (_diplomationCard == 1) // по дипломатии пока нет функционала 
                {
                    _MaterialBoom.CopyPropertiesFromMaterial(_redBoom); // меняем цвета у частиц на красный 
                    CalcDamageDiplomation(false);
                    CalcCoastFactors(false); // пересчитываем факторы исходя из цены 
                }
                else
                {
                    if (_fortuneCard == 1)
                    {
                        _MaterialBoom.CopyPropertiesFromMaterial(_greenBoom); // меняем цвета у частиц на красный 
                        CalcDamageFortune(false);
                        CalcCoastFactors(false); // пересчитываем факторы исходя из цены 
                    }
                }
            }
        }

        _dragFactor.Find("Boom").gameObject.SetActive(true); // включили фейерверк от вражеского хода 
        _hitLast = _FightCardOnTable[counter_card]; // записали, что в этом ходу была выбрана такая-то карта 
        ReadyFight2(); // вывод на экран расчётов 

        StartCoroutine(Pause3()); 
    }

    IEnumerator Pause3() // пауза анимации фейерверка 
    {
        yield return new WaitForSeconds(2);
        _dragFactor.Find("Boom").gameObject.SetActive(false); // выключили фейерверк 
        Cursor.lockState = CursorLockMode.None; // включаем курсор только после хода соперника 
        _helperMain = true; // возвращаем хелперы в рабочее состояние 
        _helper = true;

        Reset_multiplyBlock(true); // обнуляем эффект от карты Защиты на наши факторы после хода соперника 
    }
    
    void Reset_multiplyBlock(bool _isOurAttack)
    {
        if (_isOurAttack)
        {
            _multiplyBlockOurEconomic = 1;
            _multiplyBlockOurMaterials = 1;
            _multiplyBlockOurFood = 1;
            _multiplyBlockOurHealth = 1;
        }
        else
        {
            _multiplyBlockEnemyEconomic = 1;
            _multiplyBlockEnemyMaterials = 1;
            _multiplyBlockEnemyFood = 1;
            _multiplyBlockEnemyHealth = 1;
        }
    }

public void ReadyFight() // рассчитывается 1 раз в начале при нажатии кнопки "Ready" 
    {
        calcLocationFactors();

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

        if (_ourBUFFmaterial < 0) _ourBUFFmaterial = 0; // очки факторов не могут быть отрицательными 
        if (_ourBUFFeconomic < 0) _ourBUFFeconomic = 0;
        if (_ourBUFFhealth < 0) _ourBUFFhealth = 0;
        if (_ourBUFFfood < 0) _ourBUFFfood = 0;
        if (_enemyBUFFmaterial < 0) _enemyBUFFmaterial = 0; // очки факторов не могут быть отрицательными 
        if (_enemyBUFFeconomic < 0) _enemyBUFFeconomic = 0;
        if (_enemyBUFFhealth < 0) _enemyBUFFhealth = 0;
        if (_enemyBUFFfood < 0) _enemyBUFFfood = 0;

        _PlaceFactor[0].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFmaterial; // ХП Сырья 
        _PlaceFactor[1].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFfood; // ХП продовольствия
        _PlaceFactor[2].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFeconomic; // хп экономики
        _PlaceFactor[3].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFhealth; // хп здравоохранения 
        _PlaceFactorEnemy[0].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFmaterial; // ХП Сырья
        _PlaceFactorEnemy[1].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFfood; // ХП продовольствия
        _PlaceFactorEnemy[2].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFeconomic; // хп экономики
        _PlaceFactorEnemy[3].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFhealth; // хп здравоохранения 

        // if (_totalMoralePresidentEnemy <= 0) // если враг проиграл
        if (_enemyBUFFmaterial + _enemyBUFFeconomic + _enemyBUFFhealth + _enemyBUFFfood <= 0 || _totalMoralePresidentEnemy <= 0) // если враг проиграл
        {
            DataHolder._winnerHolder = true;
            DataHolder._moralePresidentHolder = _totalMoralePresident;
            StartCoroutine(Pause4());
            Cursor.lockState = CursorLockMode.None; // включаем курсор 
            //SaveTXT();
        }
        else if (_ourBUFFmaterial + _ourBUFFeconomic + _ourBUFFhealth + _enemyBUFFfood <= 0 || _totalMoralePresident <= 0) // если мы проиграли 
        {
            DataHolder._winnerHolder = false;
            StartCoroutine(Pause4());
            Cursor.lockState = CursorLockMode.None; // включаем курсор 
            //SaveTXT();
        }
        _testText2 = "\n OurMorale " + _totalMoralePresident + "\n" + " MoraleEnemy " + _totalMoralePresidentEnemy + "\n" + _testText2; // вывод данных 

        _scrollViewContent1.transform.GetComponent<Text>().text = _testText1;
        _scrollViewContent2.transform.GetComponent<Text>().text = _testText2;
        SaveTXT();
    }

    IEnumerator Pause4() // пауза перед завершением 
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(3);
    }
    void SaveTXT() // пишем Лог 
    {
        File.WriteAllText(path, ""); // очистили файл 
        File.WriteAllText(path, _testText2); // записали в лог 
    }
}