using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixelDream
{
    public interface IAnimation<T>
    {
        T Value { get; }

        void Step(float dt);
    }
}