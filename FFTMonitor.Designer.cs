namespace FastFourierMonitor
{
	partial class MonitorForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			formsPlot1 = new ScottPlot.FormsPlot();
			m_PeakFrequency_Label = new Label();
			m_PeakValue_Label = new Label();
			SuspendLayout();
			// 
			// formsPlot1
			// 
			formsPlot1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			formsPlot1.Location = new Point(13, 79);
			formsPlot1.Margin = new Padding(4, 3, 4, 3);
			formsPlot1.Name = "formsPlot1";
			formsPlot1.Size = new Size(1077, 399);
			formsPlot1.TabIndex = 9;
			// 
			// m_PeakFrequency_Label
			// 
			m_PeakFrequency_Label.AutoSize = true;
			m_PeakFrequency_Label.Location = new Point(68, 36);
			m_PeakFrequency_Label.Name = "m_PeakFrequency_Label";
			m_PeakFrequency_Label.Size = new Size(93, 15);
			m_PeakFrequency_Label.TabIndex = 8;
			m_PeakFrequency_Label.Text = "Peak Frequency:";
			// 
			// m_PeakValue_Label
			// 
			m_PeakValue_Label.AutoSize = true;
			m_PeakValue_Label.Location = new Point(354, 36);
			m_PeakValue_Label.Name = "m_PeakValue_Label";
			m_PeakValue_Label.Size = new Size(38, 15);
			m_PeakValue_Label.TabIndex = 10;
			m_PeakValue_Label.Text = "label2";
			// 
			// MonitorForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1103, 490);
			Controls.Add(m_PeakValue_Label);
			Controls.Add(formsPlot1);
			Controls.Add(m_PeakFrequency_Label);
			Name = "MonitorForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Fast Fourier Monitor";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private ScottPlot.FormsPlot formsPlot1;
		private Label m_PeakFrequency_Label;
		private Label m_PeakValue_Label;
	}
}