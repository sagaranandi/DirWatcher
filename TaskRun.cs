using System;

namespace DirWatcher
{
    public class TaskRun
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> FilesAdded { get; set; }
        public List<string> FilesDeleted { get; set; }
        public int MagicStringCount { get; set; }
        public string Status { get; set; }
    }
}
