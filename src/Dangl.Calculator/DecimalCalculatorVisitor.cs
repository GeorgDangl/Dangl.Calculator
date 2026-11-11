using Antlr4.Runtime;
using System;
using System.Globalization;

namespace Dangl.Calculator
{
    internal sealed class DecimalCalculatorVisitor : CalculatorBaseVisitor<decimal>
    {
        private readonly Func<string, decimal?> _substitutionResolver;
        private readonly Func<RangeSubstitution, decimal?> _rangeResolver;
        private readonly bool _detectUnaryMinusInPowersAtStartOfFormula;

        internal DecimalCalculatorVisitor(Func<string, decimal?> substitutionResolver,
            Func<RangeSubstitution, decimal?> rangeResolver,
            bool detectUnaryMinusInPowersAtStartOfFormula)
        {
            _substitutionResolver = substitutionResolver;
            _rangeResolver = rangeResolver;
            _detectUnaryMinusInPowersAtStartOfFormula = detectUnaryMinusInPowersAtStartOfFormula;
        }

        public override decimal VisitNumber(CalculatorParser.NumberContext context)
        {
            return Evaluate(context, () => decimal.Parse(context.NUMBER().GetText().Replace(",", "."),
                CultureInfo.InvariantCulture));
        }

        public override decimal VisitSubstitution(CalculatorParser.SubstitutionContext context)
        {
            var name = context.GetText();
            var value = _substitutionResolver?.Invoke(name);
            if (!value.HasValue)
            {
                throw new DecimalEvaluationException(context.Start.StartIndex,
                    $"The substitution '{name}' could not be resolved");
            }
            return value.Value;
        }

        public override decimal VisitRange(CalculatorParser.RangeContext context)
        {
            var range = new RangeSubstitution(context.start.Text, context.end.Text);
            var value = _rangeResolver?.Invoke(range);
            if (!value.HasValue)
            {
                throw new DecimalEvaluationException(context.Start.StartIndex,
                    $"The range '{context.GetText()}' could not be resolved");
            }
            return value.Value;
        }

        public override decimal VisitAddSub(CalculatorParser.AddSubContext context)
        {
            return Binary(context, context.expression(0), context.expression(1),
                (left, right) => context.op.Type == CalculatorParser.ADD ? left + right : left - right);
        }

        public override decimal VisitMulDiv(CalculatorParser.MulDivContext context)
        {
            return Binary(context, context.expression(0), context.expression(1),
                (left, right) => context.op.Type == CalculatorParser.MUL ? left * right : left / right);
        }

        public override decimal VisitMult(CalculatorParser.MultContext context)
        {
            return Binary(context, context.expression(0), context.expression(1), (left, right) => left * right);
        }

        public override decimal VisitMod(CalculatorParser.ModContext context)
        {
            return Binary(context, context.expression(0), context.expression(1), (left, right) => left % right);
        }

        public override decimal VisitWhole(CalculatorParser.WholeContext context)
        {
            return Binary(context, context.expression(0), context.expression(1),
                DecimalMath.WholeDivide);
        }

        public override decimal VisitUnary(CalculatorParser.UnaryContext context)
        {
            return Unary(context, context.expression(), value => -value);
        }

        public override decimal VisitUnaryPlus(CalculatorParser.UnaryPlusContext context)
        {
            return Visit(context.expression());
        }

        public override decimal VisitParenthesis(CalculatorParser.ParenthesisContext context)
        {
            return Visit(context.expression());
        }

        public override decimal VisitMin(CalculatorParser.MinContext context)
        {
            var result = Visit(context._expr[0]);
            for (var index = 1; index < context._expr.Count; index++)
            {
                result = Math.Min(result, Visit(context._expr[index]));
            }
            return result;
        }

        public override decimal VisitMax(CalculatorParser.MaxContext context)
        {
            var result = Visit(context._expr[0]);
            for (var index = 1; index < context._expr.Count; index++)
            {
                result = Math.Max(result, Visit(context._expr[index]));
            }
            return result;
        }

        public override decimal VisitAbs(CalculatorParser.AbsContext context)
        {
            return Unary(context, context.expression(), Math.Abs);
        }

        public override decimal VisitFloor(CalculatorParser.FloorContext context)
        {
            return Unary(context, context.expression(), Math.Floor);
        }

        public override decimal VisitCeil(CalculatorParser.CeilContext context)
        {
            return Unary(context, context.expression(), Math.Ceiling);
        }

        public override decimal VisitRound(CalculatorParser.RoundContext context)
        {
            return Unary(context, context.expression(), Math.Round);
        }

        public override decimal VisitRoundk(CalculatorParser.RoundkContext context)
        {
            return Binary(context, context.expression(0), context.expression(1),
                (value, places) => Math.Round(value, Convert.ToInt32(places)));
        }

