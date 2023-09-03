using GF.Extension;

namespace GF.Utils
{
    public class CmdlineUtils
    {
        public static string GetArgValue(string[] args, string name)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var token = args[i];
                if (token.ToLower() == name)
                {
                    if (i < args.Length - 1)
                    {
                        return args[i + 1];
                    }
                }
            }
            return "";
        }

        public static bool HasArg(string[] args, string name)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var token = args[i];
                if (token.ToLower() == name)
                {
                    return true;
                }
            }
            return false;
        }



        public static int GetArgInt(string[] args, string name)
        {
            return GetArgValue(args, name).ToInt();
        }

        public static float GetArgFloat(string[] args, string name)
        {
            return GetArgValue(args, name).ToFloat();
        }

        public static bool GetArgBool(string[] args, string name)
        {
            return GetArgValue(args, name) == "true";
        }


    }
}