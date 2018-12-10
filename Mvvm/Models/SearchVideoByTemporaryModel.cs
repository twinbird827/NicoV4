using NicoV4.Common;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfUtilV2.Common;

namespace NicoV4.Mvvm.Models
{
    public class SearchVideoByTemporaryModel : SearchVideoModel
    {
        public static SearchVideoByTemporaryModel Instance { get; private set; } = new SearchVideoByTemporaryModel();

        private SearchVideoByTemporaryModel() : base(true)
        {
            if (!WpfUtil.IsDesignMode() && !string.IsNullOrWhiteSpace(SettingModel.Instance.MailAddress))
            {
                Reload().ConfigureAwait(false);
            }
        }

        public override async Task Reload()
        {
            const string url = "http://www.nicovideo.jp/api/deflist/list";

            var json = await GetJsonAsync(url);

            Videos.Clear();

            foreach (dynamic item in json["mylistitem"])
            {
                var video = VideoStatusModel.Instance.GetVideo((string)item["item_id"]);

                video.VideoUrl = item["item_id"];
                video.Title = item["item_data"]["title"];
                video.Description = item["description"];
                //video.Tags = data["tags"];
                //video.CategoryTag = data["categoryTags"];
                video.ViewCounter = long.Parse(item["item_data"]["view_counter"]);
                video.MylistCounter = long.Parse(item["item_data"]["mylist_counter"]);
                video.CommentCounter = long.Parse(item["item_data"]["num_res"]);
                video.StartTime = NicoDataConverter.FromUnixTime((long)item["item_data"]["first_retrieve"]);
                //video.LastCommentTime = Converter.item(data["lastCommentTime"]);
                video.LengthSeconds = long.Parse(item["item_data"]["length_seconds"]);
                video.ThumbnailUrl = item["item_data"]["thumbnail_url"];
                video.LastResBody = item["item_data"]["last_res_body"];
                //video.CommunityIcon = data["communityIcon"];

                Videos.Add(video.VideoId);
            }
        }

        public async Task AddVideo(string id)
        {
            const string url = "http://www.nicovideo.jp/api/deflist/add?item_type=0&item_id={0}&description={1}&token={2}";

            if (!Videos.Any(v => v == id))
            {
                // URLに追加
                var txt = await GetStringAsync(string.Format(url, id, "", await GetToken()));

                // 自身に追加
                Videos.Insert(0, id);
            }
        }

        public async Task DeleteVideo(string id)
        {
            const string url = "http://www.nicovideo.jp/api/deflist/delete?id_list[0][]={0}&token={1}";

            if (Videos.Any(v => v == id))
            {
                // URLに追加
                var txt = await GetStringAsync(string.Format(url, id, await GetToken()));

                // 自身に追加
                Videos.Remove(id);
            }
        }

        private async Task<string> GetToken()
        {
            const string url = "http://www.nicovideo.jp/my/top";
            var txt = await GetStringAsync(url);
            return Regex.Match(txt, "data-csrf-token=\"(?<token>[^\"]+)\"").Groups["token"].Value;
        }
    }
}
