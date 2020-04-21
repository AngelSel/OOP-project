using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CharactersCrud;

namespace Base32Coding
{

    //плагин с кодированием Base32

    [Plugin(PluginType.Coding)]
    public class Base32Plugin : ICodingPlugin
    {
        private const string Base32AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public byte[] Coding(byte[] rezString)
        {
            if (rezString == null || rezString.Length == 0)
            {
                return new byte[0];
            }

            var bits = rezString.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Aggregate((a, b) => a + b).PadRight((int)(Math.Ceiling((rezString.Length * 8) / 5d) * 5), '0');
            var result = Enumerable.Range(0, bits.Length / 5).Select(i => Base32AllowedCharacters.Substring(Convert.ToInt32(bits.Substring(i * 5, 5), 2), 1)).Aggregate((a, b) => a + b);
            result = result.PadRight((int)(Math.Ceiling(result.Length / 8d) * 8), '=');
            byte[] resulting = Encoding.UTF8.GetBytes(result);
            return resulting;
        }

        public byte[] Decoding(byte[] fileString)
        {
            if (fileString == null || fileString.Length == 0)
            {
                return new byte[0];
            }

            string doingString = Encoding.UTF8.GetString(fileString);
            var bits = doingString.TrimEnd('=').ToUpper().ToCharArray().Select(c => Convert.ToString(Base32AllowedCharacters.IndexOf(c), 2).PadLeft(5, '0')).Aggregate((a, b) => a + b);
            var resultConvertion = Enumerable.Range(0, bits.Length / 8).Select(i => Convert.ToByte(bits.Substring(i * 8, 8), 2)).ToArray();
            return resultConvertion;
        }
        public string Name
        {
            get { return "Base32 coding"; }
        }
    }
}
