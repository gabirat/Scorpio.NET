using System;

namespace Scorpio.GUI.Utils
{
    public static class Extensions
    {
        public static void GuardNotNull(this object obj, string name)
        {
            if(obj is null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
