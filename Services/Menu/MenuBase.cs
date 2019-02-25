using Bubblim.Core;
using Discord;
using Bubblim.Core.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bubblim.Core.Services
{
    public abstract class MenuBase : IMenuItem
    {
        public string title { get; set; }
        public string summary { get; set; }
        public string instructions { get; set; }
        public string oneTimeMessage { get; set; }
        protected List<IMenuItem> _menus;
        protected List<MenuHeader> _headers;
        private bool _displayEnabled = true;
        private bool _waitingForInput;
        private EmbedBuilder _embed;
        private Stream _stream;
        private string _fileName;
        private bool _initialized;

        public MenuBase()
        {
            title = "";
            summary = "";
            instructions = "*Please type the number for the menu you wish to visit*";
            _menus = new List<IMenuItem>();
            _headers = new List<MenuHeader>();
            _waitingForInput = false;
            _embed = null;
            _initialized = false;
        }

        public virtual void Init() { }
        public virtual void OnDisplay(IUser user) { }
        public virtual string GetHeader() => "";
        public virtual string GetFooter() => "";

        public async void Display(IUser user)
        {
            if (!_initialized)
            {
                Init();
                _initialized = true;
            }

            OnDisplay(user);
            if (!_displayEnabled) return;

            string response = "";

            if(oneTimeMessage != null)
            {
                response += $"{oneTimeMessage}\n\n";
                oneTimeMessage = null;
            }

            response += GetHeader();
            response += GetTitle();
            response += GetMenuList();
            response += GetFooter();

            try
            {
                if(_stream != null)
                {
                    await user.GetOrCreateDMChannelAsync().Result.SendFileAsync(_stream, "image.png", response);
                    _stream.Dispose();
                    _stream = null;
                }
                else
                    await user.SendMessageAsync(response, false, _embed.Build());
            }
            catch(Exception e) //ArgumentException
            {
                PrettyPrint.WriteLine(e.ToString());
            }
        }

        public void AddMenu(params IMenuItem[] menus)
        {
            foreach(IMenuItem menu in menus)
            {
                _menus.Add(menu);
            }
        }

        public void AddHeader(string header)
        {
            _headers.Add(new MenuHeader()
            {
                InsertBeforeId = _menus.Count(),
                Header = header
            });
        }

        public bool MenuOptionExists(int menuId)
        {
            if (menuId <= _menus.Count && menuId > 0)
                return true;
            return false;
        }

        public IMenuItem GetMenuByOptionId(int optionId)
        {
            return _menus[optionId - 1];
        }

        public bool RequestingInput()
        {
            return _waitingForInput;
        }

        public bool RequestingInput(bool isTrue)
        {
            _waitingForInput = isTrue;
            return RequestingInput();
        }

        public bool EnableDisplay(bool enabled)
        {
            _displayEnabled = enabled;
            return enabled;
        }

        public void AttachEmbed(EmbedBuilder embed)
        {
            _embed = embed;
        }

        public void AttachFile(Stream stream, string fileName)
        {
            _stream = stream;
            _fileName = fileName;
        }

        public string GetTitle()
        {
            return $"**{title}** - {instructions}";
        }

        //public string GetMenuList()
        //{
        //    return _menus.ToNumberedEmojiString(x => x.summary);
        //}

        public string GetMenuList()
        {
            var i = 0;
            var output = _menus.ToNumberedEmojiList(x => x.summary);
            _headers.OrderByDescending(x => x.InsertBeforeId);

            foreach (var header in _headers)
            {
                output.Insert(header.InsertBeforeId + i, $"\n{header.Header}");
                i++;
            }

            return '\n' + string.Join("\n", output);
        }
    }
}
