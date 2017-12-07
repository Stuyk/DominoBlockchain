using GrandTheftMultiplayer.Server.API;
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
            API.shared.consoleOutput($"{Main.Domino} Verifying all previous blocks...");
            string previousHash = null;

            for (int i = 0; i < DataHandler.Blocks.Length; i++)
            {
                if (previousHash == null)
                {
                    previousHash = DataHandler.Blocks[0].PreviousHash;
                }

                if (DataHandler.Blocks[i].PreviousHash != previousHash)
                {
                    API.shared.consoleOutput($"{Main.Domino} The database failed to verify, stopping server in 5 seconds.");
                    API.shared.delay(5000, true, () =>
                    {
                        API.shared.stopServer();
                    });
                    return false;
                }
                    
                previousHash = DataHandler.Blocks[i].BlockHash;
            }

            API.shared.consoleOutput($"{Main.Domino} The database has verified successfully.");
            return true;
        }
    }
}
