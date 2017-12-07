using Domino.Data;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domino
{
    public class Main : Script
    {
        /// <summary>
        /// Console Output String
        /// </summary>
        public static string Domino = "Domino ->";
        /// <summary>
        /// Storage Directory
        /// </summary>
        public static string directoryPath = "resources/Domino/Transactions";
        /// <summary>
        /// File Directory + Name
        /// </summary>
        public static string filePath = $"{directoryPath}/transactions.txt";
        /// <summary>
        /// The maximum transactions allowed before a new block is generated.
        /// </summary>
        public static int MaxTransactionsPerBlock = 25;
        /// <summary>
        /// Block Reward
        /// </summary>
        public static int BlockReward = 1;
        /// <summary>
        /// Max Printed Cash Available
        /// </summary>
        public static int MaxCoinCount = 25;

        public Main()
        {
            API.onResourceStart += API_StartupResource;
        }

        /// <summary>
        /// Checks if the directory / file exists. If not it creates it.
        /// </summary>
        private void API_StartupResource()
        {
            API.consoleOutput($"{Domino} Starting resource...");
            if (!Directory.Exists(directoryPath))
            {
                API.consoleOutput($"{Domino} Creating directory...");
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(filePath))
            {
                API.consoleOutput($"{Domino} Creating genesis block...");
                generateGenesisBlock();
            }
            
            DataHandler.LoadAllBlocks();
        }

        /// <summary>
        /// Generates the genesis block for the block chain.
        /// </summary>
        private void generateGenesisBlock()
        {
            API.consoleOutput($"{Domino} Creating genesis block...");

            Block genesisBlock = new Block();
            genesisBlock.PreviousHash = EasyEncryption.SHA.ComputeSHA256Hash("Domino");
            genesisBlock.BlockHash = EasyEncryption.SHA.ComputeSHA256Hash("BlockChain");
            genesisBlock.Transactions = new List<Transaction>();

            Transaction newTransaction = new Transaction()
            {
                Value = 0,
                FromAddress = EasyEncryption.SHA.ComputeSHA256Hash("Created By"),
                TargetAddress = EasyEncryption.SHA.ComputeSHA256Hash("Stuyk")
            };

            genesisBlock.Transactions.Add(newTransaction);

            DataHandler.WriteBlock(genesisBlock);
            
            API.consoleOutput($"{Domino} Genesis block created.");
        }
    }
}
