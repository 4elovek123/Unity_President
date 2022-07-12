using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightRules : MonoBehaviour
{
    public Factor_Item[] _FactorItemPresident = new Factor_Item[3];
    public Factor_Item[] _FactorItemPresidentEnemy = new Factor_Item[3];
    //public GameObject[] _President = new GameObject[3];
    //public GameObject[] _EnemyPresident = new GameObject[3];

    int[] MoralePresident = new int[3];
    int[] MoralePresidentEnemy = new int[3];
    int _totalMoralePresident = 0;
    int _totalMoralePresidentEnemy = 0; 
    public Canvas _canvasCamera;
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

    public int _ourBUFFmaterial=0;
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
    // private int counter_card = 0; // передаём номер карты из массива карт (j в цикле) 
    public int _costCard = 0;
    private int _materialsCard = 0;
    private int _economicCard = 0;
    private int _healthCard = 0;
    private int _foodCard = 0;
    private int _attackCard = 0;
    private int _protectCard = 0;
    private int _diplomationCard = 0;
    private int _fortuneCard = 0;

    private bool _helper=true;
    private bool _boolSwitch = true;

    //Transform _rootGO;
    //private Transform _son; // сын 

    void Start()
    {
        
    }

    public void RandomCardToPlace() // заносим в массивы все карты сцены, места для карт за столом, выбираем рандомные, помещаем за стол. Рассчитывается при нажатии кнопки "Ready" 
    {
        _FightCardInScene = new Transform[Folder_FightCardInScene.transform.childCount];
        for (int i = 0; i < _FightCardInScene.Length; i++) // пробежались по всем боевым картам в сцене в папке FightCardInScene (16 штук сейчас) 
        {
            _FightCardInScene[i] = Folder_FightCardInScene.transform.GetChild(i).transform; // занесли их в массив _FightCardInScene[]
        }

        _FightCardPlace = new Transform[Folder_FightCardPlace.transform.childCount];
        _FightCardOnTable = new Transform[_FightCardPlace.Length];


        int[] _random = new int[_FightCardPlace.Length]; // формируем упорядоченный массив с 0 до _FightCardPlace.Length (до 9 включительно)
        for (int i = 0; i < _random.Length; i++) { _random[i] =i; }

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

        int[] MixArray = Mix(_random); // перемешанный массив для выбора случайных карт

        for (int i = 0; i < _FightCardPlace.Length; i++) // пробежались по всем Place будущих карт 
        {
            _FightCardPlace[i] = Folder_FightCardPlace.transform.GetChild(i).transform; // занесли их в массив _FightCardPlace[] 
            _FightCardOnTable[i] = _FightCardInScene[MixArray[i]]; // заполнили массив случайными картами
                //_FightCardOnTable[i] = Instantiate(_FightCardInScene[_random], Vector3.zero, Quaternion.identity); // скопировали рандомный префаб из массива _FightCardInScene в массив OnTable. Возможны повторения карт 
                //_FightCardOnTable[i].name = _FightCardInScene[_random].name; // задаём такое же имя, как у оригинала (без приставки Clone) 
            _FightCardOnTable[i].transform.SetParent(_FightCardPlace[i]); // поместили боевые карты в Place[] 
            _FightCardOnTable[i].transform.localPosition = Vector3.zero; // сбросили позицию
            _FightCardOnTable[i].transform.localRotation = Quaternion.identity; // сбросили вращение 

                //Debug.Log("_FightCardOnTable[" + i + "] = " + _FightCardOnTable[i]);
        }
    }

    public void StartButton()
    {
        for (int i = 0; i < _Presidents_before.Length; i++) // переносим UI президентов 
        {
            _Presidents_before[i].transform.SetParent(_Place_after[i].transform);
            _Presidents_before[i].transform.localPosition = Vector3.zero;
            _Presidents_before[i].transform.localRotation = Quaternion.identity;
            _Presidents_before[i].transform.localScale = Vector3.one;
            _Presidents_before[i].transform.GetComponent<Image>().enabled = false;
            _Presidents_before[i].transform.Find("Text_Factor").GetComponent<Text>().enabled = false;
            _Presidents_before[i].transform.Find("Text_Abilities").GetComponent<Text>().enabled = false;
            _Presidents_before[i].transform.Find("Card").GetComponent<CanvasGroup>().blocksRaycasts = false; //временное решение. Выключаем канвасгрупп рейкаст, чтобы не смогли попасть по UI президентов, когда сидим за столом 
            _ourBUFFmaterial += _FactorItemPresident[i]._BUFFmaterials;
            _ourBUFFhealth += _FactorItemPresident[i]._BUFFhealth;
            _ourBUFFfood += _FactorItemPresident[i]._BUFFfood;
            _ourBUFFeconomic += _FactorItemPresident[i]._BUFFeconomic;
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
            _enemyBUFFmaterial += _FactorItemPresidentEnemy[i]._BUFFmaterials;
            _enemyBUFFhealth += _FactorItemPresidentEnemy[i]._BUFFhealth;
            _enemyBUFFfood += _FactorItemPresidentEnemy[i]._BUFFfood;
            _enemyBUFFeconomic += _FactorItemPresidentEnemy[i]._BUFFeconomic;
        }

        ReadyFight();
        calcLocationFactorsOur();
        calcLocationFactorsEnemy(); 
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
        _PlaceFactor[2].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFeconomic; // хп экономики

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
        if (Input.GetMouseButtonDown(0) && _helper == true) // если щелкнули мышкой по боевой карте 
        {
            // Debug.Log("0");
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity))
            {
                //Debug.Log("1"); 
                for (int k = 0; k < _FightCardOnTable.Length; k++) // перебираем, попали ли по карте (пока у нас нет больше коллайдеров, так что только по ним) и по какой 
                {
                    if (_hit.transform.name == _FightCardOnTable[k].name) // && _boolSwitch == true) // выяснили имя карты, по которой попали 
                    {
                        //_boolSwitch = false;
                        // Debug.Log(_FightCardOnTable[k].name);
                        FightCard(k); // вызываем обработку 
                    }
                }
            }
        }

        //if (Input.GetMouseButtonUp(0)) _helper = true; // как только мышь отпустили, хелпер в True
    }

    void FightCard(int k) // запоминаем параметры выбранной боевой карты для расчёта 
    {
        myItemListCard = jSONControllerCard.myListFightCard; // вытягиваем лист со скрипта 
        for (int j = 0; j < myItemListCard.fight_card.Length; j++) // пробежались по всем картам из JSON-файла 
        {
            if (_FightCardOnTable[k].name == myItemListCard.fight_card[j].id) // нашли совпадения с картой из JSON
            {
                Debug.Log("3");
                _helper = false; // хелпер в 0, чтобы не обрабатывать в Update рэйкаст, пока мышь нажата
                //counter_card = j; // запомнили, в какой ячейке массива карт нужная карта 
                _costCard = myItemListCard.fight_card[j].cost;
                _materialsCard = myItemListCard.fight_card[j].materials;
                _economicCard = myItemListCard.fight_card[j].economic;
                _healthCard = myItemListCard.fight_card[j].health;
                _foodCard = myItemListCard.fight_card[j].food;
                _attackCard = myItemListCard.fight_card[j].attack;
                _protectCard = myItemListCard.fight_card[j].protect;
                _diplomationCard = myItemListCard.fight_card[j].diplomation;
                _fortuneCard = myItemListCard.fight_card[j].fortune;

                AnimationCard(_FightCardOnTable[k]);
            }
        }
    }

    void AnimationCard(Transform y) // анимация боевой карты, пока топорная (изменение масштаба)
    {
        Debug.Log("Anim");
        Vector3 _mainlocalCard = y.transform.localScale; 
        y.transform.localScale = new Vector3(y.transform.localScale.x*1.25f, y.transform.localScale.y * 1.25f, y.transform.localScale.z * 1.25f); // увеличиваем карту в 1,25 раза     
        StartCoroutine(waiter(y.gameObject, _mainlocalCard)); // ждём 
        Debug.Log("5"); 
    } 

    IEnumerator waiter(GameObject y, Vector3 _mainlocalCard) // пауза для анимации 
    {
        Debug.Log("6");
        yield return new WaitForSeconds(1); // ждём 1 секунду
        y.transform.localScale = _mainlocalCard; // сбрасываем масштаб
        _helper = true;
        //_boolSwitch = true;
        CalcFight(); 
    }

    void CalcFight() // Расчёт очков в бою 
    { 
        if (_attackCard == 1)
        {
            if (_materialsCard == 1) _enemyBUFFmaterial -= _costCard;
            if (_economicCard == 1) _enemyBUFFeconomic -= _costCard;
            if (_healthCard == 1) _enemyBUFFhealth -= _costCard;
            if (_foodCard == 1) _enemyBUFFfood -= _costCard;
        }

        if (_protectCard == 1)
        {
            if (_materialsCard == 1) _ourBUFFmaterial += _costCard;
            if (_economicCard == 1) _ourBUFFeconomic += _costCard;
            if (_healthCard == 1) _ourBUFFhealth += _costCard;
            if (_foodCard == 1) _ourBUFFfood += _costCard;
        }
        calcLocationFactorsOur();
        calcLocationFactorsEnemy();
        ReadyFight(); 
    }

    public void ReadyFight() // рассчитывается при нажатии кнопки "Ready" 
    {
        for (int i = 0; i < 3; i++) // считаем мораль 
        {
            MoralePresident[i] = _FactorItemPresident[i]._BUFFmaterials + _FactorItemPresident[i]._BUFFeconomic + _FactorItemPresident[i]._BUFFhealth + _FactorItemPresident[i]._BUFFfood;
            _totalMoralePresident += MoralePresident[i];
            MoralePresidentEnemy[i] = _FactorItemPresidentEnemy[i]._BUFFmaterials + _FactorItemPresidentEnemy[i]._BUFFeconomic + _FactorItemPresidentEnemy[i]._BUFFhealth + _FactorItemPresidentEnemy[i]._BUFFfood;
            _totalMoralePresidentEnemy += MoralePresidentEnemy[i];
        }

        _canvasCamera.transform.Find("Text_TotalMorale").GetComponent<Text>().text = "You morale " + _totalMoralePresident;
        _canvasCamera.transform.Find("Text_TotalMoraleEnemy").GetComponent<Text>().text = "Enemy morale " + _totalMoralePresidentEnemy;
    } 
} 