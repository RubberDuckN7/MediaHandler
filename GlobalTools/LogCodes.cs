using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * So to centeralize all return codes be that error or just logging all should be used from here.
 * In future all explanations could be also extracted through "tools" from this project, example use
 * return code as an index for getting information in global string array about error code.
 * 
 * This is to avoid sending back from external or creating lots of file that may not be cleaned up in
 * effort to send a Exception message and so on.
 */ 

namespace GlobalTools
{
    /*
     * TODO: 
     *  - Add more specific error codes, as the system grows (hopefully).
     *  - Write a more selfexplanatory description for each error code.
     */

    // It must be public due to access from outer namespaces.
    public class LogCodes
    {
        public enum RETURN
        {
            OK = 0,
            CORRUPT_ARGUMENT,
            MISSING_ARGUMENT,
            CONVERT_EXCEPTION,
            EMPTY_PROCESS_NAME,
            FILE_NOT_FOUND,
            NO_SUCH_DIRECTORY,
            ABORTED,
            UNSUPPORTED_FORMAT,
            NULL_EXCEPTION,
        }

        // Between the programs only a return code is passed and may not be selfexplanatory.
        // That's why there will be a description for each return code. If in future more are needed
        // just add more to enum and increase NR_OF_CODES to amount of enum RETURN members and fill in description.
        private static int NR_OF_CODES = 0;
        private static string[] DESCRIPTIONS = null; 

        public static void InitializeDescriptions()
        {
            NR_OF_CODES = 10;
            DESCRIPTIONS = new string[NR_OF_CODES];

            DESCRIPTIONS[0] = "OK";
            DESCRIPTIONS[1] = "Passed invalid arguments.";
            DESCRIPTIONS[2] = "Parts of arguments are missing.";
            DESCRIPTIONS[3] = "Converting process failed with exception.";
            DESCRIPTIONS[4] = "Process name is empty.";
            DESCRIPTIONS[5] = "File not found.";
            DESCRIPTIONS[6] = "No such directory.";
            DESCRIPTIONS[7] = "Aborted.";
            DESCRIPTIONS[8] = "Unsupported format.";
            DESCRIPTIONS[9] = "Null exception.";
        }

        // It is error proof and should work correctly.
        public static string GetDescription(int pCode)
        {
            return GetDescription((RETURN)pCode);
        }

        public static string GetDescription(RETURN pCode)
        {
            if(NR_OF_CODES == 0)
            {
                return "INVALID CODE: Descriptions are not initialized.";
            }

            int index = (int)pCode;
            if(index >= NR_OF_CODES || index < 0)
            {
                return "INVALID CODE: Index is out of range.";
            }
            return DESCRIPTIONS[index];
        }
    }
}
