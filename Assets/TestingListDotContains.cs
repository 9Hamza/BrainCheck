using UnityEngine;

public class TestingListDotContains : MonoBehaviour
{

    public SOList soList;
    
    // Start is called before the first frame update
    void Start()
    {
        // clear list
        soList.Items.Clear();
        
        // add item 1
        TestItem item1 = new TestItem("cup", "S6N9", 4);
        soList.Items.Add(item1);
        
        // check if you can add item 2 which has the same values of item 1
        TestItem item2 = new TestItem("cup", "S6N9", 4);
        // bool itemExists = soList.Items.Contains(item2);
        bool itemExists = false;
        foreach (var item in soList.Items)
        {
            if (item.Equals(item2))
            {
                itemExists = true;
            }
            else
            {
                itemExists = false;
            }
        }
        Debug.Log($"item exists: {itemExists}");
        if (itemExists == false)
        {
            soList.Items.Add(item2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
