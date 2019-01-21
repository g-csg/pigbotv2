using System;

namespace PigBot.Database
{
    public class CoolDownPolicy
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public string CommandName { get; set; }
        public int CooldownSeconds { get; set; }
        public DateTime LastExecution { get; set; }
    }
}