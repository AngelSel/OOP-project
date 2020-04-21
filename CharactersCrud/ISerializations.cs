using System;
using System.Collections.Generic;


namespace CharactersCrud
{
    public interface ISerializations
    {
        byte[] Serialization( List<Elements.Character> characterList);
        List<Elements.Character> Deserializations(byte[] information);
    }
}
