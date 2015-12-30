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
                    Console.ReadLine();
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
                    Console.ReadLine();
                    return 2;
                }
            }

            Console.WriteLine("Type unsupported.");
            Console.ReadLine();
            return -1;
        }

        private static int ConvertAndCompressVideo(string pFilePath, string pOutputFolder)
        {
            return 0;
        }

        private static int CompressVideo(string pFilePath, string pOutputFolder)
        {
            return 0;
        }

        private static int ProcessImage(string pFilePath, string pOutputFolder)
        {

            return 0;
        }

        static int Main(string[] args)
        {
            //args = new string[1] {"C:/Users/Spyro/Documents/Gith rep/TaskManager/TaskManager/bin/Debug/Input/image/cpu.png"};

            ///////////////////////////// CHECK FOR EMPTY ARGUMENTS /////////////////////////////
            if(args.Length == 0)
            {
                Console.WriteLine("No arguments.");
                Console.ReadLine();
                return -1;
            }

            ///////////////////////////// CONSTRUCT FILE PATHS /////////////////////////////
            string filePath = "";
            for (int i = 0; i < args.Length; i++)
            {
                filePath += args[i] + " ";
            }

            string folderInput = Path.GetDirectoryName(filePath);
            string folderOutput = Path.Combine(folderInput, "Output");
            string fileName = Path.GetFileName(filePath);

            ///////////////////////////// INITIIALIZE GLOBAL SETTINGS /////////////////////////////
            GlobalTools.LogCodes.InitializeDescriptions();
            Settings.SettingsHelper.InitializeDefaultSettings();
            Settings.SettingsHelper.InitializeSettingsFromAppConfig();

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
            //handler.AddForDeletionFile(filePath);

            Processes.TaskInterface task = null;

            int affinity = 0;

            ///////////////////////////// CREATE PROCESS DEPENDING ON TYPE /////////////////////////////
            if(type == 1)
            {
                Console.WriteLine("Actual format: " + extension + " format needed: " + Settings.ConversionSettings.FORMAT);

                int core = 1;

                for (int i = 0; i < Settings.SchedulerSettings.CPU_CORES.Length; i++)
                {
                    core = Settings.SchedulerSettings.CPU_CORES[i];
                    affinity = affinity | (1 << core - 1);
                }

                ///////////////////////////// VIDEO TYPE SKIP CONVERSION /////////////////////////////
                if (extension == (Settings.ConversionSettings.FORMAT))
                {
                    string mainFolder = folderOutput; // Path.Combine(folderOutput, Path.GetFileNameWithoutExtension(fileName));
                    string convertFolder = folderInput;
                    string convertedFileName = fileName;

                    Console.WriteLine("Compression process is created.");

                    task = new Processes.CompressionProcess(handler, mainFolder, convertFolder, convertedFileName);

                    task.CPUMask = affinity;
                    task.Run();
                }
                ///////////////////////////// VIDEO DO CONVERSION /////////////////////////////
                else
                {
                    Console.WriteLine("Conversion process is created.");

                    task = new Processes.ConversionProcess(handler, folderInput, folderOutput, fileName);

                    task.CPUMask = affinity;
                    task.Run();

                    Console.WriteLine("Compression process is created.");

                    string convertedFileName = Path.GetFileNameWithoutExtension(fileName) + "." + Settings.ConversionSettings.FORMAT;
                    string convertedFolder = Path.Combine(folderOutput, "Converted");

                    //string outputFolder = Path.Combine("")

                    string mainFolder = folderOutput; // Path.Combine(folderOutput, Path.GetFileNameWithoutExtension(fileName));
                    task = new Processes.CompressionProcess(handler, mainFolder, convertedFolder, convertedFileName);
                    //
                    task.CPUMask = affinity;
                    task.Run();
                }    
            }
            else if(type == 2)
            {
                int width = Processes.ImageHelper.GetImageTypeFromPath(filePath);
                task = new Processes.ImageProcess(handler, folderInput, folderOutput, fileName, width);

                int core = 1;
                affinity = affinity | (1 << core - 1);

                task.CPUMask = affinity;
                task.Run();
            }


            // if(task == null)
            // {
            //     Console.WriteLine("Failed to create process.");
            //     Console.ReadLine();
            //     return -1;
            // }
            // 
            // Console.WriteLine("Press enter to execute process.");
            // 
            // //Processes.ProcessTaskInterface proc = (Processes.ProcessTaskInterface)task;
            // 
            // //Console.WriteLine("Name: " + proc.ProcessName);
            // //Console.WriteLine("Arguments: " + proc.ProcessArguments);
            // Console.ReadLine();


            
            
            


            // if (type == 1) // Video
            // {
            //     string format = Settings.ConversionSettings.FORMAT;
            // 
            //     string folderOutput = Settings.ConversionSettings.OUTPUT_FOLDER;
            // 
            //     Cleanup.ResourceHandler handler = new Cleanup.ResourceHandler();
            //     handler.AddForDeletionFile(Path.Combine(folderInput, fileName));
            // 
            //     Console.WriteLine("=======================================================================");
            // 
            //     if (ext == ("." + format))
            //     {
            //         string mainFolder = Path.Combine(folderOutput, Path.GetFileNameWithoutExtension(fileName));
            //         string convertFolder = folderInput;
            //         string convertedFileName = fileName;
            // 
            //         Console.WriteLine("SCANNER: Same format, adding compression task - " + fileName);
            // 
            //         int workload = Workloads.Calculate(info);
            // 
            //         task = new MediaProcesses.CompressionProcess(handler, workload, mainFolder, convertFolder, convertedFileName);
            //     }
            //     else
            //     {
            //         Console.WriteLine("SCANNER: Adding for conversion file: " + fileName);
            // 
            //         int workload = Workloads.Calculate(info);
            // 
            //         task = new MediaProcesses.ConversionProcess(handler, workload, folderInput, folderOutput, fileName);
            //     }
            // }
            // else if (supported == 2) // Image
            // {
            //     
            //     
            // 
            //     
            //     
            // 
            //     
            // 
            //     
            // 
            //     
            //     
            // 
            //     
            // }
            // 

            







            Console.WriteLine("Program ended.");
            Console.ReadLine();

            

























            return 0;

            // /*
            //  * Filnamn, kärnor info. , tider, filinfo, workload, size, updated size, errors,
            //  * programstart time, start - finished time
            //  */
            // 
            // //return;
            // 
            // // So if there is unforseen event that settings has not been initialized, have at least some
            // // default values.
            // 
            // 
            // // Read all settings here.
            // // ----------------------------------------------------------------------------------------------------
            // 
            // 
            // // ----------------------------------------------------------------------------------------------------
            // string output = Settings.ConversionSettings.OUTPUT_FOLDER; // "../Resources/Output/";
            // 
            // // Alright, I honestly don't know exactly why but it seems that 
            // // this may not execute fast enough and continues without waiting for the 
            // // finished removal/creation before proceding and thus it will give error
            // // in process task that such folder does not exist.
            // // Addint sleep time seems to "fix" it.
            // if (Directory.Exists(output) == true)
            // {
            //     Directory.Delete(output, true);
            //     System.Threading.Thread.Sleep(200);
            // }
            // Directory.CreateDirectory(output);
            // System.Threading.Thread.Sleep(200);
            // 
            // string input = "../Resources/Input/";
            // string restore = "../Resources/Input/Restore/";
            // 
            // if(Directory.Exists(input) == false)
            // {
            //     Directory.CreateDirectory(input);
            // }
            // if(Directory.Exists(restore) == false)
            // {
            //     Directory.CreateDirectory(restore);
            // }
            // System.Threading.Thread.Sleep(200);
            // 
            // // Fill Input folder
            // // ----------------------------------------------------------------------------------------------------
            // string[] restoreFiles = Directory.GetFiles(restore);
            // Console.WriteLine("Restoring files: " + restoreFiles.Length);
            // for (int i = 0; i < restoreFiles.Length; i++)
            // {
            //     string file = Path.GetFileName(restoreFiles[i]);
            //     file = file.Replace(" ", "");
            //     string dest = Path.Combine(input, file);
            //     if (File.Exists(dest) == false)
            //     {
            //         File.Copy(restoreFiles[i], dest);
            //     }
            // }
            // 
            // Console.WriteLine("All files are moved to Input folder.");
            // 
            // // Init
            // // ----------------------------------------------------------------------------------------------------
            // // Make sure all configs are loaded BEFORE any work is started.
            // GlobalScheduleHandler.Initialize("../Resources/Config/conf_scheduler.ini",
            //                                  "../Resources/Config/conf_conversion.ini",
            //                                  "../Resources/Config/conf_compression.ini",
            //                                  "../Resources/Config/conf_image.ini");
            // 
            // // Start scanner
            // // ----------------------------------------------------------------------------------------------------
            // MediaProcesses.ScanningFolder scan = new MediaProcesses.ScanningFolder();
            // System.Threading.Thread scanningThread = new System.Threading.Thread(new System.Threading.ThreadStart(scan.Run));
            // scanningThread.Start();
            // 
            // // just this once, wait for all tasks to be added.
            // scanningThread.Join();
            // 
            // // Start
            // // ----------------------------------------------------------------------------------------------------
            // 
            // Console.WriteLine("Starting TaskManager, it will loop forever until closed.");
            // 
            // // This maybe will be changed a little into a better way to run on a separate thread
            // // and this main thread will wait for a certain input to "close" properly. 
            // // For example a while(true) ReadLine("Q to exit"); or something.
            // GlobalScheduleHandler.ProcessTasks();
            // 
            // // End 
            // // ----------------------------------------------------------------------------------------------------
            // Console.WriteLine("Program ended. Press enter.");
            // Console.ReadLine();
        }
    }
}
