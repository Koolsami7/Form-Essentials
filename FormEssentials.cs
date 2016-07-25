using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace FormEssentials
{
    public static class Forms
    {
        public static bool isFormOpen(Form nigform)
        {
            return Application.OpenForms[nigform.Name] as Form != null;
        }

        public static class TextBoxes
        {
            public static List<TextBox> GetTextBoxesByNum(string name, Control control)
            {
                List<TextBox> tb = new List<TextBox>();
                foreach (Control con in control.Controls)
                    if (con.Name.Contains(name))
                        tb.Add(con as TextBox);
                return tb.OrderByDescending(x => int.Parse(x.Name.Replace(name, ""))).AsEnumerable().Reverse().ToList();
            }
        }

        public static class ListViews
        {
            public static void addToListView(ListView LeListView, string item, params object[] moreItems)
            {
                ListViewItem lv_item = new ListViewItem(item);
                if (moreItems.Length > 0)
                    for (int i = 0; i < moreItems.Length; i++)
                        lv_item.SubItems.Add(moreItems[i].ToString());
                LeListView.Items.Add(lv_item);
            }

            public static bool doesItemExistLV(ListView LeListView, string search)
            {
                foreach (ListViewItem lv_item in LeListView.Items)
                    if (lv_item.Text.ToLower() == search.ToLower())
                        return true;
                    else return false;
                return false;
            }

            public static bool doesItemContainLV(ListView LeListView, string search, int loc)
            {
                foreach (ListViewItem lv_item in LeListView.Items)
                    if (lv_item.SubItems[loc].Text.ToLower().Contains(search.ToLower()))
                        return true;
                    else return false;
                return false;
            }

            public static int whereIsInLV(ListView lv, string search)
            {
                if (lv.Items.Count > 0)
                    for (int x = 0; x < lv.Items.Count; x++)
                        if (lv.Items[x].Text == search)
                            return x;
                return -1;
            }

            public static void removeDupesLV(ListView lv)
            {
                if (lv.Items.Count > 0)
                    for (int x = 0; x < lv.Items.Count; x++)
                        for (int y = 1; y < lv.Items.Count; y++)
                            if (x != y)
                                if (lv.Items[x].Text == lv.Items[y].Text)
                                    lv.Items[y].Remove();
            }
        }
    }

    public static class Encodings
    {
        public static string Base64(string input, bool encode = true)
        {
            return encode ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input)) : System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(input));
        }
    }

    public static class Hashing
    {
        public static string MD5(string arg)
        {
            string result = "";
            MD5CryptoServiceProvider _MD5 = new MD5CryptoServiceProvider();
            _MD5.ComputeHash(Encoding.ASCII.GetBytes(arg));
            for (int i = 0; i < _MD5.Hash.Length; i++) {
                result += _MD5.Hash[i].ToString("x2");
            } return result;
        }

        public static string MD5(byte[] arg)
        {
            string result = "";
            MD5CryptoServiceProvider _MD5 = new MD5CryptoServiceProvider();
            _MD5.ComputeHash(arg);
            for (int i = 0; i < _MD5.Hash.Length; i++) {
                result += _MD5.Hash[i].ToString("x2");
            } return result;
        }

        public static string MD5(FileStream arg) { return MD5(arg); }
    }

    public static class Conversion
    {
        public static string BytesToHexString(byte[] Buffer)
        {
            string str = "";
            for (int i = 0; i < Buffer.Length; i++)
                str = str + Buffer[i].ToString("X2");
            return str;
        }

        public static string ConvertStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(c));
            return hex;
        }
    }

    public static class SelfHashing
    {
        public static string GetExecutingFileHash()
        {
            return MD5(GetSelfBytes());
        }

        private static string MD5(byte[] input)
        {
            return MD5(ASCIIEncoding.ASCII.GetString(input));
        }

        private static string MD5(string input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] originalBytes = ASCIIEncoding.Default.GetBytes(input);
            byte[] encodedBytes = md5.ComputeHash(originalBytes);
            return BitConverter.ToString(encodedBytes).Replace("-", "");
        }

        private static byte[] GetSelfBytes()
        {
            string path = Application.ExecutablePath;
            FileStream running = File.OpenRead(path);
            byte[] exeBytes = new byte[running.Length];
            running.Read(exeBytes, 0, exeBytes.Length);
            running.Close();
            return exeBytes;
        }
    }

    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        public IniFile(string INIPath) { path = INIPath; }

        public void IniWriteValue(string Section, string Key, string Value)
        { WritePrivateProfileString(Section, Key, Value, path); }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, path);
            return temp.ToString();
        }
    }

    public static class Bytes
    {
        public static byte[] ReverseBytes(byte[] input) { return input.Reverse().ToArray(); }
        public static byte[] ReverseBytes(string input) { return Encoding.UTF8.GetBytes(input).Reverse().ToArray(); }

        public static bool isEmpty(byte[] input) { return input.Length <= 0; }

        public static bool ComapaireBytes(byte[] input1, byte[] input2)
        {
            if (input1.Length != input2.Length)
                return false;
            for (int i = 0; i < input1.Length; i++)
                if (input1[i] != input2[i])
                    return false;
            return true;
        }
    }

    public static class DateTimes
    {
        public static string TimeElapsed(DateTime date1, DateTime date2)
        {
            TimeSpan ts = (date1 - date2);
            return String.Format("{0} Year(s), {1} Month(s), {2} Day(s), {3} Hour(s), {4} Minute(s), {5} Second(s)", Math.Truncate(ts.TotalDays / 365), Math.Truncate(ts.TotalDays % 365) / 30, Math.Truncate(ts.TotalDays % 365) % 30, ts.Hours, ts.Minutes, ts.Seconds);
        }
    }

    public static class WindowsConsole
    {
        public static bool ExecConsole(string cmd, bool runas = false, ProcessWindowStyle style = ProcessWindowStyle.Hidden)
        {
            try
            {
                Process proc = new Process();
                ProcessStartInfo procStartInfo = new ProcessStartInfo();
                procStartInfo.WindowStyle = style;
                procStartInfo.Verb = runas ? "runas" : "";
                procStartInfo.FileName = "cmd.exe";
                procStartInfo.Arguments = cmd;
                proc.StartInfo = procStartInfo;
                proc.Start();
                return true;
            }
            catch { return false; }
        }
    }
}