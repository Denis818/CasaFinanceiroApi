﻿using System.Globalization;

namespace Domain.Extensions.Help
{
    public static class HelpExtensios
    {
        public static double RoundTo(this double soucer, int doublePlaces)
        {
            if(soucer <= 0)
                return soucer;

            return Math.Round(soucer, doublePlaces);
        }

        public static double RountToZeroIfNegative(this double soucer) => Math.Max(soucer, 0);

        public static string ToFormatPriceBr(this double soucer) =>
            soucer.ToString("F2", new CultureInfo("pt-BR"));
    }
}
