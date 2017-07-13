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
        private readonly IList<KeyValuePair<string, int>> _values;
        private static readonly Random Random = new Random();

        public WalkerAlias(IList<KeyValuePair<string, int>> values)
        {
            _weightLength = values.Count();
            _values = values;
            _inx = new List<int>();
            _probabilities = new List<double>();

            var cumulativeSum = values.Select(x => x.Value).Sum();
            var shortOptions = new List<int>();
            var longOptions = new List<int>();

            foreach (var value in _values)
            {
                _inx.Add(-1);

                var probability = (double)value.Value * _weightLength / cumulativeSum;

                _probabilities.Add(probability);
            }

            for (var i = 0; i < _probabilities.Count(); i++)
            {
                if (_probabilities[i] < 1)
                    shortOptions.Add(i);
                if (_probabilities[i] > 1)
                    longOptions.Add(i);
            }

            while (shortOptions.Any() && longOptions.Any())
            {
                var currentShort = shortOptions.Pop();
                var currentLong = longOptions.Last();

                _inx[currentShort] = currentLong;
                _probabilities[currentLong] -= (1 - _probabilities[currentShort]);

                if (_probabilities[currentLong] < 1)
                {
                    shortOptions.Add(currentLong);
                    longOptions.Pop();
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
