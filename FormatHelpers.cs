using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockuMoveEvaluator
{
    public static class FormatHelpers
    {
        public static string TwoDP(this double input)
        {
            input = Math.Round(input, 2);
            input = 100 * input;

            if (input % 100 == 0)
                return (input / 100).ToString();

            if (input % 10 == 0)
                return (input / 100).ToString("0.0");

            return (input/100).ToString("0.00");
        }

        public static string TwoDP(this int input)
        {
            return input.ToString();
        }
    }
}
