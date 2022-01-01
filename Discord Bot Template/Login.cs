using System;
using System.Configuration;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Discord_Bot_Template
{
    public class Program
    {
        private DiscordSocketClient _client;
        private SlashCommand_Handler _command;

        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.DirectMessages,
                LogLevel = LogSeverity.Debug
            });

            _command = new SlashCommand_Handler(_client);

            _client.Log += Log;

            _client.Ready += Ready;

            await _client.LoginAsync(TokenType.Bot,
                ConfigurationManager.AppSettings.Get("Discord API Key"));

            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task Ready()
        {
            await _command.RegisterSlashCommands();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
