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
    public class TextInputDialogViewModel : PopupViewModelBase
    {
        private Action<TextInputValiateResult> _validateAction = null;

        public TextInputDialogViewModel( string inputHint, Action<TextInputValiateResult> validateAction = null )
        {
            _validateAction = validateAction;
            _inputHint = inputHint;

            PositiveBtnCommand = new DelegateCommand( PositiveBtnAction );
            Response = DialogResponse.Cancel;
        }

        public ICommand PositiveBtnCommand
        {
            get;
            private set;
        }

        private string _inputText = string.Empty;
        public string InputText
        {
            get
            {
                return _inputText;
            }
            set
            {
                this._inputText = value;
                RaisePropertyChanged( () => InputText );
            }
        }

        private string _inputHint = string.Empty;
        public string InputHint
        {
            get
            {
                return _inputHint;
            }
            set
            {
                this._inputHint = value;
                RaisePropertyChanged( () => InputHint );
            }
        }

        private string _errorInfo = string.Empty;
        public string ErrorInfo
        {
            get
            {
                return _errorInfo;
            }
            set
            {
                this._errorInfo = value;
                RaisePropertyChanged( () => ErrorInfo );
            }
        }

        public DialogResponse Response
        {
            get;
            private set;
        }

        public override bool Validate()
        {
            ErrorInfo = string.Empty;

            if( string.IsNullOrEmpty( InputText ) )
            {
                ErrorInfo = "输入不能为空";
                return false;
            }

            if( _validateAction != null )
            {
                TextInputValiateResult result = new TextInputValiateResult( false, InputText );
                _validateAction( result );
                if( !result.IsPass )
                {
                    ErrorInfo = result.ExtraInfo;
                    return false;
                }
            }

            return true;
        }


        private void PositiveBtnAction()
        {
            if( !Validate() )
                return;

            Response = DialogResponse.Ok;
            base.ClosePopup();
        }
    }
}
