using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject Customer;
    public Transform CustomerSpawnPoint;
    public float Speed = 1;
    
    GameObject _currentCustomer;

    CustomerData[] _customers = new CustomerData[]
    {
        new() {
            customerName = "Amanda Tea",
            customerDialogue = "I'm going to High Tea with my nieces. I need a mask that will make them laugh, and looks very pretty. Bonus points if its animal themed, they love cats!",
            customerImageName = "amandatea",
            maskScary = { Min = 0, Max = 4 },
            maskGoofy = { Min = 3, Max = 10, Points = 50 },
            maskBeauty = { Min = 3, Max = 10, Points = 40 },
        }
    };

    void Update()
    {
        if ( _currentCustomer == null )
        {
            _currentCustomer = SpawnCustomer(_customers[0]);
        }
    }

    GameObject SpawnCustomer(CustomerData customerData)
    {
        GameObject created = Instantiate(Customer, CustomerSpawnPoint.position, CustomerSpawnPoint.rotation, CustomerSpawnPoint);
        created.GetComponent<Rigidbody2D>().AddForce(new Vector2(-Speed, 0.0f));

        var name = transform.Find("Name");
        var nameText = name.Find("NameText");
        nameText.GetComponent<TextMeshProUGUI>().SetText(customerData.customerName);

        transform.Find("CustomerStopLocation").GetComponent<GenericTrigger>().TriggerEnter2D += (_, collision) =>
        {
            collision.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            name.gameObject.SetActive(true);
        };

        return created;
    }
}
