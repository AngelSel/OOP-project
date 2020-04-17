using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CharactersCrud.Elements;
using CharactersCrud;
using System.IO;

namespace SerializationsTests
{
    [TestClass]
    public class MySerializationTest
    {
        [TestMethod]
        public void MySerializationTests()
        {
            List<Character> characterList= new List<Character>();
            List<Character> resultList = new List<Character>();
            Character OutSider = new Summoner(SummCreatures.Kraken,MagicT.Water,MagicA.ArcaneSignet,"Outsider",3100,11,10,Race.Human);
            Armour someArmour = new Armour("helmet","chain");
            Character Bers = new Berserker(someArmour,BeastTypeBerserker.Wolf,WeaponType.Sword,"Juse",41,8,5,Race.Werewolf);
            characterList.Add(OutSider);
            characterList.Add(Bers);

            ISerializations someSerialization = new MySerialization();

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ss.txt");

            someSerialization.Serialization(path, characterList);

            resultList = someSerialization.Deserializations(path);

            for(int i =0;i<characterList.Count;i++)
            {
                Assert.AreEqual(characterList[i].GetType(), resultList[i].GetType());
                Assert.AreEqual(characterList[i].Name,resultList[i].Name);
                Assert.AreEqual(characterList[i].Age, resultList[i].Age);
                Assert.AreEqual(characterList[i].Level, resultList[i].Level);
                Assert.AreEqual(characterList[i].RaceType, resultList[i].RaceType);
            }

        }


    }
}
