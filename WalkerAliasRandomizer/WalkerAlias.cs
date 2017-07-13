using System;
using System.Collections.Generic;
using System.Linq;

namespace WalkerAliasRandomizer
{
    public class WalkerAlias
    {
        private readonly int _weightLength;
        private readonly List<int> _inx;
        private readonly List<double> _probabilities;
        private readonly List<int> _short;
        private readonly List<int> _long;
        private readonly IList<KeyValuePair<string, int>> _values;
        private static readonly Random Random = new Random();

        public WalkerAlias(IList<KeyValuePair<string, int>> values)
        {
            _weightLength = values.Count();
            var sumw = values.Select(x => x.Value).Sum();
            _inx = new List<int>();
            _probabilities = new List<double>();
            _short = new List<int>();
            _long = new List<int>();
            _values = values;

            foreach (var value in _values)
            {
                _inx.Add(-1);

                var probability = (double)value.Value * _weightLength / sumw;

                _probabilities.Add(probability);
            }

            for (var i = 0; i < _probabilities.Count(); i++)
            {
                if (_probabilities[i] < 1)
                    _short.Add(i);
                if (_probabilities[i] > 1)
                    _long.Add(i);
            }

            while (_short.Any() && _long.Any())
            {
                var currentShort = _short.Pop();
                var currentLong = _long.Last();

                _inx[currentShort] = currentLong;
                _probabilities[currentLong] -= (1 - _probabilities[currentShort]);

                if (_probabilities[currentLong] < 1)
                {
                    _short.Add(currentLong);
                    _long.Pop();
                }
            }
        }

        public string GetSelection()
        {
            var nextRandom = Random.NextDouble();
            var weightedRandom = GetRandom(0, _weightLength - 1);

            var selectionIndex = _probabilities[weightedRandom] >= nextRandom
                                 ? weightedRandom
                                 : _inx[weightedRandom];

            return _values[selectionIndex].Key;
        }

        private static int GetRandom(int min, int max)
        {
            return Convert.ToInt32((Math.Floor(Random.NextDouble() * (max - min + 1))) + min);
        }
    }
}
