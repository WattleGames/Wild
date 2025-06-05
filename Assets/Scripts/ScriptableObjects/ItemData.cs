using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Items/Create item")]
public class ItemData : ScriptableObject
{
    //A scriptable object used to hold item data
    //Any objects that want to know when this item is being used should subscribe to this

    public delegate void ItemEventHandler(ItemData item);

    //events for all the items in the game (Add more depending on how many items we have)
    public static ItemEventHandler onBulletItemUsed;
  

 
    [SerializeField] private Sprite _itemImage;

    [SerializeField] private string _itemName;

    public Sprite ItemImage { get {  return _itemImage; } }
    public string ItemName { get { return _itemName; } }



    public void UseItem()
    {
        //first check what the object is
        if (_itemName == "Bullet")
        {
            onBulletItemUsed?.Invoke(this);
            Debug.Log("Firing the event for " + _itemName + " !");
        }
        
        //add more checks here depending on how many items we have

      

       
    }


}
