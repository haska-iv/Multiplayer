using UnityEngine;
using Mirror;

public class Game : NetworkBehaviour
{
    private static Game _active;
    public static Game active { get { if (_active == null) _active = FindObjectOfType<Game>(); return _active; } }

    public int scoreToWin = 3;

    //this part can use list of players score and players ids
    private int player1Score;
    private int player2Score;
    private int player3Score;
    private int player4Score;

    private uint player1Id = 0;
    private uint player2Id = 0;
    private uint player3Id = 0;
    private uint player4Id = 0;

    private bool isGameOver
    {
        get
        {
            return player1Score >= scoreToWin ||
              player2Score >= scoreToWin ||
              player3Score >= scoreToWin ||
              player4Score >= scoreToWin;
        }
    }

    private void CheckWinCondition()
    {
        if (isGameOver)
            NetworkManager.singleton.StopHost();
    }

    [Command]
    public void CmdIncreaseScore(uint playerId)
    {
        if (playerId == player1Id)
            player1Score++;
        else if (playerId == player2Id)
            player2Score++;
        else if (playerId == player3Id)
            player3Score++;
        else if (playerId == player4Id)
            player4Score++;
        else
            SetId(playerId);

        CheckWinCondition();
    }

    [Command]
    public void SetId(uint playerId)
    {
        if (player1Id == 0)
            player1Id = playerId;
        else if (player2Id == 0)
            player2Id = playerId;
        else if (player3Id == 0)
            player3Id = playerId;
        else if (player4Id == 0)
            player4Id = playerId;
        CmdIncreaseScore(playerId);
    }
}