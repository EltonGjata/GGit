﻿using BrightIdeasSoftware;
using GGit.Utils;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GGit.Forms
{
    public partial class CommitForm : Form
    {
        private string _filePath { set; get; }
        private List<GStatus> statuses = new List<GStatus>();

        public CommitForm(string filePath)
        {
            _filePath = filePath;
            InitializeComponent();
            StatusInit();
        }

        private void StatusInit()
        {
            string workDir = "";
            try
            {
                workDir = getWorkDir(_filePath);
                using (var repo = new Repository(workDir))
                {
                    foreach (var item in repo.RetrieveStatus(new StatusOptions()))
                    {
                        statuses.Add(new GStatus(workDir, item.FilePath, Convert.ToInt32(item.State)));
                    }
                }
                statusList.SetObjects(statuses);
                statusList.ButtonClick += OnStatusBtnClick;
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format(err.Message));
            }
        }

        private void OnStatusBtnClick (object sender, CellClickEventArgs e)
        {
            switch (e.SubItem.Text)
            {
                case "+":
                    {
                        statuses[e.RowIndex].StageFile();
                        statusList.SetObjects(statuses);
                        break;
                    }
                case "-":
                    {
                        statuses[e.RowIndex].UnStageFile();
                        statusList.SetObjects(statuses);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Find a dir contains a dir named .git
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string getWorkDir(string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                if (!string.Equals(Path.GetDirectoryName(path), null))
                {
                    string fileDir = Path.GetDirectoryName(path);
                    DirectoryInfo dir = new DirectoryInfo(fileDir);
                    DirectoryInfo[] dirInfos = dir.GetDirectories();
                    string workDir = string.Empty;
                    foreach (DirectoryInfo info in dirInfos)
                    {
                        if (info.Name == ".git")
                        {
                            workDir = fileDir;
                            break;
                        }
                    }
                    if (string.Equals(workDir, string.Empty)) return getWorkDir(fileDir);
                    else return workDir;
                }
                else throw new Exception(string.Format("This path is not in git repository", path));
            }
            else throw new Exception(string.Format("Path '{0}' doesn't exists !", path));
        }
    }
}
