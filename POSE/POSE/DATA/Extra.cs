using System.Collections.Generic;

namespace POSE.DATA
{
    public class Extra
    {
        public string Id { get; set; }

        public string MessageId { get; set; }

        public string MessageContext { get; set; }

        public string Untranslated { get; set; }

        public HashSet<string> Choices { get; set; }

        public string Default { get; set; }

        public string Chosen { get; set; }

        private Extra()
        {
            Id = "";
            MessageId = "";
            MessageContext = "";
            Untranslated = "";
            Choices = new HashSet<string>();
            Default = "";
            Chosen = "";
        }
    }
}