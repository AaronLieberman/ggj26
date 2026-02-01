using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(CheatInspector))]
public class CheatInspectorEditor : Editor
{

    float speedMultiplier = 0;

    private void OnEnable()
    {
        ShopManager shopManager = FindObjectOfType<ShopManager>();

        speedMultiplier = shopManager.Speed;

    }
    public override void OnInspectorGUI()
    {
        ShopManager shopManager = FindObjectOfType<ShopManager>();

        //DrawDefaultInspector();

        if (GUILayout.Button("Set Next Customer Amanda Tea"))
        {
            shopManager.DebugCustomerToShow = "AmandaTea";
        }
        if (GUILayout.Button("Set Next Customer Doug Crown"))
        {
            shopManager.DebugCustomerToShow = "DougCrown";
        }
        if (GUILayout.Button("Set Next Customer Orange Steve"))
        {
            shopManager.DebugCustomerToShow = "OrangeSteve";
        }
        if (GUILayout.Button("Set Next Customer (Jason) Bane"))
        {
            shopManager.DebugCustomerToShow = "Bane";
        }

        GUILayout.Label("Set Next Customer Timer Speed Multiplier. Currently: " + shopManager.SpeedMultiplier);
        GUILayout.BeginHorizontal();
        speedMultiplier = EditorGUILayout.FloatField("New Speed Multiplier", speedMultiplier);
        if (GUILayout.Button("Set Speed Multiplier"))
        {
            shopManager.SpeedMultiplier = speedMultiplier;
        }
        GUILayout.EndHorizontal();
    }
}
