using GoTo.Service.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GoTo.Service.Domain;
using Optional.Collections;

namespace GoTo.Service.Tests {
    [TestClass]
    public class DestinationRepositoryTests {
        private IDestinationRepository repo;

        [TestInitialize]
        public void Initalize() {
            repo = new InMemoryDestinationRepository(
                Options.Create(new InMemoryDestinationRepository.Settings() {
                    Destinations = new[] {
                        new Destination.Memento()  {
                            Name= "Waidhofen an der Ybbs",
                            Lat= 47.960310,
                            Lon= 14.772283
                        },
                        new Destination.Memento(){
                            Name= "Linz",
                            Lat= 48.305598,
                            Lon= 14.286601
                        },
                    }
                }
            ));
        }

        [TestMethod]
        public void TestSearchByName() {
            Assert.AreEqual("Linz",
                repo.FindByName("linz").FirstOrNone().ValueOr(() => null)?.Name);
            Assert.AreEqual("Waidhofen an der Ybbs",
                repo.FindByName("waid").FirstOrNone().ValueOr(() => null)?.Name);
            Assert.AreEqual("Waidhofen an der Ybbs",
                repo.FindByName("Waid").FirstOrNone().ValueOr(() => null)?.Name);

            Assert.IsNull(repo.FindByName("ybba").FirstOrNone().ValueOr(() => null));
            Assert.IsNull(repo.FindByName("bla").FirstOrNone().ValueOr(() => null));
            Assert.IsNull(repo.FindByName("lanzen").FirstOrNone().ValueOr(() => null));
            Assert.IsNull(repo.FindByName("Ander").FirstOrNone().ValueOr(() => null));
        }
    }
}
