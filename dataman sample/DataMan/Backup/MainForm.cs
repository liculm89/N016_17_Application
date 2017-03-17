using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Cognex.DataMan.SDK;
using System.Threading;
using Cognex.DataMan.Discovery;
using Cognex.DataMan.Utils;

namespace Cognex.DataMan.SDK
{
    public partial class MainForm : Form
    {
        private class ResultInfo
        {
            public ResultInfo(int resultId, Image image, string imageGraphics, string readString)
            {
                ResultId = resultId;
                Image = image;
                ImageGraphics = imageGraphics;
                ReadString = readString;
            }

            public int ResultId { get; set; }
            public Image Image { get; set; }
            public string ImageGraphics { get; set; }
            public string ReadString { get; set; }
        }

        private SynchronizationContext _syncContext = null;
        private EthSystemDiscoverer _ethSystemDiscoverer = null;
        private SerSystemDiscoverer _serSystemDiscoverer = null;
        private ISystemConnector _connector = null;
        private DataManSystem _system = null;
        private object _currentResultInfoSyncLock = new object();
        private ResultInfo _currentResultInfo = new ResultInfo(-1, null, null, null);
        private bool _closing = false;
        private bool _autoconnect = false;
        private object _listAddItemLock = new object();

        public MainForm()
        {
            InitializeComponent();

            // The SDK may fire events from arbitrary thread context. Therefore if you want to change
            // the state of controls or windows from any of the SDK' events, you have to use this
            // synchronization context to execute the event handler code on the main GUI thread.
            _syncContext = WindowsFormsSynchronizationContext.Current;

            //Setting up WindowsCE-specific event handlers manually, as they are lost from the Designer.cs file upon save
#if !WindowsCE
            cbEnableKeepAlive.CheckedChanged += new System.EventHandler(this.cbEnableKeepAlive_CheckedChanged);
#else
            cbEnableKeepAlive.CheckStateChanged += new System.EventHandler(this.cbEnableKeepAlive_CheckedChanged);
#endif

#if !WindowsCE
            cbLiveDisplay.CheckedChanged += new System.EventHandler(this.cbLiveDisplay_CheckedChanged);
#else
            cbLiveDisplay.CheckStateChanged += new System.EventHandler(this.cbLiveDisplay_CheckedChanged);
#endif

#if !WindowsCE
            cbLoggingEnabled.CheckedChanged += new System.EventHandler(this.cbLoggingEnabled_CheckedChanged);
#else
            cbLoggingEnabled.CheckStateChanged += new System.EventHandler(this.cbLoggingEnabled_CheckedChanged);
#endif

#if !WindowsCE
            FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
#else
            Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_FormClosing);
#endif
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Create discoverers to discover ethernet and serial port systems.
            _ethSystemDiscoverer = new EthSystemDiscoverer();
            _serSystemDiscoverer = new SerSystemDiscoverer();

            // Subscribe to the system discoved event.
            _ethSystemDiscoverer.SystemDiscovered += new EthSystemDiscoverer.SystemDiscoveredHandler(OnEthSystemDiscovered);
            _serSystemDiscoverer.SystemDiscovered += new SerSystemDiscoverer.SystemDiscoveredHandler(OnSerSystemDiscovered);

            // Ask the discoverers to start discovering systems.
            _ethSystemDiscoverer.Discover();
            _serSystemDiscoverer.Discover();
        }

        private void MainForm_FormClosing(object sender, EventArgs e)
        {
            _closing = true;
            _autoconnect = false;

            if (null != _system && _system.IsConnected)
                _system.Disconnect();
        }

        private void OnEthSystemDiscovered(EthSystemDiscoverer.SystemInfo systemInfo)
        {
            _syncContext.Post(
                new SendOrPostCallback(
                    delegate
                    {
                        listBoxDetectedSystems.Items.Add(systemInfo);
                    }),
                    null);
        }

        private void OnSerSystemDiscovered(SerSystemDiscoverer.SystemInfo systemInfo)
        {
            _syncContext.Post(
                new SendOrPostCallback(
                    delegate
                    {
                        listBoxDetectedSystems.Items.Add(systemInfo);
                    }),
                    null);
        }

