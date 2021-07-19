using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWR.UI_Aux
{
    public class FileSystemPickers
    {
        public static string FolderPicker(string startFolder)
        {
            var form = new Microsoft.Win32.SaveFileDialog();
            startFolder = StringHandlers.Unescape(startFolder);
            form.InitialDirectory = startFolder;
            form.Title = "Select a Directory and click Save";
            form.Filter = "Directory|.";
            form.FileName = "Click Save when in desired directory";
            if (form.ShowDialog() == true)
            {
                string path = form.FileName;
                path = path.Replace("Click Save when in desired directory.", "");
                if (!System.IO.Directory.Exists(path))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.Write("Error creating folder " + path + ":" + ex.Message);
                    }
                }
                return path;
            }
            return null;
        }

        public static string FilePicker(string startFolder, string fileExtension)
        {
            fileExtension = fileExtension?.Length > 0 ? fileExtension : "*";

            var form = new Microsoft.Win32.SaveFileDialog();
            startFolder = StringHandlers.Unescape(startFolder);
            form.InitialDirectory = startFolder;
            form.OverwritePrompt = false;
            form.Title = "Select file and click Save";
            form.Filter = "File|*." + fileExtension;
            form.FileName = "Click Save when desired filename selected or type here";
            if (form.ShowDialog() == true)
                return form.FileName;

            return null;
        }
    }
}
