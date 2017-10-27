using LiteDB;
using System.IO;

namespace POSE.CORE.DATABASE
{
    public class Base<T>
    {
        public string Type { get { return typeof(T).Name.Replace(".", "_"); } }

        public string Folder { get { return "DATA"; } }

        protected LiteDatabase Storage { get; set; }

        public Base(string ProjectId = "default")
        {
            var DataPath = Path.Combine(".", Folder);
            var FilePath = Path.Combine(".", Folder, string.Join(".", ProjectId, Type));

            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            var Config = new ConnectionString
            {
                Filename = FilePath,
                Password = string.Join("—", ProjectId, "——ヽ(　￣д￣)ノ", Type),
                CacheSize = 10,
                Mode = LiteDB.FileMode.Exclusive,
                InitialSize = 512 * 1024,
                LimitSize = 768 * 1024 * 1024,
                Log = Logger.JOURNAL
            };

            Storage = new LiteDatabase(Config);
        }

        public LiteCollection<T> Collection { get { return Storage.GetCollection<T>(Type) ?? null; } }

        public static string Id()
        {
            return new ObjectId().ToString();
        }
    }
}