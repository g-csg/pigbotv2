namespace PigBot.Database
{
    public class CommandPolicy
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public string CommandName { get; set; }
        public bool Allowed { get; set; }
    }
}