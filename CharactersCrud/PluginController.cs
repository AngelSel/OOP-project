using System;
using System.IO;
using System.Reflection;
using System.Text;
using DSOFile;


namespace CharactersCrud
{
    class PluginController
    {

        public static Type GetTypeFromEnum(PluginType T)
        {
            switch (T)
            {
                case PluginType.Coding:
                    return typeof(ICodingPlugin);
                default:
                    return typeof(IPlugin);
            }
        }
        public static string GetTypenameFromEnum(PluginType T)
        {
            switch (T)
            {
                case PluginType.Coding:
                    return "Coding";
                default:
                    return "Unknown";
            }
        }

        public static char GetTypecharFromEnum(PluginType T)
        {
            switch (T)
            {
                case PluginType.Coding:
                    return 'C';
                default:
                    return '?';
            }
        }

        private IPlugin internalPlugin;
        private string myPath;
        private PluginType myType = PluginType.Unknown;

        public string PluginPath { get { return myPath; } }
        public string Filename { get { return new FileInfo(myPath).Name; } }
        public PluginType Type
        {
            get
            {
                if (internalPlugin == null)
                {
                    return PluginType.Unknown;
                }
                return myType;
            }
        }
        public string Name
        {
            get
            {
                if (internalPlugin == null)
                {
                    return "Not a recognized plugin";
                }
                return internalPlugin.Name;
            }
        }

        public IPlugin PluginInterface { get { return internalPlugin; } }
        public override string ToString()
        { return string.Format("{0}: {1}", GetTypecharFromEnum(myType), myPath); }

        public PluginController(string Path)
        { SetPlugin(Path); }


        public bool SetPlugin(string PluginFile)
        {
            Assembly asm;
            PluginType pt = PluginType.Unknown;
            Type PluginClass = null;

            if (!File.Exists(PluginFile))
            {
                return false;
            }
            asm = Assembly.LoadFile(PluginFile);

            if (asm != null)
            {
                myPath = PluginFile;
                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    object[] attrs = type.GetCustomAttributes(typeof(PluginAttribute), true);
                    if (attrs.Length > 0)
                    {
                        foreach (PluginAttribute pa in attrs)
                        {
                            pt = pa.Type;
                        }

                        PluginClass = type;

                        if (pt == PluginType.Unknown)
                        {
                            return false;
                        }

                        internalPlugin = Activator.CreateInstance(PluginClass) as IPlugin;
                        myType = pt;
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        //установка свойств файла
        public static void SetFileProperty(string filename, string pluginname)
        {
            OleDocumentProperties fileWithPlugin = new OleDocumentProperties();
            fileWithPlugin.Open(filename, false, dsoFileOpenOptions.dsoOptionDefault);
            object Value = pluginname;
            foreach (CustomProperty property in fileWithPlugin.CustomProperties)
            {
                if (property.Name == "MyPlugin")
                {
                    property.set_Value(Value);
                    fileWithPlugin.Close(true);
                    return;
                }
            }
            fileWithPlugin.CustomProperties.Add("MyPlugin", ref Value);
            fileWithPlugin.Save();
            fileWithPlugin.Close(true);
        }


        //получение свойств файла - проверка был ли файл создан с плагином или без
        public static string GetFileProperty(string fileName)
        {
            OleDocumentProperties loadedFile = new OleDocumentProperties();
            loadedFile.Open(fileName, false, dsoFileOpenOptions.dsoOptionDefault);
            foreach (CustomProperty property in loadedFile.CustomProperties)
            {
                if (property.Name == "MyPlugin")
                {
                    string pluginname = property.get_Value();
                    loadedFile.Close(true);
                    return pluginname;
                }
            }
            return null;
        }


        public byte[] ActivatePlugin(PluginController plugin, byte[] _data, bool way)
        {
            switch (plugin.Type)
            {
                case PluginType.Coding:
                    byte[] result = null;
                    if (way)
                    {
                        result = ((ICodingPlugin)plugin.internalPlugin).Coding(_data);
                    }
                    else
                    {
                        result = ((ICodingPlugin)plugin.internalPlugin).Decoding(_data);
                    }
                    return result;
                default:
                    IPlugin plugin_IPlugin = plugin as IPlugin;
                    return null;
            }
        }

        //проверка, подгружен ли данный плагин, создание его экзепляра
        public static int IsPluginExist(string plugin_name,ref PluginController currentPlugin)
        {
            string myPath = Directory.GetCurrentDirectory()+"\\CustomPlugins";
            if (plugin_name == null || plugin_name == "")
            {
                return 0;
            }
            if (!Directory.Exists(myPath))
            {
                return -1;
            }
            foreach (string f in Directory.GetFiles(myPath))
            {
                FileInfo fi = new FileInfo(f);

                if (fi.Extension.Equals(".dll") && fi.Name.Equals(plugin_name))
                {
                    currentPlugin = new PluginController(f);
                    return 1;
                }
            }
            return -1;
        }


    }
}

