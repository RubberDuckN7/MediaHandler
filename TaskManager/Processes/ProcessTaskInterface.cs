using System;
using System.Diagnostics;

namespace TaskManager.Processes
{
    public abstract class ProcessTaskInterface : TaskInterface
    {
        // Process priority? 
        private ProcessPriorityClass mProcessPriority = ProcessPriorityClass.Normal;
        private string mProcessArguments = "";
        private string mProcessName = "";

        private bool mWaitForExit = true;

        protected abstract void Started();
        protected abstract void Finished(Process pProcess, bool pSuccess, Exception pException = null);

        public override int Run()
        {
            Started();

            // Just create and initialize a process. 
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = mProcessName,
                    Arguments = mProcessArguments
                }
            };

            Console.WriteLine("Executing process: " + mProcessName);
            Console.WriteLine("Process arguments: " + mProcessArguments);

            Exception exception = null;

            // Try running it.
            bool result = true;
            try
            {
                process.Start();
                process.PriorityClass = mProcessPriority;
                process.ProcessorAffinity = (System.IntPtr)CPUMask;

                if (mWaitForExit)
                {
                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                exception = e;
                string err = e.ToString();
                Console.WriteLine("PROCESS TASK: Process task execution exception: " + err);
                result = false;
            }

            Finished(process, result, exception);

            if(result == false)
            {
                return -1;
            }

            return 0;
        }

        public string ProcessArguments
        {
            get { return mProcessArguments; }
            set { mProcessArguments = value; }
        }

        public string ProcessName
        {
            get { return mProcessName; }
            set { mProcessName = value; }
        }

        public bool WaitForExit
        {
            get { return mWaitForExit; }
            set { mWaitForExit = value; }
        }
    }
}
