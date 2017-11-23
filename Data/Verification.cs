using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domino.Data
{
    public static class Verification
    {
        public static bool VerifyAllPreviousBlocks()
        {
            string previousHash = null;

            for (int i = 0; i < Main.Blocks.Length; i++)
            {
                if (previousHash == null)
                {
                    previousHash = Main.Blocks[0].PreviousHash;
                }

                if (Main.Blocks[i].PreviousHash != previousHash)
                {
                    Console.WriteLine("[DOMINO] Database failed to verify.");
                    return false;
                }
                    
                previousHash = Main.Blocks[i].BlockHash;
            }

            Console.WriteLine("[DOMINO] Database verified successfully.");
            return true;
        }
    }
}
