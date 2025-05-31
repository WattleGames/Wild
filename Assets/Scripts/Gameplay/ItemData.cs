using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Items/Create item")]
public class ItemData : ScriptableObject
{
    //A scriptable object used to hold item data
    //Any objects that want to know when this item is being used should subscribe to this

    public delegate void ItemEventHandler(ItemData item);

    public static ItemEventHandler onItemUsed;

    [SerializeField] private string _itemName;

    public string ItemName { get { return _itemName; } }



    public void UseItem()
    {
        //fires an onItem used event
       onItemUsed?.Invoke(this);
    }


}
