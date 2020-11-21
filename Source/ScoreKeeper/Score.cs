using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ScoreKeeper
{    
    [DataContract]
    class Score
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Points { get; set; }

        [DataMember]
        public byte[] Picture { get; set; }
    }
}
