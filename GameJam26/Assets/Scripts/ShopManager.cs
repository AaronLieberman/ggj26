using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[System.Serializable]
public struct CustomerEntry
{
    public string name;
    public Sprite sprite;
}

public class ShopManager : MonoBehaviour
{
    public GameObject Customer;
    public Transform CustomerSpawnPoint;
    public float Speed = 1;
    public CustomerEntry[] Sprites;
    public float SpeedMultiplier = 1;

    GameObject _currentCustomer;
    CustomerData _currentCustomerData;
    float _timeRemaining;
    bool _customerWaiting;
    bool _customerLeaving;
    EventHandler<Collider2D> _triggerHandler;

    GameObject _nameObj;
    TextMeshProUGUI _nameText;
    GameObject _conversationObj;
    TextMeshProUGUI _conversationText;
    GenericTrigger _stopTrigger;
    GameObject _destroyTrigger;

    CustomerData[] _customers;

    void Awake()
    {
        _customers = CustomerDataLoader.Load();

        var name = transform.Find("Name");
        _nameObj = name.gameObject;
        _nameText = name.Find("NameText").GetComponent<TextMeshProUGUI>();

        var conversation = transform.Find("Conversation");
        _conversationObj = conversation.gameObject;
        _conversationText = conversation.Find("ConversationText").GetComponent<TextMeshProUGUI>();

        _stopTrigger = transform.Find("CustomerStopLocation").GetComponent<GenericTrigger>();
        _destroyTrigger = transform.Find("CustomerDestroy").gameObject;
    }

    void Update()
    {
        if (_currentCustomer == null && _customerLeaving)
        {
            _customerLeaving = false;
            _nameObj.SetActive(false);
            _conversationObj.SetActive(false);
        }

        if (_currentCustomer == null && !_customerLeaving)
        {
            _currentCustomer = SpawnCustomer(_customers[UnityEngine.Random.Range(0, _customers.Length)]);
        }

        if (_customerWaiting)
        {
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0)
                CustomerLeave(false);
        }
    }

    GameObject SpawnCustomer(CustomerData customerData)
    {
        _currentCustomerData = customerData;

        GameObject created = Instantiate(Customer, CustomerSpawnPoint.position, CustomerSpawnPoint.rotation, CustomerSpawnPoint);
        created.GetComponent<Rigidbody2D>().AddForce(new Vector2(-Speed, 0.0f));

        created.GetComponent<Image>().sprite = Sprites.SingleOrDefault(a => a.name == customerData.customerImageName).sprite;

        if (_triggerHandler != null)
            _stopTrigger.TriggerEnter2D -= _triggerHandler;

        _triggerHandler = (_, collision) =>
        {
            collision.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            _nameText.SetText(customerData.customerName);
            _nameObj.SetActive(true);

            _conversationText.SetText(customerData.customerDialogue);
            _conversationObj.SetActive(true);

            float duration = customerData.time / SpeedMultiplier;
            _customerWaiting = true;
            _timeRemaining = duration;
            _currentCustomer.GetComponent<Customer>().StartTimer(duration);
        };

        _stopTrigger.TriggerEnter2D += _triggerHandler;

        return created;
    }

    public void CustomerSatisfied()
    {
        if (!_customerWaiting) return;
        CustomerLeave(true);
    }

    void CustomerLeave(bool satisfied)
    {
        _customerWaiting = false;

        string dialog = satisfied ? _currentCustomerData.gradeADialog : _currentCustomerData.gradeFDialog;
        _conversationText.SetText(dialog);

        _customerLeaving = true;

        _destroyTrigger.SetActive(true);

        _currentCustomer.GetComponent<Rigidbody2D>().AddForce(new Vector2(Speed, 0));
    }
}
