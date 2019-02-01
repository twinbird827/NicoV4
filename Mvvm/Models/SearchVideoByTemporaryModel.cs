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
                var video = VideoStatusModel.Instance.GetVideo((string)item["item_data"]["video_id"]);

                video.VideoUrl = item["item_data"]["video_id"];
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
/*
<item type="object">
  <item_type type="string">0</item_type>
  <item_id type="string">1367028098</item_id>
  <description type="string"></description>
  <item_data type="object">
    <video_id type="string">sm20707987</video_id>
    <title type="string">【FF14ニコ超2】許されたシーン（意味深）</title>
    <thumbnail_url type="string">http://tn.smilevideo.jp/smile?i=20707987</thumbnail_url>
    <first_retrieve type="number">1367028098</first_retrieve>
    <update_time type="number">1547605203</update_time>
    <view_counter type="string">148267</view_counter>
    <mylist_counter type="string">1311</mylist_counter>
    <num_res type="string">4116</num_res>
    <group_type type="string">default</group_type>
    <length_seconds type="string">461</length_seconds>
    <deleted type="string">0</deleted>
    <last_res_body type="string">天丼するなw 逃げたシーンwwww 取り逃げwwww これは奇跡 ネタじゃない事実だぞ HQ正直でよろしい 草ァwwww オンラインつながりd あっ! ありましたね～ ふんふんふんふん 取り逃げの... </last_res_body>
    <watch_id type="string">sm20707987</watch_id>
  </item_data>
  <watch type="number">0</watch>
  <create_time type="number">1548886893</create_time>
  <update_time type="number">1548886893</update_time>
</item>
 * */
