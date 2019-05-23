using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;

namespace UboConfigTool.Domain.Models
{
    public enum RuleType
    {
        None = 0,
        Black_name = 1,
        White_name = 2
    }

    public class Rule : IEntity
    {
        public Rule()
        {
        }

        public Rule( DTORule dtoRule )
        {
            Id = dtoRule.id;
            Name = dtoRule.name;
            CreatedAt = dtoRule.created_at;
            UpdatedAt = dtoRule.updated_at;
            Type = (RuleType)dtoRule.rule_type;
        }

        public static DTORule ToDTOModel( Rule rule )
        {
            if( rule == null )
                return null;

            DTORule dtoRule = new DTORule()
            {
                id = rule.Id,
                name = rule.Name,
                created_at = rule.CreatedAt,
                updated_at = rule.UpdatedAt,
                rule_type = (int)rule.Type
            };

            return dtoRule;
        }

        public int Id { get; private set;}

        public string Name { get; set; }

        public DateTime? CreatedAt  { get; private set; }

        public DateTime? UpdatedAt  { get; private set; }

        public RuleType Type{ get; private set; }

        public object Key
        {
            get
            {
                return Id;
            }
        }
    }
}
