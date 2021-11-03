using System;

namespace API.Utilities
{
    public static class Print
    {
        public static void PrettyLog(string theOutput = "", bool theTopOnly = false, bool theBottomOnly = false, bool bothTopAddBottom = false)
        {
            var intSpaces = 82 - theOutput.Length;
            var strSpaces = "";
            for (int i = 0; i < intSpaces; i++)
            {
                strSpaces += " ";
            }

            if (theTopOnly || bothTopAddBottom)
            {
                Console.WriteLine($"+-----------------------------------------------------------------------------------+");   
            }
            if (theOutput != "")
            {
                Console.WriteLine($"+ {theOutput}{strSpaces}+");   
            }
            if (theBottomOnly || bothTopAddBottom)
            {
                Console.WriteLine($"+-----------------------------------------------------------------------------------+");   
            }
        }
    }
}