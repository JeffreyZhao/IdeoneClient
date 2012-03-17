using System;

namespace IdeoneClient
{
    public class SubmissionDetail
    {
        public int LanguageID { get; set; }

        public string LanguageName { get; set; }

        public string LanguageVersion { get; set; }

        public float Time { get; set; }

        public DateTime CreateAt { get; set; }

        public SubmissionState State { get; set; }

        public SubmissionResult Result { get; set; }

        public int Memory { get; set; }

        public int Signal { get; set; }

        public bool IsPublic { get; set; }

        public string Source { get; set; }

        public string Error { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }

        public string CompileInfo { get; set; }
    }
}
