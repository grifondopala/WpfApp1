using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataModel
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { set; get; }
        public int IDSender { set; get; }
        public string TextMessage { set; get; }
        public DateTime SendingTime { set; get; }
        public int IsCheckedByRecipient {set; get; }
    }
}
