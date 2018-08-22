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
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

using Plugin.TextToSpeech;

using GreenLight.Core.Contracts;

namespace GreenLight.Core.Services
{
    /// <summary>
    /// Synthesizes speech from text for immediate playback.
    /// </summary>
    public class TextToSpeechService : ITextToSpeechService
    {
        #region Construction

        public TextToSpeechService()
        {
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Set up a flatable text to speech plugin.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SpeakAsync(string text, CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested == false)
                {
                    //crossLocale : Locale of voice
                    //pitch : Pitch of voice (All 0.0 - 2.0f)
                    //speakRate : Speak Rate of voice (All) (0.0 - 2.0f)
                    //volume : Volume of voice (iOS/WP) (0.0-1.0)
                    await CrossTextToSpeech.Current.Speak(
                        text: text,
                        crossLocale: null,
                        pitch: 1,
                        speakRate: 1,
                        volume: null,
                        cancelToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Text to Speech OperationCanceledException");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Text to Speech " + ex.ToString());
            }
            finally
            {

            }
        }

        #endregion

        #region Member Variables
        #endregion
    }
}