        public override decimal VisitTrunc(CalculatorParser.TruncContext context)
        {
            return Unary(context, context.expression(), Math.Truncate);
        }

        public override decimal VisitSqr(CalculatorParser.SqrContext context)
        {
            return Unary(context, context.expression(), value => value * value);
        }

        public override decimal VisitSqrt(CalculatorParser.SqrtContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Sqrt);
        }

        public override decimal VisitSqRoot(CalculatorParser.SqRootContext context)
        {
            return Binary(context, context.expression(0), context.expression(1), DecimalMath.Root);
        }

        public override decimal VisitPow(CalculatorParser.PowContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            var invert = _detectUnaryMinusInPowersAtStartOfFormula && left < 0 && context.Start.StartIndex == 0;
            return Evaluate(context, () => invert ? -DecimalMath.Pow(-left, right) : DecimalMath.Pow(left, right));
        }

        public override decimal VisitExponent(CalculatorParser.ExponentContext context)
        {
            return Binary(context, context.expression(0), context.expression(1), DecimalMath.ScaleByPowerOfTen);
        }

        public override decimal VisitNegExponent(CalculatorParser.NegExponentContext context)
        {
            return Binary(context, context.expression(0), context.expression(1),
                (value, exponent) => DecimalMath.ScaleByPowerOfTen(value, -exponent));
        }

        public override decimal VisitEex(CalculatorParser.EexContext context)
        {
            return Unary(context, context.expression(), exponent => DecimalMath.ScaleByPowerOfTen(1m, exponent));
        }

        public override decimal VisitExp(CalculatorParser.ExpContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Exp);
        }

        public override decimal VisitLn(CalculatorParser.LnContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Ln);
        }

        public override decimal VisitLog(CalculatorParser.LogContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Log10);
        }

        public override decimal VisitPi(CalculatorParser.PiContext context)
        {
            return DecimalMath.Pi;
        }

        public override decimal VisitEuler(CalculatorParser.EulerContext context)
        {
            return DecimalMath.E;
        }

        public override decimal VisitSin(CalculatorParser.SinContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Sin);
        }

        public override decimal VisitCos(CalculatorParser.CosContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Cos);
        }

        public override decimal VisitTan(CalculatorParser.TanContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Tan);
        }

        public override decimal VisitCot(CalculatorParser.CotContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Cot);
        }

        public override decimal VisitArcsin(CalculatorParser.ArcsinContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Asin);
        }

        public override decimal VisitArccos(CalculatorParser.ArccosContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Acos);
        }

        public override decimal VisitArctan(CalculatorParser.ArctanContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Atan);
        }

        public override decimal VisitArctan2(CalculatorParser.Arctan2Context context)
        {
            return Binary(context, context.expression(0), context.expression(1), DecimalMath.Atan2);
        }

        public override decimal VisitArccot(CalculatorParser.ArccotContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Acot);
        }

        public override decimal VisitSinh(CalculatorParser.SinhContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Sinh);
        }

        public override decimal VisitCosh(CalculatorParser.CoshContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Cosh);
        }

        public override decimal VisitTanh(CalculatorParser.TanhContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Tanh);
        }

        public override decimal VisitRad(CalculatorParser.RadContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Radians);
        }

        public override decimal VisitDeg(CalculatorParser.DegContext context)
        {
            return Unary(context, context.expression(), DecimalMath.Degrees);
        }

        private decimal Unary(ParserRuleContext context, CalculatorParser.ExpressionContext expression,
            Func<decimal, decimal> operation)
        {
            var value = Visit(expression);
            return Evaluate(context, () => operation(value));
        }

        private decimal Binary(ParserRuleContext context, CalculatorParser.ExpressionContext left,
            CalculatorParser.ExpressionContext right, Func<decimal, decimal, decimal> operation)
        {
            var first = Visit(left);
            var second = Visit(right);
            return Evaluate(context, () => operation(first, second));
        }

        private static decimal Evaluate(ParserRuleContext context, Func<decimal> operation)
        {
            try
            {
                return operation();
            }
            catch (ArithmeticException error)
            {
                throw new DecimalEvaluationException(context.Start.StartIndex, error.Message, error);
            }
            catch (ArgumentOutOfRangeException error)
            {
                throw new DecimalEvaluationException(context.Start.StartIndex, error.Message, error);
            }
            catch (FormatException error)
            {
                throw new DecimalEvaluationException(context.Start.StartIndex, error.Message, error);
            }
        }
    }

    internal sealed class DecimalEvaluationException : Exception
    {
        internal int Position { get; }

        internal DecimalEvaluationException(int position, string message, Exception innerException = null)
            : base(message, innerException)
        {
            Position = position;
        }
    }
}
