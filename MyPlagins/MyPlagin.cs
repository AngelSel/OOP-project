using System;
using CharactersCrud;
using System.Text;

namespace MyPlagins
{

    //плагин с кодированием Base64

    [Plugin(PluginType.Coding)]
    public class MyPlagin:ICodingPlugin
    {
        public byte[] Coding(byte[] rezString)
        {
            var result = Convert.ToBase64String(rezString);

            return Encoding.UTF8.GetBytes(result);

        }

        public byte[] Decoding(byte[] fileString)
        { 
            string result = Encoding.UTF8.GetString(fileString);
            var base64EncodedBytes = Convert.FromBase64String(result);
            return base64EncodedBytes;
        }
        public string Name
        {
            get { return "Base64 coding"; }
        }

    }
}
