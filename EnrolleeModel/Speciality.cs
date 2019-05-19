using System;
using System.Collections.Generic;
using System.ComponentModel;
using ViewGenerator;

namespace EnrolleeModel
{
    [Serializable]
    [Description("Специальность")]
    public class Speciality : IComparable<Speciality>
    {
        public Guid IdSpeciality { get; set; } = Guid.NewGuid();

        [Description("Название"), TextSize(200), DataNotEmpty]
        public string Name { get; set; }

        public int CompareTo(Speciality other)
        {
            return string.Compare(this.ToString(), other.ToString());
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }

    [Serializable]
    [Description("Специальности")]
    public class Specialities : List<Speciality>
    {
        public new void Add(Speciality item)
        {
            if (base.Exists(x => x.ToString().Trim() == item.ToString().Trim()))
                throw new Exception($"Специальность \"{item}\" уже существует!");
            base.Add(item);
            base.Sort();
        }

        public void ChangeTo(Speciality old, Speciality anew)
        {
            if (base.FindAll(x => x.ToString().Trim() == anew.ToString().Trim()).Count > 0)
                throw new Exception($"Специальность \"{anew}\" уже существует!");
            base.Remove(old);
            base.Add(anew);
            base.Sort();
        }

        public new void Remove(Speciality item)
        {
            if (Helper.SpecialityUsed(item.IdSpeciality))
                throw new Exception($"Специальность \"{item}\" ещё используется!");
            base.Remove(item);
        }
    }
}
