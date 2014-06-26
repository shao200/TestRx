using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private List<string> _QueryResults;
        public List<string> QueryResults
        {
            get { return _QueryResults; }
            set { this.RaiseAndSetIfChanged(ref _QueryResults, value); }
        }
        /// <summary>
        /// Gets the command executed when the user clicks the "OK" button.
        /// </summary>
        public ReactiveCommand QueryCommand { get; private set; }

        public MainWindowViewModel()
        {
            QueryResults = new List<string>();
            _QueryWord = "12312";
            //only query if word length larger than 3
            //QueryCommand = new ReactiveCommand(
            //    this.WhenAny(
            //    x => x.queryWord,
            //    q => !string.IsNullOrEmpty(q.Value) && q.Value.Length > 3)       
            //    );

            QueryCommand = ReactiveCommand.Create(
                x =>
                    !string.IsNullOrEmpty(this.QueryWord) && this.QueryWord.Length > 3,
                x => {
                    _QueryResults.Add(QueryWord); this.raisePropertyChanged("QueryResults");
                }
              

                );

            //QueryCommand.Subscribe(x => { 
            //    queryResults = new string[] { x.ToString() }; 
            //});
        }

    }
}
