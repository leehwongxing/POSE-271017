using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace POSE.CORE
{
    public class Material
    {
        public DATA.Project Current { get; private set; }

        public string SourceFile { get { return "source"; } }
        public string ModifiedFile { get { return "modified"; } }
        public string DecodedFile { get { return "decoded"; } }
        public string EncodedFile { get { return "encoded"; } }

        public string Decompiler { get { return "msgunfmt.exe"; } }
        public string Compiler { get { return "msgfmt.exe"; } }

        private Queue<string> Queued { get; set; }
        private string Processing { get; set; }

        public Material()
        {
            Current = Project.Singular.Opening;
            var WorkingPath = Path.Combine(".", "TOOLS", "TMP");

            if (!Directory.Exists(WorkingPath))
            {
                Directory.CreateDirectory(WorkingPath);
            }

            if (!File.Exists(Current.Source))
            {
                return;
            }

            File.Delete(Path.Combine(WorkingPath, SourceFile));
            File.Delete(Path.Combine(WorkingPath, ModifiedFile));
            File.Delete(Path.Combine(WorkingPath, DecodedFile));
            File.Delete(Path.Combine(WorkingPath, EncodedFile));

            File.Copy(Current.Source, SourceFile, true);

            Queued = new Queue<string>();
            Processing = "";
        }

        public List<DATA.Translation> Transform()
        {
            var WorkingPath = Path.Combine(".", "TOOLS", "TMP", DecodedFile);

            if (!File.Exists(WorkingPath))
            {
                return null;
            }

            var Data = new List<DATA.Translation>();

            using (var Reader = new StreamReader(File.Open(WorkingPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read) as Stream, Encoding.UTF8, true, 10240))
            {
                do
                {
                    var Read = Reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(Read))
                    {
                        if (Queued.Count > 0)
                        {
                            Data.Add(Proceed());
                        }
                    }
                    else
                    {
                        Queued.Enqueue(Read);

                        if (Reader.EndOfStream)
                        {
                            Data.Add(Proceed());
                        }
                    }
                } while (!Reader.EndOfStream);
            }

            return Data;
        }

        private DATA.Translation Proceed()
        {
            var Data = DATA.Translation.CreateOne();

            while (Queued.Count > 0)
            {
                Dequeue();

                if (Processing.IndexOf("#") == 0) // If it's a comment of any sort
                {
                    continue;
                }

                if (Processing.IndexOf("msgid ") == 0) // If it's a MessageId
                {
                    Data.MessageId = ReadText();
                }

                if (Processing.IndexOf("msgctxt ") == 0) // If it's a MessageContext
                {
                    Data.MessageContext = ReadText();
                }

                if (Processing.IndexOf("msgid_plural ") == 0) // If it's a PluralMessageId
                {
                    Data.MessagePluralId = ReadText();
                }

                if (Processing.IndexOf("msgstr") == 0) // If it's an Untranslated message
                {
                    Data.AddTranslation(ReadText());
                }
            }

            Queued.Clear();
            Processing = "";

            return Data;
        }

        private string ReadText()
        {
            var Data = "";

            do
            {
                var First = Processing.IndexOf('"');
                var Last = Processing.LastIndexOf('"');

                if (Last - First - 1 > 0) // subline wouldnt be empty
                {
                    var Subline = Processing.Substring(First + 1, Last - First - 1);
                    if (!string.IsNullOrWhiteSpace(Subline))
                    {
                        Data += Regex.Unescape(Subline);
                    }
                }
                Dequeue();
            } while (Processing.IndexOf('"') == 0); // After dequeued, read until next statement

            return Data;
        }

        private void Dequeue()
        {
            if (Queued.Count == 0)
            {
                Processing = "";
                return;
            }

            Processing = Queued.Dequeue();
        }

        public void Decode()
        {
            var DecompileProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = "TMP",
                    Arguments = string.Join(" ", new string[] { "/c", Decompiler, SourceFile, ">", DecodedFile }),
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            DecompileProcess.EnableRaisingEvents = true;
            DecompileProcess.Start();
            DecompileProcess.WaitForExit();
        }

        public void Encode()
        {
            var CompileProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = "TMP",
                    Arguments = string.Join(" ", new string[] { "/c", Compiler, "-cv", "-o", EncodedFile, ModifiedFile }),
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            CompileProcess.EnableRaisingEvents = true;
            CompileProcess.Start();
            CompileProcess.WaitForExit();
        }
    }
}