using NicoV4.Common;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Models
{
    [DataContract]
    public class MylistStatusModel : BindableBase
    {
        public static MylistStatusModel Instance { get; private set; } = GetInstance();

        public MylistStatusModel()
        {
            Favorites = new List<string>();
        }

        // ****************************************************************************************************
        // ﾌﾟﾛﾊﾟﾃｨ定義
        // ****************************************************************************************************

        /// <summary>
        /// ｱｲﾃﾑ構成
        /// </summary>
        [IgnoreDataMember]
        public ObservableSynchronizedCollection<SearchVideoByMylistModel> Mylists
        {
            get { return _Mylists; }
            set { SetProperty(ref _Mylists, value); }
        }
        private ObservableSynchronizedCollection<SearchVideoByMylistModel> _Mylists;

        /// <summary>
        /// お気に入りﾘｽﾄ
        /// </summary>
        [DataMember]
        public List<string> Favorites { get; set; }

        private static MylistStatusModel GetInstance()
        {
            var instance = JsonConverter.Deserialize<MylistStatusModel>(Variables.MylistStatusPath) ?? new MylistStatusModel();
            instance._Mylists = new ObservableSynchronizedCollection<SearchVideoByMylistModel>();
            return instance;
        }

        public void AddFavorites(string id)
        {
            if (!Favorites.Any(f => f == id))
            {
                Favorites.Add(id);
            }
        }

        protected override void OnDisposed()
        {
            // ﾌﾟﾛﾊﾟﾃｨに設定された内容を外部ﾌｧｲﾙに保存します。
            JsonConverter.Serialize(Variables.MylistStatusPath, this);

            Mylists = null;
            Favorites = null;

            base.OnDisposed();
        }
    }
}
