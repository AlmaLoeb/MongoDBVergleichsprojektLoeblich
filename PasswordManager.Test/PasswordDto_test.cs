using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PasswordManagerApp.Applcation.Infastruture;
using PasswordmanagerApp.Application.Dto;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

using static PasswordmanagerApp.Test.EnumTest;
using PasswordmanagerApp.Test;
using PasswordmanagerApp.Application.Model;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Webapi.Controllers;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace PasswordmanagerApp.Application.Tests.Dto
{
    public class PasswordDtoTests : DatabaseTest
    {
        private readonly PasswordmanagerContext db;
        private readonly PasswordController _controller;
        private readonly IMapper _mapper;
        private readonly ILogger<PasswordController> _logger;

        [Fact]
        private PasswordmanagerContext GenerateDbFixtures()
        {

            var password = new Password("http://milanfffffa.info", "Vivian.Br44a", "9os4fYesO_", new Pwdpolicies(12, Strongpwd.Middle));
            var passwordpolicies = new Pwdpolicies(13, Strongpwd.Middle);
            db.Passwords.Add(password);
            db.PasswordPolicies.Add(passwordpolicies);
            db.SaveChanges();
            db.ChangeTracker.Clear();
            return db;
        }

        // Arrange
        [Fact]
        public void GetAllPassword_Return_Success()
        {
            var db = GenerateDbFixtures();

            var sa = new PasswordController(_logger, db, _mapper);

            /*// ACT
                        var password = db.Passwords.First();
                        var cmd = new NewTaskCmd(Subject: "POS", Title: "New Task 2",
                            TeamGuid: password.Guid, TeacherGuid: teacher.Guid,
                            new DateTime(2023, 3, 22), MaxPoints: 16);
                        var guid = sa.AddTask(cmd);

                        // ASSERT
                        db.ChangeTracker.Clear();
                        Assert.True(db.Tasks.Any(t => t.Guid == guid));*/
            var result = sa.GetAllPassword();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
        [Fact]
        public void GetPassword_WithInvalidGuid_ReturnsNotFoundResult()
        {
            var db = GenerateDbFixtures();

            var sa = new PasswordController(_logger, db, _mapper);

            // Arrange
            var invalidGuid = Guid.NewGuid();

            // Act
            var result = sa.GetPassword(invalidGuid);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}

    