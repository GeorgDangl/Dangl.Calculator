using Antlr4.Runtime.Misc;
using System;
using System.Globalization;

namespace Dangl.Calculator
{
    /// <summary>
    ///     This is the visitor to actually perform the mathematical calculation of a given expression.
    /// </summary>
    internal class CalculatorVisitor : CalculatorBaseVisitor<double>
    {
        private readonly Func<string, double?> _substitutionResolver;
        private readonly Func<RangeSubstitution, double?> _rangeResolver;
        private readonly CalculatorErrorListener _calculatorErrorListener;

        public CalculatorVisitor(Func<string, double?> substitutionResolver,
            Func<RangeSubstitution, double?> rangeResolver,
            CalculatorErrorListener calculatorErrorListener)
        {
            _substitutionResolver = substitutionResolver;
            _rangeResolver = rangeResolver;
            _calculatorErrorListener = calculatorErrorListener;
        }

        public override double VisitSubstitution([NotNull] CalculatorParser.SubstitutionContext context)
        {
            var substitution = context.GetText();
            var resolved = _substitutionResolver(substitution);
            if (resolved != null)
            {
                return resolved.Value;
            }

            _calculatorErrorListener
                .ReportSubstitutionNotFound(context.Start.TokenIndex, substitution);
            return 0;
        }

        public override double VisitRange([NotNull] CalculatorParser.RangeContext context)
        {
            var start = context.start?.Text;
            if (string.IsNullOrWhiteSpace(start))
            {
                _calculatorErrorListener
                    .ReportRangeNotFound(context.Start.TokenIndex, context.GetText());
                return 0;
            }

            var end = context.end?.Text;
            if (string.IsNullOrWhiteSpace(end))
            {
                _calculatorErrorListener
                    .ReportRangeNotFound(context.Start.TokenIndex, context.GetText());
                return 0;
            }

            var rangeSubstitution = new RangeSubstitution(start, end);
            var resolved = _rangeResolver(rangeSubstitution);
            if (resolved != null)
            {
                return resolved.Value;
            }

            _calculatorErrorListener
                .ReportRangeNotFound(context.Start.TokenIndex, context.GetText());
            return 0;
        }

        public override double VisitMin([NotNull] CalculatorParser.MinContext context)
        {
            var currentMin = Visit(context._expr[0]);

            if (context._expr.Count > 1)
            {
                for (var i = 1; i < context._expr.Count; i++)
                {
                    currentMin = Math.Min(currentMin, Visit(context._expr[i]));
                }
            }
            
            return currentMin;
        }

        public override double VisitMax([NotNull] CalculatorParser.MaxContext context)
        {
            var currentMax = Visit(context._expr[0]);

            if (context._expr.Count > 1)
            {
                for (var i = 1; i < context._expr.Count; i++)
                {
                    currentMax = Math.Max(currentMax, Visit(context._expr[i]));
                }
            }

            return currentMax;
        }

        public override double VisitAbs(CalculatorParser.AbsContext context)
        {
            return Math.Abs(Visit(context.expression()));
        }

        public override double VisitAddSub(CalculatorParser.AddSubContext context)
        {
            if (context.op.Type == CalculatorParser.ADD)
            {
                return Visit(context.expression(0)) + Visit(context.expression(1));
            }
            return Visit(context.expression(0)) - Visit(context.expression(1));
        }

        public override double VisitArccos(CalculatorParser.ArccosContext context)
        {
            return Math.Acos(Visit(context.expression()));
        }

        public override double VisitArccot(CalculatorParser.ArccotContext context)
        {
            return Math.PI / 2 - Math.Atan(Visit(context.expression()));
        }

        public override double VisitArcsin(CalculatorParser.ArcsinContext context)
        {
            return Math.Asin(Visit(context.expression()));
        }

        public override double VisitArctan(CalculatorParser.ArctanContext context)
        {
            return Math.Atan(Visit(context.expression()));
        }

        public override double VisitArctan2(CalculatorParser.Arctan2Context context)
        {
            return Math.Atan2(Visit(context.expression(0)), Visit(context.expression(1)));
        }

        public override double VisitCeil(CalculatorParser.CeilContext context)
        {
            return Math.Ceiling(Visit(context.expression()));
        }

        public override double VisitCos(CalculatorParser.CosContext context)
        {
            return Math.Cos(Visit(context.expression()));
        }

        public override double VisitCosh(CalculatorParser.CoshContext context)
        {
            return Math.Cosh(Visit(context.expression()));
        }

