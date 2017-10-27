using Jil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace POSE.CORE
{
    public class Project
    {
        public static Lazy<Project> LazyProject { get; private set; }

        public static Project Singular { get { return LazyProject.Value; } }

        public Dictionary<string, DATA.Project> Projects { get; private set; }

        private string DataFile { get { return ".projects"; } }

        public string CurrentProjectId { get; private set; }

        public long SessionTime { get; private set; }

        public DATA.Project Opening
        {
            get
            {
                Projects.TryGetValue(CurrentProjectId, out DATA.Project found);
                return found ?? null;
            }
        }

        static Project()
        {
            LazyProject = new Lazy<Project>(() => new Project());
        }

        private Project()
        {
            using (var Reader = new StreamReader(File.Open(DataFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read) as Stream, Encoding.UTF8, true, 10240))
            {
                Projects = JSON.Deserialize<Dictionary<string, DATA.Project>>(Reader.ReadLine());
            }
            CurrentProjectId = "";
            SessionTime = DateTime.UtcNow.Ticks;
        }

        public bool SaveProject(DATA.Project Content, bool Upsert = false)
        {
            if (Content == null || string.IsNullOrWhiteSpace(Content.Name) || string.IsNullOrWhiteSpace(Content.Source))
            {
                return false;
            }

            Projects.TryGetValue(Content.Name, out DATA.Project found);

            if (found != null && Upsert == false)
            {
                return false;
            }

            if (!File.Exists(Content.Source))
            {
                return false;
            }

            Projects.Remove(Content.Name);
            Projects.Add(Content.Name, Content);

            Save();

            return false;
        }

        public void Open(string ProjectId = "default")
        {
            if (string.IsNullOrWhiteSpace(ProjectId))
            {
                return;
            }

            Projects.TryGetValue(ProjectId, out DATA.Project found);

            if (found == null)
            {
                return;
            }
            else
            {
                CurrentProjectId = ProjectId;
            }
        }

        public void Save()
        {
            using (var Writer = new StreamWriter(File.Open(DataFile, FileMode.Create, FileAccess.Write, FileShare.None) as Stream, Encoding.UTF8, 10240))
            {
                Writer.WriteLine(JSON.Serialize(Projects));
            }
        }
    }
}