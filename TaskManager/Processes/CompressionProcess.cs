using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;

using NReco.VideoConverter;

namespace TaskManager.Processes
{
    public class CompressionProcess : ProcessTaskInterface
    {
        private static string COMPRESSED_FOLDER_NAME = "Compressed";
        private Cleanup.ResourceHandler mResourceHandler;
        private string mOutputNamedFolder;
        private string mConvertFolder;
        private string mFileName;

        public CompressionProcess(Cleanup.ResourceHandler pResourceHandler, string pOutputNamedFolder, string pConvertFolder, string pFileName)
        {
            this.mResourceHandler = pResourceHandler;
            this.mOutputNamedFolder = pOutputNamedFolder;
            this.mConvertFolder = pConvertFolder;
            this.mFileName = pFileName;
        }

        protected override void Started()
        {
            /*
             * TODO:
             *  - Recheck all arguments before starting to avoid starting a process that maybe wont 
             *    shutdown correctly and thus will keep the file busy and wont let it be deleted. 
             *  - Add better failsafe if somethings is gone wrong.
             */

            string fileConverted = Path.Combine(mConvertFolder, mFileName);

            if (File.Exists(fileConverted))
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(mFileName);

                string outputFolder = Path.Combine(mOutputNamedFolder, COMPRESSED_FOLDER_NAME);
                string outputFile = Path.Combine(outputFolder, mFileName);

                string arguments = "";

                arguments += "-i " + fileConverted + " ";
                arguments += Settings.CompressionSettings.OUTPUT_FRAMERATE;
                arguments += Settings.CompressionSettings.OUTPUT_BITRATE;
                arguments += outputFile;

                Directory.CreateDirectory(outputFolder);

                ProcessArguments = arguments;
                ProcessName = Settings.CompressionSettings.COMPRESSION_PROCESS_NAME;
            }
        }

        protected override void Finished(Process pProcess, bool pSuccess, Exception pException = null)
        {
            /*
             * TODO:
             *  - Add a way to tell the system (App) to tell the client if something has gone wrong.
             *    One way to do it, is use "ErrorReportingProcess" to create a file inside an output folder with 
             *    filename and let the "App" look for something like that.
             *    
             * - Add more if statements to make sure everything is error proof when something is missing or was not created 
             *   properly.
             */

            Console.WriteLine("_______________________________________________________________________");

            if (pSuccess)
            {
                string compressedFolder = Path.Combine(mOutputNamedFolder, COMPRESSED_FOLDER_NAME);
                string compressed = Path.Combine(compressedFolder, mFileName);

                string fileConverted = Path.Combine(mConvertFolder, mFileName);

                FileInfo infoOriginal = new FileInfo(fileConverted);
                FileInfo info = new FileInfo(compressed);

                long sizeOriginal = infoOriginal.Length;
                long sizeCompressed = info.Length;

                Cleanup.ResourceHandler.MoveFile(compressed, Path.Combine(mOutputNamedFolder, mFileName));

                mResourceHandler.AddForDeletionFolder(compressedFolder);

                var ffMpegConverter = new FFMpegConverter();

                string fileNameNoExt = Path.GetFileNameWithoutExtension(mFileName);

                string tempfile = Path.Combine(mOutputNamedFolder, "temp.jpeg");
                string thumbnail = Path.Combine(mOutputNamedFolder, fileNameNoExt + "_thumbnail.jpeg");

                ffMpegConverter.GetVideoThumbnail(Path.Combine(mOutputNamedFolder, mFileName),
                    tempfile);

                int width = 120;
                int resWidth = 72;
                int resHeight = 72;

                var imgInput = new System.Drawing.Bitmap(tempfile);

                // Initialize from Settings.
                width = ImageHelper.GetImageType(imgInput.Width * imgInput.Height);
                resWidth = Settings.ImageSettings.THUMBNAIL_RESOLUTION_WIDTH;
                resHeight = Settings.ImageSettings.THUMBNAIL_RESOLUTION_HEIGHT;

                double y = imgInput.Height;
                double x = imgInput.Width;

                var imgOutput = new System.Drawing.Bitmap(width, (int)(y * width / x));
                imgOutput.SetResolution(resWidth, resHeight); // Set DPI of image (xDpi, yDpi)

                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(imgOutput);
                graphics.Clear(System.Drawing.Color.White);
                graphics.DrawImage(imgInput, new System.Drawing.Rectangle(0, 0, width, (int)(y * width / x)),
                new System.Drawing.Rectangle(0, 0, (int)x, (int)y), System.Drawing.GraphicsUnit.Pixel);

                // Alright, overwriting doesn't work, so save as temp file then resize and remove temp file.
                imgOutput.Save(thumbnail, System.Drawing.Imaging.ImageFormat.Jpeg);
                imgInput.Dispose();

                File.Delete(tempfile);

                Console.WriteLine("COMPRESSION: Success - " + mFileName);
            }
            else
            {
                Console.WriteLine("COMPRESSION: Exception - " + pException.ToString());
            }

            mResourceHandler.SafeRelease();
        }
    }
}
