using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public GameObject Customer;
    public Transform CustomerSpawnPoint;
    public float Speed = 400;
    public Sprite[] Sprites;
    public float SpeedMultiplier = 1;
    public WaypointPath EnterPath;
    public WaypointPath ExitPath;
    public string DebugCustomerToShow;

    public Customer CurrentCustomer { get { return _currentCustomer?.GetComponent<Customer>(); } }

    GameObject _currentCustomer;
    float _timeRemaining;
    bool _customerWaiting;
    bool _customerLeaving;
    string _lastDebugCustomerToShow;

    GameObject _nameObj;
    TextMeshProUGUI _nameText;
    GameObject _conversationObj;
    TextMeshProUGUI _conversationText;

    CustomerData[] _customers;

    bool isActive = false;

    void Awake()
    {
        _customers = CustomerDataLoader.Load();

        var name = transform.Find("Name");
        _nameObj = name.gameObject;
        _nameText = name.Find("NameText").GetComponent<TextMeshProUGUI>();

        var conversation = transform.Find("Conversation");
        _conversationObj = conversation.gameObject;
        _conversationText = conversation.Find("ConversationText").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (isActive && _currentCustomer == null && !_customerLeaving)
        {
            var customerToSpawn = !string.IsNullOrWhiteSpace(DebugCustomerToShow)
                ? _customers.FirstOrDefault(c => c.customerImageName == DebugCustomerToShow)
                : null;
            customerToSpawn ??= _customers[Random.Range(0, _customers.Length)];
            _currentCustomer = SpawnCustomer(customerToSpawn);
        }

        if (_customerWaiting)
        {
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0)
                CustomerLeave(false);
        }

        if (DebugCustomerToShow != _lastDebugCustomerToShow)
        {
            CustomerLeave(false);
        }

        _lastDebugCustomerToShow = DebugCustomerToShow;
    }

    GameObject SpawnCustomer(CustomerData customerData)
    {
        GameObject created = Instantiate(Customer, transform);
        var rect = created.GetComponent<RectTransform>();
        var spawnRect = ((RectTransform)CustomerSpawnPoint).anchoredPosition;
        rect.anchoredPosition = spawnRect;

        created.GetComponent<Image>().sprite = Sprites.SingleOrDefault(a => a.name == customerData.customerImageName);
        created.GetComponent<Customer>().Data = customerData;

        var follower = created.GetComponent<PathFollower>();
        follower.Speed = Speed;
        follower.Follow(EnterPath, () => CustomerArrived(customerData));

        return created;
    }

    void CustomerArrived(CustomerData customerData)
    {
        _nameText.SetText(customerData.customerName);
        _nameObj.SetActive(true);

        _conversationText.SetText(customerData.customerDialogue);
        _conversationObj.SetActive(true);

        float duration = DebugCustomerToShow == customerData.customerName
            ? 10000
            : customerData.time / SpeedMultiplier;
        _customerWaiting = true;
        _timeRemaining = duration;
        _currentCustomer.GetComponent<Customer>().StartTimer(duration);
    }

    public void CustomerSatisfied()
    {
        if (!_customerWaiting) return;
        CustomerLeave(true);
    }

    void CustomerLeave(bool satisfied)
    {
        _customerWaiting = false;

        var data = _currentCustomer.GetComponent<Customer>().Data;
        string dialog = satisfied ? data.gradeADialog : data.gradeFDialog;
        _conversationText.SetText(dialog);

        _customerLeaving = true;

        var follower = _currentCustomer.GetComponent<PathFollower>();
        follower.Speed = Speed;
        follower.Follow(ExitPath, () =>
        {
            Destroy(_currentCustomer);
            _currentCustomer = null;
            _customerLeaving = false;
            _nameObj.SetActive(false);
            _conversationObj.SetActive(false);
        });
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
