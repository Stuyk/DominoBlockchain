using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domino.Data
{
    public class Commands : Script
    {
        [Command("MarketCap")]
        public void cmdMarketCap(Client player)
        {
            int circulation = DataHandler.GetCirculatingSupply();
            API.sendChatMessageToPlayer(player, $"{circulation}/{Main.MaxCoinCount}");
        }


        [Command("Interact")]
        public void cmdInteraction(Client player)
        {
            API.sendChatMessageToPlayer(player, "You did something, let's mine a block because of it...");

            string publicHash = DataHandler.GetPlayerPublicAddress(player);

            DataHandler.MineCurrentBlock(publicHash);
        }

        [Command("Balance")]
        public void cmdBalance(Client player)
        {
            // Check if they have a secret.
            if (!player.hasData("Domino_Public"))
                return;

            // Reward the player.
            string publicHash = player.getData("Domino_Public");

            player.sendChatMessage($"Player Balance: {DataHandler.GetPlayerBalance(publicHash)}");
        }

        [Command("SetSecret", GreedyArg = true)]
        public void cmdSecret(Client player, string secretCode)
        {
            if (secretCode == null)
                return;
            // Encrypt Player Name
            string playerHash = EasyEncryption.SHA.ComputeSHA256Hash(player.name.ToLower());
            // Encrypt Secret String into Private Hash
            string privateHash = EasyEncryption.SHA.ComputeSHA256Hash(secretCode.ToLower());
            // Encrypt Secret String + Private Hash for Public Address
            string publicHash = EasyEncryption.SHA.ComputeSHA256Hash(playerHash + privateHash);
            // Store Public Hash on Player
            player.setData("Domino_Public", publicHash);
            player.sendChatMessage("Secret Added.");
        }

        [Command("VerifySecret", GreedyArg = true)]
        public void cmdVerifySecret(Client player, string secretCode)
        {
            // Encrypt Player Name
            string playerHash = EasyEncryption.SHA.ComputeSHA256Hash(player.name.ToLower());
            // Encrypt Secret String into Private Hash
            string privateHash = EasyEncryption.SHA.ComputeSHA256Hash(secretCode.ToLower());
            // Encrypt Secret String + Private Hash for Public Address
            string publicHash = EasyEncryption.SHA.ComputeSHA256Hash(playerHash + privateHash);

            if (publicHash == player.getData("Domino_Public"))
            {
                player.sendChatMessage("Successfully Verified Secret.");
            }
            else
            {
                player.sendChatMessage("Secret did not match.");
            }
        }

        [Command("genBlocks", GreedyArg = true)]
        public void cmdGenBlocks(Client player)
        {
            for (int i = 0; i < 100; i++)
            {
                DataHandler.CreateNewTransaction(new Transaction()
                {
                    FromAddress = EasyEncryption.SHA.ComputeSHA256Hash(WordList.GetWordRepeatable()),
                    TargetAddress = EasyEncryption.SHA.ComputeSHA256Hash(WordList.GetWordRepeatable()),
                    Value = 0
                });
            }
        }
    }
}
