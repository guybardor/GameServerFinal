using System.ComponentModel.DataAnnotations;

namespace TicTacToeGameServer.Configurations
{
    public class WebSocketSharpConfiguration
    {
        [Required]
        public int? Port { get; set; }

        [Required]
        public string? Path { get; set; }
    }
}
