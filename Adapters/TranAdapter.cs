using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockScany.Classes;
using Xamarin.Essentials;

namespace StockScany
{
    class TranAdapter : BaseAdapter
    {

        Context context;
        List<TransItem> items;

        public TranAdapter(Context context, List<TransItem> items)
        {
            this.context = context;
            this.items = items;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        { 
            var view = convertView;
            var item = items[position];
            TranAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as TranAdapterViewHolder;

            if (holder == null)
            {
                holder = new TranAdapterViewHolder();

                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                view = inflater.Inflate(Resource.Layout.tran_list, parent, false);
                //replace with your item and your holder items
                //comment back in
                //view = inflater.Inflate(Resource.Layout.item, parent, false);
                //holder.Title = view.FindViewById<TextView>(Resource.Id.text);
                view.FindViewById<TextView>(Resource.Id.txtDate).Text = item.transDate;
                view.FindViewById<TextView>(Resource.Id.txtOrderId).Text = item.orderId;
                view.FindViewById<TextView>(Resource.Id.txtAmount).Text = item.orderAmount.ToString();
                view.FindViewById<TextView>(Resource.Id.txtStatus).Text = item.status;
                if (position == 0)
                {
                    view.FindViewById<TextView>(Resource.Id.txtDate).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Rgb(70,130,180)));
                    view.FindViewById<TextView>(Resource.Id.txtOrderId).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Rgb(70, 130, 180)));
                    view.FindViewById<TextView>(Resource.Id.txtAmount).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Rgb(70, 130, 180)));
                    view.FindViewById<TextView>(Resource.Id.txtStatus).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Rgb(70, 130, 180)));
                }
                view.Tag = holder;
            }

            //fill in your items
            //holder.Title.Text = "new text here";            
            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

    }

    class TranAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}