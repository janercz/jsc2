using System;
using System.Collections.Generic;
using System.Linq;
namespace ukol2;


class Ukol
{
    public record Course(int Id, string Code, string Title, int Credits, string Department);
    public record Lecturer(int Id, string FullName, string Department);
    public record Room(int Id, string Building, string RoomNumber, int Capacity);
    public record ClassSlot(int Id, int CourseId, int LecturerId, int RoomId,
                            DayOfWeek Day, TimeSpan Start, TimeSpan End, string Type); // Type: "Lecture", "Lab", "Seminar"
    public record Enrollment(int StudentId, int CourseId, int Year, string Program);
    public record Student(int Id, string FullName, string Program, int Year);
    static void Main()
    {
            // --- Data ---

            var courses = new List<Course> {
        new(1, "CS101", "Intro to Programming", 6, "CS"),
        new(2, "CS205", "Data Structures", 5, "CS"),
        new(3, "MATH201", "Linear Algebra", 5, "MATH"),
        new(4, "PHY110", "Physics I", 4, "PHY"),
        new(5, "CS310", "Databases", 5, "CS"),
        new(6, "MATH310", "Probability", 5, "MATH"),
        new(7, "CS330", "Operating Systems", 6, "CS"),
        new(8, "HUM101", "Academic Writing", 3, "HUM"),
    };

            var lecturers = new List<Lecturer> {
        new(1, "Mgr. Adam Novák", "CS"),
        new(2, "Ing. Petra Svobodová", "CS"),
        new(3, "Doc. Jan Dvořák", "MATH"),
        new(4, "PhDr. Eva Horáková", "HUM"),
        new(5, "RNDr. Tomáš Bartoš", "PHY"),
        new(6, "Ing. Karel Krátký", "CS"),
    };

            var rooms = new List<Room> {
        new(1, "B3", "105", 32),
        new(2, "B3", "205", 24),
        new(3, "C1", "014", 40),
        new(4, "A2", "301", 8),   // malá kapacita pro test přeplnění
        new(5, "C1", "120", 50),
        new(6, "B4", "010", 28),
    };

            var classSlots = new List<ClassSlot> {
        new(1,  1, 1, 1, DayOfWeek.Monday,    TimeSpan.Parse("09:00"), TimeSpan.Parse("10:30"), "Lecture"),
        new(2,  2, 2, 4, DayOfWeek.Monday,    TimeSpan.Parse("11:00"), TimeSpan.Parse("12:30"), "Lecture"),
        new(3,  3, 3, 3, DayOfWeek.Tuesday,   TimeSpan.Parse("08:00"), TimeSpan.Parse("09:30"), "Lecture"),
        new(4,  5, 2, 2, DayOfWeek.Tuesday,   TimeSpan.Parse("11:00"), TimeSpan.Parse("13:00"), "Lab"),
        new(5,  7, 6, 6, DayOfWeek.Wednesday, TimeSpan.Parse("09:00"), TimeSpan.Parse("10:30"), "Lecture"),
        new(6,  7, 6, 2, DayOfWeek.Tuesday,   TimeSpan.Parse("10:00"), TimeSpan.Parse("12:00"), "Lab"),
        new(7,  8, 4, 1, DayOfWeek.Thursday,  TimeSpan.Parse("13:00"), TimeSpan.Parse("14:30"), "Seminar"),
        new(8,  4, 5, 3, DayOfWeek.Friday,    TimeSpan.Parse("08:30"), TimeSpan.Parse("10:00"), "Lecture"),
        new(9,  6, 3, 5, DayOfWeek.Monday,    TimeSpan.Parse("15:00"), TimeSpan.Parse("16:30"), "Lecture"),
        new(10, 5, 2, 5, DayOfWeek.Thursday,  TimeSpan.Parse("09:00"), TimeSpan.Parse("10:30"), "Lecture"),
        new(11, 2, 2, 4, DayOfWeek.Wednesday, TimeSpan.Parse("14:00"), TimeSpan.Parse("15:30"), "Seminar"),
        new(12, 1, 1, 1, DayOfWeek.Monday,    TimeSpan.Parse("12:00"), TimeSpan.Parse("13:30"), "Seminar"),
        new(13, 5, 2, 5, DayOfWeek.Friday,    TimeSpan.Parse("15:30"), TimeSpan.Parse("17:00"), "Seminar"),
    };

            var students = new List<Student> {
        new(1,  "Alice Nováková",  "INF", 1),
        new(2,  "Bob Šrámek",      "INF", 1),
        new(3,  "Cyril Dvořák",    "INF", 2),
        new(4,  "Dana Veselá",     "INF", 2),
        new(5,  "Ema Horáková",    "INF", 3),
        new(6,  "Filip Král",      "INF", 3),
        new(7,  "Gita Malá",       "MAT", 2),
        new(8,  "Hynek Pokorný",   "MAT", 1),
        new(9,  "Ivana Procházková","HUM", 1),
        new(10, "Jakub Beneš",     "PHY", 2),
        new(11, "Karolína Černá",  "MAT", 3),
        new(12, "Lukáš Urban",     "INF", 1),
    };

            var enrollments = new List<Enrollment> {
        // CS101
        new(1, 1, 2025, "INF"), new(2, 1, 2025, "INF"), new(3, 1, 2025, "INF"), new(4, 1, 2025, "INF"),
        // CS205 (přeplněná místnost A2/301 kapacita 8) – 10 zápisů
        new(1, 2, 2025, "INF"), new(2, 2, 2025, "INF"), new(3, 2, 2025, "INF"), new(4, 2, 2025, "INF"),
        new(5, 2, 2025, "INF"), new(6, 2, 2025, "INF"), new(7, 2, 2025, "INF"), new(8, 2, 2025, "INF"),
        new(9, 2, 2025, "INF"), new(10, 2, 2025, "INF"),
        // MATH201
        new(11, 3, 2025, "MAT"), new(12, 3, 2025, "MAT"),
        // CS310
        new(5, 5, 2025, "INF"), new(6, 5, 2025, "INF"),
        // MATH310
        new(7, 6, 2025, "MAT"), new(8, 6, 2025, "MAT"),
        // HUM101
        new(9, 8, 2025, "HUM"),
        // PHY110
        new(10, 4, 2025, "PHY"),
    };

        //1
        Console.WriteLine("1) WHERE: Úterní laborky 10–14:");
        var tuesdayLabs = classSlots
            .Where(s => s.Day == DayOfWeek.Tuesday && s.Type == "Lab" && s.Start >= TimeSpan.Parse("10:00") && s.End <= TimeSpan.Parse("14:00"))
            .Join(courses, s => s.CourseId, c => c.Id, (s, c) => new { Slot = s, Course = c })
            .Join(rooms, sc => sc.Slot.RoomId, r => r.Id, (sc, r) => new { sc.Course, sc.Slot, Room = r });

        foreach (var item in tuesdayLabs)
        {
            Console.WriteLine($"Laborky z předmětu {item.Course.Code} v místnosti {item.Room.Building}/{item.Room.RoomNumber} od {item.Slot.Start} do {item.Slot.End}");
        }

        //2
        Console.WriteLine("\n2) ORDERBY/THENBY: Prvních 10 vyučovacích slotů seřazených podle dne a času:");
        var first10Slots = classSlots
            .OrderBy(s => s.Day)
            .ThenBy(s => s.Start)
            .Take(10)
            .Join(courses, s => s.CourseId, c => c.Id, (s, c) => new { Slot = s, Course = c });

        foreach (var item in first10Slots)
        {
            Console.WriteLine($"{item.Slot.Day} {item.Slot.Start}: {item.Course.Code}");
        }

        //3
        Console.WriteLine("\n3) JOIN: Plán kurzů s informacemi o vyučujícím a místnosti:");
        var courseSchedule = classSlots
            .Join(courses, s => s.CourseId, c => c.Id, (s, c) => new { Slot = s, Course = c })
            .Join(lecturers, sc => sc.Slot.LecturerId, l => l.Id, (sc, l) => new { sc.Slot, sc.Course, Lecturer = l })
            .Join(rooms, scl => scl.Slot.RoomId, r => r.Id, (scl, r) => new { scl.Slot, scl.Course, scl.Lecturer, Room = r })
            .OrderBy(x => x.Course.Code)
            .ThenBy(x => x.Slot.Type);

        foreach (var item in courseSchedule)
        {
            Console.WriteLine($"{item.Course.Code} ({item.Slot.Type}) - {item.Slot.Day} {item.Slot.Start}-{item.Slot.End}, {item.Lecturer.FullName}, místnost: {item.Room.Building}/{item.Room.RoomNumber}");
        }

        //4
        Console.WriteLine("\n4) GROUPBY + agregace: Celkové kredity zapsané po programech:");
        var creditsByProgram = enrollments
            .Join(courses, e => e.CourseId, c => c.Id, (e, c) => new { e.Program, c.Credits })
            .GroupBy(x => x.Program)
            .Select(g => new { Program = g.Key, TotalCredits = g.Sum(x => x.Credits) }) // CelkemKreditu -> TotalCredits
            .OrderByDescending(x => x.TotalCredits);

        foreach (var item in creditsByProgram)
        {
            Console.WriteLine($"Program {item.Program}: {item.TotalCredits} kreditů");
        }

        //5
        Console.WriteLine("\n5) ANY/ALL: Existují přeplněné vyučovací sloty?");
        
        var enrollmentCounts = enrollments
            .GroupBy(e => e.CourseId)
            .ToDictionary(g => g.Key, g => g.Count());

        var isOvercrowded = classSlots
            .Any(s => enrollmentCounts.GetValueOrDefault(s.CourseId, 0) > rooms.First(r => r.Id == s.RoomId).Capacity);

        Console.WriteLine(isOvercrowded ? "Ano" : "Ne");

        //6
        Console.WriteLine("\n6) RECORD WITH: Plán po přesunu všech slotů z budovy B3 do C1:");
        
        int replacementRoomId = rooms.First(r => r.Building == "C1").Id;

        var updatedSlots = classSlots.Select(s =>
        {
            var room = rooms.First(r => r.Id == s.RoomId);
            return room.Building == "B3" ? s with { RoomId = replacementRoomId } : s;
        });

        foreach (var s in updatedSlots)
        {
            var c = courses.First(x => x.Id == s.CourseId);
            var r = rooms.First(x => x.Id == s.RoomId);
            Console.WriteLine($"{c.Code} v {r.Building}/{r.RoomNumber} v {s.Day} od {s.Start}");
        }
    }
}

