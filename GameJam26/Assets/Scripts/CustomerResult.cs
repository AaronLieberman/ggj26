using UnityEngine;

public struct CustomerResult
{
    public CustomerData CustomerData;
    public GameObject CustomerMask;
    public int CustomerScore;

    public CustomerResult(CustomerData customerData, GameObject customerMask, int customerScore)
    {
        CustomerData = customerData;
        CustomerMask = customerMask;
        CustomerScore = customerScore;
    }
}
