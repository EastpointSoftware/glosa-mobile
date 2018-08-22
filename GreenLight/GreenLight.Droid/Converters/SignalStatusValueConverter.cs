/*
GLOSA Mobile. Green Light Optimal Speed Adviosry Mobile Application

Copyright © 2017 Eastpoint Software Limited

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */

using System;

using Android.Graphics;
using Android.Graphics.Drawables;

using MvvmCross.Platform.Converters;

using GreenLight.Core.Helpers;

namespace GreenLight.Droid.Converters
{
	public class SignalStatusValueConverter : MvxValueConverter<MovementEvent, Drawable>
	{
		protected override Drawable Convert(MovementEvent value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
            Drawable drawable = new ColorDrawable(Color.Green);
            switch (value)
            {
                case MovementEvent.Green:
                    break;
                case MovementEvent.Red:
                    drawable= new ColorDrawable(Color.Red);
                    break;
                case MovementEvent.Amber:
                    drawable = new ColorDrawable(Color.Yellow);
                    break;
                default:
                    break;
            }

			return drawable;
		}
	}
}