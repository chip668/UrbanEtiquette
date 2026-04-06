using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;   // für MessageBox

namespace Anzeige

{
    public class BrowserControl
    {
        public TextBox statusBox = null;
        bool ifActive = false;
        bool ifState = true;
        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;


        // ====================== Virtual Key Codes ======================
        public const ushort VK_TAB = 0x09;   // Tab
        public const ushort VK_RETURN = 0x0D;   // Enter
        public const ushort VK_SPACE = 0x20;   // Leertaste
        public const ushort VK_LEFT = 0x25;   // ←
        public const ushort VK_UP = 0x26;   // ↑
        public const ushort VK_RIGHT = 0x27;   // →
        public const ushort VK_DOWN = 0x28;   // ↓

        public const ushort VK_SHIFT = 0x10;   // Shift
        public const ushort VK_CONTROL = 0x11;   // Strg
        public const ushort VK_MENU = 0x12;   // Alt

        // Zahlen oben auf der Tastatur
        public const ushort VK_0 = 0x30;
        public const ushort VK_1 = 0x31;
        public const ushort VK_2 = 0x32;   // wichtig für @
        public const ushort VK_3 = 0x33;
        public const ushort VK_4 = 0x34;
        public const ushort VK_5 = 0x35;
        public const ushort VK_6 = 0x36;
        public const ushort VK_7 = 0x37;
        public const ushort VK_8 = 0x38;
        public const ushort VK_9 = 0x39;

        // Buchstaben A–Z
        public const ushort VK_A = 0x41;
        public const ushort VK_B = 0x42;
        public const ushort VK_C = 0x43;
        public const ushort VK_D = 0x44;
        public const ushort VK_E = 0x45;
        public const ushort VK_F = 0x46;
        public const ushort VK_G = 0x47;
        public const ushort VK_H = 0x48;
        public const ushort VK_I = 0x49;
        public const ushort VK_J = 0x4A;
        public const ushort VK_K = 0x4B;
        public const ushort VK_L = 0x4C;
        public const ushort VK_M = 0x4D;
        public const ushort VK_N = 0x4E;
        public const ushort VK_O = 0x4F;
        public const ushort VK_P = 0x50;
        public const ushort VK_Q = 0x51;
        public const ushort VK_R = 0x52;
        public const ushort VK_S = 0x53;
        public const ushort VK_T = 0x54;
        public const ushort VK_U = 0x55;
        public const ushort VK_V = 0x56;   // für Strg+V
        public const ushort VK_W = 0x57;
        public const ushort VK_X = 0x58;
        public const ushort VK_Y = 0x59;
        public const ushort VK_Z = 0x5A;

        // Weitere wichtige Tasten
        public const ushort VK_BACK = 0x08;   // Backspace
        public const ushort VK_ESCAPE = 0x1B;   // ESC
        public const ushort VK_INSERT = 0x2D;   // Einfg
        public const ushort VK_DELETE = 0x2E;   // Entf
        public const ushort VK_PRIOR = 0x21;   // Page Up
        public const ushort VK_NEXT = 0x22;   // Page Down
        public const ushort VK_HOME = 0x24;   // Pos1
        public const ushort VK_END = 0x23;   // Ende

        // Funktions­tasten
        public const ushort VK_F1 = 0x70;
        public const ushort VK_F2 = 0x71;
        public const ushort VK_F3 = 0x72;
        public const ushort VK_F4 = 0x73;
        public const ushort VK_F5 = 0x74;
        public const ushort VK_F6 = 0x75;
        public const ushort VK_F7 = 0x76;
        public const ushort VK_F8 = 0x77;
        public const ushort VK_F9 = 0x78;
        public const ushort VK_F10 = 0x79;
        public const ushort VK_F11 = 0x7A;
        public const ushort VK_F12 = 0x7B;

        // NumPad (optional, falls du es brauchst)
        public const ushort VK_NUMPAD0 = 0x60;
        public const ushort VK_NUMPAD1 = 0x61;
        public const ushort VK_NUMPAD2 = 0x62;
        public const ushort VK_NUMPAD3 = 0x63;
        public const ushort VK_NUMPAD4 = 0x64;
        public const ushort VK_NUMPAD5 = 0x65;
        public const ushort VK_NUMPAD6 = 0x66;
        public const ushort VK_NUMPAD7 = 0x67;
        public const ushort VK_NUMPAD8 = 0x68;
        public const ushort VK_NUMPAD9 = 0x69;


