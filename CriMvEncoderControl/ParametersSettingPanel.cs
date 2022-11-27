using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CriMvEncoderControl
{
	public class ParametersSettingPanel : UserControl
	{
		public class EncodingRequestedEventArgs : EventArgs
		{
			private EncodingParameters encParam;

			public EncodingParameters EnvParam
			{
				get
				{
					return encParam;
				}
				set
				{
					encParam = value;
				}
			}

			public EncodingRequestedEventArgs(EncodingParameters encParam)
			{
				this.encParam = encParam;
			}
		}

		public class PreviewRequestedEventArgs : EventArgs
		{
			private string filename;

			private bool extendedPlayer;

			public string FileName
			{
				get
				{
					return filename;
				}
				set
				{
					filename = value;
				}
			}

			public bool ExtendedPlayer
			{
				get
				{
					return extendedPlayer;
				}
				set
				{
					extendedPlayer = value;
				}
			}

			public PreviewRequestedEventArgs(string filename, bool extendedPlayer)
			{
				FileName = filename;
				ExtendedPlayer = extendedPlayer;
			}
		}

		public class LoadRequestedEventArgs : EventArgs
		{
			private string filename;

			public string FileName
			{
				get
				{
					return filename;
				}
				set
				{
					filename = value;
				}
			}

			public LoadRequestedEventArgs(string filename)
			{
				FileName = filename;
			}
		}

		private const int MIN_RESIZE_WIDTH = 16;

		private const int MAX_RESIZE_WIDTH = 4088;

		private const int MIN_RESIZE_HEIGHT = 16;

		private const int MAX_RESIZE_HEIGHT = 4088;

		private const char SETTINGS_FILE_COMMENT = ';';

		private const string SETTINGS_FILE_HEADER = " SFVES 1.00 - Do not edit or delete this line";

		private const string EMPTY_FILE_SPEC = "";

		private const string INPUT_FILE_SPEC = " *.avi,*.bmp,*.tga";

		private const string OUTPUT_FILE_SPEC = "*.usm";

		private const string TEXT_FILE_SPEC = "*.txt";

		private const string AUDIO_FILE_SPEC = "*.wav";

		private int numberMaxTracks;

		private int numberMaxChannels;

		private AudioSettingSubPanel[] panelsInputAudio;

		private SubtitleSettingSubPanel[] panelsInputSubtitle;

		private bool isEncodingStarted;

		private bool isEncodingCanceled;

		private IContainer components;

		private Button btnEncode;

		private Button btnOutputFile;

		private TextBox tboxOutputFile;

		private GroupBox gbCompSettings;

		private TextBox tboxResizeHeight;

		private Label lblResizeX;

		private TextBox tboxResizeWidth;

		private CheckBox chboxResizeEnabled;

		private Label lblResize;

		private Label lblFps;

		private ComboBox comboxFramerate;

		private Label lblFramerate;

		private Label lblKbps;

		private Label lblBitrate;

		private Button btnCuePointFile;

		private TextBox tboxCuePointFile;

		private ComboBox comboxAudioTrackSelect;

		private Label lblAudioSelect;

		private GroupBox gbInputAudioMaterials;

		private CheckBox chboxUseAlphaCh;

		private CheckBox chboxUseAudioTrack;

		private Button btnInputVideoFile;

		private TextBox tboxInputVideoFile;

		private AudioSettingSubPanel dummyPanelInputAudio;

		private ComboBox comboxSubtitleChannelSelect;

		private GroupBox gbInputSubtitleFile;

		private Label label1;

		private SubtitleSettingSubPanel dummyPanelInputSubtitle;

		private Button btnPreview;

		private Label label4;

		private Label label3;

		private Label label2;

		private ProgressBar progbarEncode;

		private Label lblProgress;

		private RichTextBox richtboxStdout;

		private Button btnCancel;

		private Label lblEncodingLog;

		private Label lblProgressPercent;

		private RichTextBox richtboxStderr;

		private Label lblErrorLog;

		private GroupBox gbEncoding;

		private Label label5;

		private CheckBox chboxExtendedPreview;

		private CheckBox useInputFramerate;

		private Button btnLoadSettings;

		private Button btnSaveSettings;

		private ComboBox comboxBitrate;

		private CheckBox useHCA;

		private ComboBox comboxHCAQuality;

		private Label lblQuality;

		public event EventHandler<EncodingRequestedEventArgs> EncodingRequestedEvent;

		public event EventHandler CancelRequestedEvent;

		public event EventHandler<PreviewRequestedEventArgs> PreviewRequestedEvent;

		public event EventHandler<LoadRequestedEventArgs> LoadRequestedEvent;

		public ParametersSettingPanel()
		{
			InitializeComponent();
			numberMaxTracks = 32;
			numberMaxChannels = 32;
			createMultiPanelsForAudioAndSubtitle();
		}

		private void EncodeParamControl_Load(object sender, EventArgs e)
		{
			comboxFramerate.SelectedIndex = 4;
			comboxBitrate.SelectedIndex = 5;
			comboxAudioTrackSelect.SelectedIndex = 0;
			comboxSubtitleChannelSelect.SelectedIndex = 0;
			comboxHCAQuality.SelectedIndex = 1;
		}

		private void createMultiPanelsForAudioAndSubtitle()
		{
			panelsInputAudio = new AudioSettingSubPanel[numberMaxTracks];
			SuspendLayout();
			for (int i = 0; i < numberMaxTracks; i++)
			{
				panelsInputAudio[i] = new AudioSettingSubPanel();
				panelsInputAudio[i].Font = dummyPanelInputAudio.Font;
				panelsInputAudio[i].Location = dummyPanelInputAudio.Location;
				panelsInputAudio[i].Margin = dummyPanelInputAudio.Margin;
				panelsInputAudio[i].Size = dummyPanelInputAudio.Size;
				panelsInputAudio[i].Name = "panelsInputLang" + i;
				panelsInputAudio[i].TabIndex = 50 + i;
				if (i == 0)
				{
					panelsInputAudio[i].Visible = true;
					panelsInputAudio[i].BringToFront();
				}
				else
				{
					panelsInputAudio[i].Visible = false;
				}
			}
			gbInputAudioMaterials.Controls.AddRange(panelsInputAudio);
			panelsInputSubtitle = new SubtitleSettingSubPanel[numberMaxChannels];
			for (int j = 0; j < numberMaxChannels; j++)
			{
				panelsInputSubtitle[j] = new SubtitleSettingSubPanel();
				panelsInputSubtitle[j].Location = dummyPanelInputSubtitle.Location;
				panelsInputSubtitle[j].Size = dummyPanelInputSubtitle.Size;
				panelsInputSubtitle[j].Name = "panelsInputSubtitle" + j;
				panelsInputSubtitle[j].TabIndex = 60 + j;
				if (j == 0)
				{
					panelsInputSubtitle[j].Visible = true;
					panelsInputSubtitle[j].BringToFront();
				}
				else
				{
					panelsInputSubtitle[j].Visible = false;
				}
			}
			gbInputSubtitleFile.Controls.AddRange(panelsInputSubtitle);
			ResumeLayout();
			gbInputAudioMaterials.Controls.Remove(dummyPanelInputAudio);
			gbInputSubtitleFile.Controls.Remove(dummyPanelInputSubtitle);
			dummyPanelInputAudio.Dispose();
			dummyPanelInputSubtitle.Dispose();
		}

		private bool enableEncodeButtonIfValid()
		{
			try
			{
				string directoryName = Path.GetDirectoryName(tboxOutputFile.Text.Trim().Trim('"'));
				btnEncode.Enabled = File.Exists(tboxInputVideoFile.Text.Trim().Trim('"')) && Directory.Exists(directoryName);
			}
			catch
			{
				btnEncode.Enabled = false;
			}
			return btnEncode.Enabled;
		}

		private bool doesFileExist(string filename)
		{
			return File.Exists(filename);
		}

		public bool warnIfFileNotExist(string filename)
		{
			if (doesFileExist(filename))
			{
				return true;
			}
			MessageBox.Show("This file does not exist.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}

		private bool validateNumericValue(char ch)
		{
			if ((ch >= '0' && ch <= '9') || ch == '\b')
			{
				return true;
			}
			return false;
		}

		private void btnEncode_Click(object sender, EventArgs e)
		{
			EncodingParameters encodingParameters = createEncodeParameters();
			if (checkEncodeParameters(encodingParameters))
			{
				initEncodingLogPanel();
				btnEncode.Enabled = false;
				btnPreview.Enabled = false;
				isEncodingCanceled = false;
				Invoke(this.EncodingRequestedEvent, this, new EncodingRequestedEventArgs(encodingParameters));
			}
		}

		private void btnPreview_Click(object sender, EventArgs e)
		{
			if (this.PreviewRequestedEvent != null && File.Exists(tboxOutputFile.Text.Trim().Trim('"')))
			{
				this.PreviewRequestedEvent(this, new PreviewRequestedEventArgs(tboxOutputFile.Text.Trim().Trim('"'), chboxExtendedPreview.Checked));
			}
		}

		private bool checkEncodeParameters(EncodingParameters ep)
		{
			string text = Path.GetExtension(ep.inputVideoFilePath).ToLower();
			if (!text.Equals(".avi") && !text.Equals(".bmp") && !text.Equals(".tga"))
			{
				MessageBox.Show("Only .avi, .bmp, or .tga input files are supported at this time.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (ep.enableResize && (ep.resizeWidth < 16 || ep.resizeWidth > 4088 || ep.resizeHeight < 16 || ep.resizeHeight > 4088))
			{
				MessageBox.Show("Invalid value specified for resized output.");
				return false;
			}
			return true;
		}

		private EncodingParameters createEncodeParameters()
		{
			EncodingParameters encodingParameters = new EncodingParameters();
			encodingParameters.inputVideoFilePath = tboxInputVideoFile.Text.Trim().Trim('"');
			encodingParameters.outputFilePath = tboxOutputFile.Text.Trim().Trim('"');
			encodingParameters.isTargetPS2 = false;
			encodingParameters.useAlphaChannel = chboxUseAlphaCh.Checked;
			encodingParameters.bitrate = Convert.ToInt32(comboxBitrate.Text);
			if (encodingParameters.useAlphaChannel)
			{
				encodingParameters.bitrate /= 2;
			}
			if (useInputFramerate.Checked)
			{
				encodingParameters.useInputFramerate = true;
				encodingParameters.clearFramerate();
			}
			else
			{
				encodingParameters.setFramerate(comboxFramerate.Text);
			}
			if (useHCA.Checked)
			{
				encodingParameters.useHCA = true;
				encodingParameters.hcaQuality = 5 - comboxHCAQuality.SelectedIndex;
			}
			if (chboxResizeEnabled.Checked)
			{
				int.TryParse(tboxResizeWidth.Text, out encodingParameters.resizeWidth);
				int.TryParse(tboxResizeHeight.Text, out encodingParameters.resizeHeight);
				encodingParameters.enableResize = true;
			}
			else
			{
				encodingParameters.enableResize = false;
			}
			for (int i = 0; i < numberMaxTracks; i++)
			{
				encodingParameters.langParams[i] = panelsInputAudio[i].GetLanguageParamters();
			}
			for (int j = 0; j < numberMaxChannels; j++)
			{
				encodingParameters.subtitleFilePaths[j] = panelsInputSubtitle[j].GetSubtitleTextPath();
			}
			encodingParameters.cuepointFilePath = (File.Exists(tboxCuePointFile.Text.Trim().Trim('"')) ? tboxCuePointFile.Text.Trim().Trim('"') : string.Empty);
			return encodingParameters;
		}

		private void btnInputVideoFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Video Source File(*.avi;*.bmp;*.tga)|*.avi;*.bmp;*.tga";
			openFileDialog.RestoreDirectory = true;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				tboxInputVideoFile.Text = openFileDialog.FileName;
				Invoke(this.LoadRequestedEvent, this, new LoadRequestedEventArgs(tboxInputVideoFile.Text.Trim().Trim('"')));
			}
		}

		private void btnOutputFile_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "CRI Movie File(*.usm)|*.usm";
			saveFileDialog.RestoreDirectory = true;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				tboxOutputFile.Text = saveFileDialog.FileName;
			}
		}

		private void tboxOutputFile_TextChanged(object sender, EventArgs e)
		{
			enableEncodeButtonIfValid();
		}

		private void comboxInputAudio_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			int selectedIndex = comboBox.SelectedIndex;
			for (int i = 0; i < numberMaxTracks; i++)
			{
				if (i == selectedIndex)
				{
					panelsInputAudio[i].Visible = true;
					panelsInputAudio[i].BringToFront();
				}
				else
				{
					panelsInputAudio[i].Visible = false;
				}
			}
		}

		private void tboxInputVideoFile_TextChanged(object sender, EventArgs e)
		{
			string path = tboxInputVideoFile.Text.Trim().Trim('"');
			string text = Path.GetExtension(path).ToLower();
			if (text.Equals(".avi"))
			{
				chboxUseAlphaCh.Enabled = true;
				chboxUseAudioTrack.Enabled = true;
				chboxUseAudioTrack.Checked = true;
				panelsInputAudio[0].SetDefaultAudioTrack(tboxInputVideoFile.Text.Trim().Trim('"'));
				useInputFramerate.Enabled = true;
				useInputFramerate.Checked = true;
				comboxFramerate.Enabled = false;
			}
			else if (text.Equals(".bmp") || text.Equals(".tga"))
			{
				chboxUseAlphaCh.Enabled = true;
				chboxUseAudioTrack.Enabled = false;
				useInputFramerate.Checked = false;
				useInputFramerate.Enabled = false;
				comboxFramerate.Enabled = true;
			}
			else
			{
				chboxUseAlphaCh.Checked = false;
				chboxUseAudioTrack.Checked = false;
				chboxUseAlphaCh.Enabled = false;
				chboxUseAudioTrack.Enabled = false;
				useInputFramerate.Checked = false;
				useInputFramerate.Enabled = false;
				comboxFramerate.Enabled = true;
			}
			if (File.Exists(tboxInputVideoFile.Text.Trim().Trim('"')))
			{
				tboxOutputFile.Text = Path.ChangeExtension(path, ".usm");
				btnPreview.Enabled = File.Exists(tboxOutputFile.Text.Trim().Trim('"'));
			}
			else
			{
				btnPreview.Enabled = false;
			}
			enableEncodeButtonIfValid();
		}

		private void chboxUseAudioTrack_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			if (checkBox.Checked)
			{
				panelsInputAudio[0].SetDefaultAudioTrack(tboxInputVideoFile.Text.Trim().Trim('"'));
			}
			else
			{
				panelsInputAudio[0].ResetAudioTrack();
			}
		}

		private void comboxSubtitleChannelSelect_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			int selectedIndex = comboBox.SelectedIndex;
			for (int i = 0; i < numberMaxTracks; i++)
			{
				if (i == selectedIndex)
				{
					panelsInputSubtitle[i].Visible = true;
					panelsInputSubtitle[i].BringToFront();
				}
				else
				{
					panelsInputSubtitle[i].Visible = false;
				}
			}
		}

		private void btnCuePointFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Cue Point File(*.txt)|*.txt|All Files (*.*)|*.*";
			openFileDialog.RestoreDirectory = true;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				tboxCuePointFile.Text = openFileDialog.FileName.Trim().Trim('"');
			}
		}

		private void tboxCuePointFile_TextChanged(object sender, EventArgs e)
		{
		}

		private void gbInputSubtitleFile_TextChanged(object sender, EventArgs e)
		{
		}

		private void initEncodingLogPanel()
		{
			isEncodingStarted = true;
			isEncodingCanceled = false;
			gbEncoding.Enabled = true;
			lblProgressPercent.Text = "0%";
			progbarEncode.Value = 0;
			btnCancel.Enabled = true;
			richtboxStdout.Clear();
			richtboxStderr.Clear();
		}

		public void AppendEncodingLogReceived(object sender, DataReceivedEventArgs e)
		{
			if (base.InvokeRequired)
			{
				Invoke(new DataReceivedEventHandler(AppendEncodingLogReceived), sender, e);
			}
			else if (isEncodingStarted)
			{
				appendEncodingLogText(e.Data + Environment.NewLine);
			}
		}

		public void AppendEncodingErrorReceived(object sender, DataReceivedEventArgs e)
		{
			if (base.InvokeRequired)
			{
				Invoke(new DataReceivedEventHandler(AppendEncodingErrorReceived), sender, e);
			}
			else
			{
				appendEncodingErrorText(richtboxStderr, e.Data + Environment.NewLine);
			}
		}

		public void EncodingProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (base.InvokeRequired)
			{
				Invoke(new ProgressChangedEventHandler(EncodingProgressChanged), sender, e);
			}
			else
			{
				progbarEncode.Value = e.ProgressPercentage;
				lblProgressPercent.Text = string.Format("{0}%", e.ProgressPercentage);
			}
		}

		public void EncodingExited(object sender, EventArgs e)
		{
			if (isEncodingStarted)
			{
				if (base.InvokeRequired)
				{
					Invoke(new EventHandler(EncodingExited), sender, e);
					return;
				}
				btnCancel.Enabled = false;
				btnEncode.Enabled = true;
				btnPreview.Enabled = File.Exists(tboxOutputFile.Text.Trim().Trim('"'));
				appendEncodingActionText("End Encoding!");
				isEncodingStarted = false;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			if (!isEncodingCanceled && isEncodingStarted)
			{
				btnCancel.Enabled = false;
				isEncodingCanceled = true;
				if (isEncodingStarted && this.CancelRequestedEvent != null)
				{
					this.CancelRequestedEvent(this, null);
				}
				appendEncodingActionText("Encoding canceled.\n");
				progbarEncode.Value = 100;
				lblProgressPercent.Text = string.Format("{0}%", 100);
			}
		}

		private void appendLogText(RichTextBox richbox, string message)
		{
			richbox.AppendText(message);
			richbox.ScrollToCaret();
		}

		private void appendEncodingLogText(string message)
		{
			appendLogText(richtboxStdout, message);
		}

		private void appendEncodingErrorText(RichTextBox richbox, string message)
		{
			richbox.SelectionColor = Color.Red;
			Font font2 = (richbox.SelectionFont = new Font(richbox.SelectionFont.FontFamily, richbox.SelectionFont.Size + 1f, richbox.SelectionFont.Style | FontStyle.Bold));
			appendLogText(richbox, message);
			font2.Dispose();
		}

		private void appendEncodingActionText(string message)
		{
			richtboxStdout.SelectionColor = Color.Blue;
			Font font = new Font(richtboxStdout.SelectionFont.FontFamily, richtboxStdout.SelectionFont.Size + 2f, richtboxStdout.SelectionFont.Style | FontStyle.Bold);
			richtboxStdout.SelectionFont = font;
			appendLogText(richtboxStdout, message);
			font.Dispose();
		}

		private void subPanelInputAudio_Load(object sender, EventArgs e)
		{
		}

		private void useInputFramerate_CheckedChanged(object sender, EventArgs e)
		{
			comboxFramerate.Enabled = !useInputFramerate.Checked;
		}

		private void dummyPanelInputSubtitle_Load(object sender, EventArgs e)
		{
		}

		private void tboxResizeWidth_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!validateNumericValue(e.KeyChar))
			{
				e.Handled = true;
			}
		}

		private void tboxResizeHeight_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!validateNumericValue(e.KeyChar))
			{
				e.Handled = true;
			}
		}

		private void btnSaveSettings_Click(object sender, EventArgs e)
		{
			loadSaveOptions(true);
		}

		private void btnLoadSettings_Click(object sender, EventArgs e)
		{
			loadSaveOptions(false);
		}

		private bool loadSaveOptions(bool saving)
		{
			FileDialog fileDialog = ((!saving) ? ((FileDialog)new OpenFileDialog()) : ((FileDialog)new SaveFileDialog()));
			fileDialog.Filter = "Encoder Settings File (*.sves)|*.sves";
			fileDialog.RestoreDirectory = true;
			if (fileDialog.ShowDialog() != DialogResult.OK)
			{
				return false;
			}
			if (saving)
			{
				return saveEncoderOptions(fileDialog.FileName);
			}
			return loadEncoderOptions(fileDialog.FileName);
		}

		private bool saveEncoderOptions(string filename)
		{
			EncodingParameters encodingParameters = new EncodingParameters();
			setParamsFromGUI(encodingParameters);
			string buffer;
			encodingParameters.export(out buffer);
			try
			{
				File.WriteAllText(filename, ';' + " SFVES 1.00 - Do not edit or delete this line" + Environment.NewLine);
				File.AppendAllText(filename, buffer);
			}
			catch (Exception)
			{
				MessageBox.Show("There was an error saving the settings.");
				return false;
			}
			return true;
		}

		private bool loadEncoderOptions(string filename)
		{
			List<string> list = new List<string>();
			try
			{
				StreamReader streamReader;
				using (streamReader = new StreamReader(filename))
				{
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						int num = text.IndexOf(';');
						if (num != 0)
						{
							if (num > 0)
							{
								text = text.Substring(0, num);
							}
							text = text.Trim();
							if (text.Length > 0)
							{
								list.Add(text);
							}
						}
					}
				}
			}
			catch (Exception)
			{
				MessageBox.Show("There was an error loading the settings.");
				return false;
			}
			EncodingParameters encodingParameters = new EncodingParameters();
			if (!encodingParameters.import(list.ToArray()))
			{
				MessageBox.Show("Settings were in an invalid format.  Sorry.");
				return false;
			}
			setGUIFromParams(encodingParameters);
			return true;
		}

		private bool setParamsFromGUI(EncodingParameters ep)
		{
			ep.inputVideoFilePath = cleanFN(tboxInputVideoFile.Text.Trim().Trim('"'));
			ep.outputFilePath = cleanFN(tboxOutputFile.Text.Trim().Trim('"'));
			ep.cuepointFilePath = cleanFN(tboxCuePointFile.Text.Trim().Trim('"'));
			for (int i = 0; i < 32; i++)
			{
				ep.subtitleFilePaths[i] = cleanFN(panelsInputSubtitle[i].tboxSubtitleFile.Text);
			}
			for (int j = 0; j < 32; j++)
			{
				AudioSettingSubPanel audioSettingSubPanel = panelsInputAudio[j];
				AudioParameters audioParameters = default(AudioParameters);
				if (audioSettingSubPanel.rbtnNone.Checked)
				{
					audioParameters.AudioType = InputAudioType.None;
				}
				else if (audioSettingSubPanel.rbtnMonoStereo.Checked)
				{
					audioParameters.AudioType = InputAudioType.MonoOrStereo;
					audioParameters.FilePathMonoStereo = cleanFN(audioSettingSubPanel.tboxMonoStereo.Text);
				}
				else if (audioSettingSubPanel.rbtn51ch.Checked)
				{
					audioParameters.AudioType = InputAudioType.MultiChannel;
					audioParameters.FilePath51Center = cleanFN(audioSettingSubPanel.tBox51chC.Text);
					audioParameters.FilePath51Left = cleanFN(audioSettingSubPanel.tBox51chL.Text);
					audioParameters.FilePath51LeftSurround = cleanFN(audioSettingSubPanel.tBox51chLs.Text);
					audioParameters.FilePath51Right = cleanFN(audioSettingSubPanel.tBox51chR.Text);
					audioParameters.FilePath51RightSurround = cleanFN(audioSettingSubPanel.tBox51chRs.Text);
					audioParameters.FilePath51LFE = cleanFN(audioSettingSubPanel.tBox51chLFE.Text);
				}
				ep.langParams[j] = audioParameters;
			}
			ep.useAlphaChannel = chboxUseAlphaCh.Checked;
			ep.useAudioTrack = chboxUseAudioTrack.Checked;
			ep.bitrate = Convert.ToInt32(comboxBitrate.Text);
			ep.useInputFramerate = useInputFramerate.Checked;
			ep.setFramerate(comboxFramerate.SelectedItem.ToString());
			ep.useHCA = useHCA.Checked;
			ep.hcaQuality = 5 - comboxHCAQuality.SelectedIndex;
			ep.enableResize = chboxResizeEnabled.Checked;
			int.TryParse(tboxResizeWidth.Text, out ep.resizeWidth);
			int.TryParse(tboxResizeHeight.Text, out ep.resizeHeight);
			return true;
		}

		private bool setGUIFromParams(EncodingParameters ep)
		{
			try
			{
				tboxInputVideoFile.Text = setFN(ep.inputVideoFilePath, " *.avi,*.bmp,*.tga");
				tboxOutputFile.Text = setFN(ep.outputFilePath, "*.usm");
				tboxCuePointFile.Text = setFN(ep.cuepointFilePath, "*.txt");
				for (int i = 0; i < panelsInputSubtitle.Length; i++)
				{
					panelsInputSubtitle[i].tboxSubtitleFile.Text = setFN(ep.subtitleFilePaths[i], "*.txt");
				}
				for (int j = 0; j < panelsInputAudio.Length; j++)
				{
					AudioSettingSubPanel audioSettingSubPanel = panelsInputAudio[j];
					AudioParameters audioParameters = ep.langParams[j];
					panelsInputAudio[j].tboxMonoStereo.Text = "*.wav";
					panelsInputAudio[j].tBox51chC.Text = "";
					panelsInputAudio[j].tBox51chL.Text = "";
					panelsInputAudio[j].tBox51chLs.Text = "";
					panelsInputAudio[j].tBox51chR.Text = "";
					panelsInputAudio[j].tBox51chRs.Text = "";
					panelsInputAudio[j].tBox51chLFE.Text = "";
					switch (audioParameters.AudioType)
					{
					default:
						panelsInputAudio[j].rbtnNone.Checked = true;
						break;
					case InputAudioType.MonoOrStereo:
						panelsInputAudio[j].rbtnMonoStereo.Checked = true;
						panelsInputAudio[j].tboxMonoStereo.Text = setFN(audioParameters.FilePathMonoStereo, "*.wav");
						break;
					case InputAudioType.MultiChannel:
						panelsInputAudio[j].rbtn51ch.Checked = true;
						panelsInputAudio[j].tBox51chC.Text = setFN(audioParameters.FilePath51Center, "");
						panelsInputAudio[j].tBox51chL.Text = setFN(audioParameters.FilePath51Left, "");
						panelsInputAudio[j].tBox51chLs.Text = setFN(audioParameters.FilePath51LeftSurround, "");
						panelsInputAudio[j].tBox51chR.Text = setFN(audioParameters.FilePath51Right, "");
						panelsInputAudio[j].tBox51chRs.Text = setFN(audioParameters.FilePath51RightSurround, "");
						panelsInputAudio[j].tBox51chLFE.Text = setFN(audioParameters.FilePath51LFE, "");
						break;
					}
				}
				chboxUseAlphaCh.Checked = ep.useAlphaChannel;
				chboxUseAudioTrack.Checked = ep.useAudioTrack;
				comboxBitrate.Text = Convert.ToString(ep.bitrate);
				useInputFramerate.Checked = ep.useInputFramerate;
				comboxFramerate.SelectedItem = ep.getFramerate();
				useHCA.Checked = ep.useHCA;
				comboxHCAQuality.SelectedIndex = 5 - ep.hcaQuality;
				chboxResizeEnabled.Checked = ep.enableResize;
				tboxResizeWidth.Text = ep.resizeWidth.ToString();
				tboxResizeHeight.Text = ep.resizeHeight.ToString();
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private string cleanFN(string fn)
		{
			if (fn != null && !fn.Contains("*"))
			{
				return fn;
			}
			return "";
		}

		private string setFN(string fn, string def)
		{
			if (string.IsNullOrEmpty(fn))
			{
				return def;
			}
			return fn;
		}

		private void useHCA_CheckedChanged(object sender, EventArgs e)
		{
			comboxHCAQuality.Enabled = useHCA.Checked;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.btnEncode = new System.Windows.Forms.Button();
			this.btnOutputFile = new System.Windows.Forms.Button();
			this.tboxOutputFile = new System.Windows.Forms.TextBox();
			this.gbCompSettings = new System.Windows.Forms.GroupBox();
			this.lblQuality = new System.Windows.Forms.Label();
			this.useHCA = new System.Windows.Forms.CheckBox();
			this.comboxHCAQuality = new System.Windows.Forms.ComboBox();
			this.comboxBitrate = new System.Windows.Forms.ComboBox();
			this.useInputFramerate = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tboxResizeHeight = new System.Windows.Forms.TextBox();
			this.lblResizeX = new System.Windows.Forms.Label();
			this.tboxResizeWidth = new System.Windows.Forms.TextBox();
			this.chboxResizeEnabled = new System.Windows.Forms.CheckBox();
			this.lblResize = new System.Windows.Forms.Label();
			this.lblFps = new System.Windows.Forms.Label();
			this.comboxFramerate = new System.Windows.Forms.ComboBox();
			this.lblFramerate = new System.Windows.Forms.Label();
			this.lblKbps = new System.Windows.Forms.Label();
			this.lblBitrate = new System.Windows.Forms.Label();
			this.btnCuePointFile = new System.Windows.Forms.Button();
			this.tboxCuePointFile = new System.Windows.Forms.TextBox();
			this.comboxAudioTrackSelect = new System.Windows.Forms.ComboBox();
			this.lblAudioSelect = new System.Windows.Forms.Label();
			this.gbInputAudioMaterials = new System.Windows.Forms.GroupBox();
			this.dummyPanelInputAudio = new CriMvEncoderControl.AudioSettingSubPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.comboxSubtitleChannelSelect = new System.Windows.Forms.ComboBox();
			this.gbInputSubtitleFile = new System.Windows.Forms.GroupBox();
			this.dummyPanelInputSubtitle = new CriMvEncoderControl.SubtitleSettingSubPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tboxInputVideoFile = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btnInputVideoFile = new System.Windows.Forms.Button();
			this.chboxUseAlphaCh = new System.Windows.Forms.CheckBox();
			this.chboxUseAudioTrack = new System.Windows.Forms.CheckBox();
			this.btnPreview = new System.Windows.Forms.Button();
			this.progbarEncode = new System.Windows.Forms.ProgressBar();
			this.lblProgress = new System.Windows.Forms.Label();
			this.richtboxStdout = new System.Windows.Forms.RichTextBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblEncodingLog = new System.Windows.Forms.Label();
			this.lblProgressPercent = new System.Windows.Forms.Label();
			this.richtboxStderr = new System.Windows.Forms.RichTextBox();
			this.lblErrorLog = new System.Windows.Forms.Label();
			this.gbEncoding = new System.Windows.Forms.GroupBox();
			this.chboxExtendedPreview = new System.Windows.Forms.CheckBox();
			this.btnLoadSettings = new System.Windows.Forms.Button();
			this.btnSaveSettings = new System.Windows.Forms.Button();
			this.gbCompSettings.SuspendLayout();
			this.gbInputAudioMaterials.SuspendLayout();
			this.gbInputSubtitleFile.SuspendLayout();
			this.gbEncoding.SuspendLayout();
			base.SuspendLayout();
			this.btnEncode.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnEncode.Enabled = false;
			this.btnEncode.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.btnEncode.Location = new System.Drawing.Point(644, 554);
			this.btnEncode.Name = "btnEncode";
			this.btnEncode.Size = new System.Drawing.Size(75, 26);
			this.btnEncode.TabIndex = 40;
			this.btnEncode.Text = "&Encode";
			this.btnEncode.UseVisualStyleBackColor = true;
			this.btnEncode.Click += new System.EventHandler(btnEncode_Click);
			this.btnOutputFile.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.btnOutputFile.Location = new System.Drawing.Point(352, 62);
			this.btnOutputFile.Name = "btnOutputFile";
			this.btnOutputFile.Size = new System.Drawing.Size(24, 22);
			this.btnOutputFile.TabIndex = 5;
			this.btnOutputFile.Text = "...";
			this.btnOutputFile.UseVisualStyleBackColor = true;
			this.btnOutputFile.Click += new System.EventHandler(btnOutputFile_Click);
			this.tboxOutputFile.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.tboxOutputFile.Location = new System.Drawing.Point(90, 63);
			this.tboxOutputFile.Name = "tboxOutputFile";
			this.tboxOutputFile.Size = new System.Drawing.Size(257, 20);
			this.tboxOutputFile.TabIndex = 4;
			this.tboxOutputFile.Text = " *.usm";
			this.tboxOutputFile.TextChanged += new System.EventHandler(tboxOutputFile_TextChanged);
			this.gbCompSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.gbCompSettings.Controls.Add(this.lblQuality);
			this.gbCompSettings.Controls.Add(this.useHCA);
			this.gbCompSettings.Controls.Add(this.comboxHCAQuality);
			this.gbCompSettings.Controls.Add(this.comboxBitrate);
			this.gbCompSettings.Controls.Add(this.useInputFramerate);
			this.gbCompSettings.Controls.Add(this.label5);
			this.gbCompSettings.Controls.Add(this.tboxResizeHeight);
			this.gbCompSettings.Controls.Add(this.lblResizeX);
			this.gbCompSettings.Controls.Add(this.tboxResizeWidth);
			this.gbCompSettings.Controls.Add(this.chboxResizeEnabled);
			this.gbCompSettings.Controls.Add(this.lblResize);
			this.gbCompSettings.Controls.Add(this.lblFps);
			this.gbCompSettings.Controls.Add(this.comboxFramerate);
			this.gbCompSettings.Controls.Add(this.lblFramerate);
			this.gbCompSettings.Controls.Add(this.lblKbps);
			this.gbCompSettings.Controls.Add(this.lblBitrate);
			this.gbCompSettings.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.gbCompSettings.ForeColor = System.Drawing.SystemColors.WindowText;
			this.gbCompSettings.Location = new System.Drawing.Point(396, 10);
			this.gbCompSettings.Name = "gbCompSettings";
			this.gbCompSettings.Size = new System.Drawing.Size(404, 129);
			this.gbCompSettings.TabIndex = 27;
			this.gbCompSettings.TabStop = false;
			this.gbCompSettings.Text = "Video Settings";
			this.lblQuality.AutoSize = true;
			this.lblQuality.Font = new System.Drawing.Font("Arial", 8.25f);
			this.lblQuality.Location = new System.Drawing.Point(80, 101);
			this.lblQuality.Name = "lblQuality";
			this.lblQuality.Size = new System.Drawing.Size(65, 14);
			this.lblQuality.TabIndex = 39;
			this.lblQuality.Text = "HCA quality:";
			this.useHCA.AutoSize = true;
			this.useHCA.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.useHCA.Location = new System.Drawing.Point(212, 76);
			this.useHCA.Name = "useHCA";
			this.useHCA.Size = new System.Drawing.Size(163, 18);
			this.useHCA.TabIndex = 38;
			this.useHCA.Text = "Encode audio as HCA codec";
			this.useHCA.UseVisualStyleBackColor = true;
			this.useHCA.CheckedChanged += new System.EventHandler(useHCA_CheckedChanged);
			this.comboxHCAQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboxHCAQuality.Enabled = false;
			this.comboxHCAQuality.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.comboxHCAQuality.FormattingEnabled = true;
			this.comboxHCAQuality.Items.AddRange(new object[5] { "Highest", "High (default)", "Medium", "Low (high compression)", "Lowest (highest compression)" });
			this.comboxHCAQuality.Location = new System.Drawing.Point(154, 98);
			this.comboxHCAQuality.Name = "comboxHCAQuality";
			this.comboxHCAQuality.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.comboxHCAQuality.Size = new System.Drawing.Size(197, 22);
			this.comboxHCAQuality.TabIndex = 37;
			this.comboxBitrate.AllowDrop = true;
			this.comboxBitrate.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.comboxBitrate.FormatString = "N0";
			this.comboxBitrate.FormattingEnabled = true;
			this.comboxBitrate.Items.AddRange(new object[10] { "40000", "30000", "20000", "36000", "26000", "16000", "12000", "8000", "4000", "3600" });
			this.comboxBitrate.Location = new System.Drawing.Point(83, 21);
			this.comboxBitrate.Name = "comboxBitrate";
			this.comboxBitrate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.comboxBitrate.Size = new System.Drawing.Size(65, 22);
			this.comboxBitrate.TabIndex = 36;
			this.useInputFramerate.AutoSize = true;
			this.useInputFramerate.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.useInputFramerate.Location = new System.Drawing.Point(36, 76);
			this.useInputFramerate.Name = "useInputFramerate";
			this.useInputFramerate.Size = new System.Drawing.Size(163, 18);
			this.useInputFramerate.TabIndex = 35;
			this.useInputFramerate.Text = "Use framerate from input file";
			this.useInputFramerate.UseVisualStyleBackColor = true;
			this.useInputFramerate.CheckedChanged += new System.EventHandler(useInputFramerate_CheckedChanged);
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Arial", 8.25f);
			this.label5.Location = new System.Drawing.Point(244, 51);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(14, 14);
			this.label5.TabIndex = 15;
			this.label5.Text = "H";
			this.tboxResizeHeight.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.tboxResizeHeight.Location = new System.Drawing.Point(262, 48);
			this.tboxResizeHeight.Name = "tboxResizeHeight";
			this.tboxResizeHeight.Size = new System.Drawing.Size(64, 20);
			this.tboxResizeHeight.TabIndex = 33;
			this.tboxResizeHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(tboxResizeHeight_KeyPress);
			this.lblResizeX.AutoSize = true;
			this.lblResizeX.Font = new System.Drawing.Font("Arial", 8.25f);
			this.lblResizeX.Location = new System.Drawing.Point(243, 25);
			this.lblResizeX.Name = "lblResizeX";
			this.lblResizeX.Size = new System.Drawing.Size(17, 14);
			this.lblResizeX.TabIndex = 12;
			this.lblResizeX.Text = "W";
			this.tboxResizeWidth.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.tboxResizeWidth.Location = new System.Drawing.Point(262, 22);
			this.tboxResizeWidth.Name = "tboxResizeWidth";
			this.tboxResizeWidth.Size = new System.Drawing.Size(64, 20);
			this.tboxResizeWidth.TabIndex = 32;
			this.tboxResizeWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(tboxResizeWidth_KeyPress);
			this.chboxResizeEnabled.AutoSize = true;
			this.chboxResizeEnabled.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.chboxResizeEnabled.Location = new System.Drawing.Point(332, 36);
			this.chboxResizeEnabled.Name = "chboxResizeEnabled";
			this.chboxResizeEnabled.Size = new System.Drawing.Size(40, 18);
			this.chboxResizeEnabled.TabIndex = 34;
			this.chboxResizeEnabled.Text = "On";
			this.chboxResizeEnabled.UseVisualStyleBackColor = true;
			this.lblResize.AutoSize = true;
			this.lblResize.Font = new System.Drawing.Font("Arial", 8.25f);
			this.lblResize.Location = new System.Drawing.Point(194, 23);
			this.lblResize.Name = "lblResize";
			this.lblResize.Size = new System.Drawing.Size(43, 14);
			this.lblResize.TabIndex = 9;
			this.lblResize.Text = "Resize:";
			this.lblFps.AutoSize = true;
			this.lblFps.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.lblFps.Location = new System.Drawing.Point(151, 51);
			this.lblFps.Name = "lblFps";
			this.lblFps.Size = new System.Drawing.Size(23, 14);
			this.lblFps.TabIndex = 8;
			this.lblFps.Text = "fps";
			this.comboxFramerate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboxFramerate.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.comboxFramerate.FormattingEnabled = true;
			this.comboxFramerate.Items.AddRange(new object[11]
			{
				"60", "59.94", "50", "30", "29.97", "25", "24", "23.98", "20", "15",
				"10"
			});
			this.comboxFramerate.Location = new System.Drawing.Point(83, 48);
			this.comboxFramerate.Name = "comboxFramerate";
			this.comboxFramerate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.comboxFramerate.Size = new System.Drawing.Size(65, 22);
			this.comboxFramerate.TabIndex = 31;
			this.lblFramerate.AutoSize = true;
			this.lblFramerate.Font = new System.Drawing.Font("Arial", 8.25f);
			this.lblFramerate.Location = new System.Drawing.Point(15, 51);
			this.lblFramerate.Name = "lblFramerate";
			this.lblFramerate.Size = new System.Drawing.Size(59, 14);
			this.lblFramerate.TabIndex = 6;
			this.lblFramerate.Text = "Framerate:";
			this.lblKbps.AutoSize = true;
			this.lblKbps.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.lblKbps.Location = new System.Drawing.Point(151, 24);
			this.lblKbps.Name = "lblKbps";
			this.lblKbps.Size = new System.Drawing.Size(30, 14);
			this.lblKbps.TabIndex = 5;
			this.lblKbps.Text = "kbps";
			this.lblBitrate.AutoSize = true;
			this.lblBitrate.Font = new System.Drawing.Font("Arial", 8.25f);
			this.lblBitrate.Location = new System.Drawing.Point(33, 25);
			this.lblBitrate.Name = "lblBitrate";
			this.lblBitrate.Size = new System.Drawing.Size(41, 14);
			this.lblBitrate.TabIndex = 3;
			this.lblBitrate.Text = "Bitrate:";
			this.btnCuePointFile.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.btnCuePointFile.Location = new System.Drawing.Point(352, 88);
			this.btnCuePointFile.Name = "btnCuePointFile";
			this.btnCuePointFile.Size = new System.Drawing.Size(24, 22);
			this.btnCuePointFile.TabIndex = 7;
			this.btnCuePointFile.Text = "...";
			this.btnCuePointFile.UseVisualStyleBackColor = true;
			this.btnCuePointFile.Click += new System.EventHandler(btnCuePointFile_Click);
			this.tboxCuePointFile.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.tboxCuePointFile.Location = new System.Drawing.Point(90, 89);
			this.tboxCuePointFile.Name = "tboxCuePointFile";
			this.tboxCuePointFile.Size = new System.Drawing.Size(257, 20);
			this.tboxCuePointFile.TabIndex = 6;
			this.tboxCuePointFile.Text = " *.txt";
			this.tboxCuePointFile.TextChanged += new System.EventHandler(tboxCuePointFile_TextChanged);
			this.comboxAudioTrackSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboxAudioTrackSelect.Font = new System.Drawing.Font("Arial", 8.25f);
			this.comboxAudioTrackSelect.FormattingEnabled = true;
			this.comboxAudioTrackSelect.Items.AddRange(new object[32]
			{
				"Track 0", "Track 1", "Track 2", "Track 3", "Track 4", "Track 5", "Track 6", "Track 7", "Track 8", "Track 9",
				"Track 10", "Track 11", "Track 12", "Track 13", "Track 14", "Track 15", "Track 16", "Track 17", "Track 18", "Track 19",
				"Track 20", "Track 21", "Track 22", "Track 23", "Track 24", "Track 25", "Track 26", "Track 27", "Track 28", "Track 29",
				"Track 30", "Track 31"
			});
			this.comboxAudioTrackSelect.Location = new System.Drawing.Point(96, 187);
			this.comboxAudioTrackSelect.Name = "comboxAudioTrackSelect";
			this.comboxAudioTrackSelect.Size = new System.Drawing.Size(115, 22);
			this.comboxAudioTrackSelect.TabIndex = 13;
			this.comboxAudioTrackSelect.SelectedIndexChanged += new System.EventHandler(comboxInputAudio_SelectedIndexChanged);
			this.lblAudioSelect.AutoSize = true;
			this.lblAudioSelect.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.lblAudioSelect.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblAudioSelect.Location = new System.Drawing.Point(10, 1);
			this.lblAudioSelect.Name = "lblAudioSelect";
			this.lblAudioSelect.Size = new System.Drawing.Size(88, 14);
			this.lblAudioSelect.TabIndex = 3;
			this.lblAudioSelect.Text = "Other Audio:    ";
			this.gbInputAudioMaterials.Controls.Add(this.dummyPanelInputAudio);
			this.gbInputAudioMaterials.Controls.Add(this.lblAudioSelect);
			this.gbInputAudioMaterials.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.gbInputAudioMaterials.Location = new System.Drawing.Point(7, 191);
			this.gbInputAudioMaterials.Name = "gbInputAudioMaterials";
			this.gbInputAudioMaterials.Size = new System.Drawing.Size(379, 270);
			this.gbInputAudioMaterials.TabIndex = 23;
			this.gbInputAudioMaterials.TabStop = false;
			this.dummyPanelInputAudio.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.dummyPanelInputAudio.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.dummyPanelInputAudio.Location = new System.Drawing.Point(6, 26);
			this.dummyPanelInputAudio.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.dummyPanelInputAudio.Name = "dummyPanelInputAudio";
			this.dummyPanelInputAudio.Size = new System.Drawing.Size(367, 243);
			this.dummyPanelInputAudio.TabIndex = 14;
			this.dummyPanelInputAudio.Load += new System.EventHandler(subPanelInputAudio_Load);
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 8.25f);
			this.label2.Location = new System.Drawing.Point(11, 92);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 14);
			this.label2.TabIndex = 37;
			this.label2.Text = "Cue Point File:";
			this.comboxSubtitleChannelSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboxSubtitleChannelSelect.Font = new System.Drawing.Font("Arial", 8.25f);
			this.comboxSubtitleChannelSelect.FormattingEnabled = true;
			this.comboxSubtitleChannelSelect.Items.AddRange(new object[32]
			{
				"Track 0", "Track 1", "Track 2", "Track 3", "Track 4", "Track 5", "Track 6", "Track 7", "Track 8", "Track 9",
				"Track 10", "Track 11", "Track 12", "Track 13", "Track 14", "Track 15", "Track 16", "Track 17", "Track 18", "Track 19",
				"Track 20", "Track 21", "Track 22", "Track 23", "Track 24", "Track 25", "Track 26", "Track 27", "Track 28", "Track 29",
				"Track 30", "Track 31"
			});
			this.comboxSubtitleChannelSelect.Location = new System.Drawing.Point(97, 119);
			this.comboxSubtitleChannelSelect.Name = "comboxSubtitleChannelSelect";
			this.comboxSubtitleChannelSelect.Size = new System.Drawing.Size(115, 22);
			this.comboxSubtitleChannelSelect.TabIndex = 8;
			this.comboxSubtitleChannelSelect.SelectedIndexChanged += new System.EventHandler(comboxSubtitleChannelSelect_SelectedIndexChanged);
			this.gbInputSubtitleFile.Controls.Add(this.dummyPanelInputSubtitle);
			this.gbInputSubtitleFile.Controls.Add(this.label1);
			this.gbInputSubtitleFile.Location = new System.Drawing.Point(7, 125);
			this.gbInputSubtitleFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.gbInputSubtitleFile.Name = "gbInputSubtitleFile";
			this.gbInputSubtitleFile.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.gbInputSubtitleFile.Size = new System.Drawing.Size(379, 55);
			this.gbInputSubtitleFile.TabIndex = 32;
			this.gbInputSubtitleFile.TabStop = false;
			this.gbInputSubtitleFile.TextChanged += new System.EventHandler(gbInputSubtitleFile_TextChanged);
			this.dummyPanelInputSubtitle.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.dummyPanelInputSubtitle.Location = new System.Drawing.Point(12, 19);
			this.dummyPanelInputSubtitle.Margin = new System.Windows.Forms.Padding(1);
			this.dummyPanelInputSubtitle.Name = "dummyPanelInputSubtitle";
			this.dummyPanelInputSubtitle.Size = new System.Drawing.Size(360, 31);
			this.dummyPanelInputSubtitle.TabIndex = 9;
			this.dummyPanelInputSubtitle.Load += new System.EventHandler(dummyPanelInputSubtitle_Load);
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Location = new System.Drawing.Point(9, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(93, 14);
			this.label1.TabIndex = 5;
			this.label1.Text = "Subtitle Text:     ";
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.label3.Location = new System.Drawing.Point(13, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 14);
			this.label3.TabIndex = 36;
			this.label3.Text = "Input Name:";
			this.tboxInputVideoFile.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.tboxInputVideoFile.Location = new System.Drawing.Point(90, 11);
			this.tboxInputVideoFile.Name = "tboxInputVideoFile";
			this.tboxInputVideoFile.Size = new System.Drawing.Size(257, 20);
			this.tboxInputVideoFile.TabIndex = 0;
			this.tboxInputVideoFile.Text = " *.avi,*.bmp,*.tga";
			this.tboxInputVideoFile.TextChanged += new System.EventHandler(tboxInputVideoFile_TextChanged);
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 8.25f);
			this.label4.Location = new System.Drawing.Point(13, 66);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 14);
			this.label4.TabIndex = 16;
			this.label4.Text = "Output Name:";
			this.btnInputVideoFile.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.btnInputVideoFile.Location = new System.Drawing.Point(352, 10);
			this.btnInputVideoFile.Name = "btnInputVideoFile";
			this.btnInputVideoFile.Size = new System.Drawing.Size(24, 22);
			this.btnInputVideoFile.TabIndex = 1;
			this.btnInputVideoFile.Text = "...";
			this.btnInputVideoFile.UseVisualStyleBackColor = true;
			this.btnInputVideoFile.Click += new System.EventHandler(btnInputVideoFile_Click);
			this.chboxUseAlphaCh.AutoSize = true;
			this.chboxUseAlphaCh.Enabled = false;
			this.chboxUseAlphaCh.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.chboxUseAlphaCh.Location = new System.Drawing.Point(202, 37);
			this.chboxUseAlphaCh.Name = "chboxUseAlphaCh";
			this.chboxUseAlphaCh.Size = new System.Drawing.Size(117, 18);
			this.chboxUseAlphaCh.TabIndex = 3;
			this.chboxUseAlphaCh.Text = "Use Alpha Channel";
			this.chboxUseAlphaCh.UseVisualStyleBackColor = true;
			this.chboxUseAudioTrack.AutoSize = true;
			this.chboxUseAudioTrack.Enabled = false;
			this.chboxUseAudioTrack.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.chboxUseAudioTrack.Location = new System.Drawing.Point(90, 37);
			this.chboxUseAudioTrack.Name = "chboxUseAudioTrack";
			this.chboxUseAudioTrack.Size = new System.Drawing.Size(105, 18);
			this.chboxUseAudioTrack.TabIndex = 2;
			this.chboxUseAudioTrack.Text = "Use Audio Track";
			this.chboxUseAudioTrack.UseVisualStyleBackColor = true;
			this.chboxUseAudioTrack.CheckedChanged += new System.EventHandler(chboxUseAudioTrack_CheckedChanged);
			this.btnPreview.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnPreview.Enabled = false;
			this.btnPreview.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
			this.btnPreview.Location = new System.Drawing.Point(725, 554);
			this.btnPreview.Name = "btnPreview";
			this.btnPreview.Size = new System.Drawing.Size(75, 26);
			this.btnPreview.TabIndex = 41;
			this.btnPreview.Text = "&Preview";
			this.btnPreview.UseVisualStyleBackColor = true;
			this.btnPreview.Click += new System.EventHandler(btnPreview_Click);
			this.progbarEncode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.progbarEncode.Enabled = false;
			this.progbarEncode.Location = new System.Drawing.Point(10, 40);
			this.progbarEncode.Name = "progbarEncode";
			this.progbarEncode.Size = new System.Drawing.Size(318, 22);
			this.progbarEncode.TabIndex = 0;
			this.lblProgress.AutoSize = true;
			this.lblProgress.Font = new System.Drawing.Font("Arial", 8.25f);
			this.lblProgress.Location = new System.Drawing.Point(7, 23);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(54, 14);
			this.lblProgress.TabIndex = 1;
			this.lblProgress.Text = "Progress:";
			this.richtboxStdout.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.richtboxStdout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richtboxStdout.Font = new System.Drawing.Font("Courier New", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.richtboxStdout.Location = new System.Drawing.Point(10, 82);
			this.richtboxStdout.Name = "richtboxStdout";
			this.richtboxStdout.ReadOnly = true;
			this.richtboxStdout.Size = new System.Drawing.Size(384, 215);
			this.richtboxStdout.TabIndex = 36;
			this.richtboxStdout.Text = "";
			this.richtboxStdout.WordWrap = false;
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.btnCancel.Location = new System.Drawing.Point(334, 40);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(60, 22);
			this.btnCancel.TabIndex = 35;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
			this.lblEncodingLog.AutoSize = true;
			this.lblEncodingLog.Font = new System.Drawing.Font("Arial", 8.25f);
			this.lblEncodingLog.Location = new System.Drawing.Point(7, 65);
			this.lblEncodingLog.Name = "lblEncodingLog";
			this.lblEncodingLog.Size = new System.Drawing.Size(75, 14);
			this.lblEncodingLog.TabIndex = 4;
			this.lblEncodingLog.Text = "Encoding Log:";
			this.lblProgressPercent.BackColor = System.Drawing.SystemColors.Control;
			this.lblProgressPercent.Font = new System.Drawing.Font("Arial", 8.25f);
			this.lblProgressPercent.Location = new System.Drawing.Point(66, 21);
			this.lblProgressPercent.Name = "lblProgressPercent";
			this.lblProgressPercent.Size = new System.Drawing.Size(40, 17);
			this.lblProgressPercent.TabIndex = 5;
			this.lblProgressPercent.Text = "0%";
			this.lblProgressPercent.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.richtboxStderr.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.richtboxStderr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richtboxStderr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.richtboxStderr.Location = new System.Drawing.Point(10, 317);
			this.richtboxStderr.Name = "richtboxStderr";
			this.richtboxStderr.ReadOnly = true;
			this.richtboxStderr.Size = new System.Drawing.Size(384, 77);
			this.richtboxStderr.TabIndex = 37;
			this.richtboxStderr.Text = "";
			this.richtboxStderr.WordWrap = false;
			this.lblErrorLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.lblErrorLog.AutoSize = true;
			this.lblErrorLog.Font = new System.Drawing.Font("Arial", 8.25f);
			this.lblErrorLog.Location = new System.Drawing.Point(7, 300);
			this.lblErrorLog.Name = "lblErrorLog";
			this.lblErrorLog.Size = new System.Drawing.Size(55, 14);
			this.lblErrorLog.TabIndex = 7;
			this.lblErrorLog.Text = "Error Log:";
			this.gbEncoding.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.gbEncoding.BackColor = System.Drawing.SystemColors.Control;
			this.gbEncoding.Controls.Add(this.lblErrorLog);
			this.gbEncoding.Controls.Add(this.richtboxStderr);
			this.gbEncoding.Controls.Add(this.lblProgressPercent);
			this.gbEncoding.Controls.Add(this.lblEncodingLog);
			this.gbEncoding.Controls.Add(this.btnCancel);
			this.gbEncoding.Controls.Add(this.richtboxStdout);
			this.gbEncoding.Controls.Add(this.lblProgress);
			this.gbEncoding.Controls.Add(this.progbarEncode);
			this.gbEncoding.Enabled = false;
			this.gbEncoding.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.gbEncoding.ForeColor = System.Drawing.SystemColors.WindowText;
			this.gbEncoding.Location = new System.Drawing.Point(396, 144);
			this.gbEncoding.Name = "gbEncoding";
			this.gbEncoding.Size = new System.Drawing.Size(404, 404);
			this.gbEncoding.TabIndex = 30;
			this.gbEncoding.TabStop = false;
			this.gbEncoding.Text = "Encoding";
			this.chboxExtendedPreview.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.chboxExtendedPreview.AutoSize = true;
			this.chboxExtendedPreview.Checked = true;
			this.chboxExtendedPreview.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chboxExtendedPreview.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.chboxExtendedPreview.Location = new System.Drawing.Point(446, 559);
			this.chboxExtendedPreview.Name = "chboxExtendedPreview";
			this.chboxExtendedPreview.Size = new System.Drawing.Size(189, 18);
			this.chboxExtendedPreview.TabIndex = 42;
			this.chboxExtendedPreview.Text = "Preview in extended Video player";
			this.chboxExtendedPreview.UseVisualStyleBackColor = true;
			this.btnLoadSettings.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.btnLoadSettings.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.btnLoadSettings.Location = new System.Drawing.Point(112, 554);
			this.btnLoadSettings.Name = "btnLoadSettings";
			this.btnLoadSettings.Size = new System.Drawing.Size(115, 22);
			this.btnLoadSettings.TabIndex = 45;
			this.btnLoadSettings.Text = "&Load settings...";
			this.btnLoadSettings.UseVisualStyleBackColor = true;
			this.btnLoadSettings.Click += new System.EventHandler(btnLoadSettings_Click);
			this.btnSaveSettings.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.btnSaveSettings.Location = new System.Drawing.Point(13, 553);
			this.btnSaveSettings.Name = "btnSaveSettings";
			this.btnSaveSettings.Size = new System.Drawing.Size(92, 23);
			this.btnSaveSettings.TabIndex = 46;
			this.btnSaveSettings.Text = "&Save settings...";
			this.btnSaveSettings.UseVisualStyleBackColor = true;
			this.btnSaveSettings.Click += new System.EventHandler(btnSaveSettings_Click);
			base.Controls.Add(this.btnSaveSettings);
			base.Controls.Add(this.btnLoadSettings);
			base.Controls.Add(this.chboxExtendedPreview);
			base.Controls.Add(this.comboxAudioTrackSelect);
			base.Controls.Add(this.btnPreview);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.gbCompSettings);
			base.Controls.Add(this.btnEncode);
			base.Controls.Add(this.chboxUseAudioTrack);
			base.Controls.Add(this.gbEncoding);
			base.Controls.Add(this.tboxOutputFile);
			base.Controls.Add(this.comboxSubtitleChannelSelect);
			base.Controls.Add(this.chboxUseAlphaCh);
			base.Controls.Add(this.btnCuePointFile);
			base.Controls.Add(this.btnOutputFile);
			base.Controls.Add(this.btnInputVideoFile);
			base.Controls.Add(this.gbInputSubtitleFile);
			base.Controls.Add(this.gbInputAudioMaterials);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.tboxInputVideoFile);
			base.Controls.Add(this.tboxCuePointFile);
			base.Name = "ParametersSettingPanel";
			base.Size = new System.Drawing.Size(805, 589);
			base.Load += new System.EventHandler(EncodeParamControl_Load);
			this.gbCompSettings.ResumeLayout(false);
			this.gbCompSettings.PerformLayout();
			this.gbInputAudioMaterials.ResumeLayout(false);
			this.gbInputAudioMaterials.PerformLayout();
			this.gbInputSubtitleFile.ResumeLayout(false);
			this.gbInputSubtitleFile.PerformLayout();
			this.gbEncoding.ResumeLayout(false);
			this.gbEncoding.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
