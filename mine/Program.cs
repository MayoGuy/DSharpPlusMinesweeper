using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace mine
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
            if (string.IsNullOrWhiteSpace(discordToken))
            {
                Console.WriteLine("Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
                Environment.Exit(1);
            }

            DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, SlashCommandProcessor.RequiredIntents);

            
            builder.UseCommands((serviceProvider, extension) =>
            {
                extension.AddCommands([typeof(MineSweeperCommand)]);
                SlashCommandProcessor slashCommandProcessor = new(new());
                extension.AddProcessor(slashCommandProcessor);
            }, new CommandsConfiguration()
            {
                RegisterDefaultCommandProcessors = true,
            });

            builder.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            DiscordClient client = builder.Build();
            DiscordActivity status = new("with balls", DiscordActivityType.Playing);
            await client.ConnectAsync(status, DiscordUserStatus.Online);
            await Task.Delay(-1);
        }


    }
}

