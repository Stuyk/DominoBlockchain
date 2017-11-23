using Domino.Data;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
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
        public static Queue<Block> Queue = new Queue<Block>();
        // A list of all the blocks.
        public static Block[] Blocks { get; set; }
        // Directory for Storage
        public static string directoryPath = "resources/Domino/Transactions";
        public static string filePath = $"{directoryPath}/transactions.txt";

        public Main()
        {
            API.onResourceStart += API_onResourceStart;
            API.onUpdate += UpdateQueuedBlocks;
        }

        private void UpdateQueuedBlocks()
        {
            if (Queue.Count > 0)
            {
                if (DataHandler.FileDoneWriting)
                {
                    DataHandler.FileDoneWriting = false;
                    DataHandler.WriteBlock(Queue.Dequeue());
                }
            }
        }

        private void API_onResourceStart()
        {
            startupProcess();
        }

        /// <summary>
        /// Checks if the directory / file exists. If not it creates it.
        /// </summary>
        private void startupProcess()
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(filePath))
            {
                generateGenesisBlock();
            }
            
            DataHandler.LoadAllBlocks();
            Console.WriteLine("[DOMINO] Loaded All Blocks");

            Verification.VerifyAllPreviousBlocks();


            // SANDBOX
            /*
            for (var i = 0; i < 10; i++)
            {
                Transaction tempTransaction = new Transaction()
                {
                    Value = new Random().Next(1, 25000),
                    FromAddress = EasyEncryption.SHA.ComputeSHA256Hash(WordList.GetWordRepeatable()),
                    TargetAddress = EasyEncryption.SHA.ComputeSHA256Hash("StuykGaming")
                };

                DataHandler.CreateNewBlockAddToChain(tempTransaction);
            }
            */

            /*
            for (var i = 0; i < 5000; i++)
            {
                Transaction tempTransaction = new Transaction()
                {
                    Value = new Random(i * i).Next(1, 25000),
                    FromAddress = EasyEncryption.SHA.ComputeSHA256Hash(WordList.GetWordRepeatable()),
                    TargetAddress = EasyEncryption.SHA.ComputeSHA256Hash(WordList.GetWordRepeatable())
                };

                DataHandler.CreateNewBlockAddToChain(tempTransaction);
            }
            */
            

            Verification.VerifyAllPreviousBlocks();

            Console.WriteLine($"Value: {DataHandler.GetAllRecievedTransactions("StuykGaming")}");

        }

        /// <summary>
        /// Generates the genesis block for the block chain.
        /// </summary>
        private void generateGenesisBlock()
        {
            Console.WriteLine("[DOMINO] Genesis Block Created");
            Transaction tempTransaction = new Transaction()
            {
                Value = 10,
                FromAddress = EasyEncryption.SHA.ComputeSHA256Hash(WordList.GetWord()),
                TargetAddress = EasyEncryption.SHA.ComputeSHA256Hash(WordList.GetWord())
            };

            DataHandler.WriteBlock(new Block()
            {
                PreviousHash = EasyEncryption.SHA.ComputeSHA256Hash("Domino"),
                BlockHash = EasyEncryption.SHA.ComputeSHA256Hash("BlockChain"),
                DeserializedTransaction = new Transaction()
                {
                    Value = 0,
                    FromAddress = EasyEncryption.SHA.ComputeSHA256Hash("Created By"),
                    TargetAddress = EasyEncryption.SHA.ComputeSHA256Hash("Stuyk")
                }
            });
        }
    }
}
