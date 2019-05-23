using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;

namespace UboConfigTool.Domain.Models
{
    public enum UBOConnectionType
    {
        None = 0,
        Disconnected = 1,
        Connected = 2
    }

    public class UBO : IEntity
    {
        public UBO()
        {

        }

        public UBO( DTOUBO dtoUBO )
        {
            Id = dtoUBO.id;
            Name = dtoUBO.name;
            Description = dtoUBO.description;
            SId = dtoUBO.sid;
            BId = dtoUBO.bid;
            CreatedAt = dtoUBO.created_at;
            UpdatedAt = dtoUBO.updated_at;
            UBOGroupId = dtoUBO.ubo_group_id;
            Version = dtoUBO.version;
            IPAddress = dtoUBO.ip;
            Location = dtoUBO.location;
            ConnectionStatus = (UBOConnectionType)dtoUBO.connection_status;
            VirusDBVersion = dtoUBO.virusdb_version;
            LastResponseAt = dtoUBO.last_response_at;
        }

        public static DTOUBO TODTOModel( UBO ubo )
        {
            if( ubo == null )
                return null;

            DTOUBO dtoUbo = new DTOUBO()
            {
                id = ubo.Id,
                name = ubo.Name,
                description = ubo.Description,
                sid = ubo.SId,
                bid = ubo.BId,
                created_at = ubo.CreatedAt,
                updated_at = ubo.UpdatedAt,
                ubo_group_id = ubo.UBOGroupId,
                version = ubo.Version,
                ip = ubo.IPAddress,
                location = ubo.Location,
                connection_status = (int)ubo.ConnectionStatus,
                virusdb_version = ubo.VirusDBVersion,
                last_response_at = ubo.LastResponseAt
            };

            return dtoUbo;
        }


        public int Id { get; private set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SId { get; private set; }

        public string BId  { get; private set; }

        public DateTime? CreatedAt { get; private set; }

        public DateTime? UpdatedAt  { get; private set; }

        public int UBOGroupId { get; private set; }

        public string Version  { get; private set; }

        public string IPAddress { get; set; }

        public string Location { get; set; }

        public UBOConnectionType ConnectionStatus { get; private set; }

        public string VirusDBVersion { get; private set; }

        public DateTime? LastResponseAt { get; private set; }

        public object Key
        {
            get
            {
                return Id;
            }
        }

    }
}
