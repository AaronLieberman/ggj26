using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    
    GameObject _currentCustomer;

    CustomerData[] _customers = new CustomerData[]
    {
        new() {
            customerName = "Amanda Tea",
            customerDialogue = "I'm going to High Tea with my nieces. I need a mask that will make them laugh, and looks very pretty. Bonus points if its animal themed, they love cats!",
            customerImageName = "AmandaTea",
            maskScary = { Min = 0, Max = 4 },
            maskGoofy = { Min = 3, Max = 10, Points = 50 },
            maskBeauty = { Min = 3, Max = 10, Points = 40 },
        },
        new() {
            customerName = "Francis Lyon",
            customerDialogue = "I'm going to Spain for the running of the bulls! I want to be a masked hero! Make me mysterious and manly! And make those bulls think twice about chasing me!",
            customerImageName = "FrancisLyon",
            maskScary = { Min = 0, Max = 4 },
            maskGoofy = { Min = 3, Max = 10, Points = 50 },
            maskBeauty = { Min = 3, Max = 10, Points = 40 },
        }
    };

    void Update()
    {
        if ( _currentCustomer == null )
        {
            _currentCustomer = SpawnCustomer(_customers[Random.Range(0, _customers.Length)]);
        }
    }

    GameObject SpawnCustomer(CustomerData customerData)
    {
        GameObject created = Instantiate(Customer, CustomerSpawnPoint.position, CustomerSpawnPoint.rotation, CustomerSpawnPoint);
        created.GetComponent<Rigidbody2D>().AddForce(new Vector2(-Speed, 0.0f));

        created.GetComponent<Image>().sprite = Sprites.SingleOrDefault(a => a.name == customerData.customerImageName).sprite;

        var name = transform.Find("Name");
        var conversation = transform.Find("Conversation");

        transform.Find("CustomerStopLocation").GetComponent<GenericTrigger>().TriggerEnter2D += (_, collision) =>
        {
            collision.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            var nameText = name.Find("NameText");
            nameText.GetComponent<TextMeshProUGUI>().SetText(customerData.customerName);
            name.gameObject.SetActive(true);

            var conversationText = conversation.Find("ConversationText");
            conversationText.GetComponent<TextMeshProUGUI>().SetText(customerData.customerDialogue);
            conversation.gameObject.SetActive(true);
        };

        return created;
    }
}
