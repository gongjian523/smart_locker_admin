using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;

namespace UboConfigTool.Domain.Models
{
    public class USB : IEntity
    {
        public USB()
        {

        }

        public USB( DTOUSB dtoUsb )
        {
            Id = dtoUsb.id;
            PId = dtoUsb.pid;
            VId = dtoUsb.vid;
            Name = dtoUsb.name;
            SerialNumer = dtoUsb.serial_num;
            Volume_Id = dtoUsb.volume_id;
            Capacity = dtoUsb.capacity;
            Factory = dtoUsb.factory;
            DiskId = dtoUsb.disk_id;
            DiskType = dtoUsb.disk_type;
            FileSystemFormat = dtoUsb.filesystem_format;
            CreatedAt = dtoUsb.created_at;
            UpdatedAt = dtoUsb.updated_at;
        }

        public static DTOUSB ToDTOModel( USB usb )
        {
            if( usb == null )
                return null;

            DTOUSB dtoUsb = new DTOUSB
            {
                id = usb.Id,
                pid = usb.PId,
                vid = usb.VId,
                name = usb.Name,
                serial_num = usb.SerialNumer,
                volume_id = usb.Volume_Id,
                capacity = usb.Capacity,
                factory = usb.Factory,
                disk_id = usb.DiskId,
                disk_type = usb.DiskType,
                filesystem_format = usb.FileSystemFormat,
                created_at = usb.CreatedAt,
                updated_at = usb.UpdatedAt
            };

            return dtoUsb;
        }


        public int Id { get; private set; }

        public string PId  { get; private set; }

        public string VId  { get; private set; }

        public string Name { get; set; }

        public string SerialNumer  { get; private set; }

        public string Volume_Id  { get; private set; }

        public string Capacity  { get; private set; }

        public string Factory  { get; private set; }

        public string DiskId  { get; private set; }

        public int DiskType  { get; private set; }

        public string FileSystemFormat  { get; private set; }

        public DateTime? CreatedAt  { get; private set; }

        public DateTime? UpdatedAt  { get; private set; }

        public object Key
        {
            get
            {
                return Id;
            }
        }
    }
}
