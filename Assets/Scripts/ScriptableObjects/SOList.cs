using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOList", menuName = "ScriptableObjects/List") ]
public class SOList : ScriptableObject
{
	// Keep in mind that in order for a list of class objects to be visible in the inspector, that class must have 
	// [System.Serializable] on top of it. Basically the class needs to be serializable.
	public List<TestItem> Items = new List<TestItem>();
}
