namespace TaskManager.Processes
{
    public abstract class TaskInterface
    {
        private int mCPUMask;

        public abstract int Run();

        public int CPUMask
        {
            get { return mCPUMask; }
            set { mCPUMask = value; }
        }
    }
}
