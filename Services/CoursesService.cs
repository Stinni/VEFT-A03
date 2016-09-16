using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using A03.Entities;
using A03.Models.DTOs;
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
        /// Searches for a certain semester (the current one if none is given)
        /// and returns either a list of courses being taught that semester
        /// or an empty list if none are found.
        /// </summary>
        /// <param name="semester">An empty string or a certain semester</param>
        /// <returns>A list of CourseLiteDTO models</returns>
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
                            Credits = ct.Credits,
                            MaxStudents = c.MaxStudents
                        }).ToList();
            return list;
        }

        /// <summary>
        /// Searches for a course with 'id' as it's Id and returns either a
        /// CourseLiteDTO object with that course's info or throws an
        /// AppObjectNotFoundException if no course with that 'id' is found
        /// </summary>
        /// <param name="cId">An Id of a course</param>
        /// <returns>A CourseLiteDTO model</returns>
        /// <exception cref="AppObjectNotFoundException" />
        public CourseLiteDTO GetCourseById(int cId)
        {
            var course = (from c in _db.Courses
                          join ct in _db.CourseTemplates on c.TemplateId equals ct.CourseId
                          where c.Id == cId
                          select new CourseLiteDTO
                          {
                              Id = c.Id,
                              Name = ct.Name,
                              Semester = c.Semester,
                              StartDate = Convert.ToDateTime(c.StartDate),
                              EndDate = Convert.ToDateTime(c.EndDate),
                              Credits = ct.Credits,
                              MaxStudents = c.MaxStudents
                          }).SingleOrDefault();
            if (course == null) throw new AppObjectNotFoundException();
            return course;
        }

        /// <summary>
        /// Adds a new course to the database. Takes in an AddCourseViewModel
        /// and returns the new list in a CourseLiteDTO object
        /// </summary>
        /// <param name="model">AddCourseViewModel with the needed data</param>
        /// <returns>A CourseLiteDTO object with the new course's details</returns>
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
        /// Updates the only mutable attributes (StartDate, EndDate and MaxStudents)
        /// of a course with 'id' as it's Id. Throws an AppObjectNotFoundException
        /// if a course isn't found.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <param name="model">The UpdateCourseViewModel with the new values</param>
        /// <exception cref="AppObjectNotFoundException" />
        public void UpdateCourseInfo(int cId, UpdateCourseViewModel model)
        {
            var course = (from c in _db.Courses
                          where c.Id == cId
                          select c).SingleOrDefault();
            if (course == null) throw new AppObjectNotFoundException();

            course.StartDate = Convert.ToDateTime(model.StartDate);
            course.EndDate = Convert.ToDateTime(model.EndDate);
            course.MaxStudents = model.MaxStudents;
            _db.SaveChanges();
        }

        /// <summary>
        /// Checks if any students are enrolled in a course or on it's waitinglist
        /// and removes their connections before removing the course from the database.
        /// Throws an AppObjectNotFoundException if a course with 'id' as it's Id
        /// doesn't exist.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <exception cref="AppObjectNotFoundException" />
        public void DeleteCourse(int cId)
        {
            var course = (from c in _db.Courses
                          where c.Id == cId
                          select c).SingleOrDefault();
            if(course == null) throw new AppObjectNotFoundException();

            RemoveAllConnectionsToCourse(cId);

            _db.Courses.Remove(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Sends a list of StudentLiteDTO's to the api if any student's are
        /// enrolled in the course and if a course exists with 'id' as it's Id.
        /// If no course is found, an AppObjectNotFoundException is thrown.
        /// If no students are found, an empty list is returned.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <returns>A list of StudentLiteDTO models</returns>
        /// <exception cref="AppObjectNotFoundException" />
        public List<StudentLiteDTO> GetAllStudentsInCourse(int cId)
        {
            CheckIfCourseExists(cId); // <- this function throws an exception if course doesn't exist
            var students = (from s in _db.Students
                            join sc in _db.StudentCourseRelations on s.SSN equals sc.StudentId
                            where sc.CourseId == cId && sc.Deleted == false
                            select new StudentLiteDTO
                            {
                                Name = s.Name,
                                SSN = s.SSN
                            }).ToList();
            return students;
        }

        /// <summary>
        /// Tries to add a student to a course. If either one or neither exist, or if
        /// a connection already exists, the database throws a DbUpdateException.
        /// That exception is used to figure out what went wrong and then an
        /// AppObjectNotFoundException or an AppObjectExistsException is thrown.
        /// If no DbUpdateException is thrown, a connection has made between the course
        /// and the student and (s)he's now enrolled in that course.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <param name="model">AddStudentToCourseViewModel containing the student's SSN</param>
        /// <exception cref="AppObjectNotFoundException" />
        /// <exception cref="AppObjectExistsException" />
        /// <exception cref="MaxNrOfStudentsReachedException" />
        public StudentLiteDTO AddStudentToCourse(int cId, AddStudentToCourseViewModel model)
        {
            CheckIfCourseExists(cId);
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
                    RemoveStudentFromWaitinglistIfExists(cId, model.SSN);
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
            RemoveStudentFromWaitinglistIfExists(cId, model.SSN);
            return GetStudentBySSN(model.SSN);
        }

        /// <summary>
        /// If a student with 'ssn' as SSN isn't connected to a course with
        /// 'cId' as its Id then an AppObjectNotFoundException is thrown.
        /// Else the connection is marked as deleted and the student isn't
        /// taking that course anymore.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <param name="ssn">The student's SSN</param>
        /// <exception cref="AppObjectNotFoundException" />
        public void RemoveStudentFromCourse(int cId, string ssn)
        {
            // the function used in the line below throws an exception if a relation doesn't exist
            MarkStudentCourseRelationRecordAsDeleted(cId, ssn);

            var wrelations = (from wrel in _db.StudentWaitinglistRelations
                              where wrel.CourseId == cId
                              orderby wrel.Id
                              select wrel).ToList();
            if (!wrelations.Any()) return;
            var wrelation = wrelations.First();
            AddStudentToCourse(wrelation.CourseId, new AddStudentToCourseViewModel
            {
                SSN = wrelation.StudentId
            });
            _db.StudentWaitinglistRelations.Remove(wrelation);
            _db.SaveChanges();
        }

        /// <summary>
        /// A helper function used in the RemoveStudentFromCourse function
        /// It simply marks a relation/enrollment between a course with 'cId'
        /// as it's Id and a student with 'ssn' as his/her SSN as deleted.
        /// If no relation exists, an AppObjectNotFoundException is thrown.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <param name="ssn">The student's SSN</param>
        /// <exception cref="AppObjectNotFoundException" />
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
        /// Sends a list of StudentLiteDTO's to the api if any student's are on a
        /// waitinglist for a course and if a course exists with 'id' as it's Id.
        /// If no course is found, an AppObjectNotFoundException is thrown.
        /// If no students are found, an empty list is returned.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <returns>A list of StudentLiteDTO models</returns>
        /// <exception cref="AppObjectNotFoundException" />
        public List<StudentLiteDTO> GetWaitinglistForCourse(int cId)
        {
            CheckIfCourseExists(cId); // <- this function throws an exception if course doesn't exist

            var students = (from s in _db.Students
                            join wrel in _db.StudentWaitinglistRelations on s.SSN equals wrel.StudentId
                            where wrel.CourseId == cId
                            orderby wrel.Id
                            select new StudentLiteDTO
                            {
                                Name = s.Name,
                                SSN = s.SSN
                            }).ToList();
            return students;
        }

        /// <summary>
        /// Tries to add a student to a course's waitinglist. If either one or neither
        /// exist, or if the student's already enrolled or on the waitinglist, the database
        /// throws a DbUpdateException.
        /// That exception is used to figure out what went wrong and then an
        /// AppObjectNotFoundException or an AppObjectExistsException is thrown. If no
        /// DbUpdateException is thrown, the student's been added to the course's waitinlist.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <param name="model">AddStudentToCourseViewModel containing the student's SSN</param>
        /// <exception cref="AppObjectNotFoundException" />
        /// <exception cref="AppObjectExistsException" />
        public StudentLiteDTO AddStudentToWaitinglist(int cId, AddStudentToCourseViewModel model)
        {
            CheckIfCourseExists(cId); // <- this function throws an exception if course doesn't exist

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
        /// Helper function to check if a certain course exists. If no course
        /// exists with 'cId' as it's Id, an AppObjectNotFoundException is
        /// thrown.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <exception cref="AppObjectNotFoundException" />
        private void CheckIfCourseExists(int cId)
        {
            var course = (from c in _db.Courses
                          where c.Id == cId
                          select c).SingleOrDefault();
            if(course == null) throw new AppObjectNotFoundException();
        }

        /// <summary>
        /// Checks how many students are enrolled in a certain course with
        /// 'cID' as it's Id and returns the number.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <returns>The number of students enrolled in a course</returns>
        private int NumberOfStudentsInCourse(int cId)
        {
            var students = (from sc in _db.StudentCourseRelations
                            where sc.CourseId == cId && sc.Deleted == false
                            select sc).ToList();
            return !students.Any() ? 0 : students.Count;
        }

        /// <summary>
        /// Helper function for the DeleteCourse function. Use with caution because it
        /// deletes all traces of relations to a certain course with 'cId' as it's Id.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        private void RemoveAllConnectionsToCourse(int cId)
        {
            var relations = (from rel in _db.StudentCourseRelations
                             where rel.CourseId == cId
                             select rel).ToList();
            if (relations.Any())
            {
                foreach (var rel in relations)
                {
                    _db.StudentCourseRelations.Remove(rel);
                }
            }

            var wrelations = (from wrel in _db.StudentWaitinglistRelations
                              where wrel.CourseId == cId
                              select wrel).ToList();
            if (wrelations.Any())
            {
                foreach (var wrel in wrelations)
                {
                    _db.StudentWaitinglistRelations.Remove(wrel);
                }
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Used to get a StudentLiteDTO object with a student's details. If no
        /// student exists with 'ssn' as his/her SSN, an AppObjectNotFoundException
        /// is thrown.
        /// </summary>
        /// <param name="ssn">The student's SSN</param>
        /// <returns>A StudentLiteDTO object with the student's details</returns>
        /// <exception cref="AppObjectNotFoundException" />
        // ReSharper disable once InconsistentNaming   
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

        /// <summary>
        /// Helper function for the AddStudentToCourse function. Adds a student to a
        /// course without any checks. An DbUpdateException is thrown if it's not possible
        /// and that's caught and checked in the AddStudentToCourse function.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <param name="model">AddStudentToCourseViewModel containing the student's SSN</param>
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

        /// <summary>
        /// Helper function for the AddStudentToCourse function. Checks if a student
        /// is on a waitinglist for a course with 'cId' as it's Id and deletes the
        /// relation if so.
        /// </summary>
        /// <param name="cId">The course's Id</param>
        /// <param name="ssn">The student's SSN</param>
        private void RemoveStudentFromWaitinglistIfExists(int cId , string ssn)
        {
            var wrelation = (from wrel in _db.StudentWaitinglistRelations
                             where wrel.CourseId == cId && wrel.StudentId == ssn
                             select wrel).SingleOrDefault();
            if (wrelation == null) return;

            _db.StudentWaitinglistRelations.Remove(wrelation);
            _db.SaveChanges();
        }
    }
}
