using NicoV4.Mvvm.Models;
using NicoV4.Mvvm.Views.ItemsControl;
using NicoV4.Properties;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Views.WorkSpace
{
    public class SearchVideoByTemporaryViewModel : WorkSpaceViewModel
    {
        public SearchVideoByTemporaryViewModel() : this(SearchVideoByTemporaryModel.Instance)
        {

        }

        public SearchVideoByTemporaryViewModel(SearchVideoByTemporaryModel model)
        {
            Source = model;

            Source.Reload().ConfigureAwait(false);

            Items = Source.Videos.ToSyncedSynchronizationContextCollection(
                id => new VideoItemViewModel(id),
                AnonymousSynchronizationContext.Current
            );
        }

        public SearchVideoByTemporaryModel Source { get; private set; }

        /// <summary>
        /// ﾒｲﾝ項目ﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<VideoItemViewModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private SynchronizationContextCollection<VideoItemViewModel> _Items;

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
                        var result = await MainWindowViewModel.Instance.ShowInputAsync(
                            Resources.L_ADD,
                            Resources.M_ADD_TEMPORARY);

                        var id = result.Split('/').Last();

                        await SearchVideoByTemporaryModel.Instance.AddVideo(id);
                    });
            }
        }
        public ICommand _OnAdd;

        /// <summary>
        /// 削除処理
        /// </summary>
        public ICommand OnDelete
        {
            get
            {
                return _OnDelete = _OnDelete ?? new RelayCommand(
                    async _ =>
                    {
                        var targets = Items
                            .Where(item => item.IsSelected)
                            .Select(item => item.VideoId);

                        foreach (var id in targets)
                        {
                            await SearchVideoByTemporaryModel.Instance.DeleteVideo(id);
                        }
                    });
            }
        }
        public ICommand _OnDelete;

    }
}
