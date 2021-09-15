using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace StockScany
{
    public interface IPaymentService
    {
        string PhonePE(Context context);

        string CashfreePayment(Context context, string cftoken, string orderId, string orderAmount, string appID,
            string customerName, string customerPhone, string customerEmail);
    }
}