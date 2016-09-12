using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using A03.Entities;
using A03.Models;
using A03.Models.ViewModels;
using A03.Services.Exceptions;

namespace A03.Services
{
    /// <summary>
    /// A service to connect to the database being used
    /// Takes care of all the needed queries to the database
    /// </summary>
    public class CoursesService : ICoursesService
    {
        private readonly AppDataContext _db;

        /// <exclude />
        public CoursesService(AppDataContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Searches for a certain semester (current one if none given)
        /// and returns either a list of courses being taught that semester
        /// or throws an AppObjectNotFoundException if none are found.
        /// </summary>
        /// <param name="semester">An empty string or a certain semester</param>
        /// <returns>A list of CourseLiteDTO models</returns>
        /// <throws>AppObjectNotFoundException</throws>
        public List<CourseLiteDTO> GetCoursesBySemester(string semester)
        {
            if (string.IsNullOrEmpty(semester))
            {
                semester = "20163"; // Current semester hardcoded
            }

            var list = (from c in _db.Courses
                        join ct in _db.CourseTemplates on c.TemplateId equals ct.CourseId
                        where c.Semester == semester
                        select new CourseLiteDTO
                        {
                            Id = c.Id,
                            Name = ct.Name,
                            Semester = c.Semester,
                            StartDate = Convert.ToDateTime(c.StartDate),
                            EndDate = Convert.ToDateTime(c.EndDate),
                            MaxStudents = c.MaxStudents
                        }).ToList();
            if(!list.Any())
            {
                throw new AppObjectNotFoundException();
            }
            return list;
        }

        /// <summary>
        /// Searches for a course with 'id' as it's Id and returns either a
        /// CourseLiteDTO object with that course's info or throws an
        /// AppObjectNotFoundException if no course with that 'id' is found
        /// </summary>
        /// <param name="id">An Id of a course</param>
        /// <returns>A CourseLiteDTO model</returns>
        /// <throws>AppObjectNotFoundException</throws>
        public CourseLiteDTO GetCourseById(int id)
        {
            var course = (from c in _db.Courses
                          join ct in _db.CourseTemplates on c.TemplateId equals ct.CourseId
                          where c.Id == id
                          select new CourseLiteDTO
                          {
                              Id = c.Id,
                              Name = ct.Name,
                              Semester = c.Semester,
                              StartDate = Convert.ToDateTime(c.StartDate),
                              EndDate = Convert.ToDateTime(c.EndDate),
                              MaxStudents = c.MaxStudents
                          }).SingleOrDefault();
            if (course == null) throw new AppObjectNotFoundException();
            return course;
        }

        /// <summary>
        /// TODO: FILL IN!!!
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CourseLiteDTO AddNewCourse(AddCourseViewModel model)
        {
            var course = new Course
            {
                TemplateId = model.TemplateID,
                Semester = model.Semester,
                StartDate = Convert.ToDateTime(model.StartDate),
                EndDate = Convert.ToDateTime(model.EndDate),
                MaxStudents = model.MaxStudents
            };
            _db.Courses.Add(course);
            _db.SaveChanges();

            return GetCourseById(course.Id);
        }

        /// <summary>
        /// Updates the only mutable attributes (StartDate and EndDate) of
        /// a course with 'id' as it's Id. Throws an AppObjectNotFoundException
        /// if a course isn't found.
        /// </summary>
        /// <param name="id">The course's Id</param>
        /// <param name="model">The UpdateCourseViewModel with the new values</param>
        /// <throws>AppObjectNotFoundException</throws>
        public void UpdateCourseInfo(int id, UpdateCourseViewModel model)
        {
            var course = (from c in _db.Courses
                          where c.Id == id
                          select c).SingleOrDefault();

            if (course == null)
            {
                throw new AppObjectNotFoundException();
            }

            course.StartDate = Convert.ToDateTime(model.StartDate);
            course.EndDate = Convert.ToDateTime(model.EndDate);
            course.MaxStudents = model.MaxStudents;
            _db.SaveChanges();
        }

        /// <summary>
        /// Checks if any students are enrolled in a course or on it's waitinglist
        /// and removes their connections before removing the course from the database.
        /// Throws an AppObjectNotFoundException if a course with 'id' as it's Id
        /// </summary>
        /// <param name="id">The course's Id</param>
        /// <throws>AppObjectNotFoundException</throws>
        public void DeleteCourse(int id)
        {
            var course = (from c in _db.Courses
                          where c.Id == id
                          select c).SingleOrDefault();
            if(course == null) throw new AppObjectNotFoundException();

            var relations = (from rel in _db.StudentCourseRelations
                               where rel.CourseId == id
                               select rel).ToList();
            if (relations.Any())
            {
                foreach (var rel in relations)
                {
                    _db.StudentCourseRelations.Remove(rel);
                }
            }

            var wrelations = (from wrel in _db.StudentWaitinglistRelations
                                where wrel.CourseId == id
                                select wrel).ToList();
            if (wrelations.Any())
            {
                foreach (var wrel in wrelations)
                {
                    _db.StudentWaitinglistRelations.Remove(wrel);
                }
            }

            _db.Courses.Remove(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Sends a list of StudentLiteDTO's to the api if any student's are
        /// enrolled in the course and if a course exists with 'id' as it's Id.
        /// If no course or student's are found, an AppObjectNotFoundException
        /// is thrown.
        /// </summary>
        /// <param name="id">The course's Id</param>
        /// <returns>A list of StudentLiteDTO models</returns>
        /// <throws>AppObjectNotFoundException</throws>
        public List<StudentLiteDTO> GetAllStudentsInCourse(int id)
        {
            var students = (from s in _db.Students
                            join sc in _db.StudentCourseRelations on s.SSN equals sc.StudentId
                            where sc.CourseId == id && sc.Deleted == false
                            select new StudentLiteDTO
                            {
                                Name = s.Name,
                                SSN = s.SSN
                            }).ToList();
            if (!students.Any()) throw new AppObjectNotFoundException();
            return students;
        }

        /// <summary>
        /// Tries to add a student to a course. If either one or neither exist, or if
        /// a connection already exists, the database throws a DbUpdateException.
        /// That exception is used to figure out what went wrong and then an
        /// AppObjectNotFoundException or an AppObjectExistsException is thrown.
        /// 
        /// If no DbUpdateException is thrown, a connection has made between the course
        /// and the student and (s)he's now enrolled in that course.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <param name="model">The student's SSN</param>
        /// <throws>AppObjectNotFoundException</throws>
        /// <throws>AppObjectExistsException</throws>
        /// <throws>MaxNrOfStudentsReachedException</throws>
        public StudentLiteDTO AddStudentToCourse(int cId, AddStudentToCourseViewModel model)
        {
            var maxStudents = (from c in _db.Courses
                               where c.Id == cId
                               select c.MaxStudents).SingleOrDefault();
            var studentsInCourse = NumberOfStudentsInCourse(cId);

            if (maxStudents != 0 && studentsInCourse >= maxStudents) throw new MaxNrOfStudentsReachedException();
            var relation = (from rel in _db.StudentCourseRelations
                where rel.CourseId == cId && rel.StudentId == model.SSN
                select rel).SingleOrDefault();

            if (relation == null)
            {
                try
                {
                    AddStudentToCourseNoChecks(cId, model);
                    return GetStudentBySSN(model.SSN);
                }
                catch (DbUpdateException e)
                {
                    var sqliteException = e.InnerException as SqliteException;
                    // SqLite Error code 19 is for constraint violations
                    if (sqliteException == null || sqliteException.SqliteErrorCode != 19) throw;
                    if (sqliteException.Message.Trim().StartsWith("SQLite Error 19: \'FOREIGN KEY"))
                    {
                        throw new AppObjectNotFoundException(); // CourseId and/or StudentId don't exist
                    }
                    if (sqliteException.Message.Trim().StartsWith("SQLite Error 19: \'UNIQUE"))
                    {
                        throw new AppObjectExistsException(); // There already exists a relation
                    }
                    throw;
                }
            }
            if (!relation.Deleted) { throw new AppObjectExistsException(); }

            relation.Deleted = false;
            _db.SaveChanges();
            return GetStudentBySSN(model.SSN);
        }

        /// <summary>
        /// If a student with 'sId' as SSN isn't connected to a course
        /// with 'cId' as its Id then an AppObjectNotFoundException is
        /// thrown. Else the connection is deleted and the student is
        /// taking that course anymore.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <param name="ssn">A View Model containing the student's SSN</param>
        /// <throws>AppObjectNotFoundException</throws>
        public void RemoveStudentFromCourse(int cId, string ssn)
        {
            MarkStudentCourseRelationRecordAsDeleted(cId, ssn);

            var wrelations = (from wrel in _db.StudentWaitinglistRelations
                              where wrel.CourseId == cId
                              orderby wrel.Id
                              select wrel).ToList();
            if (!wrelations.Any()) return;
            var wrelation = wrelations.First();
            AddStudentToCourse(wrelation.CourseId, new AddStudentToCourseViewModel { SSN = wrelation.StudentId });
            _db.StudentWaitinglistRelations.Remove(wrelation);
            _db.SaveChanges();
        }

        /// <summary>
        /// TODO: FILL OUT
        /// </summary>
        /// <param name="cId"></param>
        /// <param name="SSN"></param>
        private void MarkStudentCourseRelationRecordAsDeleted(int cId, string ssn)
        {
            var relation = (from rel in _db.StudentCourseRelations
                            where rel.CourseId == cId && rel.StudentId == ssn
                            select rel).SingleOrDefault();
            if (relation == null) throw new AppObjectNotFoundException();

            relation.Deleted = true;
            _db.SaveChanges();
        }

        /// <summary>
        /// TODO: You know what to do!!!
        /// </summary>
        /// <param name="id"></param>
        public List<StudentLiteDTO> GetWaitinglistForCourse(int id)
        {
            var students = (from s in _db.Students
                            join wrel in _db.StudentWaitinglistRelations on s.SSN equals wrel.StudentId
                            where wrel.CourseId == id
                            orderby wrel.Id
                            select new StudentLiteDTO
                            {
                                Name = s.Name,
                                SSN = s.SSN
                            }).ToList();
            if (!students.Any()) throw new AppObjectNotFoundException();
            return students;
        }

        /// <summary>
        /// TODO: YOU KNOW WHAT TO DO!
        /// </summary>
        /// <param name="cId"></param>
        /// <param name="model"></param>
        /// <exception cref="AppObjectNotFoundException"></exception>
        /// <exception cref="AppObjectExistsException"></exception>
        public StudentLiteDTO AddStudentToWaitinglist(int cId, AddStudentToCourseViewModel model)
        {
            var relation = (from rel in _db.StudentCourseRelations
                            where rel.CourseId == cId && rel.StudentId == model.SSN && !rel.Deleted
                            select rel).SingleOrDefault();
            if (relation != null) throw new AppObjectExistsException();

            var wrelation = (from wrel in _db.StudentWaitinglistRelations
                             where wrel.CourseId == cId && wrel.StudentId == model.SSN
                             select wrel).SingleOrDefault();
            if (wrelation != null) throw new AppObjectExistsException();

            _db.StudentWaitinglistRelations.Add(new StudentWaitinglistRelation
            {
                CourseId = cId,
                StudentId = model.SSN
            });

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                var sqliteException = e.InnerException as SqliteException;
                // SqLite Error code 19 is for constraint violations
                if (sqliteException == null || sqliteException.SqliteErrorCode != 19) throw;
                if (sqliteException.Message.Trim().StartsWith("SQLite Error 19: \'FOREIGN KEY"))
                {
                    throw new AppObjectNotFoundException(); // CourseId and/or StudentId don't exist
                }
                if (sqliteException.Message.Trim().StartsWith("SQLite Error 19: \'UNIQUE"))
                {
                    throw new AppObjectExistsException(); // There already exists a relation
                }
                throw;
            }
            return GetStudentBySSN(model.SSN);
        }

        /// <summary>
        /// TODO: ADD COMMENTS AND SUMMARY
        /// </summary>
        /// <param name="cId"></param>
        /// <returns></returns>
        private int NumberOfStudentsInCourse(int cId)
        {
            var students = (from sc in _db.StudentCourseRelations
                            where sc.CourseId == cId && sc.Deleted == false
                            select sc).ToList();
            return !students.Any() ? 0 : students.Count;
        }

        private StudentLiteDTO GetStudentBySSN(string ssn)
        {
            var student = (from s in _db.Students
                           where s.SSN == ssn
                           select new StudentLiteDTO
                           {
                               Name = s.Name,
                               SSN = s.SSN
                           }).SingleOrDefault();
            if(student == null) throw new AppObjectNotFoundException();
            return student;
        }

        private void AddStudentToCourseNoChecks(int cId, AddStudentToCourseViewModel model)
        {
            _db.StudentCourseRelations.Add(new StudentCourseRelation
            {
                CourseId = cId,
                StudentId = model.SSN
            }); // No need to add 'Deleted = false' because the DB does that by default
                // Table: StudentCourseRelations, Field: Deleted, default value = false
            _db.SaveChanges();
        }
    }
}
