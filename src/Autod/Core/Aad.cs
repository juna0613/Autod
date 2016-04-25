using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autod.Core
{
    public class Aad
    {
		public int Index { get; private set; }
		public double Value { get; private set; }

		public Aad(double value)
		{
			Value = value;
			Index = AadCalculationStack.Instance.NewIndex ();
		}

		public double Derivative(Aad variable)
		{
			var stack = AadCalculationStack.Instance;
			var data = stack.Data.ToArray();
            var vals = new double[Index + 1];
			vals [Index] = 1.0;
			for (var i = 1; i <= data.Length; ++i) 
			{
                var derivData = data [data.Length - i];
				if (derivData.DerivTarget <= Index) 
				{
					vals [derivData.DerivBy] += derivData.Value * vals [derivData.DerivTarget];
				}
			}
			return vals [variable.Index];
		}

		public static Aad operator+(Aad left, Aad right)
		{
			var res = new Aad (left.Value + right.Value);
			AadCalculationStack.Instance.Add (left.Index, res.Index, 1.0);
			AadCalculationStack.Instance.Add (right.Index, res.Index, 1.0);
			return res;
		}

		public static Aad operator-(Aad left, Aad right)
		{
			var res = new Aad (left.Value - right.Value);
			AadCalculationStack.Instance.Add (left.Index, res.Index, 1.0);
			AadCalculationStack.Instance.Add (right.Index, res.Index, -1.0);
			return res;
		}

		public static Aad operator*(Aad left, Aad right)
		{
			var res = new Aad (left.Value * right.Value);
			AadCalculationStack.Instance.Add (left.Index, res.Index, right.Value);
			AadCalculationStack.Instance.Add (right.Index, res.Index, left.Value);
			return res;
		}

		public static Aad operator/(Aad left, Aad right)
		{
			var res = new Aad (left.Value / right.Value);
			AadCalculationStack.Instance.Add (left.Index, res.Index, 1.0 / right.Value);
			AadCalculationStack.Instance.Add (right.Index, res.Index, -left.Value / right.Value / right.Value);
			return res;
		}

		public static implicit operator Aad(double x)
		{
			return new Aad (x);
		}

		public static implicit operator Aad(int x)
		{
			return new Aad (x);
		}

		public static implicit operator Aad (float x)
		{
			return new Aad (x);
		}
    }

	public class AadCalculationStack
	{
		private int _numOfVariables = 0;
		static AadCalculationStack()
		{
			Instance = new AadCalculationStack();
		}
		public static AadCalculationStack Instance {
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
			_values.Add (new AadDerivData (derivByIndex, derivTargetIndex, value));
		}
		public IEnumerable<AadDerivData> Data { get { return _values; } }
	}

	public class AadDerivData
	{
		public int DerivBy { get; private set; }
		public int DerivTarget { get; private set; }
		public double Value { get ; private set; }
		public AadDerivData(int derivBy, int derivTarget, double value)
		{
			DerivBy = derivBy;
			DerivTarget = derivTarget;
			Value = value;
		}
	}
}
