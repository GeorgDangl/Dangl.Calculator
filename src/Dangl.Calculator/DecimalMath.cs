using System;
using System.Globalization;
using System.Numerics;

namespace Dangl.Calculator
{
    internal static class DecimalMath
    {
        internal const decimal Pi = 3.1415926535897932384626433833m;
        internal const decimal E = 2.7182818284590452353602874714m;

        // Base-10 guard digits preserve decimal precision during range reduction and series evaluation.
        private const int Precision = 80;
        private static readonly BigInteger Scale = BigInteger.Pow(10, Precision);
        private static readonly BigInteger PiFixed = BigInteger.Parse(
            "314159265358979323846264338327950288419716939937510582097494459230781640628620899",
            CultureInfo.InvariantCulture);
        private static readonly BigInteger LnTwo = LogSeries(2 * Scale);
        private static readonly BigInteger LnTen = LogFixed(10 * Scale);
        private static readonly BigInteger MaxCoefficient = new BigInteger(decimal.MaxValue);
        private static readonly BigInteger OverflowLimit = (MaxCoefficient + 1) * Scale;

        internal static decimal Sqrt(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Square roots require a nonnegative value.");
            }
            return ToDecimal(SqrtFixed(ToFixed(value)));
        }

        internal static decimal Pow(decimal value, decimal exponent)
        {
            if (exponent == 0)
            {
                return 1m;
            }
            if (value == 0)
            {
                if (exponent < 0)
                {
                    throw new DivideByZeroException("Zero cannot be raised to a negative power.");
                }
                return 0m;
            }
            if (decimal.Truncate(exponent) == exponent)
            {
                var result = PowIntegerFixed(ToFixed(Math.Abs(value)), exponent);
                return ToDecimal(value < 0 && exponent % 2 != 0 ? -result : result);
            }
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Fractional powers require a nonnegative base.");
            }
            return ToDecimal(ExpFixed(Multiply(ToFixed(exponent), LogFixed(ToFixed(value)))));
        }

        internal static decimal Root(decimal degree, decimal value)
        {
            if (degree == 0)
            {
                throw new DivideByZeroException("A root must have a nonzero degree.");
            }
            if (value == 0)
            {
                if (degree < 0)
                {
                    throw new DivideByZeroException("A negative-degree root of zero is undefined.");
                }
                return 0m;
            }
            if (value < 0 && (decimal.Truncate(degree) != degree || degree % 2 == 0))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "A negative value requires an odd integer root degree.");
            }
            var root = ExpFixed(Divide(LogFixed(ToFixed(Math.Abs(value))), ToFixed(degree)));
            return ToDecimal(value < 0 ? -root : root);
        }

        internal static decimal Exp(decimal value)
        {
            return ToDecimal(ExpFixed(ToFixed(value)));
        }

        internal static decimal ScaleByPowerOfTen(decimal value, decimal exponent)
        {
            if (value == 0 || exponent < -100)
            {
                return 0m;
            }
            if (exponent > 100)
            {
                throw new OverflowException("The result is outside the decimal range.");
            }
            var scaled = ToFixed(value);
            if (decimal.Truncate(exponent) == exponent)
            {
                var power = BigInteger.Pow(10, (int)Math.Abs(exponent));
                return ToDecimal(exponent >= 0 ? scaled * power : scaled / power);
            }
            return ToDecimal(Multiply(scaled, ExpFixed(Multiply(ToFixed(exponent), LnTen))));
        }

        internal static decimal Ln(decimal value)
        {
            return ToDecimal(LogFixed(ToFixed(value)));
        }

        internal static decimal Log10(decimal value)
        {
            return ToDecimal(Divide(LogFixed(ToFixed(value)), LnTen));
        }

        internal static decimal Sin(decimal value)
        {
            return ToDecimal(SinFixed(ToFixed(value)));
        }

        internal static decimal Cos(decimal value)
        {
            return ToDecimal(CosFixed(ToFixed(value)));
        }

        internal static decimal Tan(decimal value)
        {
            var angle = ToFixed(value);
            return ToDecimal(Divide(SinFixed(angle), CosFixed(angle)));
        }

        internal static decimal Cot(decimal value)
        {
            var angle = ToFixed(value);
            return ToDecimal(Divide(CosFixed(angle), SinFixed(angle)));
        }

        internal static decimal Asin(decimal value)
        {
            ValidateInverseTrig(value);
            var argument = ToFixed(value);
            return ToDecimal(Atan2Fixed(argument, SqrtFixed(Multiply(Scale - argument, Scale + argument))));
        }

        internal static decimal Acos(decimal value)
        {
            ValidateInverseTrig(value);
            var argument = ToFixed(value);
            return ToDecimal(Atan2Fixed(SqrtFixed(Multiply(Scale - argument, Scale + argument)), argument));
        }

        internal static decimal Atan(decimal value)
        {
            return ToDecimal(AtanFixed(ToFixed(value)));
        }

        internal static decimal Atan2(decimal y, decimal x)
        {
            return ToDecimal(Atan2Fixed(ToFixed(y), ToFixed(x)));
        }

        internal static decimal Acot(decimal value)
        {
            return ToDecimal(PiFixed / 2 - AtanFixed(ToFixed(value)));
        }

        internal static decimal Sinh(decimal value)
        {
            var argument = ToFixed(value);
            return ToDecimal((ExpFixed(argument) - ExpFixed(-argument)) / 2);
        }

        internal static decimal Cosh(decimal value)
        {
            var argument = ToFixed(value);
            return ToDecimal((ExpFixed(argument) + ExpFixed(-argument)) / 2);
        }

        internal static decimal Tanh(decimal value)
        {
            var exponential = ExpFixed(-2 * BigInteger.Abs(ToFixed(value)));
            var result = Divide(Scale - exponential, Scale + exponential);
            return ToDecimal(value < 0 ? -result : result);
        }

        internal static decimal Radians(decimal value)
        {
            return ToDecimal(ToFixed(value) * PiFixed / (180 * Scale));
        }

        internal static decimal Degrees(decimal value)
        {
            return ToDecimal(ToFixed(value) * 180 * Scale / PiFixed);
        }

        private static void ValidateInverseTrig(decimal value)
        {
            if (value < -1 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Inverse sine and cosine require a value between -1 and 1.");
            }
        }

        private static BigInteger Multiply(BigInteger left, BigInteger right)
        {
            return left * right / Scale;
        }

        private static BigInteger Divide(BigInteger left, BigInteger right)
        {
            return left * Scale / right;
        }

        private static BigInteger ToFixed(decimal value)
        {
            var bits = decimal.GetBits(value);
            var coefficient = (new BigInteger((uint)bits[2]) << 64)
                + (new BigInteger((uint)bits[1]) << 32) + (uint)bits[0];
            var places = (bits[3] >> 16) & 0xff;
            var result = coefficient * (Scale / BigInteger.Pow(10, places));
            return bits[3] < 0 ? -result : result;
        }

        private static decimal ToDecimal(BigInteger value)
        {
            var magnitude = BigInteger.Abs(value);
            var divisor = BigInteger.Pow(10, Precision - 28);
            for (var places = 28; places >= 0; places--, divisor *= 10)
            {
                var coefficient = BigInteger.DivRem(magnitude, divisor, out var remainder);
                var midpoint = (remainder * 2).CompareTo(divisor);
                if (midpoint > 0 || (midpoint == 0 && !coefficient.IsEven))
                {
                    coefficient++;
                }
                if (coefficient <= MaxCoefficient)
                {
                    return new decimal(
                        unchecked((int)(uint)(coefficient & uint.MaxValue)),
                        unchecked((int)(uint)((coefficient >> 32) & uint.MaxValue)),
                        unchecked((int)(uint)(coefficient >> 64)),
                        value.Sign < 0, (byte)places);
                }
            }
            throw new OverflowException("The result is outside the decimal range.");
        }

        private static BigInteger SqrtFixed(BigInteger value)
        {
            if (value.IsZero)
            {
                return BigInteger.Zero;
            }
            var radicand = value * Scale;
            var estimate = BigInteger.One << ((radicand.ToByteArray().Length * 8 + 1) / 2);
            while (true)
            {
                var next = (estimate + radicand / estimate) / 2;
                if (next >= estimate)
                {
                    return estimate;
                }
                estimate = next;
            }
        }

        private static BigInteger PowIntegerFixed(BigInteger value, decimal exponent)
        {
            var remaining = BigInteger.Abs(new BigInteger(exponent));
            var factor = exponent < 0 ? Divide(Scale, value) : value;
            var result = Scale;
            while (remaining > 0)
            {
                if (!remaining.IsEven)
                {
                    result = Multiply(result, factor);
                    if (result >= OverflowLimit)
                    {
                        throw new OverflowException("The result is outside the decimal range.");
                    }
                }
                remaining /= 2;
                if (remaining > 0)
                {
                    factor = Multiply(factor, factor);
                    if (factor >= OverflowLimit)
                    {
                        throw new OverflowException("The result is outside the decimal range.");
                    }
                }
            }
            return result;
        }

        private static BigInteger ExpFixed(BigInteger value)
        {
            // These bounds also cover intermediates in powers, scientific notation and hyperbolic functions.
            if (value < -200 * Scale)
            {
                return BigInteger.Zero;
            }
            if (value > 200 * Scale)
            {
                throw new OverflowException("The result is outside the decimal range.");
            }
            var power = (int)(value / LnTwo);
            var reduced = value - power * LnTwo;
            var term = Scale;
            var sum = Scale;
            for (var index = 1; !term.IsZero; index++)
            {
                term = Multiply(term, reduced) / index;
                sum += term;
            }
            return power >= 0 ? sum << power : sum / (BigInteger.One << -power);
        }

        private static BigInteger LogFixed(BigInteger value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Logarithms require a positive value.");
            }
            var power = 0;
            while (value >= 2 * Scale)
            {
                value /= 2;
                power++;
            }
            while (value < Scale)
            {
                value *= 2;
                power--;
            }
            return LogSeries(value) + power * LnTwo;
        }

        private static BigInteger LogSeries(BigInteger value)
        {
            var ratio = Divide(value - Scale, value + Scale);
            var square = Multiply(ratio, ratio);
            var term = ratio;
            var sum = ratio;
            for (var index = 3; !term.IsZero; index += 2)
            {
                term = Multiply(term, square);
                sum += term / index;
            }
            return 2 * sum;
        }

        private static BigInteger ReduceAngle(BigInteger value)
        {
            value %= 2 * PiFixed;
            if (value > PiFixed)
            {
                value -= 2 * PiFixed;
            }
            if (value < -PiFixed)
            {
                value += 2 * PiFixed;
            }
            return value;
        }

        private static BigInteger SinFixed(BigInteger value)
        {
            value = ReduceAngle(value);
            var square = Multiply(value, value);
            var term = value;
            var sum = value;
            for (var index = 1; !term.IsZero; index++)
            {
                term = -Multiply(term, square) / ((2 * index) * (2 * index + 1));
                sum += term;
            }
            return sum;
        }

        private static BigInteger CosFixed(BigInteger value)
        {
            value = ReduceAngle(value);
            var square = Multiply(value, value);
            var term = Scale;
            var sum = Scale;
            for (var index = 1; !term.IsZero; index++)
            {
                term = -Multiply(term, square) / ((2 * index - 1) * (2 * index));
                sum += term;
            }
            return sum;
        }

        private static BigInteger AtanFixed(BigInteger value)
        {
            if (value.Sign < 0)
            {
                return -AtanFixed(-value);
            }
            if (value > Scale)
            {
                return PiFixed / 2 - AtanFixed(Divide(Scale, value));
            }
            var multiplier = 1;
            while (value > Scale / 8)
            {
                value = Divide(value, Scale + SqrtFixed(Scale + Multiply(value, value)));
                multiplier *= 2;
            }
            var square = Multiply(value, value);
            var term = value;
            var sum = value;
            for (var index = 3; !term.IsZero; index += 2)
            {
                term = -Multiply(term, square);
                sum += term / index;
            }
            return multiplier * sum;
        }

        private static BigInteger Atan2Fixed(BigInteger y, BigInteger x)
        {
            if (x > 0)
            {
                return AtanFixed(Divide(y, x));
            }
            if (x < 0)
            {
                return AtanFixed(Divide(y, x)) + (y.Sign < 0 ? -PiFixed : PiFixed);
            }
            return y.Sign * (PiFixed / 2);
        }
    }
}
