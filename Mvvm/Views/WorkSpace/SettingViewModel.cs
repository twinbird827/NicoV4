﻿using NicoV4.Mvvm.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.Service;

namespace NicoV4.Mvvm.Views.WorkSpace
{
    public class SettingViewModel : WorkSpaceViewModel
    {
        public SettingViewModel()
        {
            // ﾛｸﾞｲﾝ情報の初期値
            MailAddress = SettingModel.Instance.MailAddress;
            Password = SettingModel.Instance.GetPassword();
            Browser = SettingModel.Instance.Browser;

            // なぜか Password が string.Empty だと上手くﾊﾞｲﾝﾃﾞｨﾝｸﾞできないので対処
            //Password = string.IsNullOrEmpty(Password) ? null : Password;

            // ﾊﾞｰｼﾞｮﾝ情報の初期値
            var assm = System.Reflection.Assembly.GetExecutingAssembly();
            var asmcpy = Attribute.GetCustomAttribute(assm, typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
            Version = assm.GetName().Version.ToString();
            UpdateDate = (new FileInfo(assm.Location)).LastWriteTime;
            Copyright = asmcpy != null ? asmcpy.Copyright : "";

            // ｻﾑﾈｲﾙ
            Thumbnail = SettingModel.Instance.Thumbnail;

            // ﾀﾞｳﾝﾛｰﾄﾞ
            DownloadDirectory = SettingModel.Instance.DownloadDirectory;
            DownloadFileName = SettingModel.Instance.DownloadFileName;
        }

        /// <summary>
        /// ﾒｰﾙｱﾄﾞﾚｽ
        /// </summary>
        public string MailAddress
        {
            get { return _MailAddress; }
            set { SetProperty(ref _MailAddress, value); }
        }
        private string _MailAddress;

        /// <summary>
        /// ﾊﾟｽﾜｰﾄﾞ
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set { SetProperty(ref _Password, value); }
        }
        private string _Password;

        /// <summary>
        /// ﾌﾞﾗｳｻﾞﾊﾟｽ
        /// </summary>
        public string Browser
        {
            get { return _Browser; }
            set { SetProperty(ref _Browser, value); }
        }
        private string _Browser;

        /// <summary>
        /// ﾊﾞｰｼﾞｮﾝ
        /// </summary>
        public string Version
        {
            get { return _Version; }
            set { SetProperty(ref _Version, value); }
        }
        private string _Version;

        /// <summary>
        /// 更新日
        /// </summary>
        public DateTime UpdateDate
        {
            get { return _UpdateDate; }
            set { SetProperty(ref _UpdateDate, value); }
        }
        private DateTime _UpdateDate;

        /// <summary>
        /// ｺﾋﾟｰﾗｲﾄ
        /// </summary>
        public string Copyright
        {
            get { return _Copyright; }
            set { SetProperty(ref _Copyright, value); }
        }
        private string _Copyright;

        /// <summary>
        /// ｻﾑﾈｲﾙ
        /// </summary>
        public ThumbnailSize Thumbnail
        {
            get { return _Thumbnail; }
            set { SetProperty(ref _Thumbnail, value); }
        }
        private ThumbnailSize _Thumbnail;

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞﾌｧｲﾙ名の種類
        /// </summary>
        public DownloadFileName DownloadFileName
        {
            get { return _DownloadFileName; }
            set { SetProperty(ref _DownloadFileName, value); }
        }
        private DownloadFileName _DownloadFileName;

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞﾃﾞｨﾚｸﾄﾘ
        /// </summary>
        public string DownloadDirectory
        {
            get { return _DownloadDirectory; }
            set { SetProperty(ref _DownloadDirectory, value); }
        }
        private string _DownloadDirectory;

        /// <summary>
        /// ﾛｸﾞｲﾝ処理
        /// </summary>
        public ICommand OnLogin
        {
            get
            {
                return _OnLogin = _OnLogin ?? new RelayCommand(
                    async _ =>
                    {
                        // ﾛｸﾞｲﾝ実行
                        try
                        {
                            // 入力値を設定
                            SettingModel.Instance.MailAddress = MailAddress;
                            SettingModel.Instance.SetPassword(Password);

                            // ﾛｸﾞｲﾝ処理 (失敗すると例外)
                            await SettingModel.Instance.GetCookies();
                        }
                        catch
                        {
                            ServiceFactory.MessageService.Debug("failed");
                        }
                    },
                    _ => {
                        return
                            !string.IsNullOrWhiteSpace(MailAddress) &&
                            !string.IsNullOrWhiteSpace(Password);
                    });
            }
        }
        public ICommand _OnLogin;

        /// <summary>
        /// ﾌﾞﾗｳｻﾞ変更処理
        /// </summary>
        public ICommand OnBrowserSetting
        {
            get
            {
                return _OnBrowserSetting = _OnBrowserSetting ?? new RelayCommand(
                    _ =>
                    {
                        SettingModel.Instance.Browser = Browser;
                    },
                    _ => {
                        return !string.IsNullOrWhiteSpace(Browser);
                    });
            }
        }
        public ICommand _OnBrowserSetting;

        /// <summary>
        /// ｻﾑﾈｻｲｽﾞ変更処理
        /// </summary>
        public ICommand OnThumbnailSetting
        {
            get
            {
                return _OnThumbnailSetting = _OnThumbnailSetting ?? new RelayCommand(
                    _ =>
                    {
                        SettingModel.Instance.Thumbnail = Thumbnail;
                    });
            }
        }
        public ICommand _OnThumbnailSetting;

        /// <summary>
        /// ﾀﾞｳﾝﾛｰﾄﾞ関連変更処理
        /// </summary>
        public ICommand OnDownloadSetting
        {
            get
            {
                return _OnDownloadSetting = _OnDownloadSetting ?? new RelayCommand(
                    _ =>
                    {
                        SettingModel.Instance.DownloadDirectory = DownloadDirectory;
                        SettingModel.Instance.DownloadFileName = DownloadFileName;
                    },
                    _ => 
                    {
                        return Directory.Exists(DownloadDirectory);
                    });
            }
        }
        public ICommand _OnDownloadSetting;


    }
}
