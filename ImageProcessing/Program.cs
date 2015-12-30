using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlobalTools;
using System.IO;
using System.Drawing;

/*
 * This program is expecting arguments that are going to be passed to it. 
 * The format is this "key:value", no spaces. 
 * 
 * Return codes:
 *  - Value or key of an argument is missing: CORRUPT_ARGUMENT
 *  - Files not found: FILE_NOT_FOUND
 *  - Folder is missing: NO_SUCH_DIRECTORY
 *  - Argument is missing: MISSING_ARGUMENT
 *  
 * Anything that has problem with an argument is due to some bug in a program. 
 * If there is missing file/folder it may be for some other reason outside the system. 
 * One common problem is ffmpeg.exe is not closed and is running and holding a file (due to unknown reasons)
 * and the Scheduler is closed down and opened again. 
 */

namespace ImageProcessing
{
    class Program
    {
        /*
         * TODO: 
         *  - Add more formats that maybe will be used.
         *  - If there is another place for Image helper functions, move it to a better place 
         *    (For example, global tools)
         */

        // This is just as ImageHelpers.GetFormat, with a switch in it.
        static System.Drawing.Imaging.ImageFormat GetFormat(string pStrFormat)
        {
            // If there are more formats needed, just add here.
            switch(pStrFormat)
            {
                case "png": return System.Drawing.Imaging.ImageFormat.Png;
                case "jpeg": return System.Drawing.Imaging.ImageFormat.Jpeg;
                case "bmp": return System.Drawing.Imaging.ImageFormat.Bmp;
                default: return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
        }

        static int Main(string[] args)
        {
            // Read all arguments into a dictionary to read them as key:values.
            Dictionary<string, string> values = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                string[] spl = args[i].Split('&');
                if (spl == null || spl.Length < 2)
                {
                    return (int)LogCodes.RETURN.CORRUPT_ARGUMENT;
                }
                else
                {
                    // No duplicates. 
                    // Fill otherwise in with whatever option it is needed.
                    if (values.ContainsKey(spl[0]) == false)
                    {
                        values.Add(spl[0], spl[1]);
                    }
                }
            }

            Console.WriteLine("Created dictionary. Looking for arguments.");

            if (values.ContainsKey("format") == false || values["format"] == "")
            {
                Console.WriteLine("No format.");
                Console.ReadLine();
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }
            if (values.ContainsKey("fileName") == false || values["fileName"] == "")
            {
                Console.WriteLine("No filename.");
                Console.ReadLine();
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }
            if (values.ContainsKey("folderOutput") == false || values["folderOutput"] == "")
            {
                Console.WriteLine("No folderOutput.");
                Console.ReadLine();
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }
            if (values.ContainsKey("folderInput") == false || values["folderInput"] == "")
            {
                Console.WriteLine("No folderInput.");
                Console.ReadLine();
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }
            
            if (File.Exists(Path.Combine(values["folderInput"], values["fileName"])) == false)
            {
                Console.WriteLine("File does not exist.");
                Console.ReadLine();
                return (int)LogCodes.RETURN.FILE_NOT_FOUND;
            }
            if (Directory.Exists(values["folderOutput"]) == false)
            {
                Console.WriteLine("No output folder.");
                Console.ReadLine();
                return (int)LogCodes.RETURN.NO_SUCH_DIRECTORY;
            }

            if (values.ContainsKey("width") == false || values["width"] == "")
            {
                Console.WriteLine("No no width.");
                Console.ReadLine();
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }

            if (values.ContainsKey("resolutionWidth") == false || values["resolutionWidth"] == "")
            {
                Console.WriteLine("No resolution width.");
                Console.ReadLine();
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }

            if (values.ContainsKey("resolutionHeight") == false || values["resolutionHeight"] == "")
            {
                Console.WriteLine("No resolution height.");
                Console.ReadLine();
                return (int)LogCodes.RETURN.MISSING_ARGUMENT;
            }

            /*
             * TODO: 
             *  - Check for invalid values and report it back.
             *  - Make it more robust by looking up files and directories.
             *    ( Creation of temporary file and other things should have more if statements
             *      for that purpose.)
             */

            Console.WriteLine("All arguments are present, initializing...");
            
            // Some default arguments.
            int width = GlobalTools.Conversion.StringToInt32(values["width"], 40);
            int resWidth = GlobalTools.Conversion.StringToInt32(values["resolutionWidth"], 40);
            int resHeight = GlobalTools.Conversion.StringToInt32(values["resolutionHeight"], 40);

            string inputFolder = values["folderInput"];
            string outputFolder = values["folderOutput"];
            string fileName = values["fileName"];
            string format = values["format"];

            string filePath = Path.Combine(inputFolder, fileName);

            Console.WriteLine("Values of arguments:");
            Console.WriteLine(" - Input folder: " + inputFolder);
            Console.WriteLine(" - Output folder: " + outputFolder);
            Console.WriteLine(" - Filename: " + fileName);
            Console.WriteLine(" - Format: " + format);
            Console.WriteLine(" - Filepath: " + filePath);
            Console.WriteLine(" - Width: " + width);
            Console.WriteLine(" - Resolution width: " + resWidth);
            Console.WriteLine(" - Resolution height: " + resHeight);

            Console.WriteLine("Executing processing...");

            // From here on now this is the code I've copy-pasted and modified a little, 
            // should be familiar.
            // Get rotation and rotate image.
            int rotationValue = 0;
            Image imageBitmap = Image.FromFile(filePath);
            if (imageBitmap.PropertyIdList.Contains(0x0112))
            {
                rotationValue = imageBitmap.GetPropertyItem(0x0112).Value[0];
            }

            if (rotationValue > 0)
            {
                switch (rotationValue)
                {
                    case 1: // no rotation needed
                        break;
                    case 8: // rotate 270
                        imageBitmap.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                        break;

                    case 3: // rotate 180
                        imageBitmap.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                        break;

                    case 6: // rotate 90
                        imageBitmap.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                        break;
                }
            }

            // Create a folder with name as the input file.
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
            string namedFolder = outputFolder; // Path.Combine(outputFolder, fileNameNoExtension);

            if(Directory.Exists(namedFolder) == false)
            {
                Directory.CreateDirectory(namedFolder);
            }

            System.Drawing.Imaging.EncoderParameters parameters = new System.Drawing.Imaging.EncoderParameters();

            string fileOutput = Path.Combine(namedFolder, fileNameNoExtension) + "." + format;

            /*
             * TODO: 
             *  - Return a better error code to explain a more specific problem if somethings occurs. 
             */

            try
            {
                imageBitmap.Save(fileOutput, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                return (int)LogCodes.RETURN.NULL_EXCEPTION;
            }
            catch(System.Runtime.InteropServices.ExternalException e)
            {
                // Weeeeiiiiirdddd ;D
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                return (int)LogCodes.RETURN.CONVERT_EXCEPTION;
            }

            Console.WriteLine("Converting file success!");
            Console.WriteLine("Creating thumbnail...");

            // This code was copy pasted from on how to create a thumbnail from image.
            string thumbnail = Path.GetFileNameWithoutExtension(fileOutput);
            thumbnail = Path.Combine(namedFolder, thumbnail) + "_thumbnail." + format;

            var imgInput = new Bitmap(fileOutput);
            double y = imgInput.Height;
            double x = imgInput.Width;

            var imgOutput = new Bitmap(width, (int)(y * width / x));
            imgOutput.SetResolution(resWidth, resHeight); 

            Graphics graphics = Graphics.FromImage(imgOutput);
            graphics.Clear(Color.White);
            graphics.DrawImage(imageBitmap, new Rectangle(0, 0, width, (int)(y * width / x)),
            new Rectangle(0, 0, (int)x, (int)y), GraphicsUnit.Pixel);

            imgOutput.Save(thumbnail, GetFormat(format)); 

            Console.WriteLine("Image converted and thumbnail created!");

            return (int)LogCodes.RETURN.OK;
        }
    }
}
