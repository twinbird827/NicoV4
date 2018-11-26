using NicoV4.Common;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Models
{
    [DataContract]
    public class VideoStatusModel : BindableBase
    {
        public static VideoStatusModel Instance { get; private set; } = GetInstance();

        public VideoStatusModel()
        {
            NewVideos = new List<string>();
            SeeVideos = new List<string>();
        }

        // ****************************************************************************************************
        // ﾌﾟﾛﾊﾟﾃｨ定義
        // ****************************************************************************************************

        /// <summary>
        /// ｱｲﾃﾑ構成
        /// </summary>
        [IgnoreDataMember]
        public ObservableSynchronizedCollection<VideoModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableSynchronizedCollection<VideoModel> _Videos;

        /// <summary>
        /// NEWﾘｽﾄ
        /// </summary>
        [DataMember]
        public List<string> NewVideos { get; set; }

        /// <summary>
        /// SEEﾘｽﾄ
        /// </summary>
        [DataMember]
        public List<string> SeeVideos { get; set; }

        // ****************************************************************************************************
        // 公開ﾒｿｯﾄﾞ定義
        // ****************************************************************************************************

        private static VideoStatusModel GetInstance()
        {
            var instance = JsonConverter.Deserialize<VideoStatusModel>(Variables.VideoStatusPath) ?? new VideoStatusModel();
            instance._Videos = new ObservableSynchronizedCollection<VideoModel>();

            return instance;
        }

        public VideoModel GetVideo(string id)
        {
            var video = Videos.FirstOrDefault(v => v.VideoId == id);

            if (video == null)
            {
                video = new VideoModel()
                {
                    VideoId = id
                };
                Videos.Add(video);
            }

            return video;
        }

        protected override void OnDisposed()
        {
            // ﾌﾟﾛﾊﾟﾃｨに設定された内容を外部ﾌｧｲﾙに保存します。
            JsonConverter.Serialize(Variables.VideoStatusPath, this);

            Videos = null;
            NewVideos = null;
            SeeVideos = null;

            base.OnDisposed();
        }
    }
}
