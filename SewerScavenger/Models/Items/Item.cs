using SewerScavenger.Models;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SewerScavenger.Models
{
    // The Items that a character can use, a Monster may drop, or may be randomly available.
    // The items are stored in the DB, and during game time a random item is selected.
    // The system supports CRUDi operatoins on the items
    // When in test mode, a test set of items is loaded
    // When in run mode the items from from the database
    // When in online mode, the items come from an api call to a webservice

    // When characters or monsters die, they drop items into the Items Pool for the Battle

    public class Item : Entity<Item>
    {
        // Range of the item, swords are 1, hats/rings are 0, bows are >1
        public int Range { get; set; }

        // Enum of the different attributes that the item modifies, Items can only modify one item
        public AttributeEnum Attribute { get; set; }

        // Where the Item goes on the character.  Head, Foot etc.
        public ItemLocationEnum Location { get; set; }

        // The Value item modifies.  So a ring of Health +3, has a Value of 3
        public int Value { get; set; }

        public int Damage { get; set; }

        public string FormatedString { get; set; }

        // Inheritated properties
        // Id comes from BaseEntity class
        // Name comes from the Entity class... 
        // Description comes from the Entity class
        // ImageURI comes from the Entity class

        public Item()
        {
            Name = "Unknown";
            Description = "Unknown";
            Guid = null;
            ImageURI = null;

            Range = 0;
            Value = 0;
            Damage = 0;

            Location = ItemLocationEnum.Unknown;
            Attribute = AttributeEnum.Unknown;

            ImageURI = null;
            FormatedString = FormatOutput2();
        }

        public Item(Item i)
        {
            Name = i.Name;
            Description = i.Description;
            Guid = null;
            ImageURI = i.ImageURI;

            Range = i.Range;
            Value = i.Value;
            Damage = i.Damage;

            Location = i.Location;
            Attribute = i.Attribute;
            
            FormatedString = FormatOutput2();
        }

        // Helper to combine the attributes into a single line, to make it easier to display the item as a string
        public string FormatOutput()
        {
            var myReturn = Name + " , " +
                            Description + " for " +
                            Location.ToString() + " with " +
                            Attribute.ToString() +
                            "+" + Value + " , " +
                            "Range:" + Range + " , " +
                            "Damage:" + Damage;


            return myReturn.Trim();
        }

        public string FormatOutput2()
        {
            var myReturn = Name + " , " +
                            Attribute.ToString() +
                            "+" + Value + " , " +
                            "Range:" + Range + " , " +
                            "Damage:" + Damage;
            return myReturn.Trim();
        }

        public Item(string name, string description, string imageuri, string guid, int range, int value, int damage, ItemLocationEnum location, AttributeEnum attribute)
        {
            Name = name;
            Description = description;
            Guid = guid;
            ImageURI = imageuri;

            Range = range;
            Value = value;
            Damage = damage;

            Location = location;
            Attribute = attribute;
            FormatedString = FormatOutput2();
        }

        public string ConvertToString()
        {
            var dict = new Dictionary<string, string>
            {
                {"Name", Name.ToString()},
                {"Description", Description.ToString()},
                {"Image", ImageURI.ToString()},
                {"Damage", Damage.ToString()},
                {"Range", Range.ToString()},
                {"Value", Value.ToString()},
                {"Attribute", Attribute.ToString()},
                {"Location", Location.ToString()}
            };
            FormatedString = FormatOutput2();
            // Convert parameters to a key value pairs to a json object
            JObject finalContentJson = (JObject)JToken.FromObject(dict);
            return finalContentJson.ToString();
        }

        public void Update(Item newData)
        {
            if (newData == null)
            {
                return;
            }

            // Update all the fields in the Data, except for the Id
            Name = newData.Name;
            Description = newData.Description;
            Value = newData.Value;
            Attribute = newData.Attribute;
            Location = newData.Location;
            Name = newData.Name;
            Guid = newData.Guid;
            Description = newData.Description;
            ImageURI = newData.ImageURI;
            Range = newData.Range;
            Damage = newData.Damage;
            FormatedString = FormatOutput2();
        }
    }
}


