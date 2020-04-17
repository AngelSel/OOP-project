using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CharactersCrud.Elements
{

    [Serializable]
    public class Armour
    {
        public string helmet { get; set; }
        public string chainMail { get; set; }

        public Armour(string Helmet, string ChainMail)
        {
            helmet = Helmet;
            chainMail = ChainMail;
        }

    }

    public enum BeastTypeBerserker
    { 
        Wolf,
        Bear,
        Lion
    }
    public enum WeaponType
    { 
        Sword,
        Knife,
        Axe,
        Bow
    }

    [Serializable]
    public class Warrior : Character
    {
        public WeaponType Weapon { get; set; }

       
        public Warrior(WeaponType weapon,string name, int age, int level, int health,Race race) : base(name, age, level, health,race)
        {
            Weapon = weapon;
           
        }

    }

    [Serializable]
    public class Berserker: Warrior
    {
        public Armour armour { get; set; }
        public BeastTypeBerserker BeastType { get; set; }

        public Berserker(Armour somearmour, BeastTypeBerserker beastType,WeaponType weapon, string name, int age, int level, int health,Race race) :base(weapon,name, age, level, health,race)
        {
            BeastType = beastType;
            armour = somearmour;

        }

    }

}
