using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Settings
{
    public class ImageSettings
    {
        public static string PROCESS_NAME = "";
        public static string INPUT_FOLDER = "";
        public static string OUTPUT_FOLDER = "";

        public static string[] SUPPORTED_FORMATS = null;

        public static int THUMBNAIL_RESOLUTION_WIDTH = 72;
        public static int THUMBNAIL_RESOLUTION_HEIGHT = 72;

        public static long IMAGE_PROFILE_PIC_IMAGE_EXPECTED_SIZE = 100 * 1024;
        public static long IMAGE_SCHOOL_IMAGE_EXPECTED_SIZE = 150 * 1024;
        public static long IMAGE_BLOGPOST_IMAGE_EXPECTED_SIZE = 400 * 1024;
        public static long IMAGE_REFLECTION_IMAGE_EXPECTED_SIZE = 400 * 1024;
              
        public static int IMAGE_PROFILE_PIC_THUMB_HEIGHT_WIDTH = 120;
        public static int IMAGE_SCHOOL_THUMB_HEIGHT_WIDTH = 220;
        public static int IMAGE_BLOGPOST_THUMB_HEIGHT_WIDTH = 460;
    }
}
