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

namespace TWPF
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _QueryWord;
        public string QueryWord
        {
            get { return _QueryWord; }
            set {
                this.RaiseAndSetIfChanged(ref _QueryWord, value);
            }
        }

        bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        /// <summary>
        /// Gets the command executed when the user clicks the "OK" button.
        /// </summary>
        public ReactiveCommand QueryCommand { get; private set; }

        DictServiceSoapClient dc;

        /// <summary>
        /// Look up the query from WCF
        /// </summary>
        /// <param name="qw"></param>
        /// <returns></returns>
        IObservable<IList<string>> QueryWords(string qw)
        {
            var ds = from words in dc.MatchInDictAsync("wn", qw, "prefix").ToObservable()
                    from word in words
                    select word.Word;
            return ds.ToList();
        }

        ObservableAsPropertyHelper<IList<string>> _QueryResults;
        public IList<string> QueryResults { get { return _QueryResults.Value; } }

        bool InputValid(string s)
        {
            return !string.IsNullOrEmpty(s) && s.Length >= 3;
        }
        public MainWindowViewModel()
        {
            dc = new DictServiceSoapClient("DictServiceSoap");

            var wordInput = this.WhenAny(x => x.QueryWord, x => x.Value).DistinctUntilChanged();

            //only query if word length larger than 3
            QueryCommand = new ReactiveCommand(
                wordInput.Select(InputValid)
                );

            //when user inputed a new word, wait 0.7 sec, and query from internet
            this._QueryResults = wordInput
                .Where(InputValid)
                .Do(_ => IsBusy = true)
                .Throttle(TimeSpan.FromSeconds(1))
                .SelectMany(QueryWords)
                .ObserveOn(SynchronizationContext.Current) //-
                //.ObserveOnDispatcher()
                .Do(_ => IsBusy = false)
                .ToProperty(this, x => x.QueryResults); ;

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
