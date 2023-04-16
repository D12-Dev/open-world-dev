using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace OpenWorldReduxServer
{
    public static class BackupHandler
    {
        public static void CreateServerBackupHeartbeat() {

            
            while (true) 
            {
                
                DateTime NextBackupDateTime = CachedVarHandler.GetCachedVar<DateTime>("NextBackupTime");
                Thread.Sleep(10000); // Every 10 seconds run this code.
                if (DateTime.Now >= NextBackupDateTime ) // Should backup?
                {
                    CreateBackup();
                    CachedVarHandler.SetCachedVar("NextBackupTime", DateTime.Now.AddHours(Server.serverConfig.BackUpIntervalInHours));
                

                }




            }
        }





        public static void CreateBackup() {
            try
            {
                string CurDate = DateTime.Now.ToString("yyyy-MM-dd");
                string CurTime = DateTime.Now.ToString("hh.mm.ss");
                string newDir = Server.BackupFolderPath + Path.DirectorySeparatorChar + "Backup Date " + CurDate + " Time " + CurTime;
                Directory.CreateDirectory(newDir); /// Backup Date 2023-04-16 Time 04-09-45

                List<string> directoriesToCopy = new List<string>
                {
                    Server.logsFolderPath,
                    Server.playersFolderPath,
                    Server.savesFolderPath,
                    Server.settlementsFolderPath,
                    Server.factionsFolderPath,
                    Server.dataFolderPath,
                };
                foreach (string X in directoriesToCopy)
                { // Search through each Directory.
                    foreach (string Y in Directory.GetFiles(X))
                    { /// Search Through All files in that Directory
                        if (!Directory.Exists(newDir + Path.DirectorySeparatorChar + Path.GetFileName(X))) { Directory.CreateDirectory(newDir + Path.DirectorySeparatorChar + Path.GetFileName(X)); } // Copy Directory over to backup
                        //ServerHandler.WriteToConsole(newDir + Path.DirectorySeparatorChar + Path.GetFileName(X) + Path.GetFileName(Y), ServerHandler.LogMode.Title);
                        File.Copy(Y, (newDir + Path.DirectorySeparatorChar + Path.GetFileName(X) + Path.DirectorySeparatorChar + Path.GetFileName(Y)), true); // Copy file to backup folder dir


                    }
                    foreach (string ChildDir in Directory.GetDirectories(X)) /// Search Through All Directories in that Directory
                    {
                        foreach (string ChildsFile in Directory.GetFiles(ChildDir))
                        { /// Search Through All files in that Directory
                            if (!Directory.Exists(newDir + Path.DirectorySeparatorChar + Path.GetFileName(X) + Path.DirectorySeparatorChar + Path.GetFileName(ChildDir))) { Directory.CreateDirectory(newDir + Path.DirectorySeparatorChar + Path.GetFileName(X) + Path.DirectorySeparatorChar + Path.GetFileName(ChildDir)); } // Copy Directory over to backup
                                                                                                                                                                                                                                                                                                                                //ServerHandler.WriteToConsole(newDir + Path.DirectorySeparatorChar + Path.GetFileName(X) + Path.GetFileName(Y), ServerHandler.LogMode.Title);
                            File.Copy(ChildsFile, (newDir + Path.DirectorySeparatorChar + Path.GetFileName(X) + Path.DirectorySeparatorChar + Path.GetFileName(ChildDir) + Path.DirectorySeparatorChar + Path.GetFileName(ChildsFile)), true); // Copy file to backup folder dir


                        }

                    };


                };
            }
            catch (Exception Ex) { ServerHandler.WriteToConsole($"Failed to backup save!\nFull stack trace: {Ex}", ServerHandler.LogMode.Error); }
            ServerHandler.WriteToConsole("Backed up save!", ServerHandler.LogMode.Title);
        }
        
        
 




    }
}
