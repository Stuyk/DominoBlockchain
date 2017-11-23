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
        /// Used to control if a file is done writing.
        /// </summary>
        public static bool FileDoneWriting = true;

        public static void CreateNewBlockAddToChain(Transaction transaction)
        {
            Block block = new Block() {
                DeserializedTransaction = transaction
            };

            Main.Queue.Enqueue(block);
        }

        public static void WriteBlock(Block block)
        {
            FileDoneWriting = false;

            if (Main.Blocks != null)
            {
                // Make sure previous hash is set.
                block.PreviousHash = Main.Blocks[Main.Blocks.Length - 1].BlockHash;
                // Serialize a new block hash.
                block.BlockHash = EasyEncryption.SHA.ComputeSHA256Hash(JsonConvert.SerializeObject(block.DeserializedTransaction) + Main.Blocks[Main.Blocks.Length - 1].BlockHash);
            }
            
            string newBlock = JsonConvert.SerializeObject(block);

            if (Main.Blocks != null)
            {
                File.AppendAllText(Main.filePath, Environment.NewLine + newBlock);
            } else {
                File.AppendAllText(Main.filePath, newBlock);
            }

            LoadAllBlocks();
        }

        public static void LoadAllBlocks()
        {
            string[] lines = File.ReadAllLines(Main.filePath);
            Block[] blocks = new Block[lines.Length];
            
            for (int i = 0; i < lines.Length; i++)
            {
                blocks[i] = DeserializeBlock.FromJson(lines[i]);
            }

            Main.Blocks = blocks;

            FileDoneWriting = true;
        }
    }
}
