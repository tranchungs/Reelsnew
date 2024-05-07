﻿

using System;
using System.Windows.Forms;


namespace ToolFacebookAdb
{
    public partial class Form1 : Form
    {
        List<Phone> phoneList;
        ContextDataBitMap context;
        List<Account> listAccount;
        List<ProxyKey> listProxyKey;

        List<ListConfigDataInfo> listConfigDataInfo;
        List<ListConfigDataInfo> listConfigRun;
        List<LDevice> listLDCurrent;
        public Form1()
        {
            InitializeComponent();


        }
        static ManualResetEvent manualEvent = new ManualResetEvent(false);

        object locker = new object();
        private void button1_Click(object sender, EventArgs e)
        {
            listConfigRun = new List<ListConfigDataInfo>();
            listLDCurrent = LDPlayer.GetDevices2();
            foreach (ListViewItem itemLv in mainListView.Items)
            {
                if (itemLv.Checked)
                {
                    ListConfigDataInfo cfData = UtilityHelper.GetConfigDataInfo(itemLv.Text, listConfigDataInfo);
                    listConfigRun.Add(cfData);
                }
            }
            bool f_TaoPage = cbRegPage.Checked;
            bool f_Reels = cbXemReels.Checked;
            bool f_UpReels = cbDangReel.Checked;
            int countAcc = 0;
            if (Int32.TryParse(txtThread.Text, out int threadCount))
            {
                List<Thread> threads = new List<Thread>();
                for (int i = 0; i < threadCount; i++)
                {

                    Thread newThread = new Thread(() =>
                    {
                        while (true)
                        {
                            try
                            {
                                int newThreadNumber;
                                lock (locker)
                                {
                                    newThreadNumber = countAcc;
                                    if (countAcc >= listConfigRun.Count) return;
                                    countAcc++;
                                }

                                UtilityHelper.SetSharedFolder(listConfigRun[newThreadNumber].ldphone.index, listConfigRun[newThreadNumber].Folder, listLDCurrent);


                               //listConfigRun[newThreadNumber].ldphone.FakeIP(listConfigRun[newThreadNumber].proxy, locker);

                                listConfigRun[newThreadNumber].ldphone.Login(listConfigRun[newThreadNumber].account);
                                Thread.Sleep(1000);
                                if (f_TaoPage)
                                {
                                    listConfigRun[newThreadNumber].ldphone.Createpage(listConfigRun[newThreadNumber].Page, listConfigRun[newThreadNumber].account);
                                    Thread.Sleep(1000);
                                }
                                if (f_UpReels)
                                {
                                    listConfigRun[newThreadNumber].ldphone.UpReels(1);
                                    Thread.Sleep(2000);
                                }
                                if (f_Reels)
                                {
                                    listConfigRun[newThreadNumber].ldphone.Reels(30000);
                                }


                                listConfigRun[newThreadNumber].ldphone.Close();
                            }
                            catch (Exception e)
                            {

                            }

                        }
                    });
                    newThread.Start();
                    newThread.IsBackground = true;


                }
                foreach (var thread in threads)
                {
                    thread.Join();
                }
            }
            else
            {
                MessageBox.Show("Nhap so luong");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itemLv in mainListView.Items)
            {
                if (itemLv.Checked)
                {
                    bool isHave = false;
                    foreach (LDevice device in listLDCurrent)
                    {
                        if (itemLv.Text == device.Name)
                        {
                            isHave = true;
                            break;
                        }

                    }
                    if (!isHave)
                    {
                        LDPlayer.Copy($"{itemLv.Text}", "82");
                        Thread.Sleep(2000);
                        UtilityHelper.SetSharedFolder(itemLv.Text, itemLv.SubItems[3].Text, listLDCurrent);
                    }

                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {



        }

        private void button4_Click(object sender, EventArgs e)
        {
            GoogleSheetService gg = new GoogleSheetService();
            listConfigDataInfo = gg.ReadAllRows();
            InitMainListView();


        }
        private void InitMainListView()
        {
            mainListView.Items.Clear();
            foreach (ListConfigDataInfo info in listConfigDataInfo)
            {
                ListViewItem item = new ListViewItem();
                item.Text = info.ldphone.index.ToString();
                ListViewItem.ListViewSubItem sub1 = new ListViewItem.ListViewSubItem();
                sub1.Text = info.account.ToString();
                ListViewItem.ListViewSubItem sub2 = new ListViewItem.ListViewSubItem();
                sub2.Text = info.Page;
                ListViewItem.ListViewSubItem sub3 = new ListViewItem.ListViewSubItem();
                sub3.Text = info.Folder;
                ListViewItem.ListViewSubItem sub4 = new ListViewItem.ListViewSubItem();
                sub4.Text = info.proxy.ToString();
                item.SubItems.Add(sub1);
                item.SubItems.Add(sub2);
                item.SubItems.Add(sub3);
                item.SubItems.Add(sub4);

                mainListView.Items.Add(item);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listConfigDataInfo = new List<ListConfigDataInfo>();


            listProxyKey = new List<ProxyKey>();
            context = new ContextDataBitMap();
            UtilityHelper.ReadConfig();
            LDPlayer.PathLD = ConfigEnv.FolderLD;
            listLDCurrent = LDPlayer.GetDevices2();
            // KAutoHelper.ADBHelper.SetADBFolderPath(@"D:\LDPlayer\LDPlayer9");

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
           
           
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            LDPlayer.TapByPercent(LDType.Name, "LDPlayer-4", 16.4, 43.7);

        }
    }
}