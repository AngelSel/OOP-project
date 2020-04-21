using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharactersCrud
{
    public  interface IPlugin
    {
        string Name { get; }
    }

    public interface ICodingPlugin:IPlugin
    {
       byte[] Coding(byte[] stringToCode);
       byte[] Decoding(byte[] fileString);

    }

    public enum PluginType
    {
        Coding,
        Unknown
    };


    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PluginAttribute : Attribute
    {
        private PluginType _Type;
        public PluginAttribute(PluginType T) { _Type = T; }
        public PluginType Type { get { return _Type; } }
    };
}