/* 
Výstup:
1) WHERE: Úterní laborky 10–14:
Laborky z předmětu CS310 v místnosti B3/205 od 11:00:00 do 13:00:00
Laborky z předmětu CS330 v místnosti B3/205 od 10:00:00 do 12:00:00

2) ORDERBY/THENBY: Prvních 10 vyučovacích slotů seřazených podle dne a času:
Monday 09:00:00: CS101
Monday 11:00:00: CS205
Monday 12:00:00: CS101
Monday 15:00:00: MATH310
Tuesday 08:00:00: MATH201
Tuesday 10:00:00: CS330
Tuesday 11:00:00: CS310
Wednesday 09:00:00: CS330
Wednesday 14:00:00: CS205
Thursday 09:00:00: CS310

3) JOIN: Plán kurzů s informacemi o vyučujícím a místnosti:
CS101 (Lecture) - Monday 09:00:00-10:30:00, Mgr. Adam Novák, místnost: B3/105
CS101 (Seminar) - Monday 12:00:00-13:30:00, Mgr. Adam Novák, místnost: B3/105
CS205 (Lecture) - Monday 11:00:00-12:30:00, Ing. Petra Svobodová, místnost: A2/301
CS205 (Seminar) - Wednesday 14:00:00-15:30:00, Ing. Petra Svobodová, místnost: A2/301
CS310 (Lab) - Tuesday 11:00:00-13:00:00, Ing. Petra Svobodová, místnost: B3/205
CS310 (Lecture) - Thursday 09:00:00-10:30:00, Ing. Petra Svobodová, místnost: C1/120
CS310 (Seminar) - Friday 15:30:00-17:00:00, Ing. Petra Svobodová, místnost: C1/120
CS330 (Lecture) - Wednesday 09:00:00-10:30:00, Ing. Karel Krátký, místnost: B4/010
CS330 (Lab) - Tuesday 10:00:00-12:00:00, Ing. Karel Krátký, místnost: B3/205
HUM101 (Seminar) - Thursday 13:00:00-14:30:00, PhDr. Eva Horáková, místnost: B3/105
MATH201 (Lecture) - Tuesday 08:00:00-09:30:00, Doc. Jan Dvořák, místnost: C1/014
MATH310 (Lecture) - Monday 15:00:00-16:30:00, Doc. Jan Dvořák, místnost: C1/120
PHY110 (Lecture) - Friday 08:30:00-10:00:00, RNDr. Tomáš Bartoš, místnost: C1/014

4) GROUPBY + agregace: Celkové kredity zapsané po programech:
Program INF: 84 kreditů
Program MAT: 20 kreditů
Program PHY: 4 kreditů
Program HUM: 3 kreditů

5) ANY/ALL: Existují přeplněné vyučovací sloty?
Ano

6) RECORD WITH: Plán po přesunu všech slotů z budovy B3 do C1:
CS101 v C1/014 v Monday od 09:00:00
CS205 v A2/301 v Monday od 11:00:00
MATH201 v C1/014 v Tuesday od 08:00:00
CS310 v C1/014 v Tuesday od 11:00:00
CS330 v B4/010 v Wednesday od 09:00:00
CS330 v C1/014 v Tuesday od 10:00:00
HUM101 v C1/014 v Thursday od 13:00:00
PHY110 v C1/014 v Friday od 08:30:00
MATH310 v C1/120 v Monday od 15:00:00
CS310 v C1/120 v Thursday od 09:00:00
CS205 v A2/301 v Wednesday od 14:00:00
CS101 v C1/014 v Monday od 12:00:00
CS310 v C1/120 v Friday od 15:30:00*/