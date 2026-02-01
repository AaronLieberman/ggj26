using UnityEngine;

public struct CustomerResult
{
    public CustomerData CustomerData;
    public GameObject CustomerMask;
    public bool Satisfied;

    public CustomerResult(CustomerData customerData, GameObject customerMask, bool satisfied)
    {
        CustomerData = customerData;
        CustomerMask = customerMask;
        Satisfied = satisfied;
    }
}
