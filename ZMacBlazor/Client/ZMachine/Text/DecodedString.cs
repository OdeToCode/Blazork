namespace ZMacBlazor.Client.ZMachine.Text
{
    public class DecodedString
    {
        public DecodedString()
        {
            Text = "";
        }

        public string Text { get; set; }
        public int BytesConsumed { get; set; }
    }
}
