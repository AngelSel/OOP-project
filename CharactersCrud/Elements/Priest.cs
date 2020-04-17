using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CharactersCrud.Elements
{
    public enum AbilityPriest
    {
        Heal,
        Shield,
        Exorcism,
        MindControl
    }

    [Serializable]
    class Priest:Character
    {
        public AbilityPriest Ability { get; set; }
        public Priest(AbilityPriest ability,string name, int age, int level, int health, Race race) : base(name, age, level, health,race)
        {
            Ability = ability;            

        }

    }

}
