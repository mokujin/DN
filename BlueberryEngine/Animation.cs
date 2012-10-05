using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueberry
{
    public abstract class Animation<T> 
    {
        public virtual T From { get; protected set; }
        public virtual T To { get; protected set; }
        public virtual T Value { get; protected set; }
    }
}
