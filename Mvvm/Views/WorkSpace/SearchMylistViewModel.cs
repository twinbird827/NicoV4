﻿using NicoV4.Common;
using NicoV4.Mvvm.Models;
using NicoV4.Mvvm.Views.ItemsControl;
using NicoV4.Properties;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.Service;

namespace NicoV4.Mvvm.Views.WorkSpace
{
    public class SearchMylistViewModel : WorkSpaceViewModel
    {
        public SearchMylistViewModel()
        {
            Items = MylistStatusModel.Instance.Favorites.ToSyncedSynchronizationContextCollection(
                id => new MylistItemViewModel(id.Mylist),
                AnonymousSynchronizationContext.Current
            );
        }

        /// <summary>
        /// ﾒｲﾝ項目ﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<MylistItemViewModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private SynchronizationContextCollection<MylistItemViewModel> _Items;

        /// <summary>
        /// 追加処理
        /// </summary>
        public ICommand OnAdd
        {
            get
            {
                return _OnAdd = _OnAdd ?? new RelayCommand(
                async _ =>
                {
                    string result = await MainWindowViewModel.Instance.ShowInputAsync(
                        Resources.L_ADD_MYLIST,
                        Resources.M_ADD_MYLIST);

                    await AddMylist(NicoDataConverter.ToId(result));
                });
            }
        }
        public ICommand _OnAdd;

        /// <summary>
        /// 追加処理(ｸﾘｯﾌﾟﾎﾞｰﾄﾞ)
        /// </summary>
        public ICommand OnAddCopyUrl
        {
            get
            {
                return _OnAddCopyUrl = _OnAddCopyUrl ?? new RelayCommand(
                async _ =>
                {
                    await AddMylist(NicoDataConverter.ToId(Clipboard.GetText(TextDataFormat.Text)));
                });
            }
        }
        public ICommand _OnAddCopyUrl;

        /// <summary>
        /// 共通の追加処理
        /// </summary>
        /// <param name="url">追加するURL</param>
        private async Task AddMylist(string url)
        {
            SearchVideoByMylistModel mylist;
            try
            {
                mylist = await MylistStatusModel.Instance.GetMylist(NicoDataConverter.ToId(url));

                if (!mylist.Videos.Any())
                {
                    throw new ArgumentException("ﾃﾞｰﾀ件数が0件");
                }
            }
            catch
            {
                ServiceFactory.MessageService.Error("有効なUrlを指定してください。");
                return;
            }

            MylistStatusModel.Instance.AddFavorites(mylist.MylistId);
        }
    }
}
