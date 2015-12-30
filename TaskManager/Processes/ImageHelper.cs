using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.IO;

namespace TaskManager.Processes
{
    public class ImageHelper
    {
        public static int GetImageType(int pSize)
        {
            int size = pSize;
            int type = Settings.ImageSettings.IMAGE_PROFILE_PIC_THUMB_HEIGHT_WIDTH;

            if(size == Settings.ImageSettings.IMAGE_PROFILE_PIC_IMAGE_EXPECTED_SIZE)
            {
                type = Settings.ImageSettings.IMAGE_PROFILE_PIC_THUMB_HEIGHT_WIDTH;
            }
            else if(size == Settings.ImageSettings.IMAGE_SCHOOL_IMAGE_EXPECTED_SIZE)
            {
                type = Settings.ImageSettings.IMAGE_SCHOOL_THUMB_HEIGHT_WIDTH;
            }
            else if (size == Settings.ImageSettings.IMAGE_BLOGPOST_IMAGE_EXPECTED_SIZE)
            {
                type = Settings.ImageSettings.IMAGE_BLOGPOST_THUMB_HEIGHT_WIDTH;
            }

            return type;
        }

        public static int GetImageTypeFromPath(string pFilePath)
        {
            var image = new System.Drawing.Bitmap(pFilePath);

            int size = image.Width * image.Height;

            int type = Settings.ImageSettings.IMAGE_PROFILE_PIC_THUMB_HEIGHT_WIDTH;

            if (size == Settings.ImageSettings.IMAGE_PROFILE_PIC_IMAGE_EXPECTED_SIZE)
            {
                type = Settings.ImageSettings.IMAGE_PROFILE_PIC_THUMB_HEIGHT_WIDTH;
            }
            else if (size == Settings.ImageSettings.IMAGE_SCHOOL_IMAGE_EXPECTED_SIZE)
            {
                type = Settings.ImageSettings.IMAGE_SCHOOL_THUMB_HEIGHT_WIDTH;
            }
            else if (size == Settings.ImageSettings.IMAGE_BLOGPOST_IMAGE_EXPECTED_SIZE)
            {
                type = Settings.ImageSettings.IMAGE_BLOGPOST_THUMB_HEIGHT_WIDTH;
            }

            image.Dispose();

            return type;
        }
    }
}
