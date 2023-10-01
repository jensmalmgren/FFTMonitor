# FFTMonitor
A C# Winform program reads the microphone and displays a Fast Fourier frequency plot. I am using NAudio 2.0.1, ScottPlot 4.1.45, and FftSharp 2.1.0.

I am going to make a sound effect visualization program, and for that, I had to have the essential elements such as:
1. Reading the microphone data,
2. Transforming the waveforms of the microphone into frequencies,
3. Plotting the data so that I would know things were right.

I found several samples on the Internet. For example, [this](https://swharden.com/csdv/audio/fft/) by Scott W Harden, but there are others as well. Almost all the sample code I found was making use of a timer. I was not too fond of that, but I accepted it and started playing with the source code. It did not work.

The Fast Fourier data buffer got stuck, so some frequencies would _not_ die out to lower values (or zero) when the microphone was not registering input.

I spent a lot of time trying to figure out what was wrong. I updated to the latest versions of NAudio, ScottPlot, and FftSharp.

It did not help. Finally, I found it had to do with using the timer event. It kicked off amid Fourier calculations. It kicked off while buffers were still copied; it was a mess. I decided to invoke a delegate to refresh the Scott plot. From that moment, there are no more hanging Fourier calculations, stack-overflows, or anything. Just stable and doing what it was supposed to do.

Enjoy!

The Fast Fourier algorithm is fundamental to our modern lives. Derek Muller at the Veritasium YouTube channel has made a [movie](https://youtu.be/nmgFG7PUHfo?si=u0cPJpFhG6RegTgw) about the algorithm. It is great to watch to get to understand what the Fourier algorithm is about.

Derek refers to another [video](https://youtu.be/spUNpyF58BY?si=-4UGiGv8g-1QV87w) by Grant Sanderson on the 3Blue1Brown YouTube channel. That video is much more math-centric but still fun to watch.

If you are new to programming C# I suggest downloading Visual Studio from Microsoft. You can download the code as a zip archive in the green button at this repository. After downloading Visual Studio, you get the necessary libraries via the Nuget interface. You need to get NAudio, FFTSharp, and ScottPlot.

I have one microphone on my computer, which I select automatically in this program. If you have no microphone or more, it might not work. Initially, I had issues with the microphone being turned off. I had to enable it by right-clicking the speaker symbol in the taskbar. Select sounds. Click on the recording tab. Select the microphone and click Properties. There was a little button with a red stop sign on the levels tab and microphone array. You can click that to turn it on. I am still trying to understand why this is there, but it prohibits NAudio from reading the microphone on my computer.

Good luck!