        public override double VisitCot(CalculatorParser.CotContext context)
        {
            return 1 / Math.Tan(Visit(context.expression()));
        }

        public override double VisitDeg(CalculatorParser.DegContext context)
        {
            return Visit(context.expression()) * 360 / (2 * Math.PI);
        }

        public override double VisitEex(CalculatorParser.EexContext context)
        {
            return Math.Pow(10, Visit(context.expression()));
        }

        public override double VisitExponent(CalculatorParser.ExponentContext context)
        {
            return Visit(context.expression(0)) * Math.Pow(10, Visit(context.expression(1)));
        }

        public override double VisitEuler(CalculatorParser.EulerContext context)
        {
            return Math.E;
        }

        public override double VisitExp(CalculatorParser.ExpContext context)
        {
            return Math.Pow(Math.E, Visit(context.expression()));
        }

        public override double VisitFloor(CalculatorParser.FloorContext context)
        {
            return Math.Floor(Visit(context.expression()));
        }

        public override double VisitLn(CalculatorParser.LnContext context)
        {
            return Math.Log(Visit(context.expression()));
        }

        public override double VisitLog(CalculatorParser.LogContext context)
        {
            return Math.Log10(Visit(context.expression()));
        }

        public override double VisitMod(CalculatorParser.ModContext context)
        {
            return Visit(context.expression(0)) % Visit(context.expression(1));
        }

        public override double VisitMulDiv(CalculatorParser.MulDivContext context)
        {
            if (context.op.Type == CalculatorParser.MUL)
            {
                return Visit(context.expression(0)) * Visit(context.expression(1));
            }
            return Visit(context.expression(0)) / Visit(context.expression(1));
        }

        public override double VisitMult([NotNull] CalculatorParser.MultContext context)
        {
            return Visit(context.expression(0)) * Visit(context.expression(1));
        }

        public override double VisitNegExponent(CalculatorParser.NegExponentContext context)
        {
            return Visit(context.expression(0)) * Math.Pow(10, -Visit(context.expression(1)));
        }

        public override double VisitNumber(CalculatorParser.NumberContext context)
        {
            var numberAsText = context.NUMBER().GetText().Replace(",", ".");
            return double.Parse(numberAsText, CultureInfo.InvariantCulture);
        }

        public override double VisitParenthesis(CalculatorParser.ParenthesisContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitPi(CalculatorParser.PiContext context)
        {
            return Math.PI;
        }

        public override double VisitPow(CalculatorParser.PowContext context)
        {
            return Math.Pow(Visit(context.expression(0)), Visit(context.expression(1)));
        }

        public override double VisitRad(CalculatorParser.RadContext context)
        {
            return Visit(context.expression()) * 2 * Math.PI / 360;
        }

        public override double VisitRound(CalculatorParser.RoundContext context)
        {
            return Math.Round(Visit(context.expression()));
        }

        public override double VisitRoundk(CalculatorParser.RoundkContext context)
        {
            return Math.Round(Visit(context.expression(0)), Convert.ToInt32(Visit(context.expression(1))));
        }

        public override double VisitSin(CalculatorParser.SinContext context)
        {
            return Math.Sin(Visit(context.expression()));
        }

        public override double VisitSinh(CalculatorParser.SinhContext context)
        {
            return Math.Sinh(Visit(context.expression()));
        }

        public override double VisitSqr(CalculatorParser.SqrContext context)
        {
            return Visit(context.expression()) * Visit(context.expression());
        }

        public override double VisitSqRoot(CalculatorParser.SqRootContext context)
        {
            return Math.Pow(Visit(context.expression(1)), 1 / Visit(context.expression(0)));
        }

        public override double VisitSqrt(CalculatorParser.SqrtContext context)
        {
            return Math.Sqrt(Visit(context.expression()));
        }

        public override double VisitTan(CalculatorParser.TanContext context)
        {
            return Math.Tan(Visit(context.expression()));
        }

        public override double VisitTanh(CalculatorParser.TanhContext context)
        {
            return Math.Tanh(Visit(context.expression()));
        }

        public override double VisitTrunc(CalculatorParser.TruncContext context)
        {
            return Math.Truncate(Visit(context.expression()));
        }

        public override double VisitUnary(CalculatorParser.UnaryContext context)
        {
            return -1 * Visit(context.expression());
        }

        public override double VisitUnaryPlus([NotNull] CalculatorParser.UnaryPlusContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitWhole(CalculatorParser.WholeContext context)
        {
            return Math.Truncate(Visit(context.expression(0)) / Visit(context.expression(1)));
        }
    }
}
