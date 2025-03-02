namespace TicTacToeGameServer.Managers
{
    public class GlobalEnums
    {
        public enum CloseCode
        {
            Unknown = 2001, DuplicateConnection = 2002, MissingUserId = 2003
        };

        public enum ErrorCodes
        {
            Unknown = 3000, MissingMatchId = 3001, RoomClosed = 3002, RoomDoesntExist = 3003,
            MissingVariables = 3004, NotPlayerTurn = 3005, MissingVaribles = 3006
        };
    }
}
