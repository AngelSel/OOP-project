using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CharactersCrud.Elements
{
    [Serializable]
    class Priest:Character
    {
        public string Shield { get; set; }
        public Priest(string shield,string name, int age, int level, int health, Race race) : base(name, age, level, health,race)
        {
            Shield = shield;          

        }

    }


    [Serializable]
    class Exorcist:Priest
    {
        public Exorcist( string shield,string name, int age, int level, int health,Race race) : base(shield,name, age, level, health,race)
        {
           
        }

    }
}
