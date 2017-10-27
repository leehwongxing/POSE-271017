using System;

namespace POSE.DATA
{
    public class Project
    {
        public string Name { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public string Translator { get; set; }

        public string TranslationTeam { get; set; }

        public string Language { get; set; }

        public string Mime { get; private set; }

        public string ContentType { get; private set; }

        public string ContentEncoding { get; private set; }

        public string PluralForm { get; private set; }

        public long Created { get; private set; }

        public Project()
        {
            Name = "";
            Source = "";
            Destination = "";
            Translator = Environment.UserName;
            TranslationTeam = Environment.UserDomainName ?? Environment.UserName;
            Language = "English";
            Mime = "1.0";
            ContentType = "text/plain; charset=UTF-8";
            ContentEncoding = "8bit";
            PluralForm = "nplurals=3; plural=(n%10==1 && n%100!=11 ? 0 : n%10>=2 && n%10<=4 && (n%100<10 || n%100>=20) ? 1 : 2)";
            Created = DateTime.UtcNow.Ticks;
        }

        public override string ToString()
        {
            return string.Join(
                "\n",
                Name,
                Source,
                Translator
                );
        }
    }
}