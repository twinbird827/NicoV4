using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Models
{
    [DataContract]
    public class FavoriteModel : BindableBase
    {
        [DataMember]
        public string Mylist
        {
            get { return _Mylist; }
            set { SetProperty(ref _Mylist, value); }
        }
        private string _Mylist;

        [DataMember]
        public DateTime LastConfirmDatetime
        {
            get { return _LastConfirmDatetime; }
            set { SetProperty(ref _LastConfirmDatetime, value); }
        }
        private DateTime _LastConfirmDatetime;
    }
}
