using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _12_Stop_And_Smell_The_Flowers
{
    class BeeEventArgs : EventArgs
    {
        public int Id { get; private set; }
        public string Message {get; private set;}

        public BeeEventArgs(int id, string message)
        {
            this.Id = id;
            this.Message = message;
        }
    }
}
