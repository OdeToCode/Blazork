namespace ZMacBlazor.Client.ZMachine.Text
{
    public class DecodingResult
    {
        public DecodingResult()
        {
            Text = "";
        }

        public string Text { get; set; }
        public int BytesConsumed { get; set; }
    }
}
