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
using Android.Graphics;

namespace StockScany.Adapters
{
    class IndexAdapter : BaseAdapter
    {
        List<IndexItem> items;
        Context context;        
        public IndexAdapter(Context context, List<IndexItem> items)
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
            IndexAdapterViewHolder holder = null;
            IndexItem item = items[position];

            if (view != null)
                holder = view.Tag as IndexAdapterViewHolder;

            if (holder == null)
            {
                holder = new IndexAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                //view = inflater.Inflate(Resource.Layout.item, parent, false);
                view = inflater.Inflate(Resource.Layout.indice_item, parent, false);
                //holder.Title = view.FindViewById<TextView>(Resource.Id.text);
                Typeface tf = Typeface.CreateFromAsset(context.Assets, "barlow.ttf");

                if (position == 0)
                {
                    TextView tt = view.FindViewById<TextView>(Resource.Id.txtIndex);                    
                    tt.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Rgb(70,130,180)));
                    tt = view.FindViewById<TextView>(Resource.Id.txtPrice);                    
                    tt.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Rgb(70, 130, 180)));
                    tt = view.FindViewById<TextView>(Resource.Id.txtChange);                    
                    tt.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Rgb(70, 130, 180)));
                    tt = view.FindViewById<TextView>(Resource.Id.txtPerChange);                    
                    tt.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Rgb(70, 130, 180)));
                }
                else
                {
                    
                    TextView tt = view.FindViewById<TextView>(Resource.Id.txtIndex);
                    tt.Text= item.indice;
                    tt.Typeface = tf;
                    tt.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Gray));
                    tt = view.FindViewById<TextView>(Resource.Id.txtPrice);
                    tt.Text = item.price.ToString();
                    tt.Typeface = tf;
                    tt.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Gray));
                    view.FindViewById<TextView>(Resource.Id.txtChange).Text = item.change.ToString();
                    view.FindViewById<TextView>(Resource.Id.txtChange).Typeface = tf;
                    view.FindViewById<TextView>(Resource.Id.txtPerChange).Text = item.per_change.ToString();
                    view.FindViewById<TextView>(Resource.Id.txtPerChange).Typeface = tf;
                    if (decimal.Parse(item.change) >= 0)
                    {
                        view.FindViewById<TextView>(Resource.Id.txtChange).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGreen));
                        view.FindViewById<TextView>(Resource.Id.txtPerChange).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGreen));
                    }
                    else
                    {
                        view.FindViewById<TextView>(Resource.Id.txtChange).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkRed));
                        view.FindViewById<TextView>(Resource.Id.txtPerChange).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkRed));
                    }                    
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

    class IndexAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}