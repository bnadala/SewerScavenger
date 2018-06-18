using System;
using System.Collections.Generic;
using System.Linq;

namespace SewerScavenger.Models
{
    // Enum to specify the different Inventory Locations.
    // Each inventory location can hold just one item
    // All other attributes have explicted values.

    public enum ItemLocationEnum
    {
        // Not specified
        Unknown = 0,

        // The head includes, Hats, Helms, Caps, Crowns, Hair Ribbons, Bunny Ears, and anything else that sits on the head
        Head = 10,

        // Things to put around the neck, such as necklass, broaches, scarfs, neck ribbons.  Can have at the same time with Head items ex. Ribbon for Hair, and Ribbon for Neck is OK to have
        Necklass = 12,

        // The primary hand used for fighting with a sword or a staff.  
        PrimaryHand = 20,

        // The second hand used for holding a shield or dagger, or wand.  OK to have both primary and offhand loaded at the same time
        OffHand = 22,

        // Any finger, used for rings, because they can go on any finger.
        Finger = 30,

        // A finger on the Right hand for rings.  Can only have one right on the right hand
        RightFinger = 31,

        // A finger on the left hand for rings.  Can only have one ring on the left hand.  Can have ring on left and right at the same time
        LeftFinger = 32,

        // Boots, shoes, socks or anything else on the feet
        Feet = 40,

    }

    // Helper functions for the Item Locations
    public static class ItemLocationList
    {
        // Gets the lsit of locations that an Item can have.
        // Does not include the Left and Right Finger
        public static List<string> GetListItem
        {
            get
            {
                var myList = Enum.GetNames(typeof(ItemLocationEnum)).ToList();
                var myReturn = myList.Where(a =>
                                            a.ToString() != ItemLocationEnum.Unknown.ToString() &&
                                            a.ToString() != ItemLocationEnum.LeftFinger.ToString() &&
                                            a.ToString() != ItemLocationEnum.RightFinger.ToString()
                                            ).ToList();
                return myReturn;
            }
        }

        // Gets the list of locations a character can use
        // Removes Finger for example, and allows for left and right finger
        public static List<string> GetListCharacter
        {
            get
            {
                var myList = Enum.GetNames(typeof(ItemLocationEnum)).ToList();
                var myReturn = myList.Where(a => 
                                                a.ToString() != ItemLocationEnum.Finger.ToString()
                                            ).ToList();
                return myReturn;
            }
        }

        // Given the String for an enum, return its value.  That allows for the enums to be numbered 2,4,6 rather than 1,2,3
        public static ItemLocationEnum ConvertStringToEnum(string value)
        {
            return (ItemLocationEnum)Enum.Parse(typeof(ItemLocationEnum), value);
        }

    }


}
