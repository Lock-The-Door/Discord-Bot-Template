using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Discord_Bot_Template
{
    [Group("tests")]
    [RequireOwner(ErrorMessage = "This is a test command and is not intended for public use", Group = "Owner command")]
    [Summary("Group for test commands")]
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        [Summary("A test command")]
        public async Task Test()
        {
            await Context.Channel.SendMessageAsync("test");
        }

        [Command("inputTest")]
        [Summary("tests input by sending a echo")]
        public async Task InputTest(string input)
        {
            await Context.Channel.SendMessageAsync($"You said: \"{input}\"");
        }
    }

    public class Tests : SlashCommand
    {
        public Tests() // Command config
        {
            Command.Name = "tests";
            Command.Description = "Test commands";

            // Create options
            var options = new SlashCommandOptionBuilder[]
            {
                new SlashCommandOptionBuilder()
                {
                    Name = "test",
                    Description = "A test command",
                    Type = ApplicationCommandOptionType.SubCommand
                },
                new SlashCommandOptionBuilder()
                {
                    Name = "input-test",
                    Description = "Test input by echoing what you say",
                    Type = ApplicationCommandOptionType.SubCommand,

                    Options = new List<SlashCommandOptionBuilder>()
                    {
                        new SlashCommandOptionBuilder()
                        {
                            Name = "input",
                            Description = "The text to reply with",
                            Type = ApplicationCommandOptionType.String,
                            IsRequired = true,
                        }
                    }
                }
            };
            Command.AddOptions(options);
        }

        public async Task RunCommand(SocketSlashCommand command) // Base function to run must be this name
        {
            switch (command.Data.Options.First().Name)
            {
                case "test":
                    await Test(command);
                    break;
                case "input-test":
                    await InputTest(command);
                    break;
            }
        }

        private async Task Test(SocketSlashCommand command)
        {
            await command.RespondAsync("test");
        }
        private async Task InputTest(SocketSlashCommand command)
        {
            await command.RespondAsync($"You said: {command.Data.Options.First().Options.First().Value}");
        }
    }
}
