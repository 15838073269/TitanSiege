namespace MySqlDataBuild
{
    public class StringEntiy
    {
        public string source;
        public string newString;

        public StringEntiy(string source, string newString)
        {
            this.source = source;
            this.newString = newString;
        }

        public override string ToString()
        {
            return newString;
        }
    }
}
