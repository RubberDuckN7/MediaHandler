using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NReco.VideoConverter;
using System.IO;

using GlobalTools;

/*
 * For this program it will expect multiple arguments with semantic name and semicolon as a delimeter.
 * This is to avoid wrong order of arguments but will be very tied to specific names.
 * 
 * Format is like this:
 * Name:Value Name1:Value1
 * No spaces between :.
 * 
 * The order of arguments passed doesn't matter but keep format, even if there are failsafe checks 
 * try handling them outside this.
 * 
 * Error codes are kinda hardcoded here so... for future handle it some other way ? if there 
 * is in existing system already some way of global error code list.
 */ 

namespace NRecoConversion
{
    class Program
    {
        static int Main(string[] args)
        {
            // So read arguments in a pair-made format.
            Dictionary<string, string> values = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                string [] spl = args[i].Split('&');
                if(spl == null || spl.Length < 2)
                {
                    return (int)LogCodes.RETURN.CORRUPT_ARGUMENT;
                }
                else
                {
                    // No duplicates. 
                    // Fill otherwise in with whatever option it is needed.
                    if(values.ContainsKey(spl[0]) == false)
                    {
                        values.Add(spl[0], spl[1]);
                    }
                }
            }

            // Check that all the settings and other values that are needed has been added correctly
            // otherwise return error code.
            if(values.ContainsKey("format") == false || values["format"] == "")
            {
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }
            if (values.ContainsKey("fileName") == false || values["fileName"] == "")
            {
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }
            if (values.ContainsKey("folderOutput") == false || values["folderOutput"] == "")
            {
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }
            if (values.ContainsKey("folderInput") == false || values["folderInput"] == "")
            {
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }

            if(File.Exists(Path.Combine(values["folderInput"], values["fileName"])) == false)
            {
                return (int)LogCodes.RETURN.FILE_NOT_FOUND;
            }
            if(Directory.Exists(values["folderOutput"]) == false)
            {
                Console.WriteLine("No such folder: " + values["folderOutput"]);
                return (int)LogCodes.RETURN.NO_SUCH_DIRECTORY;
            }

            /*
             * TODO: 
             *  - Write failsafes for invalid values!
             *    (Right there are only checks if there is any value at all.)
             *  - Make sure all checks are being made for all the files if they exist or not.
             */

            string format = values["format"];
            string fileName = values["fileName"];
            string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
            string folder = values["folderOutput"]; // fileNameNoExt;
            //folder = Path.Combine(values["folderOutput"], folder);
            folder = Path.Combine(folder, "Converted");

            string folderInput = values["folderInput"];

            Console.WriteLine("All arguments are present.");

            // Debug
            Console.WriteLine("format - " + values["format"]);
            Console.WriteLine("fileName - " + values["fileName"]);
            Console.WriteLine("folderOutput - " + values["folderOutput"]);
            Console.WriteLine("folderInput - " + values["folderInput"]);

            Directory.CreateDirectory(folder);

            FFMpegConverter FFMpegConv;
            FFMpegConv = new FFMpegConverter();

            string convertedPath = Path.Combine(folder, fileNameNoExt + "." + format);

            int returnCode = (int)LogCodes.RETURN.OK;
            // Convert
            Console.WriteLine("Starting conversion.");

            /*
             * TODO:
             *  - Maybe add a better explanation error code for exception that was being made? 
             *    as it could be multiple. This is for robustness. 
             *    
             *    In case some conversion fails due to wrong argumet, maybe add "retry" mechanism in
             *    Process class?
             */

            try
            {
                FFMpegConv.ConvertMedia(Path.Combine(folderInput, fileName), convertedPath, format);
                Console.Write("Success!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed converting!");
                Console.WriteLine(e.ToString() + "\n");
                returnCode = (int)LogCodes.RETURN.CONVERT_EXCEPTION;
            }

            return returnCode;
        }
    }
}
