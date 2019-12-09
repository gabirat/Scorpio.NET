using System.Configuration;

namespace Scorpio.GUI.Configuration
{
    public class CameraConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public CameraConfigElementCollection Instances
        {
            get { return (CameraConfigElementCollection)this[""]; }
            set { this[""] = value; }
        }
    }
    public class CameraConfigElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CameraConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //set to whatever Element Property you want to use for a key
            return ((CameraConfigElement)element).Name;
        }
    }

    public class CameraConfigElement : ConfigurationElement
    {
        //Make sure to set IsKey=true for property exposed as the GetElementKey above
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get => (string)base["name"];
            set => base["name"] = value;
        }

        [ConfigurationProperty("apiEndpoint", IsRequired = true)]
        public string ApiEndpoint
        {
            get => (string)base["apiEndpoint"];
            set => base["apiEndpoint"] = value;
        }
    }
}
