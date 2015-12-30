using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Settings
{
    public class SchedulerSettings
    {
        public static int[] CPU_CORES = null;
        public static int MAX_FILE_SIZE_MB = 650;
        public static int MAX_CORES_PER_TASK = 0;
        public static int WORKLOAD_LARGE = 10;
        public static int WORKLOAD_MEDIUM = 5;
        public static int WORKLOAD_SMALL = 1;
    }
}
