using CollabApp.mvc.Utilities;
using System.ComponentModel.DataAnnotations;
using CollabApp.mvc.Controllers;

namespace CollabApp.mvc.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }    
        public string Sender { get; set; }
        public string Group { get; set; }
        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged(value);
            }
        }
        public DateTime SentAt { get; set; } =  DateTime.UtcNow; 

        public Message()
        {
            this.Id = IdGenerator.GenerateMessageId();
        }

        private void OnPropertyChanged(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                return; // Do nothing if input is invalid
            }

            _content = newValue; // Update _content with the new value
        }
    }
}
