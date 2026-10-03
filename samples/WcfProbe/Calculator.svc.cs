using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace WcfProbe
{
    // A behavior of the service class (the converter names CoreWCF's: one instance for every call, which Calls shows), a fault
    // of the contract (CoreWCF answers it as WCF did).
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class Calculator : ICalculator
    {
        int calls;

        public int Calls()
        {
            return Interlocked.Increment(ref calls);
        }

        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Divide(int a, int b)
        {
            if (b == 0)
            {
                throw new FaultException<CalculationFault>(new CalculationFault { Problem = "division by zero", Dividend = a }, "The divisor is zero.");
            }
            return a / b;
        }

        public Order Price(Order order)
        {
            order.Item = order.Item.ToUpperInvariant();
            order.Total = order.Quantity * order.UnitPrice;
            order.Placed = order.Placed.AddDays(1);
            order.Notes = new List<string>(order.Notes ?? new List<string>()) { "priced" };
            return order;
        }
    }
}
