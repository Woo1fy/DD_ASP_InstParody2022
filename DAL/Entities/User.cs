using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    /// <summary>
    /// Сущность пользователя 
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } 
        public string? Email { get; set; } 
        public string? PasswordHash { get; set; } 
        public DateTimeOffset BirthDate { get; set; } // Бэк не должен быть зависим от региона, поэтому указываем DateTimeOffset

        public virtual ICollection<UserSession>? Sessions { get; set; }
    }

    // Здесь будут прописываться также связи сущности и все, что с ней связано
}


