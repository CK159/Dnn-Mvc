using System.Collections.Generic;
using System.Configuration;
using System.Linq;

//Based on https://blogs.perficient.com/2017/01/05/4-easy-steps-to-custom-sections-in-web-config/
namespace SampleMVC.Modules.SampleMVC.Mvc
{
    //This class reads the defined config section (if available) and stores it locally in the static _Config variable.
    //This config data is available by calling MedGroups.GetMedGroups().
    public class MvcModuleConfig
    {
        private static readonly string configSectionName = "samplemvc.mvc";
        
        public static MvcConfigSection _Config =
            ConfigurationManager.GetSection(configSectionName) as MvcConfigSection;

        public static MvcAssemblyCollection GetAssemblies()
        {
            return _Config.Assemblies;
        }
        
        public static List<string> GetAssembliesList()
        {
            return _Config?.Assemblies.Select(a => a.Assembly).ToList() ?? new List<string>();
        }
    }

    //Extend the ConfigurationSection class.  Your class name should match your section name and be postfixed with "Section".
    public class MvcConfigSection : ConfigurationSection
    {
        //Decorate the property with the tag for your collection.
        [ConfigurationProperty("assemblies")]
        public MvcAssemblyCollection Assemblies => (MvcAssemblyCollection) this["assemblies"];
        
        //Other MVC config could go here
    }

    //Extend the ConfigurationElementCollection class.
    //Decorate the class with the class that represents a single element in the collection.
    [ConfigurationCollection(typeof(AssemblyElement))]
    public class MvcAssemblyCollection : ConfigurationElementCollection, IEnumerable<AssemblyElement>
    {
        public AssemblyElement this[int index]
        {
            get { return (AssemblyElement) BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssemblyElement) element).Assembly;
        }
        
        public new IEnumerator<AssemblyElement> GetEnumerator()
        {
            foreach (var key in this.BaseGetAllKeys())
            {
                yield return (AssemblyElement)BaseGet(key);
            }
        }
    }

    //Extend the ConfigurationElement class.  This class represents a single element in the collection.
    //Create a property for each xml attribute in your element.
    //Decorate each property with the ConfigurationProperty decorator.  See MSDN for all available options.
    public class AssemblyElement : ConfigurationElement
    {
        [ConfigurationProperty("assembly", IsRequired = true)]
        public string Assembly
        {
            get { return (string) this["assembly"]; }
            set { this["assembly"] = value; }
        }
    }
}