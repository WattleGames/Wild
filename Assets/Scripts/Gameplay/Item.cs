using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    //A script defining an item. Add to all items in the game
    //Requires items to have a trigger

    public delegate void ItemEventHandler(Item item);
    public static event ItemEventHandler onItemPickedUp;

   [SerializeField] private string _itemName;

    public string ItemName  { get { return _itemName; } }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the player picks up the item
        if(collision.gameObject.GetComponent<PlayerInventory>())
        {
            //invoke the event then delete this object
            onItemPickedUp?.Invoke(this);
            Destroy(this.gameObject);
        }
    }



}
