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
    
    public partial class Фильтры_обработок
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Фильтры_обработок()
        {
            this.Список_фильтров_над_обработкой = new HashSet<Список_фильтров_над_обработкой>();
        }
    
        public int id { get; set; }
        public string название { get; set; }
        public string описание { get; set; }
        public Nullable<System.DateTime> дата_добавления_записи { get; set; }
        public Nullable<System.DateTime> дата_последнего_изменения_записи { get; set; }
        public Nullable<bool> удален { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Список_фильтров_над_обработкой> Список_фильтров_над_обработкой { get; set; }
    }
}
