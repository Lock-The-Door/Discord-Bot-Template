using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Discord_Bot_Template
{
    public class SlashCommand
    {
        private protected SlashCommandBuilder Command = new SlashCommandBuilder();

        public void Build()
        {
            SlashCommand_Handler.slashCommands.Add(Command.Build());
        }
    }

    public class SlashCommand_Handler
    {
        private DiscordSocketClient _client;

        public static List<ApplicationCommandProperties> slashCommands = new List<ApplicationCommandProperties>();
        public List<object> slashCommandObjects = new List<object>();

        public SlashCommand_Handler(DiscordSocketClient client)
        {
            _client = client;
            client.SlashCommandExecuted += SlashCommandHandler;
        }

        public async Task RegisterSlashCommands()
        {
            // Get all slash commands
            var slashCommandsClasses = (
                from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where assemblyType.IsSubclassOf(typeof(SlashCommand))
                && !assemblyType.IsAbstract
                select assemblyType).ToArray();

            foreach (var command in slashCommandsClasses)
            {
                var commandObject = Activator.CreateInstance(command);
                slashCommandObjects.Add(commandObject);
            }

#if !DEBUG
            Console.WriteLine("Running release version, not building commands");
            return;
#else
            Console.WriteLine("Running debug version, building commands");
#endif
            foreach (var command in slashCommandObjects)
            {
                command.GetType().GetMethod("Build")
                    .Invoke(command, null);
            }

            await _client.BulkOverwriteGlobalApplicationCommandsAsync(slashCommands.ToArray());
        }


        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            object slashCommand = slashCommandObjects.First(sco => sco.GetType().Name.ToLower().EndsWith(command.Data.Name));
            var method = slashCommand.GetType().GetMethod("RunCommand");
            var runningCommand = (Task)method.Invoke(slashCommand, new object[] { command });
            await runningCommand;
        }
    }
}
