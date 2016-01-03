using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;
using System.Collections;

using System.Configuration;
using System.Collections.Specialized;

using NReco.VideoConverter;

namespace TaskManager
{
    public class Program
    {
        private static int IsSupportedFormat(string pFormat)
        {
            pFormat = pFormat.ToLower();

            Console.WriteLine("Searching for format: " + pFormat);
            // Go through supported formats
            // ----------------------------------------------------------------------------------------------------
            for (int f = 0; f < Settings.ConversionSettings.SUPPORTED_FORMATS.Length; f++)
            {
                Console.WriteLine("Checking video: " + Settings.ConversionSettings.SUPPORTED_FORMATS[f]);

                // When extension is extracted the dot is present.
                if (pFormat == (Settings.ConversionSettings.SUPPORTED_FORMATS[f]).ToLower())
                {
                    Console.WriteLine("Type video.");
                    //Console.ReadLine();
                    return 1;
                }
            }

            for (int f = 0; f < Settings.ImageSettings.SUPPORTED_FORMATS.Length; f++)
            {
                Console.WriteLine("Checking image format: " + Settings.ImageSettings.SUPPORTED_FORMATS[f].ToLower());

                // When extension is extracted the dot is present.
                if (pFormat == Settings.ImageSettings.SUPPORTED_FORMATS[f].ToLower())
                {
                    Console.WriteLine("Type image.");
                    //Console.ReadLine();
                    return 2;
                }
            }

            Console.WriteLine("Type unsupported.");
            //Console.ReadLine();
            return -1;
        }

        /// <summary>
        /// For easier reading I've put everything into 3 functions, rather than process classes.
        /// This should clarify more how everything works. 
        /// </summary>
        /// <param name="pResourceHandler"></param>
        /// <param name="pFilePath"></param>
        /// <param name="pOutputFolder"></param>
        /// <param name="pAffinity"></param>
        /// <returns></returns>
        private static int ConvertAndCompressVideo(Cleanup.ResourceHandler pResourceHandler, string pFilePath, string pOutputFolder, int pAffinity)
        {
            ///////////////////////////// FIRST STEP IS TO CONVERT FILE /////////////////////////////
            string arguments = "";
            string delimiter = "&";

            string fileName = Path.GetFileName(pFilePath);
            string rootFolder = Path.GetDirectoryName(pFilePath);

            arguments += "format" + delimiter + Settings.ConversionSettings.FORMAT + " ";
            arguments += "fileName" + delimiter + fileName + " ";
            arguments += "folderInput" + delimiter + rootFolder + " ";
            arguments += "folderOutput" + delimiter + pOutputFolder + " ";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Settings.ConversionSettings.CONVERSION_PROCESS_NAME,
                    Arguments = arguments
                }
            };

            try
            {
                process.Start();
                process.ProcessorAffinity = (System.IntPtr)pAffinity;
                process.WaitForExit();
            }
            catch (Exception e)
            {
                string err = e.ToString();
                Console.WriteLine("Failed process image: " + err);
                return -1;
            }

            ///////////////////////////// COMPRESS FILE /////////////////////////////
            string fileNoExt = Path.GetFileNameWithoutExtension(pFilePath);
            string convertedFileName = "converted_" + fileNoExt + "." + Settings.ConversionSettings.FORMAT;

            pResourceHandler.AddForDeletionFile(Path.Combine(pOutputFolder, convertedFileName));

            string newFilePath = Path.GetDirectoryName(pFilePath);
            newFilePath = Path.Combine(newFilePath, Settings.MediaSettings.OUTPUT_FOLDER);
            newFilePath = Path.Combine(newFilePath, convertedFileName);
                
            string outputFilePath = Path.Combine(pOutputFolder, fileNoExt + "." + Settings.ConversionSettings.FORMAT);

