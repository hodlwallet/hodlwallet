using System;
namespace HodlWallet2.Core.Utils
{
    public class NullableOperations
    {
        public static bool Evaluate(bool? nullable)
        {
            switch (nullable)
            {
                case true:
                    return true;
                case false:
                case null:
                    return false;
            }
            return false;
        }
    }
}
