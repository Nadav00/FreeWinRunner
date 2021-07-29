using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FWR.Auxilary
{
    public static class Namer
    {
        static List<String> Adjie = new List<string>()
        {
            "Red","Green","Blue","White","Black","Almighty","Humble","Agnostic","City","House","Ultra","Virtual","Sharp","Wild",
            "Classic","Pop","Rock","North","South","East","West","Global","Ancient","Advanced","Math","Jr.","Mrs.","Mr.","Perfect",
            "Dotted","Strict","Custum","Noon", "Night","Morning","Naval","Airborne","Straight", "Decent", "Nimble"

        };

        static List<String> Namy = new List<string>()
        {
            "Boy","Girl","Woman","Man","Dragon","Deity","Snail","Cat","Canine","Elephant","Whale","Shark","Turtle","Dolphin",
            "Ape","Giraffe","Sword","Warrior","Architect","Builder","Ship","World","Computer","Bench","Box","Fence","Car",
            "Bird","Rain","Sun","Moon","Cloud","Road","Wind","Bus","Falcon","Treasure","Vault","Sea","Island","Calling",
            "Burger", "Pizza", "Salad", "Fish", "Legend", "Exodus", "Genesis","Hero", "Ego", "Draft", "Fix", "Storm", "Case",
            "Shield", "Boot", "Icon", "Queue", "List", "Cherry", "Snow", "Volcano", "Jam", "Oil", "Powder", "Crystal",
            "Sphere", "Cube", "Art", "Show", "Arrow", "Cannon", "Guitar", "Drum", "Ice", "Plain", "Heights", "Species",
            "Raccoon", "Shot", "Action", "Aim", "Eye"
        };

        static List<List<String>> ListsArray = new List<List<string>>() { Adjie, Namy };

        public static string MakeName(int words)
        {
            string returnString = String.Empty;
            int index = 0;
            if (words > 0)
            {
                for (int x = 0; x < words; x++)
                {
                    if (x == (words - 1))
                        index = 1;
                    Thread.Sleep(20);
                    returnString += ListsArray[index][new Random().Next(0, ListsArray[index].Count - 1)] + " ";
                }
            }

            return returnString.Substring(0,returnString.Length - 1);
        }
    }
}
