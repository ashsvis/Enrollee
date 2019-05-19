using System;
using System.Collections.Generic;
using System.ComponentModel;
using ViewGenerator;

namespace EnrolleeModel
{
    [Serializable]
    [Description("Предмет")]
    public class Matter : IComparable<Matter>
    {
        public Guid IdMatter { get; set; } = Guid.NewGuid();

        [Description("Название"), TextSize(200), DataNotEmpty]
        public string Name { get; set; }

        public int CompareTo(Matter other)
        {
            return string.Compare(this.ToString(), other.ToString());
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }

    [Serializable]
    [Description("Предметы")]
    public class Matters : List<Matter>
    {
        public new void Add(Matter item)
        {
            if (base.Exists(x => x.ToString().Trim() == item.ToString().Trim()))
                throw new Exception($"Предмет \"{item}\" уже существует!");
            base.Add(item);
            base.Sort();
        }

        public void ChangeTo(Matter old, Matter anew)
        {
            if (base.FindAll(x => x.ToString().Trim() == anew.ToString().Trim()).Count > 0)
                throw new Exception($"Предмет \"{anew}\" уже существует!");
            base.Remove(old);
            base.Add(anew);
            base.Sort();
        }

        public new void Remove(Matter item)
        {
            if (Helper.MatterUsed(item.IdMatter))
                throw new Exception($"Предмет \"{item}\" ещё используется!");
            base.Remove(item);
        }
    }
}
