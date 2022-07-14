using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FightRules : MonoBehaviour
{
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
    public Transform[] _FightCardOnTable; // ������ ������ ���� �� ����� 
    private Transform[] _FightCardPlace;
    public GameObject Folder_FightCardPlace;
    public GameObject Folder_FightCardInScene;
    private int counter_card = 0; // ������� ����� ����� �� ������� ���� (j � �����) 
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

    private bool _helper = true;
    private bool _helper2 = false;
    private bool _helper3 = true;
    private bool _helper4 = true;
    private Vector3 _offset;
    private float mzCoord;

    private Transform _dragFactor=null;
    public Canvas _canvasForDragFactorText;

    private int _materials_ability_protect = 0;
    private int _economic_ability_protect = 0;
    private int _economic_ability_attack = 0;
    private int _health_ability_protect = 0;
    private int _food_ability_protect = 0; 

    //Transform _rootGO;
    //private Transform _son; // ��� 

    void Start()
    {
        
    }

    public void RandomCardToPlace() // ������� � ������� ��� ����� �����, ����� ��� ���� �� ������, �������� ���������, �������� �� ����. �������������� ��� ������� ������ "Ready" 
    {
        _FightCardInScene = new Transform[Folder_FightCardInScene.transform.childCount];
        for (int i = 0; i < _FightCardInScene.Length; i++) // ����������� �� ���� ������ ������ � ����� � ����� FightCardInScene (16 ���� ������) 
        {
            _FightCardInScene[i] = Folder_FightCardInScene.transform.GetChild(i).transform; // ������� �� � ������ _FightCardInScene[]
        }

        _FightCardPlace = new Transform[Folder_FightCardPlace.transform.childCount];
        _FightCardOnTable = new Transform[_FightCardPlace.Length];

        int[] _random = new int[_FightCardPlace.Length]; // ��������� ������������� ������ � 0 �� _FightCardPlace.Length (�� 0 �� 9 ������������)
        for (int i = 0; i < _random.Length; i++) { _random[i] =i; }

        int[] Mix(int[] num) // ������������ ��� 
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

        int[] MixArray = Mix(_random); // ������������ ������ ��� ������ ��������� ���� �� _FightCardInScene

        myItemListCard = jSONControllerCard.myListFightCard; // ���������� ���� �� ������� jSON 
        for (int i = 0; i < _FightCardPlace.Length; i++) // ����������� �� ���� Place ������� ���� 
        {
            _FightCardPlace[i] = Folder_FightCardPlace.transform.GetChild(i).transform; // ������� �� � ������ _FightCardPlace[] 
            _FightCardOnTable[i] = _FightCardInScene[MixArray[i]]; // ��������� ������ ���������� �������
            _FightCardOnTable[i].transform.SetParent(_FightCardPlace[i]); // ��������� ������ ����� � Place[] 
            _FightCardOnTable[i].transform.localPosition = Vector3.zero; // �������� �������
            _FightCardOnTable[i].transform.localRotation = Quaternion.identity; // �������� �������� 
            
            for (int j = 0; j < myItemListCard.fight_card.Length; j++) // ����������� �� ���� ������ �� JSON-����� 
            {
                if (_FightCardOnTable[i].name == myItemListCard.fight_card[j].id) // ����� ���������� � ������ �� JSON
                {
                    _FightCardOnTable[i].transform.GetComponentInChildren<Canvas>().transform.GetComponentInChildren<Text>().text = "" + myItemListCard.fight_card[j].cost; // ��������� ���� ��� ������ ����� �� ������ 
                }
            }
        }
    }

    public void StartButton()
    {
        for (int i = 0; i < _Presidents_before.Length; i++) // ��������� UI ����������� 
        {
            _Presidents_before[i].transform.SetParent(_Place_after[i].transform);
            _Presidents_before[i].transform.localPosition = Vector3.zero;
            _Presidents_before[i].transform.localRotation = Quaternion.identity;
            _Presidents_before[i].transform.localScale = Vector3.one;
            _Presidents_before[i].transform.GetComponent<Image>().enabled = false;
            _Presidents_before[i].transform.Find("Text_Factor").GetComponent<Text>().enabled = false;
            _Presidents_before[i].transform.Find("Text_Abilities").GetComponent<Text>().enabled = false;
            _Presidents_before[i].transform.Find("Card").GetComponent<CanvasGroup>().blocksRaycasts = false; //��������� �������. ��������� ����������� �������, ����� �� ������ ������� �� UI �����������, ����� ����� �� ������ 
            _ourBUFFmaterial += _FactorItemPresident[i]._BUFFmaterials;
            _ourBUFFhealth += _FactorItemPresident[i]._BUFFhealth;
            _ourBUFFfood += _FactorItemPresident[i]._BUFFfood;
            _ourBUFFeconomic += _FactorItemPresident[i]._BUFFeconomic;
            _materials_ability_protect += _FactorItemPresident[i]._materials_ability_protect; 
            _economic_ability_protect += _FactorItemPresident[i]._economic_ability_protect; 
            _economic_ability_attack += _FactorItemPresident[i]._economic_ability_attack; 
            _health_ability_protect += _FactorItemPresident[i]._health_ability_protect; 
            _food_ability_protect += _FactorItemPresident[i]._food_ability_protect; 
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
            _PresidentsEnemy_before[i].transform.Find("Card").GetComponent<CanvasGroup>().blocksRaycasts = false; //��������� �������. ��������� ����������� �������, ����� �� ������ ������� �� UI �����������, ����� ����� �� ������ 
            _enemyBUFFmaterial += _FactorItemPresidentEnemy[i]._BUFFmaterials;
            _enemyBUFFhealth += _FactorItemPresidentEnemy[i]._BUFFhealth;
            _enemyBUFFfood += _FactorItemPresidentEnemy[i]._BUFFfood;
            _enemyBUFFeconomic += _FactorItemPresidentEnemy[i]._BUFFeconomic;
        }

        ReadyFight();
        ReadyFight2(); 
        calcLocationFactorsOur();
        calcLocationFactorsEnemy(); 
    }

    void calcLocationFactorsOur() // ����������� ���� �������, ���� ������� ��� ����������� �� ������ ����������
    {
        _FactorMaterials.transform.SetParent(_PlaceFactor[0]);
        _FactorMaterials.transform.localPosition = Vector3.zero;
        _FactorMaterials.transform.localRotation = Quaternion.identity;
        _FactorMaterials.transform.localScale = new Vector3(1f, 1f, 1f);
        _PlaceFactor[0].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFmaterial; // �� �����

        _FactorFood.transform.SetParent(_PlaceFactor[1]);
        _FactorFood.transform.localPosition = Vector3.zero;
        _FactorFood.transform.localRotation = Quaternion.identity;
        _FactorFood.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactor[1].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFfood; // �� ��������������

        _FactorEconomic.transform.SetParent(_PlaceFactor[2]);
        _FactorEconomic.transform.localPosition = Vector3.zero;
        _FactorEconomic.transform.localRotation = Quaternion.identity;
        _FactorEconomic.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactor[2].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFeconomic; // �� ���������

        _FactorHealth.transform.SetParent(_PlaceFactor[3]);
        _FactorHealth.transform.localPosition = Vector3.zero;
        _FactorHealth.transform.localRotation = Quaternion.identity;
        _FactorHealth.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactor[3].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _ourBUFFhealth; // �� ��������������� 
    }

    void calcLocationFactorsEnemy() // ����������� ������� ����� 
    {
        _FactorMaterialsEnemy.transform.SetParent(_PlaceFactorEnemy[0]);
        _FactorMaterialsEnemy.transform.localPosition = Vector3.zero;
        _FactorMaterialsEnemy.transform.localRotation = Quaternion.identity;
        _FactorMaterialsEnemy.transform.localScale = new Vector3(1f, 1f, 1f);
        _PlaceFactorEnemy[0].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFmaterial; // �� �����

        _FactorFoodEnemy.transform.SetParent(_PlaceFactorEnemy[1]);
        _FactorFoodEnemy.transform.localPosition = Vector3.zero;
        _FactorFoodEnemy.transform.localRotation = Quaternion.identity;
        _FactorFoodEnemy.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactorEnemy[1].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFfood; // �� ��������������

        _FactorEconomicEnemy.transform.SetParent(_PlaceFactorEnemy[2]);
        _FactorEconomicEnemy.transform.localPosition = Vector3.zero;
        _FactorEconomicEnemy.transform.localRotation = Quaternion.identity;
        _FactorEconomicEnemy.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactorEnemy[2].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFeconomic; // �� ���������

        _FactorHealthEnemy.transform.SetParent(_PlaceFactorEnemy[3]);
        _FactorHealthEnemy.transform.localPosition = Vector3.zero;
        _FactorHealthEnemy.transform.localRotation = Quaternion.identity;
        _FactorHealthEnemy.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _PlaceFactorEnemy[3].transform.Find("Canvas").transform.GetComponentInChildren<Text>().text = "" + _enemyBUFFhealth; // �� ��������������� 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _helper) // ���� �������� ������ �� ������ ����� 
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity))
            {
                for (int k = 0; k < _FightCardOnTable.Length; k++) // ����������, ������ �� �� ����� (���� � ��� ��� ������ �����������, ��� ��� ������ �� ���) � �� ����� 
                {
                    if (_hit.transform.name == _FightCardOnTable[k].name) // �������� ��� �����, �� ������� ������ 
                    {
                        counter_card = k;

                        //AnimationCard(_FightCardOnTable[counter_card]);
                        _helper = false; // ������ � 0, ����� �� ������������ � Update �������, ���� ���� ������
                        _helper2 = true;
                        FightCard(k); // �������� ��������� 
                        mzCoord = _camera.WorldToScreenPoint(_FightCardOnTable[counter_card].position).z;
                        _offset = _FightCardOnTable[counter_card].position - GetMouseWorldPos();
                    }
                }
            } 
        }

        if (Input.GetMouseButton(0) && _helper2) 
        {
            AnimationCard(_FightCardOnTable[counter_card]);
        }
        
        if (Input.GetMouseButtonUp(0) == true)
        {
            _helper = true;
            _helper3 = true;
            _helper2 = false;
            _helper4 = true;

            ResetAnimationCard(_FightCardOnTable[counter_card]);
            if (_dragFactor != null)
            { 
                CalcOurFight();

            }
        }

    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mzCoord;
        return _camera.ScreenToWorldPoint(mousePoint);
    }
    void FightCard(int k) // ���������� ��������� ��������� ������ ����� ��� ������� 
    {
        myItemListCard = jSONControllerCard.myListFightCard; // ���������� ���� �� ������� 
        for (int j = 0; j < myItemListCard.fight_card.Length; j++) // ����������� �� ���� ������ �� JSON-����� 
        {
            if (_FightCardOnTable[k].name == myItemListCard.fight_card[j].id) // ����� ���������� � ������ �� JSON
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
                Debug.Log("����� " + _FightCardOnTable[k].name);
            }
        }

    }

    void AnimationCard(Transform _y) // �������� ������ �����, ���� �������� (��������� ��������)
    {
        if (_helper3) // ��������� ���� ���
        {
            _helper3 = false;
            _y.transform.localScale = new Vector3(_y.transform.localScale.x * 1.25f, _y.transform.localScale.y * 1.25f, _y.transform.localScale.z * 1.25f); // ����������� ����� � 1,25 ����     
        }

        _y.transform.position = GetMouseWorldPos() + _offset;

        if (_helper4 == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity, 3)) // 3 - ����� MaskRaycast, ����� �� ������ ����� 
            {
                for (int k = 0; k < _PlaceFactor.Length; k++) // ����������, ������ �� �� ������� � �� ������ 
                {
                    if (_hit.transform.name == _PlaceFactor[k].name)
                    {
                        _dragFactor = _hit.transform; // ��������� ������ ������� 
                        _canvasForDragFactorText.transform.GetComponentInChildren<Text>().text = "" + _dragFactor.name; // �������� 
                    }
                }
            }
            else
            {
                _dragFactor = null;
                _canvasForDragFactorText.transform.GetComponentInChildren<Text>().text = "";
            }
        }
    }

    void ResetAnimationCard(Transform y)
    {
        y.transform.localScale = Vector3.one; // ���������� �������
        y.transform.localPosition = Vector3.zero; // �������� ������� 
        _canvasForDragFactorText.transform.GetComponentInChildren<Text>().text = ""; 
    }

    void CalcOurFight() // ������ ����� � ��� 
    {
        if (_dragFactor != null)
        {

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
                if ((_materialsCard == 1 && _dragFactor.name == "Materials") ||
                    (_economicCard == 1 && _dragFactor.name == "Economic") ||
                    (_healthCard == 1 && _dragFactor.name == "Health") ||
                    (_foodCard == 1 && _dragFactor.name == "Food"))
                {
                    _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _deltamorale_positive; 
                    _totalMoralePresident = _totalMoralePresident - _costCard;

                    //Debug.Log("_totalMoralePresidentEnemy " + _totalMoralePresidentEnemy);
                    //Debug.Log("_deltamorale_positive " + _deltamorale_positive);
                }
                else _totalMoralePresident = _totalMoralePresident - _costCard;

            }

            if (_protectCard == 1)
            {
                if ((_materialsCard == 1 && _dragFactor.name == "Materials") ||
                    (_economicCard == 1 && _dragFactor.name == "Economic") ||
                    (_healthCard == 1 && _dragFactor.name == "Health") ||
                    (_foodCard == 1 && _dragFactor.name == "Food"))
                {
                    _totalMoralePresident = _totalMoralePresident - _deltamorale_negative - _costCard;
                }
            }
            else _totalMoralePresident = _totalMoralePresident - _costCard;

            if (_diplomationCard == 1)
            {
                if ((_materialsCard == 1 && _dragFactor.name == "Materials") ||
                    (_economicCard == 1 && _dragFactor.name == "Economic") ||
                    (_healthCard == 1 && _dragFactor.name == "Health") ||
                    (_foodCard == 1 && _dragFactor.name == "Food"))
                {
                    _totalMoralePresident = _totalMoralePresident - _costCard;
                    //Debug.Log("_diplomationCard"); 
                }
            }
            else _totalMoralePresident = _totalMoralePresident - _costCard;

            if (_fortuneCard == 1)
            {
                if ((_materialsCard == 1 && _dragFactor.name == "Materials") ||
                    (_economicCard == 1 && _dragFactor.name == "Economic") ||
                    (_healthCard == 1 && _dragFactor.name == "Health") ||
                    (_foodCard == 1 && _dragFactor.name == "Food"))
                {
                    int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                    _totalMoralePresident = _totalMoralePresident - _randomDeltaMorale[Random.Range(0, 1)] - _costCard;
                }
            }
            else _totalMoralePresident = _totalMoralePresident - _costCard;
        }

        ReadyFight2();
        StartCoroutine(PauseOurCoroutine()); // ����� 
    }

    IEnumerator PauseOurCoroutine()
    {
        Cursor.lockState = CursorLockMode.Locked; // ��������� ������ 
        yield return new WaitForSeconds(2);
        CalcEnemyFight();
    }

    void CalcEnemyFight() // ������ ����� � ��� 
    {
        int _enemy = Random.Range(0, _FightCardOnTable.Length - 1); // �������� ����� (�� ������) ������ 
        FightCard(_enemy); // �������� � ������ 
        _canvasForDragFactorText.transform.GetComponentInChildren<Text>().text = "��������� �������� ����� " + _FightCardOnTable[_enemy].name; // �������� 
        StartCoroutine(PauseEnemyCoroutine()); // ����� 
    }
    IEnumerator PauseEnemyCoroutine()
    {
        yield return new WaitForSeconds(2);
        CalcEnemyFight2();
    }

    void CalcEnemyFight2()
    {
            if (_attackCard == 1)
            {
                if (_materialsCard == 1 ||_economicCard == 1 || _healthCard == 1 || _foodCard == 1 )
                {
                    _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                    _totalMoralePresident = _totalMoralePresident - _deltamorale_positive;
                }
            }

            if (_protectCard == 1)
            {
                if (_materialsCard == 1 ||_economicCard == 1 || _healthCard == 1 || _foodCard == 1 )
                {
                    _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _deltamorale_negative - _costCard;
                }
            }

            if (_diplomationCard == 1)
            {
                if (_materialsCard == 1 ||_economicCard == 1 || _healthCard == 1 || _foodCard == 1 )
                {
                    _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _costCard;
                }
            }

            if (_fortuneCard == 1)
            {
                if (_materialsCard == 1 ||_economicCard == 1 || _healthCard == 1 || _foodCard == 1 )
                {
                    int[] _randomDeltaMorale = { _deltamorale_positive, _deltamorale_negative };
                    _totalMoralePresidentEnemy = _totalMoralePresidentEnemy - _randomDeltaMorale[Random.Range(0, 1)] - _costCard;
                }
            }

        ReadyFight2();
        Cursor.lockState = CursorLockMode.None; // �������� ������ 
        _canvasForDragFactorText.transform.GetComponentInChildren<Text>().text = "";
    } 


    public void ReadyFight() // �������������� ��� ������� ������ "Ready" 
    {
        calcLocationFactorsOur();
        calcLocationFactorsEnemy(); 

        for (int i = 0; i < 3; i++) // ������� ������ 
        {
            MoralePresident[i] = _FactorItemPresident[i]._BUFFmaterials + _FactorItemPresident[i]._BUFFeconomic + _FactorItemPresident[i]._BUFFhealth + _FactorItemPresident[i]._BUFFfood;
            _totalMoralePresident += MoralePresident[i];
            MoralePresidentEnemy[i] = _FactorItemPresidentEnemy[i]._BUFFmaterials + _FactorItemPresidentEnemy[i]._BUFFeconomic + _FactorItemPresidentEnemy[i]._BUFFhealth + _FactorItemPresidentEnemy[i]._BUFFfood;
            _totalMoralePresidentEnemy += MoralePresidentEnemy[i];
        }
    }

    public void ReadyFight2() // �������������� ��� ������� ������ "Ready" 
    {
        _canvasCamera.transform.Find("Text_TotalMorale").GetComponent<Text>().text = "You morale " + _totalMoralePresident;
        _canvasCamera.transform.Find("Text_TotalMoraleEnemy").GetComponent<Text>().text = "Enemy morale " + _totalMoralePresidentEnemy;
        Debug.Log("TotalMoraleEnemy " + _totalMoralePresidentEnemy);
        Debug.Log("TotalMorale " + _totalMoralePresident);
        Debug.Log("   ");

        if (_totalMoralePresidentEnemy <= 0)
        {
            DataHolder._winnerHolder = true;
            DataHolder._moralePresidentHolder = _totalMoralePresident;
            SceneManager.LoadScene(3);
        }
        else
    if (_totalMoralePresident <= 0)
        {
            DataHolder._winnerHolder = false;
            SceneManager.LoadScene(3);
        }
    } 
} 