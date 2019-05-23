using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Interfaces;

namespace UboConfigTool.Message
{

    public class MessageDialogViewModel : PopupViewModelBase
    {
        public MessageDialogViewModel( string title, string messgae, string negativeBtnText, string positiveBtnText, DialogType type )
        {
            Title = title;
            MessageContent = messgae;

            if( string.IsNullOrEmpty( positiveBtnText ) )
                PositiveBtnText = "确定";
            else
                PositiveBtnText = positiveBtnText;

            if( string.IsNullOrEmpty( negativeBtnText ) )
                NegativeBtnText = "取消";
            else
                NegativeBtnText = negativeBtnText;

            PositiveBtnCommand = new DelegateCommand( PositiveBtnAction );
            Response = DialogResponse.Cancel;

            NegativeBtnVisibility = (type == DialogType.Message ? false : true);
        }
            
        public ICommand PositiveBtnCommand
        {
            get;
            private set;
        }

        public override bool Validate()
        {
            return true;
        }

        public string Title
        {
            get;
            private set;
        }

        public string MessageContent
        {
            get;
            private set;
        }

        public string PositiveBtnText
        {
            get;
            private set;
        }

        public string NegativeBtnText
        {
            get;
            private set;
        }

        public DialogResponse Response
        {
            get;
            private set;
        }

        private void PositiveBtnAction()
        {
            Response = DialogResponse.Ok;
            base.ClosePopup();
        }

        public bool NegativeBtnVisibility
        {
            get;
            private set;
        }
    }
}
