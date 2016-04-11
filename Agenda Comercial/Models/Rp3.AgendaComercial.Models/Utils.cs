using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.Models
{

    public class Scorer
    {
        public string Target { get; set; }
        public int? Score { get; set; }
        public bool ReadOnly { get; set; }

        public Scorer(int? score, string target = "", bool readOnly = true)
        {
            this.Score = score;
            this.Target = target;
            this.ReadOnly = readOnly;
        }
    }
}
