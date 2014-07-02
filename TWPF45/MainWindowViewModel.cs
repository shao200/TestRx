using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using TWPF45.Dict;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using fastJSON;
using System.Windows.Threading;
using System.ComponentModel;

namespace TWPF
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _QueryWord;
        public string QueryWord
        {
            get { return _QueryWord; }
            set { this.RaiseAndSetIfChanged(ref _QueryWord, value); }
        }
        const string jsonSaveFile = "proxy.json";
        public int GetCount { get; set; }
        public Task<List<string>> LoadJsonAsync()
        {
            return Task.Factory.StartNew<List<string>>(() =>
            {
                List<string> res = null;
                try
                {
                    WebClient wc = new WebClient();
                    wc.Headers.Add("X-Mashape-Authorization", "rPSxC7K1iPGlbvA4ZgYEy9klJBuEZuYC");

                    var ips = wc.DownloadString("https://webknox-proxies.p.mashape.com/proxies/newMultiple?maxResponseTime=10&batchSize=" + GetCount);

                    res = JSON.ToObject<List<string>>(ips);
                }

                catch (Exception ex) { return new List<string> { ex.Message, ex.StackTrace }; }

                return res;
            });
        }

        bool _IsBusy;
        [DisplayName("Whether is fetcing IPs now")]
        [Description("Busy fetcing IPs")]
        public bool IsBusy
        {
            get { 
                return _IsBusy;
            }
            set {

                _IsBusy = value;
                this.raisePropertyChanged("IsBusy");
                //this.RaiseAndSetIfChanged(ref _IsBusy, value); 
            }
        }

        /// <summary>
        /// Gets the command executed when the user clicks the "OK" button.
        /// </summary>
        public ReactiveCommand QueryCommand { get; private set; }

        public ReactiveCommand Fetch { get; private set; }        

        DictServiceSoapClient dc;

        /// <summary>
        /// Look up the query from WCF
        /// </summary>
        /// <param name="qw"></param>
        /// <returns></returns>
        IObservable<IList<string>> QueryWords(string qw)
        {
            return LoadJsonAsync().ToObservable();

            var ds = from words in dc.MatchInDictAsync("wn", qw, "prefix").ToObservable()
                    from word in words
                     select word.Word;
            return ds.ToList();
        }

        ObservableAsPropertyHelper<List<string>> _QueryResults;
        public List<string> QueryResults { get { return _QueryResults.Value; } }


        IList<string> _IPs;
        public IList<string> IPs { get { return _IPs; } set { this.RaiseAndSetIfChanged(ref _IPs, value); } }



        bool InputValid(string s)
        {
            return !string.IsNullOrEmpty(s) && s.Length >= 3;
        }
        Dispatcher uiDispatcher;
        public MainWindowViewModel()
        {
            uiDispatcher=Dispatcher.CurrentDispatcher;
            dc = new DictServiceSoapClient("DictServiceSoap");
            GetCount = 2;

            var wordInput = this.WhenAny(x => x.QueryWord, x => x.Value).DistinctUntilChanged();

            //only query if word length larger than 3
            QueryCommand = new ReactiveCommand(
                wordInput.Select(InputValid)
                );

            

            Fetch = new ReactiveCommand(
                this.WhenAny(x => x.IsBusy,
                x =>
                {
                    return 
                        //true;
                        !x.Value;
                }
                ).DistinctUntilChanged()
                //wordInput.Select(InputValid)
                );
            Fetch.Subscribe(o => { IsBusy = true; });
            var ftu = Fetch.RegisterAsyncFunction(o =>
            {
                Thread.Sleep(500);
                return new List<string> { DateTime.Now.ToString(), o.ToString() };
            })
            .ObserveOn(SynchronizationContext.Current)
            ;
            //Fetch.
                
                
            //    Subscribe(_ =>
            //    {
            //        IsBusy = true;
            //        Task.Factory.StartNew(() =>
            //        {
            //            Thread.Sleep(2000);
            //            uiDispatcher.BeginInvoke(new Action(() => {

            //                IPs = new List<string> { DateTime.Now.ToString(),_.ToString()};
            //                IsBusy = false;
                        
            //            }));

            //        });
            //    }
               
            //);

            this._QueryResults = new ObservableAsPropertyHelper<List<string>>(ftu, x => {

                if (x == null) return;

                IsBusy = false;
                
                raisePropertyChanged("QueryResults");

                //raisePropertyChanged("IsBusy");
            
            });

            //Fetch.Execute

            //var fr = Fetch.Select(_ => QueryWords(string.Empty)).Do(_ => IsBusy = true);
            //Fetch.CanExecuteObservable
            //    .Do(_ => IsBusy = true)
            //    .SelectMany(_ => QueryWords(string.Empty) )
            //    .ObserveOn(SynchronizationContext.Current) //-
            //    //.ObserveOnDispatcher()
            //    .Do(_ => IsBusy = false)
            //    .ToProperty(this, x => x.QueryResults); ; ;

            //when user inputed a new word, wait 0.7 sec, and query from internet
            //this._QueryResults = wordInput
            //    .Where(InputValid)
            //    .Do(_ => IsBusy = true)
            //    .Throttle(TimeSpan.FromSeconds(1))
            //    .SelectMany(QueryWords)

            //    .ObserveOn(SynchronizationContext.Current) //-
            //    //.ObserveOnDispatcher()
            //    .Do(_ => IsBusy = false)
            //    .ToProperty(this, x => x.QueryResults); ;

            //QueryCommand = ReactiveCommand.Create(
            //    x =>
            //        !string.IsNullOrEmpty(this.QueryWord) && this.QueryWord.Length > 3,
            //    x => {
            //        _QueryResults.Add(QueryWord); this.raisePropertyChanged("QueryResults");
            //    }
              

            //    );

            //QueryCommand.Subscribe(x => { 
            //    queryResults = new string[] { x.ToString() }; 
            //});
        }

    }
}
