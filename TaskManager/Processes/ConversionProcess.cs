using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;
using GlobalTools;

namespace TaskManager.Processes
{
    public class ConversionProcess : ProcessTaskInterface
    {
        private Cleanup.ResourceHandler mResourceHandler;
        private string mRootFolder;
        private string mOutputFolder;
        private string mFileName;

        public ConversionProcess(Cleanup.ResourceHandler pResourceHandler, string pRootFolder, string pOutputFolder, string pFileName)
        {
            this.mResourceHandler = pResourceHandler;
            this.mRootFolder = pRootFolder;
            this.mOutputFolder = pOutputFolder;
            this.mFileName = pFileName;
        }

        protected override void Started()
        {
            /*
             * TODO:
             *  - Add ifs to make sure all arguments are valid to avoid starting a process. 
             *    (While NRecoConversion process has already some checks, it maybe good to avoid startup 
             *     very soon if error is found.)
             */

            // Conversion process will determine itself if file exist or not.
            string arguments = "";
            string delimiter = "&";
            arguments += "format" + delimiter + Settings.ConversionSettings.FORMAT + " ";
            arguments += "fileName" + delimiter + mFileName + " ";
            arguments += "folderInput" + delimiter + mRootFolder + " ";
            arguments += "folderOutput" + delimiter + mOutputFolder + " ";

            ProcessArguments = arguments;
            ProcessName = Settings.ConversionSettings.CONVERSION_PROCESS_NAME;
        }

        protected override void Finished(Process pProcess, bool pSuccessStart, Exception pException = null)
        {
            int code = pProcess.ExitCode;

            /*
             * TOOD:
             *  - Add robustness by checking if all files does exist and added. 
             *  - If there is a need, a good way to "restart" a process is through here, by calling for example
             *    GlobalScheduleHandler.AddTask( this ) as it already has all the arguments.
             *  - If it fails, tell in a good way the "App" that is has failed to convert. 
             *    There are multiple reasons why it could fail: wrong file, bad arguments, bad name characters? and maybe others. 
             */

            Console.WriteLine("_______________________________________________________________________");

            // So here either we got a fully working converted media file, then create 
            // a task to compress the file and add it to queue in scheduler through "Global interface".
            if (code == (int)LogCodes.RETURN.OK)
            {
                Console.WriteLine("CONVERSION: Success - " + mFileName);
            }
            else
            {
                Console.WriteLine("CONVERSION: Fail - " + GlobalTools.LogCodes.GetDescription(code));

                if (pException != null)
                {
                    Console.WriteLine("CONVERSION: Exception - " + pException.ToString());
                }
            }
        }
    }
}
