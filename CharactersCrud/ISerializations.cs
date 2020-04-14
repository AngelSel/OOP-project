using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharactersCrud
{
    public interface ISerializations
    {
        void Serialization(string fileName, List<Elements.Character> characterList);
        List<Elements.Character> Deserializations(string fileName);
    }
}