        private void AddListItem(object item)
        {
            lock (_listAddItemLock)
            {
                listBox1.Items.Add(item);
                if (listBox1.Items.Count > 500)
                    listBox1.Items.RemoveAt(0);
                if (listBox1.Items.Count > 0)
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }

        private void OnSystemConnected(object sender, EventArgs args)
        {
            _syncContext.Post(
                delegate
                {
                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = true;
                    btnTrigger.Enabled = true;
                    cbLiveDisplay.Enabled = true;
                    AddListItem("System connected");
                },
                null);
        }

        private void OnSystemDisconnected(object sender, EventArgs args)
        {
            _syncContext.Post(
                delegate
                {
                    AddListItem("System disconnected");

                    if (!_closing && _autoconnect && cbAutoReconnect.Checked)
                    {
                        frmReconnecting frm = new frmReconnecting(this, _system);

                        if (frm.ShowDialog() == DialogResult.Cancel)
                        {
                            btnConnect.Enabled = true;
                            btnDisconnect.Enabled = false;
                            btnTrigger.Enabled = false;
                            cbLiveDisplay.Enabled = false;
                        }
                    }
                    else
                    {
                        btnConnect.Enabled = true;
                        btnDisconnect.Enabled = false;
                        btnTrigger.Enabled = false;
                        cbLiveDisplay.Enabled = false;
                    }
                },
                null);
        }

        private void OnSystemWentOnline(object sender, EventArgs args)
        {
            _syncContext.Post(
                delegate
                {
                    AddListItem("System went online");
                },
                null);
        }

        private void OnSystemWentOffline(object sender, EventArgs args)
        {
            _syncContext.Post(
                delegate
                {
                    AddListItem("System went offline");
                },
                null);
        }

        private void OnReadStringArrived(object sender, ReadStringArrivedEventArgs args)
        {
            // If the current result info has the same result ID as the one we've just
            // received, store the read string in it, otherwise create a new result info object.
            lock (_currentResultInfoSyncLock)
            {
                if (_currentResultInfo.ResultId == args.ResultId)
                    _currentResultInfo.ReadString = args.ReadString;
                else
                    _currentResultInfo = new ResultInfo(args.ResultId, null, null, args.ReadString);
            }

            _syncContext.Post(
                delegate
                {
                    AddListItem("Read string arrived : resultId = " + args.ResultId + ", read string = " + args.ReadString);
                    ShowResult();
                },
                null);
        }

        private void OnXmlResultArrived(object sender, XmlResultArrivedEventArgs args)
        {
            XmlDocument doc = new XmlDocument();
            string read_string = "";

            doc.LoadXml(args.XmlResult);

            foreach (XmlNode node2 in doc.DocumentElement.ChildNodes)
            {
                if (node2.Name.Equals("general"))
                {
                    foreach (XmlNode node in node2.ChildNodes)
                    {
                        if (node.Name.Equals("full_string"))
                        {
                            read_string = node.InnerText;

                            foreach (XmlAttribute att in node.Attributes)
                            {
                                if (att.Name.Equals("encoding") && att.InnerText.Equals("base64") && !String.IsNullOrEmpty(node.InnerText))
                                {
                                    byte[] code = Convert.FromBase64String(node.InnerText);
                                    read_string = System.Text.Encoding.Default.GetString(code, 0, code.Length);
                                }
                            }

                            break;
                        }
                    }

                    // TODO: Process read string in read_string.
                }
            }
        }

        private void OnImageArrived(object sender, ImageArrivedEventArgs args)
        {
            // If the current result info has the same result ID as the one we've just
            // received, store the image in it, otherwise create a new result info object.
            lock (_currentResultInfoSyncLock)
            {
                if (_currentResultInfo.ResultId == args.ResultId)
                    _currentResultInfo.Image = args.Image;
                else
                    _currentResultInfo = new ResultInfo(args.ResultId, args.Image, null, null);
            }

            _syncContext.Post(
                delegate
                {
                    AddListItem("Image arrived (rid=" + args.ResultId.ToString() + "), size = " + args.Image.Width.ToString() + "x" + args.Image.Height.ToString());
                    ShowResult();
                },
                null);
        }

        private void OnImageGraphicsArrived(object sender, ImageGraphicsArrivedEventArgs args)
        {
            // If the current result info has the same result ID as the one we've just
            // received, store the image graphics in it, otherwise create a new result info object.
            lock (_currentResultInfoSyncLock)
            {
                if (_currentResultInfo.ResultId == args.ResultId)
                    _currentResultInfo.ImageGraphics = args.ImageGraphics;
                else
                    _currentResultInfo = new ResultInfo(args.ResultId, null, args.ImageGraphics, null);
            }

            _syncContext.Post(
                delegate
                {
                    AddListItem("Image graphics arrived (rid=" + args.ResultId.ToString() + ")");
                    ShowResult();
                },
                null);
        }

        private void OnBinaryDataTransferProgress(object sender, BinaryDataTransferProgressEventArgs args)
        {
            /*
            _syncContext.Post(
                delegate
                {
                    toolStripProgressBar1.Value = (int)(100 * (args.BytesTransferred / (double)args.TotalDataSize));
                },
                null);
            */
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;

            try
            {
                if (listBoxDetectedSystems.SelectedIndex != -1 && listBoxDetectedSystems.Items.Count > listBoxDetectedSystems.SelectedIndex)
                {
                    var system_info = listBoxDetectedSystems.Items[listBoxDetectedSystems.SelectedIndex];

                    if (system_info is EthSystemDiscoverer.SystemInfo)
                    {
                        EthSystemDiscoverer.SystemInfo eth_system_info = system_info as EthSystemDiscoverer.SystemInfo;
                        EthSystemConnector conn = new EthSystemConnector(eth_system_info.IPAddress);

                        conn.UserName = "admin";
                        conn.Password = txtPassword.Text;

                        _connector = conn;
                    }
                    else if (system_info is SerSystemDiscoverer.SystemInfo)
                    {
                        SerSystemDiscoverer.SystemInfo ser_system_info = system_info as SerSystemDiscoverer.SystemInfo;
                        SerSystemConnector conn = new SerSystemConnector(ser_system_info.PortName, ser_system_info.Baudrate);

                        _connector = conn;
                    }
                }

                _connector.Logger = new GuiLogger(tbLog, cbLoggingEnabled.Checked, ref _closing);

                _system = new DataManSystem(_connector);
                _system.DefaultTimeout = 5000;

                // Subscribe to events that are signalled when the system is connected / disconnected.
                _system.SystemConnected += new SystemConnectedHandler(OnSystemConnected);
                _system.SystemDisconnected += new SystemDisconnectedHandler(OnSystemDisconnected);

                _system.SystemWentOnline += new SystemWentOnlineHandler(OnSystemWentOnline);
                _system.SystemWentOffline += new SystemWentOfflineHandler(OnSystemWentOffline);

                // Subscribe to events that are signalled when the deveice sends auto-responses.
                _system.ReadStringArrived += new ReadStringArrivedHandler(OnReadStringArrived);
                _system.XmlResultArrived += new XmlResultArrivedHandler(OnXmlResultArrived);
                _system.ImageArrived += new ImageArrivedHandler(OnImageArrived);
                _system.ImageGraphicsArrived += new ImageGraphicsArrivedHandler(OnImageGraphicsArrived);
                _system.BinaryDataTransferProgress += new BinaryDataTransferProgressHandler(OnBinaryDataTransferProgress);

                _system.SetKeepAliveOptions(cbEnableKeepAlive.Checked, 3000, 1000);

                _system.Connect();
            }
            catch (Exception ex)
            {
                CleanupConnection();

                AddListItem("Failed to connect: " + ex.ToString());
                btnConnect.Enabled = true;
            }

            _autoconnect = true;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            cbLiveDisplay.Checked = false;
            btnDisconnect.Enabled = false;

            _autoconnect = false;

            _system.Disconnect();
            _system.SystemConnected -= new SystemConnectedHandler(OnSystemConnected);
            _system.SystemDisconnected -= new SystemDisconnectedHandler(OnSystemDisconnected);
            _system.SystemWentOnline -= new SystemWentOnlineHandler(OnSystemWentOnline);
            _system.SystemWentOffline -= new SystemWentOfflineHandler(OnSystemWentOffline);

            _system.ReadStringArrived -= new ReadStringArrivedHandler(OnReadStringArrived);
            _system.XmlResultArrived -= new XmlResultArrivedHandler(OnXmlResultArrived);
            _system.ImageArrived -= new ImageArrivedHandler(OnImageArrived);
            _system.ImageGraphicsArrived -= new ImageGraphicsArrivedHandler(OnImageGraphicsArrived);
            _system.BinaryDataTransferProgress -= new BinaryDataTransferProgressHandler(OnBinaryDataTransferProgress);

            CleanupConnection();
        }

        private void cbEnableKeepAlive_CheckedChanged(object sender, EventArgs e)
        {
            if (null != _system)
                _system.SetKeepAliveOptions(cbEnableKeepAlive.Checked, 3000, 1000);
        }

        private void cbLiveDisplay_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbLiveDisplay.Checked)
                {
                    btnTrigger.Enabled = false;

                    _system.SendCommand("SET LIVEIMG.MODE 2");
                    _system.BeginGetLiveImage(
                        ImageFormat.jpeg,
                        ImageSize.Sixteenth,
                        ImageQuality.Medium,
                        OnLiveImageArrived,
                        null);
                }
                else
                {
                    btnTrigger.Enabled = true;

                    _system.SendCommand("SET LIVEIMG.MODE 0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to set live image mode: " + ex.ToString());
            }
        }

        private void listBoxDetectedSystems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxDetectedSystems.SelectedIndex != -1 && listBoxDetectedSystems.Items.Count > listBoxDetectedSystems.SelectedIndex)
            {
                var system_info = listBoxDetectedSystems.Items[listBoxDetectedSystems.SelectedIndex];

                if (system_info is EthSystemDiscoverer.SystemInfo)
                {
                    EthSystemDiscoverer.SystemInfo eth_system_info = system_info as EthSystemDiscoverer.SystemInfo;

                    txtDeviceIP.Text = eth_system_info.IPAddress.ToString();
                }
                else if (system_info is SerSystemDiscoverer.SystemInfo)
                {
                    SerSystemDiscoverer.SystemInfo ser_system_info = system_info as SerSystemDiscoverer.SystemInfo;

                    txtDeviceIP.Text = ser_system_info.PortName;
                }

                btnConnect.Enabled = (_connector != null && !_connector.IsConnected) || (_connector == null);
            }
            else
                btnConnect.Enabled = false;
        }

        private void btnRefreshSystemList_Click(object sender, EventArgs e)
        {
            listBoxDetectedSystems.Items.Clear();
            btnConnect.Enabled = (_connector != null && !_connector.IsConnected);

            _ethSystemDiscoverer.Discover();
            _serSystemDiscoverer.Discover();
        }

        private void btnTrigger_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                _system.SendCommand("TRIGGER ON");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send TRIGGER ON command: " + ex.ToString());
            }
        }

        private void btnTrigger_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                _system.SendCommand("TRIGGER OFF");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send TRIGGER OFF command: " + ex.ToString());
            }
        }

        private void CleanupConnection()
        {
            if (null != _system)
            {
                _system.SystemConnected -= OnSystemConnected;
                _system.SystemDisconnected -= OnSystemDisconnected;
                _system.ReadStringArrived -= OnReadStringArrived;
                _system.ImageArrived -= OnImageArrived;
                _system.ImageGraphicsArrived -= OnImageGraphicsArrived;
            }

            _connector = null;
            _system = null;
        }

        private void OnLiveImageArrived(IAsyncResult result)
        {
            try
            {
                Image image = _system.EndGetLiveImage(result);
                Size image_size = Gui.FitImageInControl(image.Size, picResultImage.Size);
                Image fitted_image = Gui.ResizeImageToBitmap(image, image_size);

                _syncContext.Post(
                    delegate
                    {
                        picResultImage.Image = fitted_image;
                        picResultImage.Invalidate();
                    },
                null);

                if (cbLiveDisplay.Checked)
                {
                    _system.BeginGetLiveImage(
                        ImageFormat.jpeg,
                        ImageSize.Sixteenth,
                        ImageQuality.Medium,
                        OnLiveImageArrived,
                        null);
                }
            }
            catch
            {
            }
        }

        private void ShowResult()
        {
            Image image = null;
            string image_graphics = null;
            string read_result = null;

            // Take a reference or copy values from the locked result info object. This is done
            // so that the lock is used only for a short period of time.
            lock (_currentResultInfoSyncLock)
            {
                image = _currentResultInfo.Image;
                image_graphics = _currentResultInfo.ImageGraphics;
                read_result = _currentResultInfo.ReadString;
            }

            if (image != null)
            {
                Size image_size = Gui.FitImageInControl(image.Size, picResultImage.Size);
                Image fitted_image = Gui.ResizeImageToBitmap(image, image_size);

                if (image_graphics != null)
                {
                    ResultGraphics rg = GraphicsResultParser.Parse(image_graphics, new Rectangle(0, 0, image_size.Width, image_size.Height));
                    Graphics g = Graphics.FromImage(fitted_image);

                    ResultGraphicsRenderer.PaintResults(g, rg);
                    g.Dispose();
                }

                picResultImage.Image = fitted_image;
                picResultImage.Invalidate();
            }

            if (read_result != null)
                lbReadString.Text = read_result;
        }

        private void cbLoggingEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (_connector != null && _connector.Logger != null)
            {
                _connector.Logger.Enabled = cbLoggingEnabled.Checked;
                _connector.Logger.Log("Logging", _connector.Logger.Enabled ? "enabled" : "disabled");
            }
        }

        private class GuiLogger : ILogger
        {
            private TextBox LogBox;
            private bool? IsGuiClosing;

            public GuiLogger(TextBox logBox, bool enabled, ref bool isGuiClosing)
            {
                LogBox = logBox;
                Enabled = enabled;
                IsGuiClosing = isGuiClosing;
            }

            public bool Enabled
            {
                get;
                set;
            }

            delegate void LogHandler(string msg);
            private void LogToGui(string msg)
            {
                if (LogBox == null || IsGuiClosing == null || !IsGuiClosing.HasValue || IsGuiClosing.Value == true)
                    return;

                if (LogBox.InvokeRequired)
                {
                    LogBox.BeginInvoke(new LogHandler(LogToGui), new object[] { msg });
                    return;
                }

#if !WindowsCE
                LogBox.AppendText(msg);
#else
                LogBox.Text = LogBox.Text + msg;
#endif
            }

            public void Log(string function, string message)
            {
                LogToGui(String.Format("{0}: {1} [{2}]\r\n", function, message, DateTime.Now.ToLongTimeString()));
            }

            private static string GetBytesAsPrintable(byte[] buffer, int offset, int count)
            {
                if (buffer == null || count < 1 || offset + count > buffer.Length)
                    return "";

                StringBuilder SB = new StringBuilder(count * 6);
                for (int i = offset; i < buffer.Length && i < offset + count; ++i)
                {
                    if (buffer[i] < (byte)' ' || buffer[i] >= 127)
                    {
                        SB.Append(String.Format("<0x{0:X2}>", buffer[i]));
                    }
                    else
                    {
                        SB.Append((char)buffer[i]);
                    }
                }

                return SB.ToString();
            }

            public void LogTraffic(bool isRead, byte[] buffer, int offset, int count)
            {
                LogToGui(String.Format("{0} {1} bytes at {2}: {3}\r\n", isRead ? "Read" : "Written", count, DateTime.Now.ToLongTimeString(), GetBytesAsPrintable(buffer, offset, count)));
            }
        }
    }
}
