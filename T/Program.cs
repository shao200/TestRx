using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using T.Dict;

namespace T
{
    class Program
    {
        static void Main(string[] args)
        {

            Subject<int> s1=new Subject<int>();
            Subject<int> s2 = new Subject<int>();

            s1.CombineLatest(s2, (a, b) => a * b).Subscribe(Console.WriteLine);

            s1.OnNext(2);

            s2.OnNext(4);

            s1.OnNext(6);

            s2.OnNext(8);




            var txt = new TextBox { Dock = DockStyle.Fill,Multiline=true };
            var topPanel = new Panel { Controls = { txt }, Dock = DockStyle.Top,Height=28};
            var lst = new ListBox { Dock = DockStyle.Fill };
            var fillPanel = new Panel { Controls = { lst }, Dock = DockStyle.Fill };
            var frm = new Form
            {
                Controls = { topPanel, fillPanel }
            };
            var input = (from evt in Observable.FromEventPattern<EventArgs>(txt, "TextChanged")
                         select ((TextBox)evt.Sender).Text)
                         .Throttle(TimeSpan.FromSeconds(1))
                         .DistinctUntilChanged()
                         .Do(x => Console.WriteLine(x));

            //this.textBox1Input.TextChanged
            //var fgs = (from evt in
            //               Observable.FromEventPattern<EventHandler>(txt, "TextChanged")
            //           select ((TextBox)evt.Sender).Text)
            //        .Throttle(TimeSpan.FromSeconds(1))
            //        .DistinctUntilChanged();

            var dc = new DictServiceSoapClient("DictServiceSoap");

            var matchInDict = Observable.FromAsyncPattern<string, string, string, DictionaryWord[]>(dc.BeginMatchInDict, dc.EndMatchInDict);

            Func<string, IObservable<DictionaryWord[]>> matchInWordNetByPrefix = term => matchInDict("wn", term, "prefix");

            var res = from term in input
                      from word in matchInWordNetByPrefix(term)
                      .Finally(() => Console.WriteLine("Disposed request for " + term))
                      .TakeUntil(input)
                      select word;

            //var res = matchInWordNetByPrefix("react");
            using (res.ObserveOn(lst).Subscribe(
                words => { lst.Items.Clear(); lst.Items.AddRange((from word in words select word.Word).ToArray()); },
               ex => { Console.WriteLine(ex); Console.WriteLine(ex.StackTrace); }                
                ))
                Application.Run(frm);
        }
    }
}
