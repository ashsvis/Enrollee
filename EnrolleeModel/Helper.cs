using System;
using System.Linq;

namespace EnrolleeModel
{
    /// <summary>
    /// Класс-помощник
    /// </summary>
    public static class Helper
    {
        private static Root _root;

        /// <summary>
        /// Запоминаем ссылку на корневой объект модели
        /// </summary>
        /// <param name="root"></param>
        public static void DefineRoot(Root root)
        {
            _root = root;
        }

        /// <summary>
        /// Получаем полное имя абитуриента по его Id
        /// </summary>
        /// <param name="idEnrollee"></param>
        /// <returns></returns>
        public static string EnrolleeName(Guid idEnrollee)
        {
            var enrollee = _root.Enrollees.FirstOrDefault(item => item.IdEnrollee == idEnrollee);
            if (enrollee == null) return "error enrollee";
            return $"{enrollee.Surname} {enrollee.FirstName[0]}.{enrollee.LastName[0]}.";
        }

        /// <summary>
        /// Получаем название предмета по его Id
        /// </summary>
        /// <param name="idMatter"></param>
        /// <returns></returns>
        public static string MatterById(Guid idMatter)
        {
            var matter = _root.Matters.FirstOrDefault(item => item.IdMatter == idMatter);
            return matter != null ? matter.ToString() : idMatter.ToString();
        }

        /// <summary>
        /// Определяем, что предмет используется в других таблицах
        /// </summary>
        /// <param name="idMatter"></param>
        /// <returns></returns>
        public static bool MatterUsed(Guid idMatter)
        {
            return _root.PassMatters.Any(item => item.IdMatter == idMatter);
        }

        /// <summary>
        /// Получаем имя специальности по его Id
        /// </summary>
        /// <param name="idSpeciality"></param>
        /// <returns></returns>
        public static string SpecialityById(Guid idSpeciality)
        {
            var speciality = _root.Specialities.FirstOrDefault(item => item.IdSpeciality == idSpeciality);
            return speciality != null ? speciality.ToString() : idSpeciality.ToString();
        }

        /// <summary>
        /// Определяем, что специальность используется в других таблицах
        /// </summary>
        /// <param name="idSpeciality"></param>
        /// <returns></returns>
        public static bool SpecialityUsed(Guid idSpeciality)
        {
            return _root.PassMatters.Any(item => item.IdSpeciality == idSpeciality) ||
                _root.Enrollees.Any(item => item.IdSpeciality == idSpeciality);
        }
    }
}
