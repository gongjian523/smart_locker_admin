using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.Interfaces
{
    public enum DialogType
    {
        OKCancle = 0,
        Message
    }

    public enum DialogResponse
    {
        Cancel = 0,
        Ok
    }

    public class TextInputValiateResult
    {
        public TextInputValiateResult(bool isPass, string textInput = "")
        {
            IsPass = isPass;
            TextInput = textInput;
        }
        public bool IsPass
        {
            get;
            set;
        }

        public string TextInput
        {
            get;
            set;
        }

        public string ExtraInfo
        {
            get;
            set;
        }
    }

    public interface IMessageService
    {
        void ShowMessage( string message );

        DialogResponse ShowMessage( string tile, string message, string negativeBtnText = "", string positiveBtnText = "", DialogType dialogType = DialogType.OKCancle );

        DialogResponse ShowTextInputMessage( ref string expectedText, string inputHintText, Action<TextInputValiateResult> validateAction = null );
    }
}
