using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueberry
{
    public class IntAnimation
    {
        private int _from;
        private int _to;
        private int _value;
        private bool _loop;
        private double _temp; // время, прошедшее с последнего
        private double _period;

        public int From { get { return _from; } }

        public int To { get { return _to; } }

        public int Value { get { return _value; } }

        public bool Loop { get { return _loop; } }

        public double Period { get { return _period; } }

        public IntAnimation(int from, int to, double period, bool loop)
        {
            _from = from;
            _to = to;
            _period = period;
            _loop = loop;
            _value = _from;
        }

        public void Reset()
        {
            _value = _from;
            _temp = 0;
        }

        public void Animate(double dtime)
        {
            _temp += dtime;
            //_value += (int)(_temp / _period);
            if (_from < _to)
                _value += (int)(_temp / _period);
            else
                _value -= (int)(_temp / _period);
            _temp = _temp % _period;
            if (_loop)
                if ((_from < _to && _value > _to) || (_from > _to && _value < _to))
                    _value = _from;
        }
    }
}