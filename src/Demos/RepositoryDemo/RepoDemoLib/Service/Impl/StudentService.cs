using System;
namespace RepoDemoLib.Service.Impl
{
	public class StudentService
	{
        private IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepositoryWork)
        {
            _studentRepository = studentRepositoryWork;
        }
    }
}

