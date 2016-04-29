using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autod.Core
{
    public class Fad
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="v">Value of this variable</param>
        /// <param name="d">1.0 if this variable is the target of derivative else 0</param>
        public Fad(double v, double d = 0.0)
        {
            Value = v;
            Derivative = d;
        }

        public virtual double Value { get; private set; }
        public virtual double Derivative { get; private set; }

        public static Fad operator+ (Fad left, Fad right)
        {
            return new Fad(left.Value + right.Value, left.Derivative + right.Derivative);
        }

        public static Fad operator- (Fad left, Fad right)
        {
            return new Fad(left.Value - right.Value, left.Derivative - right.Derivative);
        }

        public static Fad operator* (Fad left, Fad right)
        {
            return new Fad(left.Value * right.Value, left.Value * right.Derivative + left.Derivative * right.Value);
        }

        public static Fad operator/ (Fad left, Fad right)
        {
            return new Fad(left.Value / right.Value, left.Derivative / right.Value - left.Value * right.Derivative / right.Value / right.Value);
        }

        // no implicit operator to remove number of variables
        public static Fad operator+(Fad left, double d)
        {
            return new Fad(left.Value + d, left.Derivative);
        }

        public static Fad operator+(double d, Fad right)
        {
            return right + d;
        }

        public static Fad operator-(Fad left, double d)
        {
            return new Fad(left.Value - d, left.Derivative);
        }

        public static Fad operator-(double d, Fad right)
        {
            return new Fad(d - right.Value, -right.Derivative);
        }
        
        public static Fad operator *(double d, Fad right)
        {
            return right * d;
        }

        public static Fad operator*(Fad left, double d)
        {
            return new Fad(left.Value * d, left.Derivative * d);
        }

        public static Fad operator /(Fad left, double d)
        {
            return new Fad(left.Value / d, left.Derivative / d);
        }

        public static Fad operator /(double d, Fad right)
        {
            return new Fad(d / right.Value, -d * right.Derivative / right.Value / right.Value);
        }

        public static Fad Exp(Fad x)
        {
            return new Fad(Math.Exp(x.Value), Math.Exp(x.Value) * x.Derivative);
        }

        public static Fad Sin(Fad x)
        {
            return new Fad(Math.Sin(x.Value), Math.Cos(x.Value) * x.Derivative);
        }

        public static Fad Cos(Fad x)
        {
            return new Fad(Math.Cos(x.Value), Math.Sin(x.Value) * x.Derivative);
        }

        public static Fad Log(Fad x)
        {
            return new Fad(Math.Log(x.Value), x.Derivative / x.Value);
        }

        public static Fad Sqrt(Fad x)
        {
            return new Fad(Math.Sqrt(x.Value), 0.5 * x.Derivative / Math.Sqrt(x.Value));
        }

        public static Fad Tan(Fad x)
        {
            return new Fad(Math.Tan(x.Value), 1.0 + Math.Tan(x.Value) * Math.Tan(x.Value));
        }
        
        public static Fad Max(Fad lhs, Fad rhs)
        {
            return new Fad(Math.Max(lhs.Value, rhs.Value), lhs.Value > rhs.Value ? lhs.Derivative : rhs.Derivative);
        }

        public static Fad Min(Fad lhs, Fad rhs)
        {
            return new Fad(Math.Min(lhs.Value, rhs.Value), lhs.Value < rhs.Value ? lhs.Derivative : rhs.Derivative);
        }
    }
}
