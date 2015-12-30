using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;

namespace TaskManager.Processes
{
    class ImageProcess : ProcessTaskInterface
    {
        Cleanup.ResourceHandler mResourceHandler;
        int mWidth = 0;
        string mInputFolder;
        string mOutputFolder;
        string mFileName;

        public ImageProcess(Cleanup.ResourceHandler pHandler, string pInputFolder, string pOutputFolder, string pFileName, int pWidth)
        {
            this.mResourceHandler = pHandler;
            this.mInputFolder = pInputFolder;
            this.mOutputFolder = pOutputFolder;
            this.mFileName = pFileName;

            this.mWidth = pWidth;
        }

        protected override void Started()
        {
            string arguments = "";

            string delimiter = "&";

            arguments += "folderInput" + delimiter + mInputFolder + " ";
            arguments += "folderOutput" + delimiter + mOutputFolder + " ";
            arguments += "fileName" + delimiter + mFileName + " ";
            arguments += "format" + delimiter + "jpeg" + " ";
            arguments += "width" + delimiter + mWidth + " ";
            arguments += "resolutionWidth" + delimiter + Settings.ImageSettings.THUMBNAIL_RESOLUTION_WIDTH + " ";
            arguments += "resolutionHeight" + delimiter + Settings.ImageSettings.THUMBNAIL_RESOLUTION_HEIGHT;

            ProcessArguments = arguments;
            ProcessName = Settings.ImageSettings.PROCESS_NAME;
        }

        protected override void Finished(Process pProcess, bool pSuccess, Exception pException = null)
        {
            if(pException != null)
            {
                Console.WriteLine("Exception: " + pException.ToString());
            }

            mResourceHandler.SafeRelease();
        }
    }
}