        enum BlockType { Wenn, Sonst, Schalter, Fall, Standard }
        class BlockInfo
        {
            public BlockType Type;
            public int IsActive; // 1 = Befehle ausführen danach 2
            public string SwitchValue { get; set; } // <-- für schalter
        }

        Stack<BlockInfo> blockStack = new Stack<BlockInfo>();

        private bool EvaluateCondition(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg))
                return false;

            string[] items = arg.Split('=');
            if (items.Length != 2)
                return false; // falsches Format

            string left = items[0].Trim();
            string right = items[1].Trim();

            return left == right; // true, wenn gleich
        }

        /// <summary>
        /// Einfacher und lesbarer Interpreter für Browser-Automatisierung
        /// </summary>
        public void ExecuteScriptFile(string script)
        {
            try
            {
                string text = File.ReadAllText(script);
                ExecuteScript(text.Replace("\r\n", "\n"));
            }
            catch
            {

            }
        }
        private string TranslateCommand(string cmd)
        {
            string result = cmd;

            result = result.Replace("setzen", "set");
            result = result.Replace("dateiwarten", "filewait");
            result = result.Replace("if ", "wenn ");
            result = result.Replace("else", "sonst");
            result = result.Replace("switch", "schalter");
            result = result.Replace("case", "fall");
            result = result.Replace("default", "standard");
            if (cmd == "end")
                result = result.Replace("end", "ende");
            result = result.Replace("geschwindigkeit", "speed");
            result = result.Replace("hochladen", "upload");
            result = result.Replace("continue", "weiter");
            result = result.Replace("warten", "wait");
            result = result.Replace("auswählen", "select");
            result = result.Replace("aktivieren", "activate");
            result = result.Replace(";", "#");
            result = result.Replace("kom", "rem");
            result = result.Replace("wurzel", "root");
            result = result.Replace("jetzt", "now");
            result = result.Replace("jetzt2", "now2");
            result = result.Replace("heute", "today");
            result = result.Replace("heute3", "today3");
            result = result.Replace("nachricht", "message");
            result = result.Replace("name", "name");
            result = result.Replace("adresse", "url");
            result = result.Replace("starten", "start");
            result = result.Replace("tabulator", "tab");
            result = result.Replace("eingabe", "enter");
            result = result.Replace("leer", "space");
            result = result.Replace("unten", "down");
            result = result.Replace("oben", "up");
            result = result.Replace("links", "left");
            result = result.Replace("rechts", "right");
            result = result.Replace("tippe", "type");
            result = result.Replace("zwischenablage", "clipboard");
            result = result.Replace("schlafen", "sleep");
            result = result.Replace("stopp", "stop");
            result = result.Replace("haltepunkt", "breakpoint");
            result = result.Replace("bp", "bp");

            return result;
        }
        public void ExecuteScript(string script)
        {
            String savecurrentdir = Directory.GetCurrentDirectory();
            Dictionary<string, string> Variables = new Dictionary<string, string>();
            Variables.Add("%now%", DateTime.Now.ToString("HH:mm"));
            Variables.Add("%hour%", DateTime.Now.ToString("HH"));
            Variables.Add("%minute%", DateTime.Now.ToString("mm"));
            Variables.Add("%today%", DateTime.Now.ToString("dd.MM.yyyy"));
            Variables.Add("%day%", DateTime.Now.ToString("dd"));
            Variables.Add("%month%", DateTime.Now.ToString("MM"));
            Variables.Add("%year%", DateTime.Now.ToString("yyyy"));
            Variables.Add("%root%", Directory.GetCurrentDirectory());
            Variables.Add("%programm%", Directory.GetCurrentDirectory());


            foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                string key = "%" + env.Key.ToString() + "%";
                string value = env.Value?.ToString() ?? "";
                // Falls der Key schon existiert, ersetzen
                if (Variables.ContainsKey(key))
                    Variables[key] = value;
                else
                    Variables.Add(key, value);
            }

            if (string.IsNullOrWhiteSpace(script))
                return;

            var lines = script.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var rawLine in lines)
            {
                if (statusBox != null)
                {
                    statusBox.Text = rawLine;
                    statusBox.Refresh();
                }
                string line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//") || line.StartsWith("#"))
                    continue;

                try
                {
                    var parts = SplitCommand(line);
                    string cmd = parts[0].ToLowerInvariant().Trim();
                    string arg = parts.Length > 1 ? parts[1].Trim() : "";
                    string arg2 = parts.Length > 2 ? parts[2].Trim() : "";
                    cmd = TranslateCommand(cmd);

                    if (cmd == "set")
                    {
                        string[] items = arg.Split('=');

                        if (Variables.ContainsKey("%" + items[0] + "%"))
                            Variables["%" + items[0] + "%"] = items[1];  // vorhandenen Wert ersetzen
                        else
                            Variables.Add("%" + items[0] + "%", items[1]); // neu anlegen
                    }
                    else if (cmd == "filewait")
                    {
                        string[] items = arg.Split('='); // "variable=Pfad\zur\Datei"
                        string key = "%" + items[0] + "%";
                        string path = CleanQuotes(items[1]);

                        long fileSize = 1000 * ((File.Exists(path) ? new FileInfo(path).Length : 0) / 100000);
                        Thread.Sleep((int)fileSize);
                    }
                    else
                    {
                        foreach (string v in Variables.Keys)
                        {
                            cmd = cmd.Replace(v, Variables[v]);
                            arg = arg.Replace(v, Variables[v]);
                        }

                        switch (cmd)
                        {
                            case "wenn":
                                blockStack.Push(new BlockInfo { Type = BlockType.Wenn, IsActive = EvaluateCondition(arg) ? 1 : 0 });
                                break;

                            case "sonst":
                                if (blockStack.Peek().Type != BlockType.Wenn)
                                    throw new Exception("sonst ohne wenn");
                                blockStack.Peek().IsActive = blockStack.Peek().IsActive == 1 ? 0 : 1;
                                break;

                            case "schalter":
                                blockStack.Push(new BlockInfo { Type = BlockType.Schalter, IsActive = 0, SwitchValue = arg });
                                break;

                            case "fall":
                                var currentBlock = blockStack.Peek();
                                if (currentBlock.Type != BlockType.Schalter)
                                    throw new Exception("fall ohne schalter");
                                currentBlock.IsActive = (arg == currentBlock.SwitchValue) ? 1 : (currentBlock.IsActive > 0 ? 2 : 0);
                                break;

                            case "standard":
                                var standardBlock = blockStack.Peek();
                                if (standardBlock.Type != BlockType.Schalter)
                                    throw new Exception("standard ohne schalter");
                                if (standardBlock.IsActive != 2)
                                    standardBlock.IsActive = 1; // wird nur ausgeführt, wenn kein fall gepasst hat
                                break;

                            case "ende":
                                if (blockStack.Count == 0)
                                    throw new Exception("ende ohne Block");
                                blockStack.Pop();
                                break;

                            default:
                                // Nur ausführen, wenn kein übergeordneter Block deaktiviert
                                if (!blockStack.Any() || blockStack.All(b => b.IsActive == 1))
                                {
                                    switch (cmd)
                                    {
                                        case "wenn":
                                            {
                                                ifActive = true;
                                                string[] items = arg.Split('=');
                                                if (items.Length == 2)
                                                    ifState = items[0] == items[1];
                                                else
                                                    ifState = false;
                                                break;
                                            }
                                        case "sonst":
                                            {
                                                if (!ifActive)
                                                    throw new Exception("else ohne if");
                                                ifState = !ifState;
                                                break;
                                            }
                                        case "ende":
                                            {
                                                if (!ifActive)
                                                    throw new Exception("endif ohne if");
                                                ifActive = false;
                                                ifState = true;
                                                break;
                                            }
                                        case "speed":
                                            Speed = int.TryParse(arg, out int value) ? value : 0;
                                            break;
                                        case "ifupload":
                                            if (arg!="")
                                            {
                                                LoadImage(arg);
                                            }
                                            else
                                                Thread.Sleep(1000);
                                            break;

                                        case "upload":
                                            LoadImage(arg);
                                            break;

                                        case "weiter":
                                            Thread.Sleep(1000);
                                            PressEnter();
                                            Thread.Sleep(4000);
                                            break;

                                        case "wait":
                                            MessageBox.Show(CleanQuotes(arg), "Haltepunkt", MessageBoxButtons.OK);
                                            break;

                                        case "select0":
                                            {
                                                selectscript(arg, -1, 500);
                                            }
                                            break;
                                        case "select":
                                            {
                                                selectscript(arg, 0, 500);
                                            }
                                            break;
                                        case "select1":
                                            {
                                                selectscript(arg, 1, 500);
                                            }
                                            break;
                                        case "selectright":
                                            {
                                                string[] items = CleanQuotes(arg).Split(':');
                                                string[] options = { "ja", "nein" };
                                                if (items.Length > 1)
                                                    options = items[1].Split(',');
                                                int index = Array.IndexOf(options, items[0]);
                                                if (index > -1)
                                                    PressRight(ParseCount(arg, index + 1), 500);
                                            }
                                            break;
                                        case "activate":
                                            Activate();
                                            break;
                                        case "#":
                                            break;

                                        case "rem":
                                            break;

                                        case "root":
                                            if (arg != "")
                                                Directory.SetCurrentDirectory(CleanQuotes(arg));
                                            else
                                                Directory.SetCurrentDirectory("C:\\");
                                            break;
                                        case "now":
                                            TypeText(DateTime.Now.ToString("HH:mm"));
                                            break;
                                        case "now2":
                                            TypeText(DateTime.Now.ToString("HH"));
                                            PressTab(ParseCount(arg, 1));
                                            TypeText(DateTime.Now.ToString("mm"));
                                            break;
                                        case "today":
                                            TypeText(DateTime.Now.ToString("dd.MM.yyyy"));
                                            break;
                                        case "today3":
                                            TypeText(DateTime.Now.ToString("dd"));
                                            PressTab(ParseCount(arg, 1));
                                            TypeText(DateTime.Now.ToString("MM"));
                                            PressTab(ParseCount(arg, 1));
                                            TypeText(DateTime.Now.ToString("yyyy"));
                                            break;
                                        case "message":
                                            if (MessageBox.Show(CleanQuotes(arg), "Script", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                                return;
                                            break;

                                        case "name":
                                            Name = CleanQuotes(arg);
                                            break;

                                        case "url":
                                            Url = CleanQuotes(arg);
                                            break;

                                        case "start":
                                            Start();
                                            break;

                                        case "tab":
                                            PressTab(ParseCount(arg, 1));
                                            break;

                                        case "enter":
                                            PressEnter(ParseCount(arg, 1));
                                            break;

                                        case "space":
                                            PressSpace(ParseCount(arg, 1));
                                            break;

                                        case "down":
                                            PressDown(ParseCount(arg, 1));
                                            break;

                                        case "up":
                                            PressUp(ParseCount(arg, 1));
                                            break;

                                        case "left":
                                            PressLeft(ParseCount(arg, 1));
                                            break;

                                        case "right":
                                            PressRight(ParseCount(arg, 1));
                                            break;

                                        case "esc":
                                            PressEsc(ParseCount(arg, 1));
                                            break;

                                        case "type":
                                            TypeText(CleanQuotes(arg));
                                            break;

                                        case "clipboard":
                                            SendByClipBoard(CleanQuotes(arg));
                                            break;

                                        case "sleep":
                                            if (int.TryParse(arg, out int ms) && ms > 0)
                                                Thread.Sleep(ms);
                                            else
                                                Thread.Sleep(1000);
                                            break;

                                        case "stop":
                                        case "brakpoint":
                                        case "bp":
                                            return;
                                            break;

                                        case "print":
                                            PressKeyExt(VK_P, VK_CONTROL, 1);
                                            break;


                                        default:
                                            // Fallback: als Text eingeben
                                            TypeText(line);
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler in Zeile:\n{line}\n\n{ex.Message}",
                        "Script-Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                Thread.Sleep(Speed);
            }

            if (statusBox != null)
            {
                statusBox.Text = "End Script.";
                statusBox.Refresh();
            }
            Directory.SetCurrentDirectory(savecurrentdir);
        }

        private void selectscript(string arg, int dist, int delay)
        {
            string[] items = CleanQuotes(arg).Split(':');
            string[] options = { "ja", "nein" };
            if (items.Length > 1)
                options = items[1].Split(',');
            int index = Array.IndexOf(options, items[0]) + dist;
            if (index > -1)
                PressDown(ParseCount(arg, index + 1), delay);
        }

        private void PressKeyDown(ushort key)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = key;
            inputs[0].U.ki.dwFlags = 0; // Key down
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        private void PressKeyUp(ushort key)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = key;
            inputs[0].U.ki.dwFlags = KEYEVENTF_KEYUP; // Key up
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        private void PressKeyExt(ushort key, ushort extkey, int count)
        {
            PressKeyDown(extkey);
            PressKey(key, count);
            PressKeyUp(extkey);
        }
        private void LoadImage(string arg)
        {
            PressEnter();
            Thread.Sleep(4000);
            SendByClipBoard(CleanQuotes(arg));
            Thread.Sleep(2000);
            PressEnter();
            Thread.Sleep(4000);
        }

        private string[] SplitCommand(string line)
        {
            int firstSpace = line.IndexOf(' ');
            if (firstSpace == -1)
                return new[] { line };

            return new[]
            {
                line.Substring(0, firstSpace).Trim(),
                line.Substring(firstSpace + 1).Trim()
            };
        }
        private int ParseCount(string arg, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(arg))
                return defaultValue;

            return int.TryParse(arg, out int count) && count > 0 ? count : defaultValue;
        }
        private string CleanQuotes(string text)
        {
            return text.Trim('"', '\'');
        }
        // ==================== Strukturen ====================
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint type;
            public InputUnion U;
        }
        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }
        // ==================== DllImports ====================
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();
        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        // ==================== Konstanten für ShowWindow ====================
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_MAXIMIZE = 3;
        // ... (die anderen kannst du behalten)
        public string Url { get; private set; }
        public string Name { get; private set; }
        public int Speed { get; private set; } = 0;
        private IntPtr hwnd = IntPtr.Zero;
        public BrowserControl(string name, string url, TextBox statusbox = null)
        {
            Url = url;
            Name = name;
            statusBox = statusbox;
        }
        public BrowserControl(TextBox statusbox = null)
        {
            Url = "";
            Name = "";
            statusBox = statusbox;
        }

        public static string GetDefaultBrowserWindowClass()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice"))
            {
                string progId = key?.GetValue("ProgId")?.ToString();

                if (progId == null)
                    return "Chrome_WidgetWin_1";   // Fallback

                switch (progId)
                {
                    case "FirefoxURL":
                        return "MozillaWindowClass";

                    case "ChromeHTML":
                    case "MSEdgeHTM":
                    case "OperaStable":
                    case "BraveHTML":
                        return "Chrome_WidgetWin_1";

                    default:
                        return "Chrome_WidgetWin_1";
                }
            }
        }

        public void Start()
        {
            Tools.ShellExecute(IntPtr.Zero, "open", Url, "", "", 5);

            string windowClass = GetDefaultBrowserWindowClass();

            for (int i = 0; i < 20; i++)   // max ~4 Sekunden
            {
                hwnd = FindWindow(windowClass, null);
                if (hwnd != IntPtr.Zero)
                    break;

                Thread.Sleep(200);
            }

            Activate();
        }

        public void Start_old()
        {
            Tools.ShellExecute(IntPtr.Zero, "open", Url, "", "", 5);
            hwnd = FindWindow("MozillaWindowClass", null);   // oder Chrome_WidgetWin_1 etc.

            Activate();
        }
        public void Activate()
        {
            Thread.Sleep(4000);
            ShowWindow(hwnd, SW_MAXIMIZE);   // oder SW_SHOWNORMAL
            Thread.Sleep(2000);

            // Optional: Fenster in den Vordergrund bringen
            SetForegroundWindow(hwnd);
        }
        // ====================== Die neue, funktionierende PressKey ======================
        public void PressKey(ushort key, int count = 1, int delay = 0)
        {
            for (int i = 0; i < count; i++)
            {
                INPUT[] inputs = new INPUT[2];

                // Key Down
                inputs[0].type = INPUT_KEYBOARD;
                inputs[0].U.ki.wVk = key;
                inputs[0].U.ki.wScan = 0;
                inputs[0].U.ki.dwFlags = 0;
                inputs[0].U.ki.time = 0;
                inputs[0].U.ki.dwExtraInfo = GetMessageExtraInfo();

                // Key Up
                inputs[1].type = INPUT_KEYBOARD;
                inputs[1].U.ki.wVk = key;
                inputs[1].U.ki.wScan = 0;
                inputs[1].U.ki.dwFlags = KEYEVENTF_KEYUP;
                inputs[1].U.ki.time = 0;
                inputs[1].U.ki.dwExtraInfo = GetMessageExtraInfo();

                int cbSize = Marshal.SizeOf(typeof(INPUT));
                uint sent = SendInput((uint)inputs.Length, inputs, cbSize);

                if (sent == 0)
                {
                    int err = Marshal.GetLastWin32Error();
                    MessageBox.Show($"SendInput fehlgeschlagen! Error: {err}\n(87 = ungültiger Parameter)", "Fehler");
                }
                if (delay>0)
                    Thread.Sleep(delay);
            }
        }
        // ====================== Deine anderen Methoden ======================
        public void PressTab(int count = 1, int delay = 0) => PressKey(VK_TAB, count, delay);
        public void PressEnter(int count = 1, int delay = 0) => PressKey(VK_RETURN, count, delay);
        public void PressSpace(int count = 1, int delay = 0) => PressKey(VK_SPACE, count, delay);
        public void PressLeft(int count = 1, int delay = 0) => PressKey(VK_LEFT, count, delay);
        public void PressUp(int count = 1, int delay = 0) => PressKey(VK_UP, count, delay);
        public void PressRight(int count = 1, int delay = 0) => PressKey(VK_RIGHT, count, delay);
        public void PressDown(int count = 1, int delay = 0) => PressKey(VK_DOWN, count, delay);
        public void PressEsc(int count = 1, int delay = 0) => PressKey(VK_ESCAPE, count, delay);

        [DllImport("user32.dll")]
        static extern void keybd_event(ushort vk, byte scan, uint flags, int extra);

        // const int KEYEVENTF_KEYUP = 0x0002;

        void KeyDown(ushort vk) => keybd_event(vk, 0, 0, 0);
        void KeyUp(ushort vk) => keybd_event(vk, 0, KEYEVENTF_KEYUP, 0);

        public void TypeText(string text)
        {
            foreach (char c in text)
            {
                short v = VkKeyScan(c);
                byte key = (byte)(v & 0xFF);
                byte mod = (byte)((v >> 8) & 0xFF);

                // Modifier drücken
                if ((mod & 1) != 0) KeyDown(VK_SHIFT);
                if ((mod & 2) != 0) KeyDown(VK_CONTROL);
                if ((mod & 4) != 0) KeyDown(VK_MENU);

                // Haupttaste drücken
                KeyDown(key);
                KeyUp(key);

                // Modifier loslassen
                if ((mod & 4) != 0) KeyUp(VK_MENU);
                if ((mod & 2) != 0) KeyUp(VK_CONTROL);
                if ((mod & 1) != 0) KeyUp(VK_SHIFT);
            }
        }
        public void TypeText_old(string text)
        {
            foreach (char c in text)
            {
                short v = VkKeyScan(c);
                ushort key = (ushort)(v & 0xFF);
                ushort modifiers = (ushort)((v >> 8) & 0xFF);

                if ((modifiers & 1) != 0) PressKey(VK_SHIFT);
                if ((modifiers & 2) != 0) PressKey(VK_CONTROL);
                if ((modifiers & 4) != 0) PressKey(VK_MENU);

                PressKey(key);

                // Modifier wieder loslassen (einfache Variante)
                if ((modifiers & 1) != 0) PressKey(VK_SHIFT); // nochmal = loslassen bei manchen Tastaturen
            }
        }
        public void SendByClipBoard_old(string text)
        {
            Clipboard.SetText(text);
            Thread.Sleep(1000);
            PressKey(VK_CONTROL | 'V');
        }
        private void PressControlDown()
        {
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_KEYBOARD;
            input[0].U.ki.wVk = VK_CONTROL;
            input[0].U.ki.dwFlags = 0;
            input[0].U.ki.dwExtraInfo = GetMessageExtraInfo();

            SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
        }
        private void PressControlUp()
        {
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_KEYBOARD;
            input[0].U.ki.wVk = VK_CONTROL;
            input[0].U.ki.dwFlags = KEYEVENTF_KEYUP;
            input[0].U.ki.dwExtraInfo = GetMessageExtraInfo();

            SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
        }
        public void SendByClipBoard(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Clipboard.SetText(text);
            Thread.Sleep(300);

            // Fenster sicher in den Vordergrund holen
            // if (hwnd != IntPtr.Zero)
            //    SetForegroundWindow(hwnd);

            Thread.Sleep(200);

            PressControlDown();
            PressKey(VK_V);      // V
            PressControlUp();

            Thread.Sleep(300);
        }
    }
}