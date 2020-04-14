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
        public void Serialization(string fileName, List<Character> characterList)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, characterList);
            }

        }

        public List<Character> Deserializations(string fileName)
        {
            List<Character> FileCharacter;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                FileCharacter = (List<Character>)formatter.Deserialize(fs);
            }
            return FileCharacter;

        }
    }

    public class JSONSerialization : ISerializations
    {
        public void Serialization(string fileName, List<Character> characterList)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string Serialized = JsonConvert.SerializeObject(characterList, settings);
            using (StreamWriter jsfile = File.CreateText(fileName))
            {
                jsfile.WriteLine(Serialized);
            }

        }


        public List<Character> Deserializations(string fileName)
        {
            string Serialized;
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            using (StreamReader jsfile = File.OpenText(fileName))
            {
                Serialized = jsfile.ReadLine();
            }
            List<Character> FileCharacters = JsonConvert.DeserializeObject<List<Character>>(Serialized, settings);
            return FileCharacters;

        }

    }

    public class MySerialization: ISerializations
    {
        public void Serialization(string fileName, List<Character> characterList)
        {
            string serializedString = null;
            foreach (Character item in characterList)
            {
                ObjectSerialization(ref serializedString, item);
                serializedString += "};";
            }
            serializedString += "$";
            using (StreamWriter txtfile = File.CreateText(fileName))
            {
                txtfile.WriteLine(serializedString);
            }

        }

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

        public List<Character> Deserializations(string fileName)
        {
            string Serialized = null;
            using (StreamReader file = File.OpenText(fileName))
            {
                Serialized = file.ReadLine();
            }
            List<Character> Local = new List<Character>();
            while (Serialized != "$")
            {
                object temp = Parser(ref Serialized);               
                Local.Add((Character)temp);
            }
            return Local;


        }

        
        private static Type FieldType(ref string str, int ind)
        {
            Type objtype = Type.GetType(str.Substring(0, ind), false, true);
            str = str.Remove(0, ind + 1);
            return objtype;
        }

        private static string GetValue(ref string str)
        {
            int ind = str.IndexOf(',');
            string s = str.Substring(0, ind);
            str = str.Remove(0, ind + 1);
            return s;
        }

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
