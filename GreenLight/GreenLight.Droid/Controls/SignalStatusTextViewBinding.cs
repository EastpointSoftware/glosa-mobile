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

using Android.Widget;

using MvvmCross.Binding.Droid.Target;

namespace GreenLight.Droid.Controls
{
	/// <summary>
	/// Change the signal status background image.
	/// </summary>
	public class SignalStatusTextViewBinding : MvxAndroidTargetBinding
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="SignalStatusTextViewBinding"/> class.
		/// </summary>
		/// <param name="textView">The text view.</param>
		public SignalStatusTextViewBinding(TextView textView) : base(textView)
		{
			_textView = textView;
		}

		#endregion

		#region App Life-Cycle

		/// <summary>
		/// Sets the value implementation.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="value">The value.</param>
		protected override void SetValueImpl(object target, object value)
		{
			if (!string.IsNullOrEmpty(_textView.Text))
			{
				_currentValue = Convert.ToBoolean(_textView.Text);

				SetTextViewBackground();
			}
		}

		#endregion

		#region Properties
		#endregion

		#region Implementation

		/// <summary>
		/// Gets the type of the target.
		/// </summary>
		public override Type TargetType => typeof(bool);

		private void SetTextViewBackground()
		{
			if (_currentValue)
			{
				_textView.SetBackgroundResource(Resource.Drawable.greenCircle);
			}
			else
			{
				_textView.SetBackgroundResource(Resource.Drawable.redCircle);
			}
		}

		#endregion

		#region Member Variables

		private TextView _textView;
		private bool _currentValue;

		#endregion


	}
}