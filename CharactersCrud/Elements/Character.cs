using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CharactersCrud.Elements
{
    public enum Race
    {
        Human,
        Elf,
        Dwarf,
        Druid,
        Troll,
        Vampire,
        Werewolf
    }


    [Serializable]
    public abstract class Character
    {
        public enum TCategorys {Magician,Summoner,Priest,Exorcist,Warrior,Berserker };
        public string Name { get; set; }
        public int Age { get; set; }
        public int Level { get; set; }

        public int Health { get; set; }
        public Race RaceType { get; set; }

        public Character(string name, int age, int level, int health,Race race)
        {
            Name = name;
            Age = age;
            Level = level;
            Health = health;
            RaceType = race;

        }
    }
}
