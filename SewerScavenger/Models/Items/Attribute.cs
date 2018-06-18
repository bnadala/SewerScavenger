
using System;
using System.Collections.Generic;
using System.Linq;

namespace SewerScavenger.Models
{
    // Enum to specify the different attributes allowd.
    // Not spcified is considered not initialize and returns unknown
    // All other attributes have explicted values.

    public enum AttributeEnum
    {
        // Not specified
        Unknown = 0,    

        // The speed of the character, impacts movement, and initiative
        Speed = 10,

        // The defense score, to be used for defending against attacks
        Defense = 12,

        // The Attack score to be used when attacking
        Attack = 14,

        // Current Health which is always at or below MaxHealth
        CurrentHealth = 16,

        // The highest value health can go
        MaxHealth = 18,
    }

    // Helper functions for the AttribureList
    public static class AttributeList
    {

        // Returns a list of strings of the enum for Attribute
        // Removes the attributes that are not changable by Items such as Unknown, MaxHealth
        public static List<string> GetListItem
        {
            get
            {
                var myList = Enum.GetNames(typeof(AttributeEnum)).ToList();
                var myReturn = myList.Where(a => 
                                                a.ToString() != AttributeEnum.Unknown.ToString() && 
                                                a.ToString() != AttributeEnum.MaxHealth.ToString()
                                            ).ToList();
                return myReturn;
            }
        }

        // Returns a list of strings of the enum for Attribute
        // Removes the unknown
        public static List<string> GetListCharacter
        {
            get
            {
                var myList = Enum.GetNames(typeof(AttributeEnum)).ToList();
                var myReturn = myList.Where(a => 
                                                a.ToString() != AttributeEnum.Unknown.ToString()
                                            ).ToList();
                return myReturn;
            }
        }

        // Given the String for an enum, return its value.  That allows for the enums to be numbered 2,4,6 rather than 1,2,3
        public static AttributeEnum ConvertStringToEnum(string value)
        {
            return (AttributeEnum)Enum.Parse(typeof(AttributeEnum), value);
        }
    }

}
