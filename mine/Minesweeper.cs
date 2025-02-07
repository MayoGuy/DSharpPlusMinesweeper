using DSharpPlus.Entities;

class MineSweeper 
{
    public int[,] board;
    public bool[,] opened;

    public bool Lost;

    public void CreatMines()
    {
        Random random = new();
        for (int i = 0; i < 5; i++)
        {
            int x = random.Next(0, 5);
            int y = random.Next(0, 5);
            if (board[x, y] == 9)
                i--;
            else
                board[x, y] = 9;
        }
    }

    static List<(int, int)> GetNeighbours(int x, int y) {
        int[,] directions = {{-1, -1}, {-1, 0}, {-1, 1}, {0, -1}, {0, 1}, {1, -1}, {1, 0}, {1, 1}};
        List<(int, int)> neighbours = [];
        for (int i = 0; i < 8; i++)
        {
            int nc = y+directions[i, 0];
            int nr = x+directions[i, 1];
            if (0 <= nr && nr < 5 && 0 <= nc && nc < 5)
            {
                neighbours.Add((nr, nc));
            }
        }
        return neighbours;
    }

    

    public void CreateNumbers()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (board[i, j] == 9)
                {
                    List<(int, int)> neighbours = GetNeighbours(i, j);
                    for (int n = 0; n < neighbours.Count; n++)
                    {
                        int nc = neighbours[n].Item1;
                        int nr = neighbours[n].Item2;
                        if (board[nc, nr] != 9)
                            board[nc, nr] += 1;
                    }
                }
                
            }
        }
    }

    public void Open(int x, int y)
    {
        opened[x, y] = true;
        if (board[x, y] == 9)
        {
            Lost = true;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    opened[i, j] = true;
                }
            }
            return;
        }
        if (board[x, y] == 0)
        {
            List<(int, int)> neighbours = GetNeighbours(x, y);
            for (int i = 0; i < neighbours.Count; i++)
            {
                int nx = neighbours[i].Item1;
                int ny = neighbours[i].Item2;
                if (!opened[nx, ny]) {
                    Open(nx, ny);
                }
            }
        }
    }


     public DiscordInteractionResponseBuilder BuildResponse( DiscordUser user)
    {
        bool disabled = false;
        var builder = new DiscordInteractionResponseBuilder();

        if (Lost) {builder.WithContent("You lost!"); disabled = true;}

        else if (CheckWin()) builder.WithContent("You won!");

        else builder.WithContent("MineSweeper Game!");

        for (int i = 0; i < 5; i++)
        {
            DiscordComponent[] components = new DiscordComponent[5];
            for (int j = 0; j < 5; j++)
            {
                string text = opened[i, j] && board[i, j] != 0 ? board[i, j].ToString() : "​";
                DiscordComponentEmoji emoji = new("💥");
                if (board[i, j] == 9 && opened[i, j])
                    components[j] = new DiscordButtonComponent(DiscordButtonStyle.Danger, $"{user.Id}_mine_{i}_{j}", "💣", emoji:emoji, disabled:disabled);
                else if (board[i, j] == 9 && CheckWin())
                    components[j] = new DiscordButtonComponent(DiscordButtonStyle.Success, $"{user.Id}_mine_{i}_{j}", "​", disabled:disabled);
                else
                    components[j] = new DiscordButtonComponent(DiscordButtonStyle.Primary, $"{user.Id}_mine_{i}_{j}", text, disabled:opened[i, j] || disabled);
            }
            builder.AddComponents(components);
        }
        return builder;
    }

    public bool CheckWin() {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (!opened[i, j] && board[i, j] != 9)
                    return false;
            }
        }
        return true;
    }

    public MineSweeper()
    {
        board = new int[5, 5];
        opened = new bool[5, 5];
        CreatMines();
        CreateNumbers();
    }

}