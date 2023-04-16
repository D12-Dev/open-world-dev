using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class CachedVarHandler
    {






        public static T GetCachedVar<T>(string varName) 
        {
        // get the current value of NextBackupTime
            
            if (Server.cachedVariables == null)
            {
                return default(T);
            }

            var propertyInfo = typeof(CachedVariableFile).GetProperty(varName);
            if (propertyInfo == null)
            {
                return default(T);
            }
            return (T)propertyInfo.GetValue(Server.cachedVariables, null);
        }

        public static void SetCachedVar(string VarToSet, object NewVar)
        {
            if (Server.cachedVariables == null)
            {
                return;
            }

            var propertyInfo = typeof(CachedVariableFile).GetProperty(VarToSet);
            if (propertyInfo == null)
            {
                return;
            }
            propertyInfo.SetValue(Server.cachedVariables, NewVar);
            Serializer.SerializeToFile(Server.cachedVariables, Server.CachedVarsPath);
        }




    }
}
