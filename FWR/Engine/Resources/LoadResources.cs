using FWR.UI_Aux;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FWR.Engine.Resources
{
    public static class LoadResources
    {
        public static List<Resource> PerformLoadResources()
        {
            List<Resource> _resources = new List<Resource>();

            string resourcesPath = Path.Combine(StringHandlers.Unescape(Runtime.config.MAIN_DIR), Const.EnvironmentSubfolder);
            string[]  jsonFiles = Directory.GetFiles(resourcesPath, "*.*", SearchOption.AllDirectories).Where(fn => Path.GetExtension(fn).ToLower() == ".json").Select(x => Path.GetFullPath(x)).ToArray();
            foreach(string file in jsonFiles)
            {
                Resource resource = new Resource { ResourceJsonFilePath = file };
                _resources.Add(resource);
            }
            return _resources;
        }
    }
}
