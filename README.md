# FFTMonitor
A C# winform program reading the microphone displays a Fast Fourier frequency plot. I am using NAudio 2.0.1, ScottPlot 4.1.45, and FftSharp 2.1.0.

I am going to make a sound effect visualization program, and for that, I had to have the essential elements such as:
1. Reading the microphone data,
2. Transforming the waveforms of the microphone into frequencies,
3. Plotting the data so that I would know things were right.

I found several samples on the Internet. For example, https://swharden.com/csdv/audio/fft/, but there are others as well. Almost all the sample code I found was making use of a timer. I was not too fond of that, but I accepted it and started playing with the source code. It did not work.

The Fast Fourier data buffer got stuck, so some frequencies would not die out to lower values (or zero) when the microphone was not registering input.

I spent a lot of time trying to figure out what was wrong. I updated to the latest versions of NAudio, ScottPlot, and FftSharp.

It did not help. Finally, I found it had to do with using the timer event. It kicked off amid Fourier calculations. It kicked off while buffers were still copied; it was a mess. I decided to invoke a delegate to refresh the Scott plot. From that moment, there are no more hanging Fourier calculations, stack-overflows, or anything. Just stable and doing what it was supposed to do.

Enjoy.
