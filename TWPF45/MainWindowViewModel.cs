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

        public MainWindowViewModel()
        {
            dc = new DictServiceSoapClient("DictServiceSoap");

            var wordInput = this.WhenAny(x => x.QueryWord, x => x.Value).DistinctUntilChanged();

            _QueryWord = "12";
            //only query if word length larger than 3
            QueryCommand = new ReactiveCommand(
                wordInput.Select(x => x.Length >= 3)
                );

            //when user inputed a new word, wait 0.7 sec, and query from internet
            this._QueryResults = wordInput
                .Where(w=>w.Length>3)
                .Throttle(TimeSpan.FromSeconds(1))
                .SelectMany(QueryWords)
                .ObserveOn(SynchronizationContext.Current) //-
                //.ObserveOnDispatcher()
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
