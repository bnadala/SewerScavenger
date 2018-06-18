namespace SewerScavenger.Models
{
    // Items, Monsters, and Characters all share these items
    public class Entity<T> : BaseEntity<T>
    {

        // The name of the item/character/monster to show to the user.  Example: Mike the Cleric, or Bunny Ears, or Super Sized Slime
        public string Name { get; set; }

        // Description of the Item to show to the user, Example: Lets you Hop into the action
        public string Description { get; set; }

        // Location to the image for the item.  Will come from the server as a fully qualified URI example:  https://developer.android.com/images/robot-tiny.png
        public string ImageURI { get; set; }

        public string Image { get; set; }
    }
}


