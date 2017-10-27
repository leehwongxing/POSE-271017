using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace POSE.DATA
{
    public class Translation
    {
        public string Id { get; set; }

        public string MessageId { get; set; }

        public string MessagePluralId { get; set; }

        public string MessageContext { get; set; }

        public string[] Untranslated { get; set; }

        public string[] Translated { get; set; }

        public string[] Modified { get; set; }

        public HashSet<string> Flags { get; set; }

        public string Hashed { get { return Hash(); } }

        public long Created { get; set; }

        private int TranslatingId { get; set; }

        private Translation()
        {
            Id = CORE.DATABASE.Base<Translation>.Id();
            MessageId = "";
            MessagePluralId = "";
            MessageContext = "";
            Untranslated = new string[3];
            Translated = new string[3];
            Modified = new string[3];
            Flags = new HashSet<string>();
            Created = CORE.Project.Singular.SessionTime;
            TranslatingId = 0;
        }

        public static Translation CreateOne()
        {
            return new Translation();
        }

        public string Hash()
        {
            var Content = Encoding.UTF8.GetBytes(string.Join("×", Untranslated));
            var RawHash = new SHA384Managed().ComputeHash(Content);
            var Hash = "";

            foreach (var oneByte in RawHash)
            {
                Hash += String.Format("{0:x2}", oneByte);
            }
            return Hash;
        }

        public void AddTranslation(string Translation = "", int Id = -1)
        {
            if (Id == -1)
            {
                Id = TranslatingId;
                TranslatingId++;
            }

            Translated[Id] = Translation;
            Modified[Id] = Translation;
        }
    }
}