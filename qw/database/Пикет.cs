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
    
    public partial class Пикет
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Пикет()
        {
            this.Пикет_ИзмерительноеОборудование = new HashSet<Пикет_ИзмерительноеОборудование>();
            this.Пикет_ПромежуточныеИзмерения = new HashSet<Пикет_ПромежуточныеИзмерения>();
            this.Пикет_ПромежуточныеТрансформантыИзмерения = new HashSet<Пикет_ПромежуточныеТрансформантыИзмерения>();
            this.Пикет_Сотрудники = new HashSet<Пикет_Сотрудники>();
            this.Пикет_ТрансформантаИзмерения = new HashSet<Пикет_ТрансформантаИзмерения>();
        }
    
        public int id { get; set; }
        public string название_пикета { get; set; }
        public Nullable<int> id_профиля { get; set; }
        public Nullable<int> id_координат_нахождения { get; set; }
        public Nullable<int> id_окончательного_результата { get; set; }
        public Nullable<System.DateTime> дата_и_время_получения_окончательного_результата_измерения { get; set; }
        public Nullable<System.DateTime> дата_добавления_записи { get; set; }
        public Nullable<System.DateTime> дата_последнего_изменения_записи { get; set; }
    
        public virtual Координаты_точки Координаты_точки { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Пикет_ИзмерительноеОборудование> Пикет_ИзмерительноеОборудование { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Пикет_ПромежуточныеИзмерения> Пикет_ПромежуточныеИзмерения { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Пикет_ПромежуточныеТрансформантыИзмерения> Пикет_ПромежуточныеТрансформантыИзмерения { get; set; }
        public virtual Профиль Профиль { get; set; }
        public virtual Результаты_измерения Результаты_измерения { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Пикет_Сотрудники> Пикет_Сотрудники { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Пикет_ТрансформантаИзмерения> Пикет_ТрансформантаИзмерения { get; set; }
    }
}
