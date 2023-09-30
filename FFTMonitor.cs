using FftSharp;
using Microsoft.VisualBasic;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace FastFourierMonitor
{
	public partial class MonitorForm : Form
	{
		private double m_dFrequencyResolutionInHz;
		int m_iBytesPerSamplePerChannel = -1;
		int m_iBytesPerSample = -1;

		private int m_iNumberOfMilliseconds = 2;
		public readonly MMDevice[] AudioDevices = new MMDeviceEnumerator()
			.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
			.ToArray();

		private double[] m_ardAudioValues = new double[0];

		private WasapiCapture AudioDevice;

		System.Numerics.Complex[] m_arFFTForward_Complex = new System.Numerics.Complex[0];

		private double[] m_ardPowerSpectrumInRMS = new double[0];
		private double[] m_ardPowerSpectrumInRMSToPlot = new double[0];

		public MonitorForm()
		{
			InitializeComponent();

			MMDevice selectedDevice = AudioDevices[0];
			AudioDevice = selectedDevice.DataFlow == DataFlow.Render
				? new WasapiLoopbackCapture(selectedDevice)
				: new WasapiCapture(selectedDevice, true, m_iNumberOfMilliseconds);


			WaveFormat _WaveFormat = AudioDevice.WaveFormat;

			m_iBytesPerSamplePerChannel = AudioDevice.WaveFormat.BitsPerSample / 8;
			m_iBytesPerSample = m_iBytesPerSamplePerChannel * AudioDevice.WaveFormat.Channels;

			int _iMaxAudioDataLength = (_WaveFormat.SampleRate * m_iNumberOfMilliseconds) / m_iBytesPerSample;
			int _iNrOfFFTPoints = 2;
			while (_iNrOfFFTPoints * 2 <= _iMaxAudioDataLength)
			{
				_iNrOfFFTPoints *= 2;
			}

			m_ardAudioValues = new double[_iNrOfFFTPoints];
			m_arFFTForward_Complex = FftSharp.FFT.Forward(m_ardAudioValues);
			m_ardPowerSpectrumInRMS = FftSharp.FFT.Magnitude(m_arFFTForward_Complex);

			m_ardPowerSpectrumInRMSToPlot = new double[m_ardPowerSpectrumInRMS.Length];
			m_dFrequencyResolutionInHz = FftSharp.FFT.FrequencyResolution(m_ardPowerSpectrumInRMS.Length, _WaveFormat.SampleRate);

			formsPlot1.Plot.AddSignal(m_ardPowerSpectrumInRMSToPlot, 1.0 / m_dFrequencyResolutionInHz);
			formsPlot1.Plot.YLabel("Spectral Power RMS");
			formsPlot1.Plot.XLabel("Frequency (kHz)");
			formsPlot1.Plot.Title($"{_WaveFormat.Encoding} ({_WaveFormat.BitsPerSample}-bit) {_WaveFormat.SampleRate} KHz");
			formsPlot1.Plot.SetAxisLimits(0, 6000, 0, .0075);
			formsPlot1.Refresh();

			FormClosing += MonitorForm_FormClosing;
			AudioDevice.DataAvailable += WaveIn_DataAvailable;
			AudioDevice.RecordingStopped += AudioDevice_RecordingStopped;
			AudioDevice.StartRecording();
		} // public MonitorForm()

		private void MonitorForm_FormClosing(object? sender, FormClosingEventArgs e)
		{
			if (AudioDevice != null)
			{
				e.Cancel = true;
				System.Diagnostics.Debug.WriteLine($"Closing audio device: {AudioDevice}");
				AudioDevice.StopRecording();
			}
		} // MonitorForm_FormClosing()

		private void AudioDevice_RecordingStopped(object? sender, StoppedEventArgs e)
		{
			AudioDevice.Dispose();
			AudioDevice = null;
			this.Close();
		} // AudioDevice_RecordingStopped()

		private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
		{
			int _iBufferSampleCount = Math.Min(e.Buffer.Length / m_iBytesPerSample, m_ardAudioValues.Length);

			if (m_iBytesPerSamplePerChannel == 2 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
			{
				for (int i = 0; i < _iBufferSampleCount; i++)
				{
					m_ardAudioValues[i] = BitConverter.ToInt16(e.Buffer, i * m_iBytesPerSample);
				}
			}
			else if (m_iBytesPerSamplePerChannel == 4 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
			{
				for (int i = 0; i < _iBufferSampleCount; i++)
				{
					m_ardAudioValues[i] = BitConverter.ToInt32(e.Buffer, i * m_iBytesPerSample);
				}
			}
			else if (m_iBytesPerSamplePerChannel == 4 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
			{
				for (int i = 0; i < _iBufferSampleCount; i++)
				{
					m_ardAudioValues[i] = BitConverter.ToSingle(e.Buffer, i * m_iBytesPerSample);
				}
			}
			else
			{
				throw new NotSupportedException(AudioDevice.WaveFormat.ToString());
			}

			m_arFFTForward_Complex = FftSharp.FFT.Forward(FftSharp.Pad.ZeroPad(m_ardAudioValues));
			m_ardPowerSpectrumInRMS = FftSharp.FFT.Magnitude(m_arFFTForward_Complex);

			Array.Copy(m_ardPowerSpectrumInRMS, m_ardPowerSpectrumInRMSToPlot, m_ardPowerSpectrumInRMS.Length);

			for (int i = 0; i < m_ardPowerSpectrumInRMSToPlot.Length; i++)
			{
				if (m_ardPowerSpectrumInRMSToPlot[i] < 0.001)
				{
					m_ardPowerSpectrumInRMSToPlot[i] = 0.0;
				}
			}

			RefreshThePlot(formsPlot1);

			int _iPeakIndexOfPowerSpectrum = 0;
			double _dPeakPowerSpectrumValueRMS = 0;
			for (int i = 0; i < m_ardPowerSpectrumInRMS.Length; i++)
			{
				if (m_ardPowerSpectrumInRMS[i] > m_ardPowerSpectrumInRMS[_iPeakIndexOfPowerSpectrum])
				{
					_iPeakIndexOfPowerSpectrum = i;
					_dPeakPowerSpectrumValueRMS = m_ardPowerSpectrumInRMS[i];
				}
			}

			double _dPeakFrequency = m_dFrequencyResolutionInHz * _iPeakIndexOfPowerSpectrum;
			LabelSetText(m_PeakFrequency_Label, $"Peak Frequency: {_dPeakFrequency:N0} Hz");
			string _strPeakPowerSpectrumValueRMS = _dPeakPowerSpectrumValueRMS.ToString("#.000000");
			LabelSetText(m_PeakValue_Label, $"Peak value: {_strPeakPowerSpectrumValueRMS}");
		} // WaveIn_DataAvailable()

		private delegate void RefreshRequestDelegate(FormsPlot ip_FormsPlot);
		public static void RefreshThePlot(FormsPlot ip_FormsPlot)
		{
			if (ip_FormsPlot.InvokeRequired)
			{
				ip_FormsPlot.Invoke(new RefreshRequestDelegate(RefreshThePlot), new object[] { ip_FormsPlot });
			}
			else
			{
				ip_FormsPlot.RefreshRequest();
			}
		} // RefreshThePlot()

		private delegate void LabelSetText_Delegate(System.Windows.Forms.Label ip_Label, string ip_strLabel);
		public static void LabelSetText(System.Windows.Forms.Label ip_Label, string ip_strLabel)
		{
			if (ip_Label.IsDisposed)
			{
				return;
			}
			if (ip_Label.InvokeRequired)
			{
				ip_Label.Invoke(new LabelSetText_Delegate(LabelSetText), new object[] { ip_Label, ip_strLabel });
			}
			else
			{
				ip_Label.Text = ip_strLabel;
			}
		} // LabelSetText()
	}
}
