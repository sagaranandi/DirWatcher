using System;

namespace DirWatcher.DataModel
{
    public class TaskRun
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double TotalRuntimeInSeconds { get; private set; }
        public List<string> FilesAdded { get; set; }
        public List<string> FilesDeleted { get; set; }
        public List<string> FilesModified { get; set; }
        public int MagicStringCount { get; set; }
        public string Status { get; set; }

        public TaskRun()
        {
            TotalRuntimeInSeconds = 0; // Initialize to 0
        }
    }

  
}
