using System;

namespace Web.Models
{
    public class MessageInfo
    {
        public string Text { get; set; }

        public string Date { get; set; }

        public MessageInfo()
        {
            
        }

        /// <inheritdoc />
        public MessageInfo(string text)
        {
            Text = text;
            Date = DateTime.Now.ToString("g");
        }
    }
}
