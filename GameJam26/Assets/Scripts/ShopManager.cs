using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject Customer;
    
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

    void Start()
    {

    }

    void Update()
    {
        if ( _currentCustomer == null )
        {
            _currentCustomer = SpawnCustomer();
        }
    }

    GameObject SpawnCustomer()
    {
        return null;
    }
}
