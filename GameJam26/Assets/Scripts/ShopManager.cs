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
    public float CustomerTimerDuration = 20f;

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

    CustomerData[] _customers = new CustomerData[]
    {
        new() {
            customerName = "Amanda Tea",
            customerDialogue = "I'm going to High Tea with my nieces. I need a mask that will make them laugh, and looks very pretty. Bonus points if its animal themed, they love cats!",
            customerImageName = "AmandaTea",
            maskScary = { Min = 0, Max = 4 },
            maskGoofy = { Min = 3, Max = 10, Points = 50 },
            maskBeauty = { Min = 3, Max = 10, Points = 40 },
            gradeADialog = "Oh my goodness, this is PERFECT! My nieces are going to love it!",
            gradeFDialog = "I don't have all day... I'll just go without a mask.",
        },
        new() {
            customerName = "Francis Lyon",
            customerDialogue = "I'm going to Spain for the running of the bulls! I want to be a masked hero! Make me mysterious and manly! And make those bulls think twice about chasing me!",
            customerImageName = "FrancisLyon",
            maskScary = { Min = 0, Max = 4 },
            maskGoofy = { Min = 3, Max = 10, Points = 50 },
            maskBeauty = { Min = 3, Max = 10, Points = 40 },
            gradeADialog = "Now THAT is a mask worthy of a bull fighter! Ole!",
            gradeFDialog = "Forget it, I'll just face the bulls bare-faced.",
        }
    };

    void Awake()
    {
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

            _customerWaiting = true;
            _timeRemaining = CustomerTimerDuration;
            _currentCustomer.GetComponent<Customer>().StartTimer(CustomerTimerDuration);
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
