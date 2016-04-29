using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autod.Core
{
    public class Aad
    {
        private int Index { get; set; }
        public double Value { get { return _v.Value; } }
        private Lazy<double> _v;
        private bool IsVariable { get; set; }
        private const int CONST_INDEX_NUM = -100;
        public Aad(double value, bool isConstant = false) : this(() => value, isConstant)
        {
        }

        public Aad(Func<double> val, bool isConstant = false)
        {
            _v = new Lazy<double>(val, true);
            Index = isConstant ? CONST_INDEX_NUM : AadCalculationStack.Instance.NewIndex();
            IsVariable = !isConstant;
        }

        public static implicit operator Aad(double value)
        {
            return new Aad(value, isConstant: true);
        }

        public double Derivative(Aad variable)
        {
            if (!variable.IsVariable) return 0.0;
            var stack = AadCalculationStack.Instance;
            var data = stack.Data.ToArray();
            var vals = new double[Index + 1];
            vals[Index] = 1.0;
            for (var i = 1; i <= data.Length; ++i)
            {
                var derivData = data[data.Length - i];
                if (derivData.DerivTarget <= Index)
                {
                    vals[derivData.DerivBy] += derivData.Value * vals[derivData.DerivTarget];
                }
            }
            return vals[variable.Index];
        }

        public static Aad operator +(Aad left, Aad right)
        {
            var res = new Aad(() => left.Value + right.Value);
            if (left.IsVariable) AadCalculationStack.Instance.Add(left.Index, res.Index, 1.0);
            if (right.IsVariable) AadCalculationStack.Instance.Add(right.Index, res.Index, 1.0);
            return res;
        }

        public static Aad operator -(Aad left, Aad right)
        {
            var res = new Aad(() => left.Value - right.Value);
            if (left.IsVariable) AadCalculationStack.Instance.Add(left.Index, res.Index, 1.0);
            if (right.IsVariable) AadCalculationStack.Instance.Add(right.Index, res.Index, -1.0);
            return res;
        }

        public static Aad operator *(Aad left, Aad right)
        {
            var res = new Aad(() => left.Value * right.Value);
            if (left.IsVariable) AadCalculationStack.Instance.Add(left.Index, res.Index, () => right.Value);
            if (right.IsVariable) AadCalculationStack.Instance.Add(right.Index, res.Index, () => left.Value);
            return res;
        }

        public static Aad operator /(Aad left, Aad right)
        {
            var res = new Aad(() => left.Value / right.Value);
            if (left.IsVariable) AadCalculationStack.Instance.Add(left.Index, res.Index, () => 1.0 / right.Value);
            if (right.IsVariable) AadCalculationStack.Instance.Add(right.Index, res.Index, () => -left.Value / right.Value / right.Value);
            return res;
        }

        public static Aad Exp(Aad x)
        {
            var res = new Aad(() => Math.Exp(x.Value));
            if(x.IsVariable) AadCalculationStack.Instance.Add(x.Index, res.Index, () => Math.Exp(x.Value));
            return res;
        }

        public static Aad Sin(Aad x)
        {
            var res = new Aad(() => Math.Sin(x.Value));
            if (x.IsVariable) AadCalculationStack.Instance.Add(x.Index, res.Index, () => -Math.Cos(x.Value));
            return res;
        }

        public static Aad Cos(Aad x)
        {
            var res = new Aad(() => Math.Cos(x.Value));
            if (x.IsVariable) AadCalculationStack.Instance.Add(x.Index, res.Index, () => Math.Sin(x.Value));
            return res;
        }

        public static Aad Tan(Aad x)
        {
            var res = new Aad(() => Math.Tan(x.Value));
            if (x.IsVariable) AadCalculationStack.Instance.Add(x.Index, res.Index, () => 1.0 + Math.Tan(x.Value) * Math.Tan(x.Value));
            return res;
        }

        public static Aad Max(Aad lhs, Aad rhs)
        {
            var res = new Aad(() => Math.Max(lhs.Value, rhs.Value));
            if (lhs.IsVariable) AadCalculationStack.Instance.Add(lhs.Index, res.Index, () => lhs.Value > rhs.Value ? 1.0 : 0.0);
            if (rhs.IsVariable) AadCalculationStack.Instance.Add(rhs.Index, res.Index, () => rhs.Value > lhs.Value ? 1.0 : 0.0);
            return res;
        }
    }

    public class AadCalculationStack
    {
        private int _numOfVariables = 0;
        static AadCalculationStack()
        {
            Instance = new AadCalculationStack();
        }
        public static AadCalculationStack Instance
        {
            get;
            private set;
        }
        public int NewIndex()
        {
            _numOfVariables++;
            return _numOfVariables;
        }
        private List<AadDerivData> _values = new List<AadDerivData>();
        public void Add(int derivByIndex, int derivTargetIndex, double value)
        {
            Add(derivByIndex, derivTargetIndex, () => value);
        }
        public void Add(int derivByIndex, int derivTargetIndex, Func<double> f)
        {
            _values.Add(new AadDerivData(derivByIndex, derivTargetIndex, f));
        }
        
        public IEnumerable<AadDerivData> Data { get { return _values; } }
    }

    public class AadDerivData
    {
        public int DerivBy { get; private set; }
        public int DerivTarget { get; private set; }
        public double Value { get { return _v.Value; } }
        private readonly Lazy<double> _v;
        public AadDerivData(int derivBy, int derivTarget, double value) : this(derivBy, derivTarget, () => value)
        {
        }

        public AadDerivData(int derivBy, int derivTarget, Func<double> f)
        {
            DerivBy = derivBy;
            DerivTarget = derivTarget;
            _v = new Lazy<double>(f, true);
        }
    }
}
