using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;


public class MineSweeperCommand
{
    [Command("minesweeper"), Description("Starts a game of minesweeper.")]
    public async ValueTask ExecuteAsync(SlashCommandContext context) 
    {
        MineSweeper ms = new();
        var builder = ms.BuildResponse(context.User);
        await context.RespondAsync(builder);

        var message = await context.GetResponseAsync();
        if (message == null)
            return;
        
        while (true)
        {
            var resp = await message.WaitForButtonAsync();

            if (resp.TimedOut)
            {
                await resp.Result.Message.ModifyAsync("Game timed out!");
                break;
            }
            int x = int.Parse(resp.Result.Id.Split("_")[2]);
            int y = int.Parse(resp.Result.Id.Split("_")[3]);
            ms.Open(x, y);
            var respbuilder = ms.BuildResponse(context.User);
            await resp.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, respbuilder);

            if (ms.CheckWin() || ms.Lost) 
                break;
        }
        
    }
}