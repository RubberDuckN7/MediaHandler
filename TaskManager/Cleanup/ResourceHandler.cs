using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

/*
 * So the one biggest "bug" or a problem that has arised is a cleanup process and create folders
 * if they don't exist or access them in a safe way. Writing hundreds och checks doesn't cut it. 
 * 
 * There are few functions for this class to do (not actual C# function). 
 *  - Access directory and look for file
 *  - Create directory
 *  - Move file
 *  - Store all directories and files in two separate list for cleanup
 *  
 * So all things and information about cleaning up for ProcessTasks can be passed before executed. 
 * Especially useful is due to safe release and wont crash if for some reason a folder did NOT exist and
 * tried to release it.
 * 
 */ 

namespace TaskManager.Cleanup
{
    public class ResourceHandler
    {
        private List<string> mFolders = null;
        private List<string> mFiles = null;

        public ResourceHandler()
        {
            mFolders = new List<string>();
            mFiles = new List<string>();
        }

        public void AddForDeletionFolder(string pFolderPath)
        {
            mFolders.Add(pFolderPath);
        }

        public void AddForDeletionFile(string pFilePath)
        {
            mFiles.Add(pFilePath);
        }

        public void SafeRelease()
        {
            // Delete specifically all files first. For the lulz. :)
            for(int i = 0; i < mFiles.Count; i++)
            {
                string path = mFiles[i];

                try
                {
                    File.Delete(path);
                }
                catch(IOException e)
                {
                    Console.WriteLine(e.ToString() + "\n");
                }
                catch(ArgumentException e)
                {
                    Console.WriteLine(e.ToString() + "\n");
                }
                catch(UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.ToString() + "\n");
                }

            }

            for(int i = 0; i < mFolders.Count; i++)
            {
                string path = mFolders[i];

                try
                {
                    // So the reason for recursively is that at this point we want to delete everything given
                    // in this folder. If there should be something that should not be deleted, something went wrong
                    // completely long way from here. 
                    Directory.Delete(path, true);

                }// What kind of promise in word "Safe" would it be if all possible exceptions wouldn't be catched? 
                catch(IOException e)
                {
                    Console.WriteLine(e.ToString() + "\n");
                }
                catch(UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.ToString() + "\n");
                }
                catch(ArgumentException e)
                {
                    Console.WriteLine(e.ToString() + "\n");
                }
            }
        }

        public static bool FileExist(string pFilePath)
        {
            return File.Exists(pFilePath);
        }

        public static bool FolderExist(string pFolderPath)
        {
            return Directory.Exists(pFolderPath);
        }

        // Static methods!
        public static bool MoveFile(string pPathFrom, string pPathFileTo)
        {
            if(File.Exists(pPathFrom) == false)
            {
                return false;
            }
            // If there is a file already you will get IOException so ... better be already
            // here.
            if(File.Exists(pPathFileTo) != false)
            {
                return false;
            }

            try
            {
                File.Move(pPathFrom, pPathFileTo);
                return true;
            }
            catch(IOException e)
            {
                Console.WriteLine(e.ToString() + "\n");
                return false;
            }
            catch(UnauthorizedAccessException e)
            {
                Console.WriteLine(e.ToString() + "\n");
                return false;
            }
        }

        public static bool CreateDirectory(string pFolderPath)
        {
            if (Directory.Exists(pFolderPath))
            {
                return true;
            }

            try
            {
                Directory.CreateDirectory(pFolderPath);
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString() + "\n");
                return false;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.ToString() + "\n");
                return false;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.ToString() + "\n");
                return false;
            }
        }
    }
}
