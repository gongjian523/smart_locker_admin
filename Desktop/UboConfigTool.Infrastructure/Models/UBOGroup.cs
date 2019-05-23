using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;

namespace UboConfigTool.Infrastructure.Models
{
    public class UBOGroup : IAggregateRoot
    {
        public UBOGroup()
        {

        }

        public UBOGroup( DTOUBOGroup dtoUBOGroup )
        {
            Id = dtoUBOGroup.id;
            Name = dtoUBOGroup.name;
            Description = dtoUBOGroup.description;
            LastDeployReqestAt = dtoUBOGroup.last_deploy_request_at;
            LastInfoChangedAt = dtoUBOGroup.last_info_changed_at;

            if( dtoUBOGroup.ubo_devices != null )
            {
                dtoUBOGroup.ubo_devices.ForEach( ubo =>
                    {
                        _UBODevices.Add( new UBO( ubo ) );
                    } );
            }

            if( dtoUBOGroup.usbs != null )
            {
                dtoUBOGroup.usbs.ForEach( usb =>
                {
                    _USBs.Add( new USB( usb ) );
                } );
            }

            if( dtoUBOGroup.rules != null )
            {
                dtoUBOGroup.rules.ForEach( rule =>
                    {
                        _Rules.Add( new Rule( rule ) );
                    } );
            }
        }

        public USB GetUSB( int usbId )
        {
            return this._USBs.FirstOrDefault( usb => usb.Id == usbId );
        }

        public int Id { get; private set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? LastDeployReqestAt { get; private set; }

        public DateTime? LastInfoChangedAt { get; private set; }

        private List<UBO> _UBODevices = new List<UBO>();
        public IList<UBO> UBODevices
        {
            get
            {
                return _UBODevices.AsReadOnly();
            }
            private set
            {
                if(value != null)
                {
                    _UBODevices = new List<UBO>(value);
                }
            }
        }

        private List<USB> _USBs = new List<USB>();
        public IList<USB> USBs
        {
            get
            {
                return _USBs.AsReadOnly();
            }
            private set
            {
                if( value != null )
                {
                    _USBs = new List<USB>( value );
                }
            }
        }

        private List<Rule> _Rules = new List<Rule>();
        public IList<Rule> Rules
        {
            get
            {
                return _Rules.AsReadOnly();
            }
            private set
            {
                if(value != null)
                {
                    _Rules = new List<Rule>( value );
                }
            }
        }

        public object Key
        {
            get
            {
                return Id;
            }
        }
    }
}
