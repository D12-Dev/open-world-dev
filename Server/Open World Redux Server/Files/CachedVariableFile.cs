using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class CachedVariableFile
    {
        public DateTime _nextBackupTime = DateTime.Now;

        public DateTime NextBackupTime
        {
            get { return _nextBackupTime; }
            set { _nextBackupTime = value; }
        }
    }
}
