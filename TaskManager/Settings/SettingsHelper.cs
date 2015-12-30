using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Collections.Specialized;

namespace TaskManager.Settings
{
    public class SettingsHelper
    {
        public static void InitializeDefaultSettings()
        {
            // So the order of attributes doesn't matter and invalid options wont crash the file/system. 
            Settings.ConversionSettings.MAX_FILE_SIZE_MB = 10;

            Settings.ConversionSettings.CONVERSION_PROCESS_NAME = "NRecoConversion.exe";
            Settings.ConversionSettings.FORMAT = "mp4";
            Settings.ConversionSettings.INPUT_FOLDER = "../Resources/Input/";
            Settings.ConversionSettings.OUTPUT_FOLDER = "../Resources/Output/";
            Settings.ConversionSettings.ERROR_LOG_NAME = "error.log";

            Settings.ConversionSettings.SUPPORTED_FORMATS = new string[7];
            Settings.ConversionSettings.SUPPORTED_FORMATS[0] = "mov";
            Settings.ConversionSettings.SUPPORTED_FORMATS[1] = "m4v";
            Settings.ConversionSettings.SUPPORTED_FORMATS[2] = "mpeg";
            Settings.ConversionSettings.SUPPORTED_FORMATS[3] = "avi";
            Settings.ConversionSettings.SUPPORTED_FORMATS[4] = "mp4";
            Settings.ConversionSettings.SUPPORTED_FORMATS[5] = "flv";
            Settings.ConversionSettings.SUPPORTED_FORMATS[6] = "wmv";

            Settings.ImageSettings.PROCESS_NAME = "ImageProcessing.exe";
            Settings.ImageSettings.SUPPORTED_FORMATS = new string[4];
            Settings.ImageSettings.SUPPORTED_FORMATS[0] = "jpeg";
            Settings.ImageSettings.SUPPORTED_FORMATS[1] = "jpg";
            Settings.ImageSettings.SUPPORTED_FORMATS[2] = "gif";
            Settings.ImageSettings.SUPPORTED_FORMATS[3] = "png";

            // If something is wrong with other settings, not much is know for current system and thus this will
            // be hardcoded core. Alternatevly it could use Environment to get number of cores and
            // assign them, but how safe that is, is unknown for now...
            Settings.SchedulerSettings.CPU_CORES = new int[1];
            Settings.SchedulerSettings.CPU_CORES[0] = 1;

            Settings.CompressionSettings.COMPRESSION_PROCESS_NAME = "ffmpeg.exe";
            Settings.CompressionSettings.ERROR_LOG_NAME = "error.log";
        }

        public static void InitializeSettingsFromAppConfig()
        {
            // Start reading from App.config file.

            // Settings for SchedulerManager
            // ==========================================================================================================
            NameValueCollection scheduler = ConfigurationManager.GetSection("settings/scheduler") as NameValueCollection;

            if (scheduler == null)
            {
                Console.WriteLine("Settings for scheduler are not initialized! Using default settings.");
            }
            else
            {
                List<int> cores = new List<int>();
                foreach (string key in scheduler.AllKeys)
                {
                    string vl = scheduler.Get(key);
                    if (key.Contains("Core"))
                    {
                        int c = GlobalTools.Conversion.StringToInt32(vl, -1);
                        if (c > -1)
                        {
                            cores.Add(c);
                        }
                        else
                        {
                            Console.WriteLine("Corrupted core from settings, key: " + key + " Value: " + vl);
                        }
                    }
                }

                Settings.SchedulerSettings.CPU_CORES = cores.ToArray();
                cores.Clear();
            }
            // Settings for Conversion process
            // ==========================================================================================================
            NameValueCollection conversion = ConfigurationManager.GetSection("settings/conversion") as NameValueCollection;

            if (conversion == null)
            {
                Console.WriteLine("Settings for conversion are not initialized! Using default settings.");
            }
            else
            {
                Settings.ConversionSettings.MAX_FILE_SIZE_MB = GlobalTools.Conversion.StringToInt32(conversion.Get("MaxFileSize"), 0);
                Settings.ConversionSettings.CONVERSION_PROCESS_NAME = conversion.Get("ProcessName");
                Settings.ConversionSettings.INPUT_FOLDER = conversion.Get("InputFolder");
                Settings.ConversionSettings.OUTPUT_FOLDER = conversion.Get("OutputFolder");
                Settings.ConversionSettings.ERROR_LOG_NAME = conversion.Get("ErrorLogName");
                Settings.ConversionSettings.FORMAT = conversion.Get("Format");

                List<string> formats = new List<string>();

                foreach(string key in conversion.AllKeys)
                {
                    if(key.Contains("SupportedFormat"))
                    {
                        formats.Add(conversion[key]);
                    }
                }

                Settings.ConversionSettings.SUPPORTED_FORMATS = formats.ToArray();
                formats.Clear();
            }


            // Settings for Compression process
            // ==========================================================================================================
            NameValueCollection compression = ConfigurationManager.GetSection("settings/compression") as NameValueCollection;

            if (compression == null)
            {
                Console.WriteLine("Settings for compression are not initialized! Using default settings.");
            }
            else
            {
                Settings.CompressionSettings.COMPRESSION_PROCESS_NAME = compression.Get("ProcessName");
                Settings.CompressionSettings.ERROR_LOG_NAME = compression.Get("ErrorLogName");
            }

            // Settings for Image processing
            // ==========================================================================================================
            NameValueCollection imageProcessing = ConfigurationManager.GetSection("settings/imageProcessing") as NameValueCollection;
            NameValueCollection thumbnail = ConfigurationManager.GetSection("settings/thumbnail") as NameValueCollection;

            if (imageProcessing == null)
            {
                Console.WriteLine("Settings for image are not initialized! Using default settings.");
            }
            else if(thumbnail == null)
            {
                Console.WriteLine("Settings for thumbnail are not initialized! Using default settings.");
            }
            else
            {
                Settings.ImageSettings.PROCESS_NAME = imageProcessing.Get("ProcessName");
                Settings.ImageSettings.INPUT_FOLDER = imageProcessing.Get("InputFolder");
                Settings.ImageSettings.OUTPUT_FOLDER = imageProcessing.Get("OutputFolder");

                Settings.ImageSettings.THUMBNAIL_RESOLUTION_WIDTH = GlobalTools.Conversion.StringToInt32(thumbnail.Get("ResolutionWidth"), 72);
                Settings.ImageSettings.THUMBNAIL_RESOLUTION_HEIGHT = GlobalTools.Conversion.StringToInt32(thumbnail.Get("ResolutionHeight"), 72);

                List<string> formats = new List<string>();

                foreach(string key in imageProcessing.AllKeys)
                {
                    if(key.Contains("SupportedFormat"))
                    {
                        formats.Add(imageProcessing.Get(key));
                    }
                }

                Settings.ImageSettings.SUPPORTED_FORMATS = formats.ToArray();
                formats.Clear();
            }
        }
    }
}
