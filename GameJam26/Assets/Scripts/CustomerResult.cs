using UnityEngine;

public struct CustomerResult
{
    public CustomerData CustomerData;
    public GameObject CustomerMask;
    public int CustomerScore;
    public bool CustomerSatisfied;

    public CustomerResult(CustomerData customerData, GameObject customerMask, int customerScore, bool customerSatisfied)
    {
        CustomerData = customerData;
        CustomerMask = customerMask;
        CustomerScore = customerScore;
        CustomerSatisfied = customerSatisfied;
    }
}
