using UnityEngine;

public class Mask : MonoBehaviour
{   
    [SerializeField] Transform MountPoints;
    [SerializeField] Transform MountObjContainer;


    [SerializeField] Transform[] MountPointsBaseHead;
    [SerializeField] Transform[] MountPointsEyeLeft;
    [SerializeField] Transform[] MountPointsEyeRight;

    [SerializeField] Transform[] MountPointsHornLeft;
    [SerializeField] Transform[] MountPointsHornRight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
//        Instantiate(myPrefab).transform;
//        MountPointsBaseHead.Append()
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Resync mounts")]
    public void ResyncMounts()
    {
        var fields = this.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        foreach (var f in fields)
        {
            if (!f.Name.StartsWith("MountPoints")) continue;
            if (f.FieldType != typeof(Transform[])) continue;

            string suffix = f.Name.Substring("MountPoints".Length);
            var container = MountPoints.Find(suffix);
            if (container == null)
            {
                Debug.LogWarning($"ResyncMounts: couldn't find child '{suffix}' under '{MountPoints.name}'. Setting '{f.Name}' to empty array.");
                f.SetValue(this, new Transform[0]);
                continue;
            }

            var list = new System.Collections.Generic.List<Transform>();
            for (int i = 0; i < container.childCount; i++)
                list.Add(container.GetChild(i));

            f.SetValue(this, list.ToArray());
            Debug.Log($"ResyncMounts: '{f.Name}' resynced with {list.Count} mounts from '{suffix}'.");
        }
    }
}
