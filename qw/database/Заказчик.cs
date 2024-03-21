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
    
    public partial class Заказчик
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Заказчик()
        {
            this.Проект = new HashSet<Проект>();
        }
    
        public int id { get; set; }
        public string имя { get; set; }
        public string юр__адрес { get; set; }
        public string физ__адрес { get; set; }
        public string инн { get; set; }
        public string кпк { get; set; }
        public string р_с { get; set; }
        public string представитель { get; set; }
        public string номер_телефона { get; set; }
        public string эл__почта { get; set; }
        public string ссылка_на_сайт { get; set; }
        public Nullable<System.DateTime> дата_добавления_записи { get; set; }
        public Nullable<System.DateTime> дата_последнего_изменения_записи { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Проект> Проект { get; set; }

        public int количество_проектов
        {
            get { return this.Проект.Count; } // Используем Count для подсчета количества проектов
        }
    }
}
