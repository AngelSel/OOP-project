using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CharactersCrud.Elements;

namespace CharactersCrud
{
    public class BinarySerialization:ISerializations
    {
        public byte[] Serialization(List<Character> characterList)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            formatter.Serialize(memStream, characterList);
            byte[] byteResult = memStream.ToArray();
            return byteResult;

        }

        public List<Character> Deserializations(byte[] info)
        {
            List<Character> characterList;
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream(info);
            characterList = (List<Character>)formatter.Deserialize(memStream);
            return characterList;

        }
    }



    public class JSONSerialization : ISerializations
    {
        public byte[] Serialization(List<Character> characterList)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string Serialized = JsonConvert.SerializeObject(characterList, settings);
            byte[] serializedResult = Encoding.UTF8.GetBytes(Serialized);
            return serializedResult;
        }


        public List<Character> Deserializations(byte[] info)
        {
            string Serialized = Encoding.UTF8.GetString(info);
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            List<Character> characterList = JsonConvert.DeserializeObject<List<Character>>(Serialized, settings);
            return characterList;
        }

    }



    public class MySerialization: ISerializations
    {
        public byte[] Serialization(List<Character> characterList)
        {
            string serializedString = null;
            foreach (Character item in characterList)
            {
                ObjectSerialization(ref serializedString, item);
                serializedString += "};";
            }
            serializedString += "$";
            byte[] serialisedArray = Encoding.UTF8.GetBytes(serializedString);
            return serialisedArray;

        }

        //сериализация отдельного объекта персонажа
        private static void ObjectSerialization(ref string str, object obj)
        {
            Character item = (Character)obj;
            Type type = obj.GetType();

            str += obj.GetType().ToString() + "{";

            foreach (PropertyInfo param in type.GetProperties())
            {
                if (param.PropertyType.IsClass && !(param.PropertyType.Equals(typeof(string))))
                {
                    ObjectSerializationArmour(ref str, param.GetValue(item));
                    str += "},";
                }
                else
                {
                    str += param.PropertyType + ":" + param.GetValue(item) + ",";
                }
            }
        }

        //сериализация класса Armour
        private static void ObjectSerializationArmour(ref string str, object obj)
        {
            Armour item = (Armour)obj;
            Type type = item.GetType();
            str += type.ToString() + "{";

            foreach (PropertyInfo param in type.GetProperties())
            {
                    str += param.PropertyType + ":" + param.GetValue(item) + ",";
            }
        }

        public List<Character> Deserializations(byte[] serializedArray)
        {
            string Serialized = Encoding.UTF8.GetString(serializedArray);
            List<Character> charactersList = new List<Character>();
            while (Serialized != "$")
            {
                object temp = Parser(ref Serialized);
                charactersList.Add((Character)temp);
            }
            return charactersList;
        }
      
        //получение типа поля
        private static Type FieldType(ref string str, int ind)
        {
            Type objtype = Type.GetType(str.Substring(0, ind), false, true);
            str = str.Remove(0, ind + 1);
            return objtype;
        }

        //получение значения поля
        private static string GetValue(ref string str)
        {
            int ind = str.IndexOf(',');
            string s = str.Substring(0, ind);
            str = str.Remove(0, ind + 1);
            return s;
        }

        //преобразование объекта в 'сериализованную' строку
        private static object Parser(ref string str)
        {
            int indexLeft = str.IndexOf('{');
            Type objtype = FieldType(ref str, indexLeft);
            PropertyInfo[] fields = objtype.GetProperties();
            Type[] types = new Type[fields.Length];
            Object[] values = new Object[fields.Length];
            int i = 0;
            foreach (var param in fields)
            {
                int index1 = str.IndexOf(':');
                int index2 = str.IndexOf('{');
                if (index1 < index2 || index2 == -1)
                {
                    types[i] = FieldType(ref str, index1);
                    string value_str = GetValue(ref str);
                    if (types[i].Name == "Int32")
                        values[i] = Convert.ToInt32(value_str);
                    else if (types[i].Name == "String")
                        values[i] = value_str;
                    else if (types[i].Name == "Boolean")
                        values[i] = Convert.ToBoolean(value_str);
                    else
                    {
                        values[i] = Enum.Parse(types[i], value_str);
                    }
                }
                else
                {
                    values[i] = Parser(ref str);
                }
                i++;
            }
            str = str.Remove(0, 2);
            Object obj = Activator.CreateInstance(objtype, values);

            if(obj.GetType().Equals(typeof(Armour)))
            {
                Armour itemChar1 = (Armour)obj;
                return itemChar1;
            }
            else 
            {
                Character itemChar = (Character)obj;
                return itemChar;
            }
        }
    }
}
