using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TWcf.Dict;

namespace TWcf
{
    class Program
    {
        static void Main(string[] args)
        {

            var txt = new TextBox();
            var lst = new ListBox { Top = txt.Height + 10 };
            var frm = new Form
            {
                Controls = { txt, lst }
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

            var dc = new DictServiceSoapClient();

            var matchInDict = Observable.FromAsyncPattern<string, string, string, DictionaryWord[]>(dc.BeginMatchInDict, dc.EndMatchInDict);

            Func<string, IObservable<DictionaryWord[]>> matchInWordNetByPrefix = term => matchInDict("wn", term, "prefix");

            var res = input.SelectMany(w => matchInWordNetByPrefix(w));

            //var res = matchInWordNetByPrefix("react");
            using (res.ObserveOn(lst).Subscribe(inp => { lst.Items.Clear();lst.Items.AddRange(inp); }))
                Application.Run(frm);
        }
    }
}
