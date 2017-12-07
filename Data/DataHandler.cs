using GrandTheftMultiplayer.Server.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domino.Data
{
    public static class DataHandler
    {
        /// <summary>
        /// Where we keep a Queue of all the blocks that need to be pushed to the Database.
        /// </summary>
        public static Queue<Block> Queue = new Queue<Block>();
        
        /// <summary>
        /// An array of all the blocks available.
        /// </summary>
        public static Block[] Blocks { get; set; }

        /// <summary>
        /// The CurrentBlock we're writing transactions to.
        /// </summary>
        public static Block CurrentBlock { get; set; }

        /// <summary>
        /// Used to control if a file is done writing.
        /// </summary>
        public static bool FileDoneWriting = true;

        /// <summary>
        /// Create a new transaction to add to the newest block.
        /// </summary>
        /// <param name="transaction"></param>
        public static void CreateNewTransaction(Transaction transaction)
        {
            if (CurrentBlock.Transactions.Count >= Main.MaxTransactionsPerBlock)
                GenerateNewBlock();

            CurrentBlock.Transactions.Add(transaction);
        }

        /// <summary>
        /// Mine the Current Block Available.
        /// </summary>
        public static void MineCurrentBlock()
        {
            if (Queue.Count > 0)
            {
                if (!FileDoneWriting)
                    return;

                FileDoneWriting = false;
                WriteBlock(Queue.Dequeue());

                Console.WriteLine($"Current Queue Left: {DataHandler.Queue.Count}");
            }
        }

        /// <summary>
        /// Create a new Block or Queue the current block if it's not null.
        /// </summary>
        public static void GenerateNewBlock()
        {
            // If our Main.CurrentBlock has a value, Queue it. Otherwise make a new block.
            if (CurrentBlock != null)
                Queue.Enqueue(CurrentBlock);

            CurrentBlock = new Block();
            CurrentBlock.Transactions = new List<Transaction>();
        }

        /// <summary>
        /// Write our Block + Transactions to the file.
        /// </summary>
        /// <param name="block"></param>
        public static void WriteBlock(Block block)
        {
            FileDoneWriting = false;

            if (Blocks != null)
            {
                block.PreviousHash = Blocks[Blocks.Length - 1].BlockHash;
                block.BlockHash = EasyEncryption.SHA.ComputeSHA256Hash(JsonConvert.SerializeObject(block.Transactions) + Blocks[Blocks.Length - 1].BlockHash + block.PreviousHash);
            }
            
            string newBlock = JsonConvert.SerializeObject(block);

            if (Blocks != null)
            {
                File.AppendAllText(Main.filePath, Environment.NewLine + newBlock);
            } else {
                File.AppendAllText(Main.filePath, newBlock);
            }

            LoadAllBlocks();
        }

        public static void LoadAllBlocks()
        {
            API.shared.consoleOutput($"{Main.Domino} Loading all blocks...");
            string[] lines = File.ReadAllLines(Main.filePath);
            Block[] blocks = new Block[lines.Length];
            
            for (int i = 0; i < lines.Length; i++)
            {
                blocks[i] = DeserializeBlock.FromJson(lines[i]);
            }

            Blocks = blocks;

            FileDoneWriting = true;
            API.shared.consoleOutput($"{Main.Domino} Blocks done loading.");

            Verification.VerifyAllPreviousBlocks();
        }

        /*
        public static long GetAllSentTransactions(string address)
        {
            long recievedTotal = 0;

            foreach (Block blck in Main.Blocks)
            {
                if (blck.Transaction.FromAddress == EasyEncryption.SHA.ComputeSHA256Hash(address))
                {
                    recievedTotal += blck.Transaction.Value;
                }
            }

            return recievedTotal;
        }

        public static long GetAllRecievedTransactions(string address)
        {
            long recievedTotal = 0;

            foreach (Block blck in Main.Blocks)
            {
                if (blck.Transaction.TargetAddress == EasyEncryption.SHA.ComputeSHA256Hash(address))
                {
                    recievedTotal += blck.Transaction.Value;
                }
            }

            return recievedTotal;
        }
        */
    }
}
