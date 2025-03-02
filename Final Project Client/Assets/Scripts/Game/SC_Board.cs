using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Board 
{
    public enum SlotState
    {
        Empty,X,O,Error
    };
    public enum WinnerState
    {
       NoWinner, Winner, Tie
    };

    private int maxSlots = 9;
    private int moveCount;
    private List<SlotState> board;

    public int MaxSlots { get => maxSlots;}

    public SC_Board()
    {
        moveCount = 0;
        board = new List<SlotState>();
        for (int i = 0; i < MaxSlots; i++)
            board.Add(SlotState.Empty);
    }

    public SlotState GetSlotState(int _Index)
    {
        if (_Index >= 0 && _Index < board.Count)
            return board[_Index];
        return SlotState.Error;
    }

    public void SetSlotState(int _Index,SlotState _NewState)
    {
        if (_Index >= 0 && _Index < board.Count)
        {
            moveCount++;
            board[_Index] = _NewState;
        }
    }

    public WinnerState IsMatchOver()
    {
        if (CheckWinner(0, 1, 2) || CheckWinner(3, 4, 5) || CheckWinner(6, 7, 8) ||
            CheckWinner(0, 3, 6) || CheckWinner(1, 4, 7) || CheckWinner(2, 5, 8) ||
            CheckWinner(0, 4, 8) || CheckWinner(2, 4, 6))
            return WinnerState.Winner;

        if (moveCount == MaxSlots)
            return WinnerState.Tie;

        return WinnerState.NoWinner;
    }

    private bool CheckWinner(int _Index0,int _Index1,int _Index2)
    {
        if (board[_Index0] != SlotState.Empty && board[_Index0] != SlotState.Error
            && board[_Index0] == board[_Index1] && board[_Index1] == board[_Index2])
            return true;
        return false;
    }

    public int GetEmptySlot()
    {
        List<int> _slotIndex = new List<int>();
        for(int i=0; i < MaxSlots; i++)
        {
            if (board[i] == SlotState.Empty)
                _slotIndex.Add(i);
        }
        return _slotIndex[Random.Range(0, _slotIndex.Count)];
    }
}
