using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ViewGenerator;

namespace EnrolleeModel
{
    [Serializable]
    [Description("Абитуриент")]
    public class Enrollee : IComparable<Enrollee>
    {
        public Guid IdEnrollee { get; set; } = Guid.NewGuid();

        [Description("Регистрационный номер"), TextSize(50), DataNotEmpty, TableBrowsable(false)]
        public string RegistrationNumber { get; set; }

        [Description("Фамилия"), DataNotEmpty, TextSize(50)]
        public string Surname { get; set; }

        [Description("Имя"), DataNotEmpty, TextSize(50)]
        public string FirstName { get; set; }

        [Description("Отчество"), DataNotEmpty, TextSize(50)]
        public string LastName { get; set; }

        [Description("Дата рождения")]
        public DateTime BirthDay { get; set; }

        [Description("Название среднего учебного заведения"), DataNotEmpty, TableBrowsable(false), TextSize(50)]
        public string SecodarySchoolName { get; set; }

        [Description("Номер среднего учебного заведения"), DataNotEmpty, TableBrowsable(false), TextSize(50)]
        public string SecodarySchoolNumber { get; set; }

        [Description("Город среднего учебного заведения"), DataNotEmpty, TableBrowsable(false), TextSize(50)]
        public string SecodarySchoolTown { get; set; }

        [Description("Дата окончания"), TableBrowsable(false)]
        public DateTime GraduationDate { get; set; }

        [Description("Наличие красного диплома или\nзолотой/серебряной медали"), TableBrowsable(false)]
        public bool GoldMedal { get; set; }

        [Description("Город"), DataNotEmpty, TableBrowsable(false), TextSize(50)]
        public string Town { get; set; }

        [Description("Улица"), DataNotEmpty, TableBrowsable(false), TextSize(50)]
        public string Street { get; set; }

        [Description("Номер дома"), DataNotEmpty, TableBrowsable(false), TextSize(50)]
        public string HouseNumber { get; set; }

        [Description("Телефон"), TableBrowsable(false), TextSize(50)]
        public string PhoneNumber { get; set; }

        [Description("Выбранная специальность"), DataLookup("IdSpeciality", "Specialities"), TableBrowsable(false)]
        public Guid IdSpeciality { get; set; }

        public int CompareTo(Enrollee other)
        {
            return string.Compare(this.ToString(), other.ToString());
        }

        public override string ToString()
        {
            return $"{Surname} {FirstName} {LastName}";
        }
    }


    [Serializable]
    [Description("Абитуриенты")]
    public class Enrollees : List<Enrollee>
    {
        public new void Add(Enrollee item)
        {
            if (base.Exists(x => x.RegistrationNumber.Trim() == item.RegistrationNumber.Trim()))
                throw new Exception($"Абитуриент с номером \"{item.RegistrationNumber}\" уже существует!");
            base.Add(item);
            base.Sort();
        }

        public void ChangeTo(Enrollee old, Enrollee anew)
        {
            if (base.FindAll(x => x.RegistrationNumber.Trim() == anew.RegistrationNumber.Trim()).Count > 0)
                throw new Exception($"Абитуриент с номером \"{anew.RegistrationNumber}\" уже существует!");
            base.Remove(old);
            base.Add(anew);
            base.Sort();
        }

        public new void Remove(Enrollee item)
        {
            base.Remove(item);
        }

        public List<Enrollee> FilteredBySpeciality(Guid idSpeciality)
        {
            return this.Where(item => item.IdSpeciality == idSpeciality).ToList();
        }

    }

}
