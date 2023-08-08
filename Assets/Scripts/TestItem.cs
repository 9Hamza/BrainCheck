using System;

[System.Serializable]
public class TestItem : IEquatable<TestItem>
{
    public string ItemName;
    public string ItemID;
    public int ItemStatus;

    public TestItem(string itemName, string itemID, int itemStatus)
    {
        this.ItemName = itemName;
        this.ItemID = itemID;
        this.ItemStatus = itemStatus;
    }

    public bool Equals(TestItem other)
    {
        return other != null &&
               other.ItemName == ItemName &&
               other.ItemID == ItemID &&
               other.ItemStatus == ItemStatus;
    }
}