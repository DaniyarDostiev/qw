//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace qw.database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Трансформанта_измерения
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Трансформанта_измерения()
        {
            this.Пикет_ПромежуточныеТрансформантыИзмерения = new HashSet<Пикет_ПромежуточныеТрансформантыИзмерения>();
            this.Пикет_ТрансформантаИзмерения = new HashSet<Пикет_ТрансформантаИзмерения>();
        }
    
        public int id { get; set; }
        public string частота { get; set; }
        public string значение_кажущегося_сопратевления__Rok_ { get; set; }
        public Nullable<System.DateTime> дата_добавления_записи { get; set; }
        public Nullable<System.DateTime> дата_последнего_изменения_записи { get; set; }
        public Nullable<bool> удален { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Пикет_ПромежуточныеТрансформантыИзмерения> Пикет_ПромежуточныеТрансформантыИзмерения { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Пикет_ТрансформантаИзмерения> Пикет_ТрансформантаИзмерения { get; set; }
    }
}
