using System;

namespace PigBot.Database
{
    public class FourchanPost
    {
        public int Id { get; set; }
        public string DateTime { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public string FileUrl { get; set; }
    }
}