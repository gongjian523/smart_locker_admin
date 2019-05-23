using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Interfaces;

namespace UboConfigTool.Message
{
    [Export( typeof( IMessageService ) )]
    [PartCreationPolicy( CreationPolicy.Shared )]
    public class MessageService : IMessageService
    {
        public void ShowMessage( string message )
        {

        }

        public DialogResponse ShowMessage( string title, string message, string negativeBtnText, string positiveBtnText, DialogType dialogType = DialogType.OKCancle )
        {
            MessageDialogViewModel msgDlgVM = new MessageDialogViewModel( title, message, negativeBtnText, positiveBtnText, dialogType );

            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            if( regionMgr == null )
                return DialogResponse.Cancel;

            var regionContextBak = regionMgr.Regions[RegionNames.SecondaryRegion].Context;

            regionMgr.Regions[RegionNames.SecondaryRegion].Context = msgDlgVM;
            regionMgr.RequestNavigate( RegionNames.SecondaryRegion, new Uri( "MessageDialog", UriKind.Relative ) );

            regionMgr.Regions[RegionNames.SecondaryRegion].Context = regionContextBak;

            return msgDlgVM.Response;
        }


        public DialogResponse ShowTextInputMessage( ref string expectedText, string inputHintText, Action<TextInputValiateResult> validateAction )
        {
            TextInputDialogViewModel msgInputDlgVM = new TextInputDialogViewModel( inputHintText, validateAction );

            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            if( regionMgr == null )
                return DialogResponse.Cancel;

            var regionContextBak = regionMgr.Regions[RegionNames.SecondaryRegion].Context;

            regionMgr.Regions[RegionNames.SecondaryRegion].Context = msgInputDlgVM;
            regionMgr.RequestNavigate( RegionNames.SecondaryRegion, new Uri( "TextInputDialog", UriKind.Relative ) );

            regionMgr.Regions[RegionNames.SecondaryRegion].Context = regionContextBak;

            expectedText = msgInputDlgVM.InputText;

            return msgInputDlgVM.Response;
        }
    }
}
