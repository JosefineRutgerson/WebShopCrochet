using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebShop.Interfaces;
using WebShop.ViewModels;
using Xunit;

namespace WebShop.Controllers
{
    class StaticDateTime : IDateTime
    {
        public DateTime Now {
            get
            {
                return new DateTime(2016, 9, 1, 6, 0, 0);
            }
        }
    }
    public class HomeControllerTest
    {        
        public HomeControllerTest()
        {

        }
        [Fact]
        public void HomeControllerContactTest()
        {
            // Arrange
            var datetime = new StaticDateTime();
            var controller = new HomeController(datetime);

            // Act
            var result = controller.Contact();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            //Assert.Equal(datetime.Now, viewResult.ViewData["Message"]);
            var model = Assert.IsAssignableFrom<ContactViewModel>(viewResult.ViewData.Model);
            Assert.Equal(datetime.Now.ToString(), model.CurrentDateAndTime);
            Assert.Equal(2, model.Names.Count());
            Assert.Equal("bosse", model.Names.ElementAt(0));
        }
    }
}
