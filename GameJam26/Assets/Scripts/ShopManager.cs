using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public GameObject NamePlate;
    public GameObject ConversationPlate;
    public GameObject CustomerBox; // for display order

    public GameObject Customer;
    public Transform CustomerSpawnPoint;
    public float Speed = 400;
    public Sprite[] Sprites;
    public float SpeedMultiplier = 1;
    public WaypointPath EnterPath;
    public WaypointPath ExitPath;
    public string DebugCustomerToShow;

    public Customer CurrentCustomer { get { return _currentCustomer; } }
    public bool CustomerExists { get { return _currentCustomer != null; } }
    public bool MaskIsDeliverable { get { return _customerWaiting && !CurrentCustomer.transform.Find("Mask(Clone)"); } }

    Customer _currentCustomer;
    float _timeRemaining;
    bool _customerWaiting;
    bool _customerLeaving;
    string _lastDebugCustomerToShow;
    MaskDisplayManager _maskDisplayManager;
    ScoreCalculator _scoreCalculator;
    int _nextCustomerToSpawn = 0;

    TextMeshProUGUI _nameText;
    TextMeshProUGUI _conversationText;

    CustomerData[] _customers;

    bool isActive = false;

    void Awake()
    {
        _customers = CustomerDataLoader.Load();

        for (int i = _customers.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = _customers[i];
            _customers[i] = _customers[j];
            _customers[j] = temp;
        }
        _customers = _customers.OrderBy(c => c.difficultyTier).ToArray();

        _maskDisplayManager = Utilities.GetRootComponentRecursive<MaskDisplayManager>();
        _scoreCalculator = Utilities.GetRootComponentRecursive<ScoreCalculator>();
    }

    void Start()
    {
        _nameText = NamePlate.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        _conversationText = ConversationPlate.transform.Find("ConversationText").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (_customerWaiting)
        {
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0)
            {
                if (GameObject.Find("GameManager").GetComponent<GameManager>().DeliverMask())
                {
                    // HACKHACK
                    _timeRemaining = 1000;
                }
                else
                {
                    CustomerLeave(false);
                }
            }
        }

        if (_lastDebugCustomerToShow != null && DebugCustomerToShow != _lastDebugCustomerToShow)
        {
            CustomerLeave(false);
        }

        _lastDebugCustomerToShow = DebugCustomerToShow;
    }

    public bool AttemptCustomerSpawn()
    {
        if (isActive && _currentCustomer == null && !_customerLeaving)
        {
            var customerToSpawn = !string.IsNullOrWhiteSpace(DebugCustomerToShow)
                ? _customers.FirstOrDefault(c => c.customerImageName == DebugCustomerToShow)
                : null;
            customerToSpawn ??= _customers[_nextCustomerToSpawn % _customers.Length];
            _nextCustomerToSpawn++;
            _currentCustomer = SpawnCustomer(customerToSpawn);

            return true;
        }

        return false;
    }

    Customer SpawnCustomer(CustomerData customerData)
    {
        GameObject created = Instantiate(Customer, CustomerBox.transform);
        var rect = created.GetComponent<RectTransform>();
        var spawnRect = ((RectTransform)CustomerSpawnPoint).anchoredPosition;
        rect.anchoredPosition = spawnRect;

        created.GetComponent<Image>().sprite = GetCustomerSprite(customerData.customerImageName);
        created.GetComponent<Customer>().Data = customerData;

        var follower = created.GetComponent<PathFollower>();
        follower.Speed = Speed;
        follower.Follow(EnterPath, () => CustomerArrived(customerData));

        return created.GetComponent<Customer>();
    }

    void CustomerArrived(CustomerData customerData)
    {
        _nameText.SetText(customerData.customerName);
        NamePlate.SetActive(true);

        _conversationText.SetText(customerData.customerDialogue);
        ConversationPlate.SetActive(true);

        float duration = DebugCustomerToShow == customerData.customerName
            ? 10000
            : customerData.time / SpeedMultiplier;
        _customerWaiting = true;
        _timeRemaining = duration;
        _currentCustomer.StartTimer(duration);
    }

    public void CustomerSatisfied()
    {
        if (!_customerWaiting) return;
        CustomerLeave(true);
    }

    void CustomerLeave(bool satisfied)
    {
        Mask customerMask = _currentCustomer.GetComponentInChildren<Mask>();
        _currentCustomer.CustomerResult = new CustomerResult(
            _currentCustomer.Data,
            customerMask ? customerMask.gameObject : null,
            customerMask ? _scoreCalculator.GetActiveMaskScore() : 0,
            satisfied
        );

        _maskDisplayManager.ClearMaskDisplay();

        _customerWaiting = false;

        _currentCustomer.StopTimer();
        var data = _currentCustomer.Data;
        string dialog = satisfied ? data.gradeADialog : data.gradeFDialog;
        _conversationText.SetText(dialog);

        _customerLeaving = true;

        var follower = _currentCustomer.GetComponent<PathFollower>();
        follower.Speed = Speed;
        follower.Follow(ExitPath, () =>
        {
            //Destroy(_currentCustomer);
            _currentCustomer = null;
            _customerLeaving = false;
            NamePlate.SetActive(false);
            ConversationPlate.SetActive(false);
        });
    }

    public Sprite GetCustomerSprite(string customerSpriteName)
    {
        return Sprites.SingleOrDefault(a => a.name == customerSpriteName);
    }

    public void ActivateManager()
    {
        isActive = true;
    }

    public void DeactivateManager()
    {
        isActive = false;
    }
}
