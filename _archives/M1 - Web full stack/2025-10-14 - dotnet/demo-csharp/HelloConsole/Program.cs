using System;
using System.Threading.Tasks;
using HelloConsole.TpStudentMarks;

namespace HelloConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(500);

            var svc = new Service();

            int count = svc.AskStudentCount();

            var names = svc.CollectNames(count);

            var students = svc.CollectMarksForNames(names);

            var average = svc.AverageMark(students);

            svc.PrintSummary(students, average);

        }
    }
}
