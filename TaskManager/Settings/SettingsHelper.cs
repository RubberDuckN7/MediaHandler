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
        public static void InitializeSettingsFromAppConfig()
        {
            // Start reading from App.config file.

            // Settings for SchedulerManager
            // ==========================================================================================================
            NameValueCollection media = ConfigurationManager.GetSection("settings/media") as NameValueCollection;

            if (media == null)
            {
                Console.WriteLine("Media settings not found!");
            }
            else
            {
                List<int> videoCores = new List<int>();
                List<int> imageCores = new List<int>();

                foreach (string key in media.AllKeys)
                {
                    string vl = media.Get(key);
                    if (key.Contains("VideoCore"))
                    {
                        int c = GlobalTools.Conversion.StringToInt32(vl, -1);
                        if (c > -1)
                        {
                            videoCores.Add(c);
                        }
                        else
                        {
                            Console.WriteLine("Corrupted core from settings, key: " + key + " Value: " + vl);
                        }
                    }
                    else if(key.Contains("ImageCore"))
                    {
                        int c = GlobalTools.Conversion.StringToInt32(vl, -1);
                        if (c > -1)
                        {
                            imageCores.Add(c);
                        }
                        else
                        {
                            Console.WriteLine("Corrupted core from settings, key: " + key + " Value: " + vl);
                        }
                    }
                    else if (key == "OutputFolder")
                    {
                        Settings.MediaSettings.OUTPUT_FOLDER = vl;
                    }
                }

                Settings.MediaSettings.CORES_VIDEO = videoCores.ToArray();
                Settings.MediaSettings.CORES_IMAGE = imageCores.ToArray();

                videoCores.Clear();
                imageCores.Clear();
            }

            // Settings for Thumbnail process
            // ==========================================================================================================
            NameValueCollection thumbnail = ConfigurationManager.GetSection("settings/thumbnail") as NameValueCollection;

            if (thumbnail == null)
            {
                Console.WriteLine("Settings for thumbnail are not initialized! Using default settings.");
            }
            else
            {
                Settings.ThumbnailSettings.RESOLUTION_WIDTH = GlobalTools.Conversion.StringToInt32(thumbnail.Get("ResolutionWidth"), 72);
                Settings.ThumbnailSettings.RESOLUTION_HEIGHT = GlobalTools.Conversion.StringToInt32(thumbnail.Get("ResolutionHeight"), 72);
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
                Settings.ConversionSettings.CONVERSION_PROCESS_NAME = conversion.Get("ProcessName");
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
            }

            // Settings for Image processing
            // ==========================================================================================================
            NameValueCollection imageProcessing = ConfigurationManager.GetSection("settings/image") as NameValueCollection;

            if (imageProcessing == null)
            {
                Console.WriteLine("Settings for image are not initialized! Using default settings.");
            }
            else
            {
                Settings.ImageSettings.PROCESS_NAME = imageProcessing.Get("ProcessName");

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