            return CompressVideo(pResourceHandler, newFilePath, outputFilePath, pAffinity);
        }

        private static int CompressVideo(Cleanup.ResourceHandler pResourceHandler, string pFilePath, string pFilePathOut, int pAffinity)
        {
            ///////////////////////////// CREATE ARGUMENTS AND START COMPRESSING PROCESS /////////////////////////////
            string arguments = "";

            arguments += "-i " + pFilePath + " ";
            arguments += pFilePathOut;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Settings.CompressionSettings.COMPRESSION_PROCESS_NAME,
                    Arguments = arguments
                }
            };

            try
            {
                process.Start();
                process.ProcessorAffinity = (System.IntPtr)pAffinity;
                process.WaitForExit();
            }
            catch (Exception e)
            {
                string err = e.ToString();
                Console.WriteLine("Failed process image: " + err);
                return -1;
            }

            ///////////////////////////// THIS IS THE SAME CODE I'VE GOT, IT CREATES THUMBNAIL /////////////////////////////
            string compressedFolder = Path.GetDirectoryName(pFilePath);
            string compressed = pFilePathOut;

            var ffMpegConverter = new FFMpegConverter();

            string fileNameNoExt = Path.GetFileNameWithoutExtension(pFilePath);
            string outputFolder = Path.GetDirectoryName(pFilePathOut);

            string tempfile = Path.Combine(outputFolder, "temp.jpeg");
            string thumbnail = Path.Combine(outputFolder, fileNameNoExt + "_thumbnail.jpeg");

            ffMpegConverter.GetVideoThumbnail(pFilePathOut,
                tempfile);

            int width = 120;
            int resWidth = 72;
            int resHeight = 72;

            var imgInput = new System.Drawing.Bitmap(tempfile);

            // Initialize from Settings.
            width = ImageHelper.GetImageType(imgInput.Width * imgInput.Height);
            resWidth = Settings.ThumbnailSettings.RESOLUTION_WIDTH;
            resHeight = Settings.ThumbnailSettings.RESOLUTION_HEIGHT;

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

            return 0;
        }

        private static int ProcessImage(Cleanup.ResourceHandler pResourceHandler, string pFilePath, string pOutputFolder, int pAffinity)
        {
            // INITIALIZE ARGUMENTS FOR PROCESS!!!!
            string arguments = "";
            string delimiter = "&";

            string inputFolder = Path.GetDirectoryName(pFilePath);
            string fileName = Path.GetFileName(pFilePath);

            int width = ImageHelper.GetImageTypeFromPath(pFilePath);

            arguments += "folderInput" + delimiter + inputFolder + " ";
            arguments += "folderOutput" + delimiter + pOutputFolder + " ";
            arguments += "fileName" + delimiter + fileName + " ";
            arguments += "format" + delimiter + "jpeg" + " ";
            arguments += "width" + delimiter + width + " ";
            arguments += "resolutionWidth" + delimiter + Settings.ThumbnailSettings.RESOLUTION_WIDTH + " ";
            arguments += "resolutionHeight" + delimiter + Settings.ThumbnailSettings.RESOLUTION_HEIGHT;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Settings.ImageSettings.PROCESS_NAME,
                    Arguments = arguments
                }
            };

            try
            {
                process.Start();
                process.ProcessorAffinity = (System.IntPtr)pAffinity;
                process.WaitForExit();
            }
            catch (Exception e)
            {
                string err = e.ToString();
                Console.WriteLine("Failed process image: " + err);
                return -1;
            }

            return 0;
        }

        static int Main(string[] args)
        {
            //args = new string[1] {"Input/image/cpu.png"};
            //args = new string[1] { "Input/movie1/movie.mp4" };
            //args = new string[1] { "Input/movie2/movie.mov" };

            ///////////////////////////// CHECK FOR EMPTY ARGUMENTS /////////////////////////////
            if(args.Length == 0)
            {
                Console.WriteLine("No arguments.");
                Console.ReadLine();
                return -1;
            }

            ///////////////////////////// INITIIALIZE GLOBAL SETTINGS /////////////////////////////
            GlobalTools.LogCodes.InitializeDescriptions();

            Settings.SettingsHelper.InitializeSettingsFromAppConfig();

            ///////////////////////////// CONSTRUCT FILE PATHS /////////////////////////////
            string filePath = "";
            for (int i = 0; i < args.Length; i++)
            {
                filePath += args[i] + " ";
            }

            string folderInput = Path.GetDirectoryName(filePath);
            string folderOutput = Path.Combine(folderInput, Settings.MediaSettings.OUTPUT_FOLDER);
            string fileName = Path.GetFileName(filePath);

            ///////////////////////////// CHECK IF FILES/FOLDERS EXIST /////////////////////////////
            if(File.Exists(filePath) == false)
            {
                Console.WriteLine("File does not exist.");
                Console.WriteLine("Path: " + filePath);
                Console.ReadLine();
                return -1;
            }

            ///////////////////////////// DETERMINE TYPE AND RETURN IF UNSUPPORTED /////////////////////////////
            string extension = Path.GetExtension(filePath);
            extension = extension.Replace(".", "");
            extension = extension.Replace(" ", "");

            int type = IsSupportedFormat(extension);

            if(type < 0)
            {
                return -1;
            }

            ///////////////////////////// CREATE FOLDER IF NOT EXIST /////////////////////////////
            if(Directory.Exists(folderOutput) == false)
            {
                Directory.CreateDirectory(folderOutput);
            }

            ///////////////////////////// ADD FILES TO BE DELETED WHEN FINISHED /////////////////////////////
            Cleanup.ResourceHandler handler = new Cleanup.ResourceHandler();
            handler.AddForDeletionFile(filePath);

            ///////////////////////////// HERE STARTS THE PROCESSES /////////////////////////////

            int resultCode = 0;
            int affinity = 0;

            if (type == 1)
            {
                bool isSameFormat = (extension == Settings.ConversionSettings.FORMAT);

                for (int i = 0; i < Settings.MediaSettings.CORES_VIDEO.Length; i++)
                {
                    int core = Settings.MediaSettings.CORES_VIDEO[i];
                    affinity = affinity | (1 << core - 1);
                }

                ///////////////////////////// IT IS SAME FORMAT, SKIP CONVERSION /////////////////////////////
                if(isSameFormat)
                {
                    string outputFilePath = Path.Combine(folderOutput, fileName);
                    resultCode = CompressVideo(handler, filePath, outputFilePath, affinity);
                }
                ///////////////////////////// DO CONVERSION AND COMPRESSION /////////////////////////////
                else
                {
                    resultCode = ConvertAndCompressVideo(handler, filePath, folderOutput, affinity);
                }
            }
            else if(type == 2)
            {
                for (int i = 0; i < Settings.MediaSettings.CORES_IMAGE.Length; i++)
                {
                    int core = Settings.MediaSettings.CORES_IMAGE[i];
                    affinity = affinity | (1 << core - 1);
                }

                resultCode = ProcessImage(handler, filePath, folderOutput, affinity);
            }


            ///////////////////////////// DELETE INPUT AND OTHER TEMPORARY CREATED FILES. /////////////////////////////
            //handler.SafeRelease();

            Console.WriteLine("Program ended.");
            Console.ReadLine();
            return 0;
        }
    }
}
