using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKIconsToXAML {
    public class VKIconSVGParser : IDisposable {
        public Tuple<bool, string, string> Parse(string path) {
            try {
                HtmlDocument hd = new HtmlDocument();
                hd.DetectEncodingAndLoad(path);

                HtmlNode svg = hd.DocumentNode.ChildNodes["svg"];
                string w = svg.Attributes["width"].Value;
                string h = svg.Attributes["height"].Value;
                w = w.Contains("px") ? w.Substring(0, w.Length - 2) : w;
                h = h.Contains("px") ? h.Substring(0, h.Length - 2) : h;
                
                string name = ParseName(path.Split('\\').Last());

                FindNonStandartIconPath(hd.DocumentNode.ChildNodes["svg"]);
                if(String.IsNullOrEmpty(currentdata)) throw new ArgumentNullException("currentdata", "Unable to find a icon path data.");
                string pathd = currentdata;

                Tuple<string, string> r = GenerateXAML(name, w, h, pathd);
                return new Tuple<bool, string, string>(true, r.Item1, r.Item2);
            } catch(Exception ex) {
                string z = $"<!-- Parse error!\n\nHResult: 0x{ex.HResult.ToString("x8")}]\n\nMessage: \n{ex.Message}\n\nStack trace: \n{ex.StackTrace}\n-->";
                return new Tuple<bool, string, string>(false, z, z);
            }
        }

        private string ParseName(string s) {
            s = s.Substring(0, s.Length - 4); // remove ".svg"
            string[] sp = s.Split('_');
            if (sp.Length < 2) return "";
            string size = sp.Last();
            string names = "";
            for (int i = 0; i < sp.Length - 1; i++) {
                names += $"{sp[i][0].ToString().ToUpper()}{sp[i].Substring(1)}";
            }
            return $"Icon{size}{names}";
        }

        private string currentdata = "";
        private void FindNonStandartIconPath(HtmlNode node) {
            foreach (var hn in node.ChildNodes) {
                if(hn.Name.ToLower() == "path" && hn.Attributes["d"].ValueLength > currentdata.Length) {
                    currentdata = hn.Attributes["d"].Value;
                } else {
                    FindNonStandartIconPath(hn);
                }
            }
        }

        private Tuple<string, string> GenerateXAML(string name, string w, string h, string data) {
            string xaml = "";
            //string xaml = $"<!-- Перейдите в github.com/Elorucov/vkui-uwp/ \n";
            //xaml += $"чтобы узнать подробности о подключении иконок. -->\n";
            //xaml += $"\n";
            xaml += $"<DataTemplate x:Key=\"{name}\">\n";
            xaml += $"    <Viewbox Stretch=\"Uniform\">\n";
            xaml += $"        <PathIcon HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\" Width=\"{w}\" Height=\"{h}\" Data=\"{data}\"/>\n";
            xaml += $"    </Viewbox>\n";
            xaml += $"</DataTemplate>";

            string xaml2 = $"<ContentPresenter Foreground=\"Blue\" Width=\"{h}\" Height=\"{h}\" ContentTemplate=\"{{StaticResource {name}}}\"/>";

            return new Tuple<string, string>(xaml, xaml2);
        }

        public void Dispose() {
            currentdata = null;
        }
    }
}
