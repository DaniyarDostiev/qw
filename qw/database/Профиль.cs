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
    using System.Linq;

    public partial class Профиль
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Профиль()
        {
            this.Профиль_СписокОбработок = new HashSet<Профиль_СписокОбработок>();
            this.Профиль_ТелеметрическиеИзмерения = new HashSet<Профиль_ТелеметрическиеИзмерения>();
            this.Профиль_ТочкиИзломов = new HashSet<Профиль_ТочкиИзломов>();
            this.Профиль_ЭлектромагнитныеИзмерения = new HashSet<Профиль_ЭлектромагнитныеИзмерения>();
            this.Пикет = new HashSet<Пикет>();
        }
    
        public int id { get; set; }
        public string название_профиля { get; set; }
        public Nullable<int> id_координат_начала { get; set; }
        public Nullable<int> Id_координат_конца { get; set; }
        public Nullable<int> id_площади { get; set; }
        public Nullable<int> id_методики { get; set; }
        public Nullable<double> длина_профиля { get; set; }
        public Nullable<System.DateTime> дата_начала_работ { get; set; }
        public Nullable<System.DateTime> дата_окончания_работ { get; set; }
        public Nullable<System.DateTime> дата_добавления_записи { get; set; }
        public Nullable<System.DateTime> дата_последнего_изменения_записи { get; set; }
        public Nullable<bool> удален { get; set; }
    
        public virtual Координаты_точки Координаты_точки { get; set; }
        public virtual Координаты_точки Координаты_точки1 { get; set; }
        public virtual Методика Методика { get; set; }
        public virtual Площадь Площадь { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Профиль_СписокОбработок> Профиль_СписокОбработок { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Профиль_ТелеметрическиеИзмерения> Профиль_ТелеметрическиеИзмерения { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Профиль_ТочкиИзломов> Профиль_ТочкиИзломов { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Профиль_ЭлектромагнитныеИзмерения> Профиль_ЭлектромагнитныеИзмерения { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Пикет> Пикет { get; set; }

        //public int количество_пикетов
        //{
        //    get
        //    {
        //        //return this.Пикет.Where(x => x.);
        //    }
        //}
    }
}
