using System;
using System.Collections.Generic;
using System.ComponentModel;
using ViewGenerator;

namespace EnrolleeModel
{
    [Serializable]
    public enum PassKind
    {
        Устно,      //Verbally
        Писменно    //BlackAndWhite
    }

    [Serializable]
    [Description("Сдаваемый предмет")]
    public class PassMatter : IComparable<PassMatter>
    {
        public Guid IdPassMatter { get; set; } = Guid.NewGuid();

        [Description("Специальность"), DataLookup("IdSpeciality", "Specialities")]
        public Guid IdSpeciality { get; set; }

        [Description("Предмет"), DataLookup("IdMatter", "Matters")]
        public Guid IdMatter { get; set; }

        [Description("Форма сдачи")]
        public PassKind PassForm { get; set; }

        public int CompareTo(PassMatter other)
        {
            return string.Compare(this.ToString(), other.ToString());
        }

        public override string ToString()
        {
            return $"{Helper.SpecialityById(IdSpeciality)}, {Helper.MatterById(IdMatter)} ({PassForm})";
        }
    }

    [Serializable]
    [Description("Сдаваемые предметы")]
    public class PassMatters : List<PassMatter>
    {
        public new void Add(PassMatter item)
        {
            if (base.Exists(x => x.ToString().Trim() == item.ToString().Trim()))
                throw new Exception($"Сдаваемый предмет \"{item}\" уже существует!");
            base.Add(item);
            base.Sort();
        }

        public void ChangeTo(PassMatter old, PassMatter anew)
        {
            if (base.FindAll(x => x.ToString().Trim() == anew.ToString().Trim()).Count > 0)
                throw new Exception($"Сдаваемый предмет \"{anew}\" уже существует!");
            base.Remove(old);
            base.Add(anew);
            base.Sort();
        }

        public new void Remove(PassMatter item)
        {
            base.Remove(item);
        }
    }
}
